using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osu_Helper.Model
{
    public class BeatMap
    {
        #region 属性
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 艺术家
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// 作图者
        /// </summary>
        public string Mapper { get; set; }
        /// <summary>
        /// 曲风
        /// </summary>
        public string Styles { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 附加特效（如StoryBoard、video……）
        /// </summary>
        public string Sb { get; set; }

        #endregion

        #region BeatMap对象的附加属性

        /// <summary>
        /// BeatMap全名。格式：（Id Title - Artist）
        /// </summary>
        public string FullName
        {
            get { return string.Format(" {0}  {1} - {2} ", this.Id, this.Title, this.Artist); }
        }

        /// <summary>
        /// 歌曲试听地址
        /// </summary>
        public Uri TasteUrl
        {
            get { return new Uri("http://s.ppy.sh/mp3/preview/" + this.Id.ToString() + ".mp3"); }
        }
        /// <summary>
        /// 歌曲预览图地址（小）
        /// http://s.ppy.sh/mt/00000
        /// </summary>
        public Uri ShowPicUrl
        {
            get { return new Uri("http://s.ppy.sh/mt/" + this.Id.ToString()); }
        }
        /// <summary>
        /// 歌曲下载地址（分流站：http://osz.so）
        /// http://osz.so/00000
        /// </summary>
        public Uri DownUrl_osz_so
        {
            get { return new Uri("http://osz.so/" + this.Id.ToString()); }
        }
        /// <summary>
        /// 歌曲下载地址（分流站：http://bloodcat.com）
        /// http://bloodcat.com/osu/d/00000
        /// </summary>
        public Uri DownUrl_bloodcat
        {
            get { return new Uri("http://bloodcat.com/osu/d/" + this.Id.ToString()); }
        }
        /// <summary>
        /// 歌曲下载地址（分流站：（http://osu.uu.gl）
        /// http://osu.uu.gl/pid/00000
        /// </summary>
        public Uri DownUrl_osu_uu_gl
        {
            get { return new Uri("http://osu.uu.gl/pid/" + this.Id.ToString()); }
        }

        #endregion
    }
}
