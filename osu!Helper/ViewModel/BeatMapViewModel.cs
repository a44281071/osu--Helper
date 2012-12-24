using GalaSoft.MvvmLight;
using osu_Helper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace osu_Helper.ViewModel
{
    public class BeatMapViewModel : ViewModelBase
    {

        #region 属性
        /// <summary>
        /// “有一个”设计方式的BeatMap对象
        /// </summary>
        public BeatMap Beat_Map { get; set; }
        
        #endregion

        #region 附加属性

        private bool _isCheckBoxSelected;
        /// <summary>
        /// 该对象是否被选中
        /// </summary>
        public bool IsCheckBoxSelected
        {
            get { return _isCheckBoxSelected; }
            set
            {
                _isCheckBoxSelected = value;
                this.RaisePropertyChanged("IsCheckBoxSelected");
            }
        }

        private bool? _isTasting;
        /// <summary>
        /// 该对象的试听状态（null=未激活，true=试听中，false=暂停中
        /// </summary>
        public bool? IsTasting
        {
            get { return _isTasting; }
            set
            {
                _isTasting = value;
                this.RaisePropertyChanged("IsTasting");
            }
        }

        #endregion

        #region 普通方法
        private string ShowThisInfo()
        {
            return Beat_Map.Id.ToString();
        }
        #endregion
    }
}
