using HtmlAgilityPack;
using osu_Helper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace osu_Helper
{
    class Xpath2Model
    {
        /// <summary>
        /// 获取osu官网最新Rank的歌曲列表
        /// </summary>
        /// <param name="config">程序配置信息</param>
        /// <returns>歌曲列表</returns>
        public List<BeatMap> GetBeatMaps(ConfigModel config)
        {
            List<BeatMap> beatMaps = GetBeatMapsListByHAP(config);
            return beatMaps;
        }

        #region 通过HAP插件解析网页内容，获取Ranked歌曲列表
        /// <summary>
        /// 通过HAP插件解析网页内容，获取Ranked歌曲列表
        /// </summary>
        /// <param name="html">网页的文本内容</param>
        /// <returns>BeatMap列表</returns>
        private List<BeatMap> GetBeatMapsListByHAP(ConfigModel xpModel)
        {
            List<BeatMap> beatMaps = new List<BeatMap>();
            beatMaps.Clear();
            HtmlDocument hDoc = new HtmlWeb().Load(xpModel.WebRankListUrl);  //加载网页，实例化对象。

            HtmlNode rootNode = hDoc.DocumentNode;
            HtmlNodeCollection categoryNodeList = rootNode.SelectNodes(xpModel.CategoryListXPath); //定位HTML标签至遍历处，获取内容（包含BeatMapList列表集合）
            HtmlNode temp = null;
            BeatMap beatMap = null;

            foreach (HtmlNode categoryNode in categoryNodeList)
            {
                temp = HtmlNode.CreateNode(categoryNode.OuterHtml);  //获取一个包含BeatMapList的内容。
                //读出值：
                beatMap = new BeatMap();
                beatMap.Id = temp.SelectSingleNode(xpModel.IdXPath).GetAttributeValue("Id", 0000);
                beatMap.Title = temp.SelectSingleNode(xpModel.TitleXPath).InnerText;
                beatMap.Artist = temp.SelectSingleNode(xpModel.ArtistXPath).InnerText;
                beatMap.Mapper = temp.SelectSingleNode(xpModel.MapperXPath).InnerText;
                beatMap.Styles = temp.SelectSingleNode(xpModel.StylesXPath).InnerText;
                beatMap.Language = temp.SelectSingleNode(xpModel.LanguageXPath).InnerText;
                if (temp.SelectSingleNode(xpModel.SbXPath) != null)
                {
                    beatMap.Sb = temp.SelectSingleNode(xpModel.SbXPath).GetAttributeValue("class", "NoSb");
                }
                else
                {
                    beatMap.Sb = "NoSb";
                }
                beatMaps.Add(beatMap);
            }

            return beatMaps;
        }
        #endregion

        #region 获取网页的内容（未使用）
        /// <summary>
        /// 获取网页的内容
        /// </summary>
        /// <param name="url">网页地址</param>
        private string GetHtmlPageText(string url)
        {
            WebRequest rGet = WebRequest.Create(url);
            WebResponse rSet = rGet.GetResponse();
            Stream s = rSet.GetResponseStream();
            StreamReader sr = new StreamReader(s, Encoding.GetEncoding("GB2312"));
            StringBuilder sb = new StringBuilder();
            String Str;

            while ((Str = sr.ReadLine()) != null)
            {
                //sb.Append(Str + "\n");
                sb.AppendLine(Str);
            }

            sr.Close();
            s.Close();
            rSet.Close();

            return sb.ToString();
        }

        #endregion
    }
}
