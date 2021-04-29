using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Telegram_bot_Wpf.View
{
    /// <summary>
    /// Логика взаимодействия для ViewVideo.xaml
    /// </summary>
    public partial class ViewVideo : Window
    {
        ObservableCollection<Files> listFiles { get; set; }
        public ViewVideo(string userPath)
        {
            InitializeComponent();
            listFiles = new ObservableCollection<Files>();
            DirectoryInfo di = new DirectoryInfo(userPath);
            foreach (var fi in di.GetFiles())
            {
                string[] words = fi.Name.Split('_');
                switch (words[0])
                {
                    case "video":
                        listFiles.Add(new Files(fi.Name, "video", fi.Length, fi.LastWriteTime));
                        break;
                    default:
                        break;
                }
            }
            WPFDataGrid.ItemsSource = listFiles;
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
