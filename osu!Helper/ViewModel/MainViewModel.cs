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
            #region ϵ�г�ʼ��
            this.Set_MainConfig();  //��ʼ��Config����
            this.Set_StaticInfo();  //��ʼ������ı�Ĵ�����Ϣ��
            this.Set_DownLoader();  //��ʼ��������           
            #endregion

            #region Command ��
            ComGetNewRankedBeatMapListByWeb = new RelayCommand(() => C_GetNewRankedBeatMapListByWeb(), () => true);
            ComDownLoaderAddFile = new RelayCommand(() => C_DownLoader_AddFile(), () => true);
            ComDownLoaderAddFile = new RelayCommand(() => C_DownLoader_AddFile(), () => true);
            ComDownLoaderPause = new RelayCommand(() => C_DownLoader_Pause(), () => true);
            ComDownLoaderStart = new RelayCommand(() => C_DownLoader_Start(), () => true);
            ComPlaySongTaste = new RelayCommand<BeatMapViewModel>(p => C_PlaySongTaste(p), p => true);
            ComDownLoaderAddBeatMap = new RelayCommand<BeatMapViewModel>(p => C_AddBeatMapToDown(p), p => true);
            #endregion
        }

        #region Command ע��
        public ICommand ComGetNewRankedBeatMapListByWeb { get; private set; }  //��ȡWeb������Ranked��BeatMap�б�
        public ICommand ComPlaySongTaste { get; private set; }  //���Ÿ�������
        public ICommand ComDownLoaderAddBeatMap { get; private set; }  //�����������BeatMap����
        public ICommand ComDownLoaderAddFile { get; private set; }  //���������ֶ��������
        public ICommand ComDownLoaderStart { get; private set; }  //����������ʼ����
        public ICommand ComDownLoaderPause { get; private set; }  //����������ͣ����
        #region ����ʵ��ʽICommandʾ����δʹ�ã�
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

        #region �ֶ�_DownLoader
        /// <summary>
        /// ��ʱ��
        /// </summary>
        DispatcherTimer timer;
        /// <summary>
        /// ���߳�ͬ�����ֶ�
        /// </summary>
        static object syncObject = new object();
        /// <summary>
        /// ͳ���ٶȵļ��
        /// </summary>
        int interval = 1000;
        List<string> downList = new List<string>();
        #endregion
        #region �ֶ� ʵ��������

        XML_Config zXml_Config = new XML_Config();
        XML_BeatMap zXml_BeatMap = new XML_BeatMap();
        Xpath2Model zXPath2Model = new Xpath2Model();
        MediaPlayer zMediaPlayer = new MediaPlayer();

        #endregion

        #region ����_������
        private Informations _Information;
        /// <summary>
        ///  ҳ���а󶨵ľ�̬Ԫ��
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
        /// �������������Ϣ
        /// </summary>
        public ConfigModel ConfigMain
        {
            get { return _configMain; }
            set { _configMain = value; }
        }

        private double _prograssBar_Value;
        /// <summary>
        /// ������UI��PrograssBar_Show
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
        /// ���������ļ�����Ŀ¼�����Ե�ַ��
        /// </summary>
        public string ConfigDir
        {
            get { return _configDir; }
            set { _configDir = value; }
        }

        #endregion  //����_������
        #region ����_DownLoader
        /// <summary>
        /// �����ļ��б�
        /// </summary>
        public ObservableCollection<DownloadItemViewModel> DownloadFileList { get; private set; }
        private string _downloadUrl;
        /// <summary>
        /// Ҫ��ӵ�����·��
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
        /// �ܵ������ٶ�
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

        #endregion  //����_DownLoader
        #region ����_BeatMapNewRanked

        private string _NowPlayingId;
        /// <summary>
        /// ���ڲ��ŵĸ����� Id
        /// </summary>
        public string NowPlayingId
        {
            get { return _NowPlayingId; }
            set { _NowPlayingId = value; }
        }
        private BeatMapViewModel _selectedBeatMap;
        /// <summary>
        /// �б���ѡ����Item�ں��Ķ���
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
        /// �б���ʾBeatMaps�б��ӹٷ���վ���б�ҳ�����HTMLҳ���ȡ����Rank��BeatMap
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
        /// ͳ��ѡ�еĸ����б��������
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
        #endregion // ����_BeatMapNewRanked

        #region Command �󶨵ķ���

        private void C_GetNewRankedBeatMapListByWeb()
        {
            Task tGet = new Task(Action_GetNewRankedBeatMapListByWeb);
            tGet.Start();
        }
        private void C_PlaySongTaste(BeatMapViewModel p)
        {
            string id = p.Beat_Map.Id.ToString();  //��ȡҪ���ŵ�BeatMap��Id
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
            string bmDnUrl = p.Beat_Map.DownUrl_osz_so.ToString();  //��ȡҪ���ŵ�BeatMap�ĵ�ַ
            bool isInDownList = downList.Contains(bmDnUrl);

            if (isInDownList)
            {
                MessageBoxResult result = MessageBox.Show("���ִ˶�����������������б��Ƿ�������ӣ�", "�ظ�������ʾ", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
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

        #endregion  //Command �󶨵ķ���

        #region ��ͨ����
        /// <summary>
        /// ���Ÿ�������-���ز�����
        /// </summary>
        /// <param name="id">������id</param>
        private void SongTaste_Play(string id)
        {
            zMediaPlayer.Dispose();
            zMediaPlayer.Open(string.Format("{0}{1}{2}", "http://s.ppy.sh/mp3/preview/", id, ".mp3"));  //���ظ���
            zMediaPlayer.Play();  //���Ÿ���
        }

        /// <summary>
        /// �����ͣ���򲥷š�������ţ�����ͣ��
        /// </summary>
        private void SongTaste_PlayOrPause()
        {
            zMediaPlayer.PlayOrPause();
        }

        /// <summary>
        /// ��ʼ�� ������ ��������
        /// </summary>
        private void Set_DownLoader()
        {
            timer = new DispatcherTimer();
            //ÿ��ͳ��һ���ٶ�
            timer.Interval = TimeSpan.FromMilliseconds(interval);
            timer.Tick += DownLoader_timer_Tick;
            timer.Start();

            DownloadFileList = new ObservableCollection<DownloadItemViewModel>();
            DownloadUrl = @"http://xyq.gdl.netease.com/MHXY-JD-2.0.153.exe";
        }

        /// <summary>
        /// ��ʼ�� ������ ҳ���а󶨵ľ�̬Ԫ��
        /// </summary>
        private void Set_StaticInfo()
        {
            this.Information = new Informations();
            Information.Main_Info = "OsuHelper (v0.1) by_SanTai.Zhou";
            Information.Main_Title = "OsuHelper";
        }

        /// <summary>
        /// ��ʼ�� ������ �������������Ϣ
        /// </summary>
        private void Set_MainConfig()
        {
            this.ConfigDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "zhZher");
            this.ConfigMain = zXml_Config.XmlToXPathModelTry();
        }

        /// <summary>
        /// �ӹ����ϻ�ȡ����Ranked�б�����BeatMap�������ݸ�ViewModel�µĶ���
        /// </summary>
        private void Action_GetNewRankedBeatMapListByWeb()
        {
            this.PrograssBar_Value = 30;
            var beatMaps = zXPath2Model.GetBeatMaps(ConfigMain);  //�ӹ�����ȡ����Ranked�б�
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
        /// ��ȡ��ѡ�е��б����ȡ����Ϊ����ѡ�С��Ķ����Id�������б�
        /// </summary>
        /// <returns>ѡ�е�BeatMap��Id�ļ���</returns>
        private List<int> Get_Selected_BeatMapsNewRankListItems()
        {
            List<int> Selected_BeatMapsNewRankListItems = this.BeatMapsNewRankList.Where(i => i.IsCheckBoxSelected == true).Select(i => i.Beat_Map.Id).ToList();
            return Selected_BeatMapsNewRankListItems;
        }

        /// <summary>
        /// ������-��ʱͳ����
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