using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Timers;
using IPSCM.UI.Annotations;

namespace IPSCM.UI.Pages
{
    /// <summary>
    /// Interaction logic for ParkingStatisticsPage.xaml
    /// </summary>
    public partial class ParkingStatisticsPage : INotifyPropertyChanged
    {
        public delegate void CurrentPageChangedEventHandler(object sender, PropertyChangedEventArgs args);

        public event CurrentPageChangedEventHandler OnCurrentPageChanged;
        private UInt32 CurrentPageValue;
        private UInt32 MaxPageValue;
        private UInt64 ParkingCountValue;
        private Decimal ParkingIncomeValue;

        public ParkingStatisticsPage()
        {
            this.Config = Configuration.FileConfig.FindConfig("GUI.cfg");
            this.ParkingStatistics = new DataTable();
            this.DataContext = this;
            this.InitializeComponent();
            this.StatisticsData.ItemsSource = this.ParkingStatistics.AsDataView();
            this.CurrentPage = 1;
            this.ParkingCount = 3;
            this.ParkingIncome = 15;
        }

        private Configuration.Config Config;

        public String PageTitle
        {
            get
            {
                return String.Format(Properties.Resources.TodayParkingSummaryFormater, this.ParkingCount,
                    this.ParkingIncome);
            }
        }
        public Decimal ParkingIncome
        {
            get { return this.ParkingIncomeValue; }
            set
            {
                this.ParkingIncomeValue = value;
                this.OnPropertyChanged("ParkingIncome");
                this.OnPropertyChanged("PageTitle");
            }
        }

        public UInt64 ParkingCount
        {
            get { return this.ParkingCountValue; }
            set
            {
                this.ParkingCountValue = value;
                this.OnPropertyChanged("ParkingCount");
                this.OnPropertyChanged("PageTitle");
            }
        }

        public UInt32 CurrentPage
        {
            get { return this.CurrentPageValue; }
            set
            {
                this.CurrentPageValue = value;
                this.OnPropertyChanged("CurrentPage");
            }
        }

        public UInt32 MaxPage
        {
            get { return this.MaxPageValue; }
            set
            {
                this.MaxPageValue = value;
                this.OnPropertyChanged("MaxPage");
            }
        }

        public DataTable ParkingStatistics { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.MaxPage += 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ParkingStatisticsPage_OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        public void RefreshData(Decimal ParkingIncome, UInt32 ParkingCount, UInt32 maxPage, DataView gridDataView)
        {
            this.ParkingIncome = ParkingIncome;
            this.ParkingCount = ParkingCount;
            this.MaxPage = maxPage;
            this.StatisticsData.ItemsSource = gridDataView;
        }

        private void NextPageButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.CurrentPage < this.MaxPage)
                this.CurrentPage++;
        }

        private void LastPageButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.CurrentPage > 1)
                this.CurrentPage--;
        }

        private void JumpToPageButton_Click(object sender, RoutedEventArgs e)
        {
            UInt32 val;
            UInt32.TryParse(this.PageJumpTextBox.Text, out val);
            this.CurrentPage = val;
        }
    }
}
