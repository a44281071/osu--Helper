using osu_Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace osu_Helper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TranButton_Click_1(object sender, RoutedEventArgs e)
        {
            osu_Helper.ViewModel.BeatMapViewModel bm = this.ListBox_Show.SelectedItem as osu_Helper.ViewModel.BeatMapViewModel;
            
            MessageBox.Show(bm.Beat_Map.Id.ToString());
        }

        private void button_ShowList_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
