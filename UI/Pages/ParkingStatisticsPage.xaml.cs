using System.Data;
using System.Windows.Controls;

namespace IPSCM.UI.Pages
{
    /// <summary>
    /// Interaction logic for ParkingStatisticsPage.xaml
    /// </summary>
    public partial class ParkingStatisticsPage : Page
    {
        public DataTable ParkingStatistics { get; set; }

        public ParkingStatisticsPage()
        {
            this.ParkingStatistics=new DataTable();
            this.ParkingStatistics.Columns.Add("First");
            this.ParkingStatistics.Columns.Add("Second");

            this.ParkingStatistics.Rows.Add("11", "12");
            this.ParkingStatistics.Rows.Add("21", "22");
            this.StatisticsDatas.ItemsSource = this.ParkingStatistics.DefaultView;

            InitializeComponent();
        }
    }
}
