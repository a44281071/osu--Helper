using osu_Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace osu_Helper
{
    class XML_BeatMap
    {
        public XML_BeatMap()
        {
            this.FileName = "BeatMapList";
        }
        public string FileName { get; set; }

        File_Helper xmlHelper = new File_Helper();

        /// <summary>
        /// 读取一个xml文件，并获得BeatMap列表。
        /// </summary>
        /// <returns></returns>
        public List<BeatMapViewModel> ReadXmlToList(string path)
        {
            List<BeatMapViewModel> bms = new List<BeatMapViewModel>();
            XElement xDoc = xmlHelper.LoadXml(path);
            var beatMaps = xDoc.Descendants("BeatMap");
            foreach (var d in beatMaps)
            {
                BeatMapViewModel bmvmTemp = new BeatMapViewModel();
                bmvmTemp.Beat_Map.Id = Int32.Parse(d.Element("Id").Value);  //此处强制转换可能发生异常
                bmvmTemp.Beat_Map.Title = d.Element("Title").Value;
                bmvmTemp.Beat_Map.Artist = d.Element("Artist").Value;
                bmvmTemp.Beat_Map.Mapper = d.Element("Mapper").Value;
                bmvmTemp.Beat_Map.Styles = d.Element("Styles").Value;
                bmvmTemp.Beat_Map.Language = d.Element("Language").Value;
                bmvmTemp.Beat_Map.Sb = d.Element("Sb").Value;
            }
            return bms;
        }

        /// <summary>
        /// 保存BeatMap列表到xml文件
        /// </summary>
        /// <param name="maps">BeatMap列表</param>
        /// <param name="path">保存文件路径</param>
        public void SaveListToXml(ObservableCollection<BeatMapViewModel> maps, string path)
        {
            XElement xe = new XElement("SongsList", new XAttribute("生成时间", DateTime.Now));
            foreach (BeatMapViewModel map in maps)
            {
                XElement xe3 = new XElement("BeatMap", new XAttribute("Id", map.Beat_Map.Id.ToString()),
                    new XElement("Title", map.Beat_Map.Title),
                    new XElement("Artist", map.Beat_Map.Artist),
                    new XElement("Mapper", map.Beat_Map.Mapper),
                    new XElement("Styles", map.Beat_Map.Styles),
                    new XElement("Language", map.Beat_Map.Language),
                    new XElement("Sb", map.Beat_Map.Sb)
                    );

                xe.Add(xe3);
            }
            xmlHelper.SaveXml(xe, path, true);
        }
    }
}
