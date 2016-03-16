using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Comm
{
    public class FileHelper
    {
        /// <summary>
        /// 保存post文件
        /// </summary>
        /// <param name="filename">表单名</param>
        /// <param name="absolutePath">web路径</param>
        /// <param name="maxsize">最大大小</param>
        /// <param name="filter">文件类型</param>
        /// <param name="error">错误信息</param>
        /// <param name="savefilename">保存的文件名(可选)</param>
        /// <returns></returns>
        public static bool SaveHttpPostFile(string filename, string absolutePath, int maxsize, string filter, out string error, string savefilename = "")
        {
            try
            {
                if (!Directory.Exists(absolutePath))
                {
                    CreateDir(absolutePath);
                }
                error = "";
                System.Web.HttpPostedFile file = HttpContext.Current.Request.Files[filename];
                if (file == null)
                {
                    error = "没有要保存的文件";
                    return false;
                }
                if (file.ContentLength > maxsize)
                {
                    error = "文件大小超出限制";
                    return false;
                }
                else if (file.ContentLength == 0)
                {
                    error = "文件大小不能为0";
                    return false;
                }
                string filefullname = file.FileName;
                string fileformat = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(filefullname.LastIndexOf('.') + 1) : "";
                string fileindex = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(0, filefullname.LastIndexOf('.')) : filefullname;
                if (filter.Contains('|'))
                {
                    string[] filters = filter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (filters.Length > 0 && !filters.Contains(fileformat))
                    {
                        error = "文件格式不正确";
                        return false;
                    }
                }
                string newfilename = "";
                if (savefilename == "")
                {
                    newfilename = Math.Abs(Guid.NewGuid().GetHashCode()) + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + fileformat;
                }
                else
                {
                    newfilename = savefilename + "." + fileformat;
                }
                file.SaveAs(absolutePath + newfilename);
                error = newfilename;
                return true;
            }
            catch
            {
                error = "内部错误";
                return false;
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="absolutePath">web绝对路径</param>
        /// <param name="maxsize">文件大小</param>
        /// <param name="filter">文件格式,*-所有格式</param>
        /// <param name="error">错误信息</param>
        /// <param name="savefilename"></param>
        /// <returns></returns>
        public static bool SaveHttpPostFileDefault(string absolutePath, int maxsize, string filter, out string error, string savefilename = "")
        {
            try
            {
                if (!Directory.Exists(absolutePath))
                {
                    CreateDir(absolutePath);
                }
                error = "";
                if (HttpContext.Current.Request.Files.Count == 0)
                {
                    error = "没有要保存的文件";
                    return false;
                }
                System.Web.HttpPostedFile file = HttpContext.Current.Request.Files[0];
                if (file == null)
                {
                    error = "没有要保存的文件";
                    return false;
                }
                if (file.ContentLength > maxsize)
                {
                    error = "文件大小超出限制";
                    return false;
                }
                else if (file.ContentLength == 0)
                {
                    error = "文件大小不能为0";
                    return false;
                }
                string filefullname = file.FileName;
                string fileformat = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(filefullname.LastIndexOf('.') + 1) : "";
                string fileindex = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(0, filefullname.LastIndexOf('.')) : filefullname;
                if (filter != "*" && filter.Contains('|'))
                {
                    string[] filters = filter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (filters.Length > 0 && !filters.Contains(fileformat))
                    {
                        error = "文件格式不正确";
                        return false;
                    }
                }
                string newfilename = "";
                if (savefilename == "")
                {
                    newfilename = Math.Abs(Guid.NewGuid().GetHashCode()) + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + fileformat;
                }
                else
                {
                    newfilename = savefilename + "." + fileformat;
                }
                file.SaveAs(absolutePath + newfilename);
                error = newfilename;
                return true;
            }
            catch
            {
                error = "内部错误";
                return false;
            }
        }

        /// <summary>
        /// http文件转byte
        /// </summary>
        /// <param name="filename">表单文件名称</param>
        /// <param name="maxsize">文件大小</param>
        /// <param name="filter">支持的文件格式</param>
        /// <param name="error">错误信息</param>
        /// <returns>失败返回null,成功返回byte[]</returns>
        public static byte[] ConvertHttpFileToBytes(string filename, int maxsize, string filter, out string error)
        {
            try
            {
                error = "";
                System.Web.HttpPostedFile file = HttpContext.Current.Request.Files[filename];
                if (file == null)
                {
                    error = "没有要保存的文件";
                    return null;
                }
                if (file.ContentLength > maxsize)
                {
                    error = "文件大小超出限制";
                    return null;
                }
                else if (file.ContentLength == 0)
                {
                    error = "文件大小不能为0";
                    return null;
                }
                string filefullname = file.FileName;
                string fileformat = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(filefullname.LastIndexOf('.') + 1) : "";
                string fileindex = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(0, filefullname.LastIndexOf('.')) : filefullname;
                if (filter.Contains('|'))
                {
                    string[] filters = filter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (filters.Length > 0 && !filters.Contains(fileformat))
                    {
                        error = "文件格式不正确";
                        return null;
                    }
                }
                byte[] buffer = new byte[file.ContentLength];
                Stream stream = file.InputStream;
                int total = 0;
                while (total < file.ContentLength)
                {
                    int ret = stream.Read(buffer, total, file.ContentLength);
                    total += ret;
                }
                stream.Close();
                return buffer;
            }
            catch
            {
                error = "内部错误";
                return null;
            }
        }

        /// <summary>
        /// 根据相对路径删除web文件
        /// </summary>
        /// <param name="fullpath">相对路径,如/image/samlpe.jpg</param>
        /// <returns>bool</returns>
        public static bool DeleteFileByPath(string fullpath)
        {
            try
            {
                string p = "";
                string n = "";
                if (fullpath.IndexOf("/") > -1)
                {
                    p = fullpath.Substring(0, fullpath.LastIndexOf("/") + 1);
                    n = fullpath.Substring(fullpath.LastIndexOf("/") + 1);
                }

                string path = HttpContext.Current.Server.MapPath(p);
                if (System.IO.File.Exists(path + n))
                {
                    System.IO.File.Delete(path + n);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 保存post表单文件,加上水印,保存为200*180大小
        /// </summary>
        /// <param name="filename">表单文件名称</param>
        /// <param name="savepath">保存路径</param>
        /// <param name="maxsize">文件大小</param>
        /// <param name="filter">文件格式</param>
        /// <param name="error">错误信息</param>
        /// <param name="savefilename">文件保存名称</param>
        /// <param name="hasmark">是否加水印</param>
        /// <returns></returns>
        public static bool SaveHttpPostFileWithLogo(string filename, string absolutePath, int maxsize, string filter, out string error, bool hasmark, int width, int height)
        {
            try
            {
                if (!Directory.Exists(absolutePath))
                {
                    CreateDir(absolutePath);
                }
                error = "";
                System.Web.HttpPostedFile file = HttpContext.Current.Request.Files[filename];
                if (file == null)
                {
                    error = "没有要保存的文件";
                    return false;
                }
                if (file.ContentLength > maxsize)
                {
                    error = "文件大小超出限制";
                    return false;
                }
                else if (file.ContentLength == 0)
                {
                    error = "文件大小不能为0";
                    return false;
                }
                string filefullname = file.FileName;
                string fileformat = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(filefullname.LastIndexOf('.') + 1) : "";
                string fileindex = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(0, filefullname.LastIndexOf('.')) : filefullname;
                if (filter.Contains('|'))
                {
                    string[] filters = filter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (filters.Length > 0 && !filters.Contains(fileformat))
                    {
                        error = "文件格式不正确";
                        return false;
                    }
                }
                string newfilename = "";
                newfilename = Math.Abs(Guid.NewGuid().GetHashCode()) + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + fileformat;
                //获取图片
                byte[] buffer = new byte[file.ContentLength];
                Stream stream = file.InputStream;
                int total = 0;
                while (total < file.ContentLength)
                {
                    int ret = stream.Read(buffer, total, file.ContentLength);
                    total += ret;
                }
                Image simg = Image.FromStream(stream);
                int sw = width > 0 ? width : simg.Width;
                int sh = height > 0 ? height : simg.Height;
                Bitmap img = new Bitmap(sw, sh);
                Graphics g = Graphics.FromImage(img);
                g.DrawImage(simg, new Rectangle(0, 0, sw, sh));
                simg.Dispose();
                if (hasmark)
                {
                    string logopath = HttpContext.Current.Server.MapPath("/Content/webimages/") + "inner_logo.png";
                    if (File.Exists(logopath))
                    {
                        //图片缩放
                        Image logo = Image.FromFile(logopath);

                        g.DrawImage(logo, new RectangleF(10, img.Height * 0.9f - img.Height * 0.2f * logo.Width / logo.Height, img.Height * 0.2f * logo.Width / logo.Height, img.Height * 0.2f));
                        //g.DrawImage(logo, img.Width - logo.Width - 10, img.Height - logo.Height - 10);
                        logo.Dispose();
                    }
                }
                g.Dispose();
                stream.Close();

                img.Save(absolutePath + newfilename, ImageFormat.Jpeg);
                error = newfilename;
                return true;
            }
            catch
            {
                error = "内部错误";
                return false;
            }
        }

        //public static bool SaveHttpPostFileWithLogo(string absolutePath, int maxsize, string filter, out string error, string savefilename = "", bool hasmark = true, string logoPath = "")
        //{
        //    try
        //    {
        //        if (!Directory.Exists(absolutePath))
        //        {
        //            CreateDir(absolutePath);
        //        }
        //        error = "";
        //        if (HttpContext.Current.Request.Files.Count == 0)
        //        {
        //            error = "没有要保存的文件";
        //            return false;
        //        }
        //        System.Web.HttpPostedFile file = HttpContext.Current.Request.Files[0];
        //        if (file == null)
        //        {
        //            error = "没有要保存的文件";
        //            return false;
        //        }
        //        if (file.ContentLength > maxsize)
        //        {
        //            error = "文件大小超出限制";
        //            return false;
        //        }
        //        else if (file.ContentLength == 0)
        //        {
        //            error = "文件大小不能为0";
        //            return false;
        //        }
        //        string filefullname = file.FileName;
        //        string fileformat = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(filefullname.LastIndexOf('.') + 1) : "";
        //        string fileindex = filefullname.LastIndexOf('.') > -1 ? filefullname.Substring(0, filefullname.LastIndexOf('.')) : filefullname;
        //        if (filter.Contains('|'))
        //        {
        //            string[] filters = filter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        //            if (filters.Length > 0 && !filters.Contains(fileformat))
        //            {
        //                error = "文件格式不正确";
        //                return false;
        //            }
        //        }
        //        string newfilename = "";
        //        if (savefilename == "")
        //        {
        //            newfilename = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + fileformat;
        //        }
        //        else
        //        {
        //            newfilename = savefilename + "." + fileformat;
        //        }
        //        //获取图片
        //        byte[] buffer = new byte[file.ContentLength];
        //        Stream stream = file.InputStream;
        //        int total = 0;
        //        while (total < file.ContentLength)
        //        {
        //            int ret = stream.Read(buffer, total, file.ContentLength);
        //            total += ret;
        //        }
        //        Image img = Image.FromStream(stream);
        //        Graphics g = Graphics.FromImage(img);
        //        logoPath = logoPath == "" ? logoPath = HttpContext.Current.Server.MapPath("/Content/webimages/") + "inner_logo.png" : logoPath;
        //        if (File.Exists(logoPath))
        //        {
        //            Image logo = Image.FromFile(logoPath);
        //            g.DrawImage(logo, img.Width - logo.Width - 5, img.Height - logo.Height - 5);
        //        }
        //        else
        //        {
        //            g.DrawString("荆门百科网", new Font("宋体", 22), Brushes.Black, new PointF(20, img.Height - 20));

        //        }
        //        g.Dispose();
        //        stream.Close();

        //        img.Save(absolutePath + newfilename);
        //        error = newfilename;
        //        return true;
        //    }
        //    catch
        //    {
        //        error = "内部错误";
        //        return false;
        //    }
        //}


        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SaveImageFromBase64(string absolutePath, string filename, string data)
        {
            try
            {
                if (!Directory.Exists(absolutePath))
                {
                    CreateDir(absolutePath);
                }
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(data));
                Bitmap img = new Bitmap(stream);
                string newfilename = filename + "_" + Math.Abs(Guid.NewGuid().GetHashCode()) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                img.Save(absolutePath + newfilename, ImageFormat.Png);
                img.Dispose();
                stream.Close();
                return newfilename;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 图片转base64
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string ConvertImageToBase64(string filename)
        {
            try
            {
                System.Web.HttpPostedFile file = HttpContext.Current.Request.Files[filename];
                if (file == null)
                {
                    return "0";
                }
                Stream s = file.InputStream;
                byte[] arr = new byte[s.Length];
                s.Read(arr, 0, (int)s.Length);
                String strbaser64 = Convert.ToBase64String(arr);
                return strbaser64;
            }
            catch
            {
                return "1";
            }
        }

        /// <summary>
        /// base64 转图片
        /// </summary>
        /// <param name="base64data"></param>
        /// <returns></returns>
        public static Bitmap ConvertBase64ToImage(string base64data)
        {
            try
            {
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64data));
                Bitmap img = new Bitmap(stream);
                stream.Close();
                return img;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取目录所有文件
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns>list<string[2]>,0-name,1-relativepath</returns>
        public static List<string[]> GetDirectoryFiles(string absolutePath)
        {
            List<string[]> list = new List<string[]>();

            if (!absolutePath.EndsWith("\\"))
            {
                absolutePath += "\\";
            }
            string rootPath = HttpContext.Current.Server.MapPath("/");
            string tempdir = absolutePath.Substring(rootPath.Length).Replace("\\", "/");
            if (Directory.Exists(absolutePath))
            {
                string[] filenames = Directory.GetFiles(absolutePath);
                string[] keyvalue = null;
                string filepath = string.Empty;
                for (int i = 0; i < filenames.Length; i++)
                {
                    keyvalue = new string[2];
                    filepath = filenames[i].Replace("\\", "/");
                    keyvalue[0] = filepath.Substring(filepath.LastIndexOf("/") + 1);
                    keyvalue[1] = tempdir.StartsWith("/") ? tempdir : "/" + tempdir + keyvalue[0];
                    list.Add(keyvalue);
                }
            }
            return list;
        }

        /// <summary>
        /// 创建web路径
        /// </summary>
        /// <param name="absolutePath"></param>
        public static void CreateDir(string absolutePath)
        {
            if (!absolutePath.EndsWith("\\"))
            {
                absolutePath += "\\";
            }
            string rootpath = HttpContext.Current.Server.MapPath("/");
            string tempdir = absolutePath.Substring(rootpath.Length);
            string[] dirs = tempdir.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            string allpath = rootpath;
            for (int i = 0; i < dirs.Length; i++)
            {
                allpath = allpath + dirs[i] + "\\";
                if (System.IO.Directory.Exists(allpath) == false)
                {
                    System.IO.Directory.CreateDirectory(allpath);
                }
            }
        }
    }
}
