using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using osu_Helper;
using osu_Helper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace osu_Helper.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            #region 系列初始化
            this.Set_MainConfig();  //初始化Config配置
            this.Set_StaticInfo();  //初始化不会改变的窗体信息。
            this.Set_DownLoader();  //初始化下载器           
            #endregion

            #region Command 绑定
            ComGetNewRankedBeatMapListByWeb = new RelayCommand(() => C_GetNewRankedBeatMapListByWeb(), () => true);
            ComDownLoaderAddFile = new RelayCommand(() => C_DownLoader_AddFile(), () => true);
            ComDownLoaderAddFile = new RelayCommand(() => C_DownLoader_AddFile(), () => true);
            ComDownLoaderPause = new RelayCommand(() => C_DownLoader_Pause(), () => true);
            ComDownLoaderStart = new RelayCommand(() => C_DownLoader_Start(), () => true);
            ComPlaySongTaste = new RelayCommand<BeatMapViewModel>(p => C_PlaySongTaste(p), p => true);
            ComDownLoaderAddBeatMap = new RelayCommand<BeatMapViewModel>(p => C_AddBeatMapToDown(p), p => true);
            #endregion
        }

        #region Command 注册
        public ICommand ComGetNewRankedBeatMapListByWeb { get; private set; }  //获取Web上最新Ranked的BeatMap列表
        public ICommand ComPlaySongTaste { get; private set; }  //播放歌曲试听
        public ICommand ComDownLoaderAddBeatMap { get; private set; }  //下载器，添加BeatMap任务。
        public ICommand ComDownLoaderAddFile { get; private set; }  //下载器，手动添加任务
        public ICommand ComDownLoaderStart { get; private set; }  //下载器，开始下载
        public ICommand ComDownLoaderPause { get; private set; }  //下载器，暂停下载
        #region 主动实现式ICommand示例（未使用）
        private RelayCommand<BeatMapViewModel> _comShowInfo;
        /// <summary>
        /// Gets the MyCommand.
        /// </summary>
        public RelayCommand<BeatMapViewModel> ComShowInfo
        {
            get
            {
                return _comShowInfo
                    ?? (_comShowInfo = new RelayCommand<BeatMapViewModel>(p =>
                    {
                        //DoSomething(p);
                    }));
            }
        }

        #endregion

        #endregion

        #region 字段_DownLoader
        /// <summary>
        /// 计时器
        /// </summary>
        DispatcherTimer timer;
        /// <summary>
        /// 多线程同步的字段
        /// </summary>
        static object syncObject = new object();
        /// <summary>
        /// 统计速度的间隔
        /// </summary>
        int interval = 1000;
        List<string> downList = new List<string>();
        #endregion
        #region 字段 实例化方法

        XML_Config zXml_Config = new XML_Config();
        XML_BeatMap zXml_BeatMap = new XML_BeatMap();
        Xpath2Model zXPath2Model = new Xpath2Model();
        MediaPlayer zMediaPlayer = new MediaPlayer();

        #endregion

        #region 属性_主窗体
        private Informations _Information;
        /// <summary>
        ///  页面中绑定的静态元素
        /// </summary>
        public Informations Information
        {
            get { return _Information; }
            set
            {
                _Information = value;
                this.RaisePropertyChanged("Information");
            }
        }

        private ConfigModel _configMain;
        /// <summary>
        /// 主程序的配置信息
        /// </summary>
        public ConfigModel ConfigMain
        {
            get { return _configMain; }
            set { _configMain = value; }
        }

        private double _prograssBar_Value;
        /// <summary>
        /// 主界面UI—PrograssBar_Show
        /// </summary>
        public double PrograssBar_Value
        {
            get { return _prograssBar_Value; }
            set
            {
                _prograssBar_Value = value;
                this.RaisePropertyChanged("PrograssBar_Value");
            }
        }

        private string _configDir;
        /// <summary>
        /// 程序配置文件所在目录（绝对地址）
        /// </summary>
        public string ConfigDir
        {
            get { return _configDir; }
            set { _configDir = value; }
        }

        #endregion  //属性_主窗体
        #region 属性_DownLoader
        /// <summary>
        /// 下载文件列表
        /// </summary>
        public ObservableCollection<DownloadItemViewModel> DownloadFileList { get; private set; }
        private string _downloadUrl;
        /// <summary>
        /// 要添加的下载路径
        /// </summary>
        public string DownloadUrl
        {
            get { return _downloadUrl; }
            set
            {
                _downloadUrl = value;
                this.RaisePropertyChanged("DownloadUrl");
            }
        }
        private double _speed;
        /// <summary>
        /// 总的下载速度
        /// </summary>
        public double Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                this.RaisePropertyChanged("Speed");
            }
        }

        #endregion  //属性_DownLoader
        #region 属性_BeatMapNewRanked

        private string _NowPlayingId;
        /// <summary>
        /// 正在播放的歌曲的 Id
        /// </summary>
        public string NowPlayingId
        {
            get { return _NowPlayingId; }
            set { _NowPlayingId = value; }
        }
        private BeatMapViewModel _selectedBeatMap;
        /// <summary>
        /// 列表中选定的Item内含的对象
        /// </summary>
        public BeatMapViewModel SelectedBeatMap
        {
            get { return _selectedBeatMap; }
            set
            {
                _selectedBeatMap = value;
                this.RaisePropertyChanged("SelectedBeatMap");
            }
        }

        private List<BeatMapViewModel> _beatMapsNewRankList;
        /// <summary>
        /// 列表显示BeatMaps列表，从官方网站的列表页面解析HTML页面获取最新Rank的BeatMap
        /// </summary>
        public List<BeatMapViewModel> BeatMapsNewRankList
        {
            get { return _beatMapsNewRankList; }
            set
            {
                _beatMapsNewRankList = value;
                this.RaisePropertyChanged("BeatMapsNewRankList");
            }
        }

        private int _selectedCount;
        /// <summary>
        /// 统计选中的歌曲列表项的数量
        /// </summary>
        public int SelectedCount
        {
            get { return _selectedCount; }
            set
            {
                _selectedCount = value;
                this.RaisePropertyChanged("SelectedCount");
            }
        }
        #endregion // 属性_BeatMapNewRanked

        #region Command 绑定的方法

        private void C_GetNewRankedBeatMapListByWeb()
        {
            Task tGet = new Task(Action_GetNewRankedBeatMapListByWeb);
            tGet.Start();
        }
        private void C_PlaySongTaste(BeatMapViewModel p)
        {
            string id = p.Beat_Map.Id.ToString();  //获取要播放的BeatMap的Id
            if (this.NowPlayingId == id)
            {
                this.SongTaste_PlayOrPause();
            }
            else
            {
                this.NowPlayingId = id;
                this.SongTaste_Play(id);
            }
        }
        private void C_AddBeatMapToDown(BeatMapViewModel p)
        {
            string bmDnUrl = p.Beat_Map.DownUrl_osz_so.ToString();  //获取要播放的BeatMap的地址
            bool isInDownList = downList.Contains(bmDnUrl);

            if (isInDownList)
            {
                MessageBoxResult result = MessageBox.Show("发现此对象曾经添加至下载列表，是否重新添加？", "重复下载提示", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.No)
                    return;
            }
            downList.Add(bmDnUrl);
            var file = new DownloadItemViewModel(bmDnUrl);
            DownloadFileList.Add(file);
        }

        void C_DownLoader_AddFile()
        {
            if (!string.IsNullOrEmpty(DownloadUrl))
            {
                var file = new DownloadItemViewModel(DownloadUrl);
                DownloadFileList.Add(file);
                DownloadUrl = null;
            }
        }
        void C_DownLoader_Pause()
        {
            DownloadFileList.AsParallel().ForAll(t =>
            {
                t.PauseDownload();
            });
        }
        void C_DownLoader_Start()
        {
            DownloadFileList.AsParallel().ForAll(t =>
            {
                t.StartDownload();
            });
        }

        #endregion  //Command 绑定的方法

        #region 普通方法
        /// <summary>
        /// 播放歌曲试听-加载并播放
        /// </summary>
        /// <param name="id">歌曲的id</param>
        private void SongTaste_Play(string id)
        {
            zMediaPlayer.Dispose();
            zMediaPlayer.Open(string.Format("{0}{1}{2}", "http://s.ppy.sh/mp3/preview/", id, ".mp3"));  //加载歌曲
            zMediaPlayer.Play();  //播放歌曲
        }

        /// <summary>
        /// 如果暂停，则播放。如果播放，则暂停。
        /// </summary>
        private void SongTaste_PlayOrPause()
        {
            zMediaPlayer.PlayOrPause();
        }

        /// <summary>
        /// 初始化 构造器 下载器。
        /// </summary>
        private void Set_DownLoader()
        {
            timer = new DispatcherTimer();
            //每秒统计一次速度
            timer.Interval = TimeSpan.FromMilliseconds(interval);
            timer.Tick += DownLoader_timer_Tick;
            timer.Start();

            DownloadFileList = new ObservableCollection<DownloadItemViewModel>();
            DownloadUrl = @"http://xyq.gdl.netease.com/MHXY-JD-2.0.153.exe";
        }

        /// <summary>
        /// 初始化 构造器 页面中绑定的静态元素
        /// </summary>
        private void Set_StaticInfo()
        {
            this.Information = new Informations();
            Information.Main_Info = "OsuHelper (v0.1) by_SanTai.Zhou";
            Information.Main_Title = "OsuHelper";
        }

        /// <summary>
        /// 初始化 构造器 主程序的配置信息
        /// </summary>
        private void Set_MainConfig()
        {
            this.ConfigDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "zhZher");
            this.ConfigMain = zXml_Config.XmlToXPathModelTry();
        }

        /// <summary>
        /// 从官网上获取最新Ranked列表，生成BeatMap，并传递给ViewModel下的对象。
        /// </summary>
        private void Action_GetNewRankedBeatMapListByWeb()
        {
            this.PrograssBar_Value = 30;
            var beatMaps = zXPath2Model.GetBeatMaps(ConfigMain);  //从官网获取最新Ranked列表
            this.PrograssBar_Value = 90;
            List<BeatMapViewModel> temps = new List<BeatMapViewModel>();
            foreach (var beatmap in beatMaps)
            {
                BeatMapViewModel item = new BeatMapViewModel();
                item.Beat_Map = beatmap;
                temps.Add(item);
            }
            BeatMapsNewRankList = new List<BeatMapViewModel>();
            BeatMapsNewRankList = temps;
            this.PrograssBar_Value = 100;
        }

        /// <summary>
        /// 获取，选中的列表项。获取属性为“已选中”的对象的Id，生成列表。
        /// </summary>
        /// <returns>选中的BeatMap的Id的集合</returns>
        private List<int> Get_Selected_BeatMapsNewRankListItems()
        {
            List<int> Selected_BeatMapsNewRankListItems = this.BeatMapsNewRankList.Where(i => i.IsCheckBoxSelected == true).Select(i => i.Beat_Map.Id).ToList();
            return Selected_BeatMapsNewRankListItems;
        }

        /// <summary>
        /// 下载器-即时统计器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DownLoader_timer_Tick(object sender, EventArgs e)
        {
            double speed = 0;
            DownloadFileList.AsParallel().ForAll(t =>
            {
                t.InitSpeed(interval);
                speed += t.Speed;
            });
            this.Speed = speed;
        }
        #endregion
    }

}