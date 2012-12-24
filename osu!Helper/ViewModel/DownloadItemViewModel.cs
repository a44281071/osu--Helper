using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace osu_Helper.ViewModel
{
    public class DownloadItemViewModel : ViewModelBase
    {
        #region 构造器
        public DownloadItemViewModel(string downloadUrl)
        {
            this.DownloadUrl = downloadUrl;
            client = new WebClient();
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;
            string fileFullName = FileNameFromServer();
            string downDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            this.FileName = Path.Combine(downDir, fileFullName);
            StartDownload();
        }

        void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            this.IsDownloading = false;
            this.Speed = 0;
            if (!e.Cancelled)
            {
                this.IsCompleted = true;
            }
        }
        #endregion

        #region 字段
        /// <summary>
        /// 用户多线程同步的对象
        /// </summary>
        static object syncObject = new object();
        /// <summary>
        /// 用于计算速度的临时变量
        /// </summary>
        private long downloadedBytes = 0;
        /// <summary>
        /// 用户下载的通信对象
        /// </summary>
        readonly WebClient client;
        /// <summary>
        /// 每次读取的字节数
        /// </summary>
        int readBytes = 100 * 1024;
        #endregion

        #region 属性
        private string _downloadUrl;
        /// <summary>
        /// 下载路径
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
        private long _downloadBytes;
        /// <summary>
        /// 已下载的字节数
        /// </summary>
        public long DownloadBytes
        {
            get { return _downloadBytes; }
            set
            {
                _downloadBytes = value;
                this.RaisePropertyChanged("DownloadBytes");
            }
        }
        private long _totalBytes;
        /// <summary>
        /// 要下载的字节总数(文件大小)
        /// </summary>
        public long TotalBytes
        {
            get { return _totalBytes; }
            set
            {
                _totalBytes = value;
                this.RaisePropertyChanged("TotalBytes");
            }
        }
        private bool _isDownloading;
        /// <summary>
        /// 是否正在下载
        /// </summary>
        public bool IsDownloading
        {
            get { return _isDownloading; }
            set
            {
                _isDownloading = value;
                this.RaisePropertyChanged("IsDownloading");
            }
        }

        /// <summary>
        /// 当前进度
        /// </summary>
        public double Progress
        {
            get { return DownloadBytes * 100.0 / TotalBytes; }
        }

        private double _speed;
        /// <summary>
        /// 即时速度
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

        private bool _isCompleted;
        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                _isCompleted = value;
                this.RaisePropertyChanged("IsCompleted");
            }
        }

        private string _fileName;
        /// <summary>
        /// 保存在本地的文件名称
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                this.RaisePropertyChanged("FileName");
            }
        }

        #endregion

        #region 普通方法
        /// <summary>
        /// 下载进度变化时触发的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.TotalBytes = e.TotalBytesToReceive;
            this.DownloadBytes = e.BytesReceived;
        }
        /// <summary>
        /// 计算速度
        /// </summary>
        /// <param name="milliseconds"></param>
        public void InitSpeed(int milliseconds)
        {
            if (IsCompleted) return;
            if (!IsDownloading) return;
            if (milliseconds <= 0) return;
            lock (syncObject)
            {
                var haveDownloaded = this.DownloadBytes - downloadedBytes;
                this.Speed = (haveDownloaded * 1000.0) / milliseconds;
                downloadedBytes = this.DownloadBytes;
            }
        }


        public string FileNameFromServer()
        {
            // Creates an HttpWebRequest for the specified URL. 
            HttpWebRequest reqst = (HttpWebRequest)WebRequest.Create(this.DownloadUrl);
            // Sends the HttpWebRequest and waits for response.
            HttpWebResponse resps = (HttpWebResponse)reqst.GetResponse();

            string tempS = "Content-Disposition";
            //获取指定的标头对应的值。        
            string[] tempHeaderValues = resps.Headers.GetValues(tempS);
            string[] valuesCut = tempHeaderValues[0].Split('\"');

            string fileFullName = null;
            if (tempHeaderValues == null)  //未获取到含下载文件的标头，自定义文件名为系统时间
            {
                System.DateTime temTime = System.DateTime.Now;
                fileFullName = string.Format("{0}{1}{2}{3}{4}", temTime.DayOfYear, temTime.Hour, temTime.Minute, temTime.Second, ".file");
                return fileFullName;
            }
            fileFullName = valuesCut[1];  //获取到标头，截取内容获取文件名。

            resps.Close();
            return fileFullName;
        }
        /// <summary>
        /// 开始下载
        /// </summary>
        public void StartDownload()
        {
            IsDownloading = true;
            HttpWebRequest request = (HttpWebRequest)FileWebRequest.Create(this.DownloadUrl);
            if (DownloadBytes > 0)
            {
                request.AddRange(DownloadBytes);
            }
            request.BeginGetResponse(ar =>
            {
                var response = request.EndGetResponse(ar);
                if (this.TotalBytes == 0) this.TotalBytes = response.ContentLength;
                using (var writer = new FileStream(this.FileName, FileMode.OpenOrCreate))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        while (IsDownloading)
                        {
                            byte[] data = new byte[readBytes];
                            int readNumber = stream.Read(data, 0, data.Length);
                            if (readNumber > 0)
                            {
                                writer.Write(data, 0, readNumber);
                                this.DownloadBytes += readNumber;

                            }
                            if (this.DownloadBytes == this.TotalBytes)
                            {
                                Complete();
                            }
                        }
                    }
                }
            }, null);
        }
        public void Complete()
        {
            this.IsCompleted = true;
            this.IsDownloading = false;
            this.Speed = 0;
        }
        public void PauseDownload()
        {
            IsDownloading = false;
            this.Speed = 0;
        }
        public void DeleteFile()
        {

        }
        #endregion
    }
}
