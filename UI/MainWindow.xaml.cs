using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IPSCM.UI.Pages;

namespace IPSCM.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainPage MainPage { get;private set; }
        public AboutPage AboutPage { get; private set; }

        public MainWindow()
        {
            this.MainPage = new MainPage();
            this.AboutPage = new AboutPage();
            InitializeComponent();
            this.DataStatistics.IsChecked = true;
            this.ContentFrame.Content = this.MainPage;
            
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void DataStatistics_Click(object sender, RoutedEventArgs e)
        {
            this.ContentFrame.Content = this.MainPage;
        }

        private void AboutUsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.ContentFrame.Content = this.AboutPage;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            this.Visibility=Visibility.Hidden;
            e.Cancel = true;
        }
    }
}
