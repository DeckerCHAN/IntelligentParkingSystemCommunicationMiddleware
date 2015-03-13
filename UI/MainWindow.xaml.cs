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
using System.Windows.Shapes;
using IPSCM.UI.Pages;

namespace IPSCM.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Page MainPage { get; set; }
        private Page AboutPage { get; set; }

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
    }
}
