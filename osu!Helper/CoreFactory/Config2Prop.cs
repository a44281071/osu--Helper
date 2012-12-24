using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace osu_Helper
{
    public class Config2Do
    {
        public string ToBeatMapDownUrl(ConfigModel cfg, string id)
        {
            return Path.Combine(cfg.DownUrl_osz_so, id);
        }
    }
}
