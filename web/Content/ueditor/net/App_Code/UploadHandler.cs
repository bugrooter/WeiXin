using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

/// <summary>
/// UploadHandler 的摘要说明
/// </summary>
public class UploadHandler : Handler
{

    public UploadConfig UploadConfig { get; private set; }
    public UploadResult Result { get; private set; }

    public UploadHandler(HttpContext context, UploadConfig config)
        : base(context)
    {
        this.UploadConfig = config;
        this.Result = new UploadResult() { State = UploadState.Unknown };
    }

    public override void Process()
    {
        byte[] uploadFileBytes = null;
        string uploadFileName = null;

        if (UploadConfig.Base64)
        {
            uploadFileName = UploadConfig.Base64Filename;
            uploadFileBytes = Convert.FromBase64String(Request[UploadConfig.UploadFieldName]);
        }
        else
        {
            var file = Request.Files[UploadConfig.UploadFieldName];
            uploadFileName = file.FileName;

            if (!CheckFileType(uploadFileName))
            {
                Result.State = UploadState.TypeNotAllow;
                WriteResult();
                return;
            }
            if (!CheckFileSize(file.ContentLength))
            {
                Result.State = UploadState.SizeLimitExceed;
                WriteResult();
                return;
            }

            uploadFileBytes = new byte[file.ContentLength];
            try
            {
                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
            }
            catch (Exception)
            {
                Result.State = UploadState.NetworkError;
                WriteResult();
            }
        }

        Result.OriginFileName = uploadFileName;

        var savePath = PathFormatter.Format(uploadFileName, UploadConfig.PathFormat);
        var localPath = Server.MapPath(savePath);
        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(localPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            }
            //是否加水印
            if (!UploadConfig.haveWatermark)
            {
                File.WriteAllBytes(localPath, uploadFileBytes);
            }
            else
            {
                //水印处理
                MemoryStream ms = new MemoryStream(uploadFileBytes);
                var markPath = Server.MapPath("/Content/images/watermark.png");
                WriteFileWatermark(ms, markPath, "", localPath);
                ms.Close();
            }
            Result.Url = savePath;
            Result.State = UploadState.Success;
        }
        catch (Exception e)
        {
            Result.State = UploadState.FileAccessError;
            Result.ErrorMessage = e.Message;
        }
        finally
        {
            WriteResult();
        }
    }

    private void WriteResult()
    {
        this.WriteJson(new
        {
            state = GetStateMessage(Result.State),
            url = Result.Url,
            title = Result.OriginFileName,
            original = Result.OriginFileName,
            error = Result.ErrorMessage
        });
    }

    private string GetStateMessage(UploadState state)
    {
        switch (state)
        {
            case UploadState.Success:
                return "SUCCESS";
            case UploadState.FileAccessError:
                return "文件访问出错，请检查写入权限";
            case UploadState.SizeLimitExceed:
                return "文件大小超出服务器限制";
            case UploadState.TypeNotAllow:
                return "不允许的文件格式";
            case UploadState.NetworkError:
                return "网络错误";
        }
        return "未知错误";
    }

    private bool CheckFileType(string filename)
    {
        var fileExtension = Path.GetExtension(filename).ToLower();
        return UploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension);
    }

    private bool CheckFileSize(int size)
    {
        return size < UploadConfig.SizeLimit;
    }

    /// <summary>    
    /// Creating a Watermarked Photograph with GDI+ for .NET    
    /// </summary>    
    /// <param name="rSrcImgPath">原始图片</param>    
    /// <param name="rMarkImgPath">水印图片的物理路径</param>    
    /// <param name="rMarkText">水印文字（不显示水印文字设为空串）</param>    
    /// <param name="rDstImgPath">输出合成后的图片的物理路径</param>    
    private void WriteFileWatermark(MemoryStream srcimg, string rMarkImgPath, string rMarkText, string rDstImgPath)
    {
        Image imgPhoto = Image.FromStream(srcimg);
        int phWidth = imgPhoto.Width;
        int phHeight = imgPhoto.Height;
        Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
        bmPhoto.SetResolution(72, 72);
        Graphics grPhoto = Graphics.FromImage(bmPhoto);
        Image imgWatermark = new Bitmap(rMarkImgPath);
        int wmWidth = imgWatermark.Width;
        int wmHeight = imgWatermark.Height;    
        grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
        grPhoto.DrawImage(
             imgPhoto,
             new Rectangle(0, 0, phWidth, phHeight),
             0,
             0,
             phWidth,
             phHeight,
             GraphicsUnit.Pixel);
        int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
        Font crFont = null;
        SizeF crSize = new SizeF();
        for (int i = 0; i < 7; i++)
        {
            crFont = new Font("arial", sizes[i],
                  FontStyle.Bold);
            crSize = grPhoto.MeasureString(rMarkText,
                  crFont);
            if ((ushort)crSize.Width < (ushort)phWidth)
                break;
        } 
        int yPixlesFromBottom = (int)(phHeight * .05);
        float yPosFromBottom = ((phHeight -
             yPixlesFromBottom) - (crSize.Height / 2));
        float xCenterOfImg = (phWidth / 2);
        StringFormat StrFormat = new StringFormat();
        StrFormat.Alignment = StringAlignment.Center;  
        SolidBrush semiTransBrush2 =
             new SolidBrush(Color.FromArgb(153, 0, 0, 0));
        grPhoto.DrawString(rMarkText,
             crFont,
             semiTransBrush2,
             new PointF(xCenterOfImg + 1, yPosFromBottom + 1),
             StrFormat);
        SolidBrush semiTransBrush = new SolidBrush(
             Color.FromArgb(153, 255, 255, 255));
        grPhoto.DrawString(rMarkText,
             crFont,
             semiTransBrush,
             new PointF(xCenterOfImg, yPosFromBottom),
             StrFormat);
        Bitmap bmWatermark = new Bitmap(bmPhoto);
        bmWatermark.SetResolution(
             imgPhoto.HorizontalResolution,
             imgPhoto.VerticalResolution);
        Graphics grWatermark =
             Graphics.FromImage(bmWatermark);
        ImageAttributes imageAttributes =
             new ImageAttributes();
        ColorMap colorMap = new ColorMap();
        colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
        colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
        ColorMap[] remapTable = { colorMap };
        imageAttributes.SetRemapTable(remapTable,
             ColorAdjustType.Bitmap);
        float[][] colorMatrixElements = {     
                                             new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},    
                                             new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},    
                                             new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},    
                                             new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},    
                                             new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}    
                                        };
        ColorMatrix wmColorMatrix = new
             ColorMatrix(colorMatrixElements);
        imageAttributes.SetColorMatrix(wmColorMatrix,
             ColorMatrixFlag.Default,
             ColorAdjustType.Bitmap);
        int markWidth;
        int markHeight;
        if (phWidth <= wmWidth)
        {
            markWidth = phWidth - 10;
            markHeight = (markWidth * wmHeight) / wmWidth;
        }
        else if (phHeight <= wmHeight)
        {
            markHeight = phHeight - 10;
            markWidth = (markHeight * wmWidth) / wmHeight;
        }
        else
        {
            markWidth = wmWidth;
            markHeight = wmHeight;
        }
        int xPosOfWm = ((phWidth - markWidth) - 10);
        int yPosOfWm = phHeight-markHeight-10;
        grWatermark.DrawImage(imgWatermark,
             new Rectangle(xPosOfWm, yPosOfWm, markWidth,
             markHeight),
             0,
             0,
             wmWidth,
             wmHeight,
             GraphicsUnit.Pixel,
             imageAttributes);
        imgPhoto = bmWatermark;
        grPhoto.Dispose();
        grWatermark.Dispose();
        imgPhoto.Save(rDstImgPath, ImageFormat.Jpeg);
        imgPhoto.Dispose();
        imgWatermark.Dispose();
    }
}

public class UploadConfig
{
    /// <summary>
    /// 文件命名规则
    /// </summary>
    public string PathFormat { get; set; }

    /// <summary>
    /// 上传表单域名称
    /// </summary>
    public string UploadFieldName { get; set; }

    /// <summary>
    /// 上传大小限制
    /// </summary>
    public int SizeLimit { get; set; }

    /// <summary>
    /// 上传允许的文件格式
    /// </summary>
    public string[] AllowExtensions { get; set; }

    /// <summary>
    /// 文件是否以 Base64 的形式上传
    /// </summary>
    public bool Base64 { get; set; }

    /// <summary>
    /// Base64 字符串所表示的文件名
    /// </summary>
    public string Base64Filename { get; set; }

    /// <summary>
    /// 是否添加水印
    /// </summary>
    public bool haveWatermark { get; set; }
}

public class UploadResult
{
    public UploadState State { get; set; }
    public string Url { get; set; }
    public string OriginFileName { get; set; }

    public string ErrorMessage { get; set; }
}

public enum UploadState
{
    Success = 0,
    SizeLimitExceed = -1,
    TypeNotAllow = -2,
    FileAccessError = -3,
    NetworkError = -4,
    Unknown = 1,
}

