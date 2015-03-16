using System.Windows;
using System.Windows.Controls;

namespace IPSCM.UI.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Page ParkPage { get; set; }
        private Page TicketPage { get; set; }

        public MainPage()
        {
            this.ParkPage = new ParkingStatisticsPage();
            this.TicketPage = new TicketUsageStatisticsPage();

            InitializeComponent();
            ChangeToParkingPage.IsChecked = true;
        }

        private void ChangeToParkingPage_OnChecked(object sender, RoutedEventArgs e)
        {
            this.Frame.Content = this.ParkPage;
        }

        private void ChangeToTicketPage_OnChecked(object sender, RoutedEventArgs e)
        {
            this.Frame.Content = this.TicketPage;
        }
    }
}
