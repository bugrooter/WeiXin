using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;

namespace Comm.Core
{
    public class core
    {
        public static Dictionary<string, object> ObjectCacheDic = new Dictionary<string, object>();
        public static object CreateObject(string className)
        {
            Type t = Type.GetType(className);
            object obj = null;
            if (t != null)
            {
                obj = Activator.CreateInstance(t);
            }
            return obj;
        }

        public static object InvokeMethod(string className, string methodName, params object[] pars)
        {
            try
            {
                object obj = null;
                if (ObjectCacheDic.ContainsKey(className))
                {
                    obj = ObjectCacheDic[className];
                }
                else
                {
                    obj = CreateDll(className);
                    if (obj == null)
                    {
                        return null;
                    }
                    //ObjectCacheDic.Add(className, obj);
                    
                    //Type t = Type.GetType(className);
                    //if (t != null)
                    //{
                    //    obj = Activator.CreateInstance(t);
                    //    if (obj == null)
                    //    {
                    //        return null;
                    //    }
                    //    ObjectCacheDic.Add(className, obj);
                    //}
                }
                MethodInfo o = obj.GetType().GetMethod(methodName);
                return o.Invoke(obj, pars);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static object CreateDll(string className)
        {
            string sourceFileName = HttpContext.Current.Server.MapPath("~/public/plugin/" + className.Replace(".", "/")) + ".cs";
            FileInfo sourceFile = new FileInfo(sourceFileName);
            FileStream fs = sourceFile.OpenRead();
            string code = string.Empty;
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            code = System.Text.Encoding.UTF8.GetString(buffer);
            byte[] b = System.Text.Encoding.UTF8.GetBytes(code);
            if (b[0]==239&&b[1]==187&&b[2]==191)
            {
                byte[] bt = new byte[b.Length - 3];
                Array.Copy(b, 3, bt, 0, bt.Length);
                code = System.Text.Encoding.UTF8.GetString(bt);
            }
            string head = string.Empty;
            int a = 0;
            if (code.IndexOf("namespace") > -1)
            {
                a = code.IndexOf("namespace");
            }
            else
            {
                a = code.IndexOf("public");
            }
            head = code.Substring(0, a).Replace("\r\n", "");
            code = code.Substring(a);
            CodeDomProvider provider = null;
            provider = new Microsoft.CSharp.CSharpCodeProvider();
            if (provider != null)
            {
                CompilerParameters cp = new CompilerParameters();
                cp.GenerateExecutable = false;
                cp.OutputAssembly = Math.Abs(DateTime.Now.GetHashCode())+"";
                cp.GenerateInMemory = true;
                cp.TreatWarningsAsErrors = false;
                string[] aa = head.Split(new char[] { ';' },StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < aa.Length; i++)
                {
                    if (!string.IsNullOrEmpty(aa[i]+""))
                    {
                        string bin = HttpContext.Current.Server.MapPath("~/Bin/");
                        if (System.IO.File.Exists(bin+(aa[i].Replace("using ", "") + ".dll").Trim()))
                        {
                            cp.ReferencedAssemblies.Add(bin + (aa[i].Replace("using ", "") + ".dll").Trim());
                        }
                        else
                        {
                            cp.ReferencedAssemblies.Add((aa[i].Replace("using ", "") + ".dll").Trim());
                        }
                    }
                }
                if (!cp.ReferencedAssemblies.Contains("System.dll"))
                {
                    cp.ReferencedAssemblies.Add("System.dll");
                }
                if (!cp.ReferencedAssemblies.Contains("System.Web.dll"))
                {
                    cp.ReferencedAssemblies.Add("System.Web.dll");
                }

                CompilerResults cr = provider.CompileAssemblyFromSource(cp, code);
                if (cr.Errors.Count < 1)
                {
                    Assembly asm = cr.CompiledAssembly;
                    object obj = asm.CreateInstance("plugin."+className);
                    return obj;
                }
            }
            return null;
        }
    }
}