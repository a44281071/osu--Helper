using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace osu_Helper
{
    class XML_Config
    {
        public XML_Config()
        {
            this.FileName = "Config";
        }
        /// <summary>
        /// 配置文件名
        /// </summary>   
        public string FileName { get; set; }

        File_Helper fHelper = new File_Helper();

        /// <summary>
        /// 用XML文件初始化Config对象
        /// </summary>
        /// <param name="xe">xml</param>
        /// <returns>Config对象</returns>
        private ConfigModel ReadTo_ConfigModel(XElement xe)
        {
            ConfigModel config = new ConfigModel();
            config.WebRankListUrl = xe.Element("WebRankListUrl").Value;
            config.CategoryListXPath = xe.Element("CategoryListXPath").Value;
            config.IdXPath = xe.Element("IdXPath").Value;
            config.TitleXPath = xe.Element("TitleXPath").Value;
            config.ArtistXPath = xe.Element("ArtistXPath").Value;
            config.MapperXPath = xe.Element("MapperXPath").Value;
            config.StylesXPath = xe.Element("StylesXPath").Value;
            config.LanguageXPath = xe.Element("LanguageXPath").Value;
            config.SbXPath = xe.Element("SbXPath").Value;
            config.PictureUrl = xe.Element("PictureUrl").Value;
            config.SongTasteUrl = xe.Element("SongTasteUrl").Value;
            config.DownUrl_osz_so = xe.Element("DownUrl_osz_so").Value;
            return config;
        }

        /// <summary>
        /// 重置Config.xml文件
        /// </summary>
        /// <returns>重置结果</returns>
        public XElement ResetXML()
        {
            XElement xe = new XElement("MapRankList_XPath",
                new XElement("WebRankListUrl", "http://osu.ppy.sh/p/beatmaplist"),
                new XElement("CategoryListXPath", "//html[1]/body[1]/div[1] /div[1]/div[1]/div[4]/div[3]/div[4]/div"),
                new XElement("IdXPath", "."),
                new XElement("TitleXPath", "./div[3]/a[1]"),
                new XElement("ArtistXPath", "./div[3]/span[1]"),
                new XElement("MapperXPath", "./div[4]/div[1]/a[1]"),
                new XElement("StylesXPath", "./div[5]/div[1]/a[1]"),
                new XElement("LanguageXPath", "./div[5]/div[1]/a[2]"),
                new XElement("SbXPath", "./div[3]/i[1]"),
                new XElement("PictureUrl", "http://s.ppy.sh/mt/"),
                new XElement("SongTasteUrl", "http://s.ppy.sh/mp3/preview/")
                );

            fHelper.SaveXml(xe, FileName, true);
            return xe;
        }

        /// <summary>
        /// 加载Config.xml设置文件至对象
        /// </summary>
        /// <returns>设置内容对象</returns>
        public ConfigModel XmlToXPathModelTry()
        {
            XElement xfile = fHelper.LoadXml(FileName);
            ConfigModel config = ReadTo_ConfigModel(xfile);
            return config;
        }
    }
}
