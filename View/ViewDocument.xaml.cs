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
using Telegram_bot_Wpf;

namespace Telegram_bot_Wpf.View
{
    /// <summary>
    /// Логика взаимодействия для ViewDocument.xaml
    /// </summary>
    public partial class ViewDocument : Window
    {
        ObservableCollection<Files> listFiles { get; set; }
        public ViewDocument()
        {
            InitializeComponent();
        }
        public ViewDocument(string userPath)
        {
            InitializeComponent();
            listFiles = new ObservableCollection<Files>();
            DirectoryInfo di = new DirectoryInfo(userPath);
            foreach (var fi in di.GetFiles())
            {
                string[] words = fi.Name.Split('_');
                switch (words[0])
                {
                    case "photo":
                        break;
                    case "audio":
                        break;
                    case "voice":
                        break;
                    case "video":
                        break;
                    default:
                        listFiles.Add(new Files(fi.Name, "document", fi.Length, fi.LastWriteTime));
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
