using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows;

namespace MongoDBHelper
{
    /// <summary>
    /// 系统日志
    /// </summary>
    public class LogHelper
    {
        public static string path
        {
            get
            {
                string applicationPath = "";
                if (System.Web.HttpContext.Current != null)//WebForm
                {
                    applicationPath = System.Web.HttpContext.Current.Server.MapPath("~");
                }
                else if (System.Web.Hosting.HostingEnvironment.IsHosted)//SVC
                {
                    applicationPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                }
                else if (AppDomain.CurrentDomain.SetupInformation.ApplicationBase != null)//服务
                {
                    applicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                }
                else if (Environment.CurrentDirectory != null)//控制台
                {
                    applicationPath = Environment.CurrentDirectory;
                }
                else//WinForm
                {
                    applicationPath = System.Windows.Forms.Application.StartupPath;
                }
                return applicationPath + "//Logs//";
            }
        }

        /**
         * 向日志文件写入调试信息
         * @param className 类名
         * @param content 写入内容
         */
        public static void writeDebug(string className, string content)
        {
            WriteLog("Debug", className, content);
        }

        /**
        * 向日志文件写入运行时信息
        * @param className 类名
        * @param content 写入内容
        */
        public static void writeInfor(string className, string content)
        {
            WriteLog("Infor", className, content);
        }

        /**
        * 向日志文件写入出错信息
        * @param className 类名
        * @param content 写入内容
        */
        public static void writeError(string className, string content)
        {
            WriteLog("Error", className, content);
        }

        /**
        * 实际的写日志操作
        * @param type 日志记录类型
        * @param className 类名
        * @param content 写入内容
        */
        protected static void WriteLog(string type, string className, string content)
        {
            if (!Directory.Exists(path))//如果日志目录不存在就创建
            {
                Directory.CreateDirectory(path);
            }        

            string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//获取当前系统时间
            string strYmd = System.DateTime.Now.ToString("yyyy-MM-dd");
            string fileName = type + strYmd + ".txt";//每天按照日期建立一个不同的文件名  
            StreamWriter sr;
            string strPath = path;
            if (File.Exists("" + strPath + "" + fileName))
            {
                sr = File.AppendText("" + strPath + "" + fileName);
            }
            else
            {
                sr = File.CreateText("" + strPath + "" + fileName);
            }
            //向日志文件写入内容
            string write_content = currentTime + " - " + type + " " + className + ": " + content;
            sr.WriteLine(write_content);
            sr.Close();

        }
        
    }
}
