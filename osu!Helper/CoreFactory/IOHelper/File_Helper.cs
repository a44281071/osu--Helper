using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace osu_Helper
{
    /// <summary>
    /// 基本文件（夹）操作类
    /// </summary>
    class File_Helper
    {
        public File_Helper()
        {
            this.ConfigDir = "zhZher";
            this.AppDir = AppDomain.CurrentDomain.BaseDirectory;
            //this.AppDir = Environment.CurrentDirectory;
        }

        /// <summary>
        /// 程序所在目录
        /// </summary>
        public string AppDir { get; set; }
        /// <summary>
        /// 程序配置文件所在目录
        /// </summary>      
        public string ConfigDir { get; set; }

        /// <summary>
        /// 加载xml文件
        /// </summary>
        /// <param name="name">xml文件名(不含扩展名)</param>
        /// <returns></returns>
        public XElement LoadXml(string name)
        {
            XElement xe = XElement.Load(GetXMLFileFullName(name));
            return xe;
        }

        /// <summary>
        /// 保存xml对象至文件
        /// </summary>
        /// <param name="xe">要保存的内容</param>
        /// <param name="xmlName">保存的文件名（不含扩展名）</param>
        /// <param name="isFullXmlName">True=程序内配置路径地址，只含文件名（不含扩展名）====False=文件绝对地址</param>
        public void SaveXml(XElement xe, string xmlName, bool isFullXmlName)
        {
            if (isFullXmlName)
            {
                CreatNewDirTry();
                xe.Save(GetXMLFileFullName(xmlName));
            }
            else
            {               
                xe.Save(GetXMLFileFullName(xmlName));
            }
        }

        /// <summary>
        /// 转换文件全名（绝对地址、含扩展名）
        /// </summary>
        /// <param name="name">文件名（不含目录、扩展名）</param>
        /// <returns></returns>
        private string GetXMLFileFullName(string name)
        {
            return Path.Combine(AppDir, ConfigDir, name+".xml");
        }

        /// <summary>
        /// 新建程序配置文件夹，如果已存在则不创建
        /// </summary>
        private void CreatNewDirTry()
        {
            if (!Directory.Exists(ConfigDir))
            {
                Directory.CreateDirectory(ConfigDir);
            }
        }
    }
}
