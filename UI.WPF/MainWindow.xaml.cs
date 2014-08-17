using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SQL_button_Click(System.Object sender,
            System.Windows.RoutedEventArgs e)
        {
            MainWindow modalWindow = new MainWindow();
            modalWindow.ShowDialog();
            //this.Close();
        }

        private void ObjectStar_button_Click(System.Object sender,
            System.Windows.RoutedEventArgs e)
        {
            MainWindow modalWindow = new MainWindow();
            modalWindow.Show();
            this.Close();
        }

        private void MainWindow_Loaded(System.Object sender,
            System.Windows.RoutedEventArgs e)
        {
            double height = SystemParameters.WorkArea.Height;
            double width = SystemParameters.WorkArea.Width;
            this.Top = ((height - this.Height) / 2);
            this.Left = ((width - this.Width) / 2);
        }
    }
}
