using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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

        private UInt32 CurrentPageValue;
        private UInt32 MaxPageValue;
        private UInt64 ParkingCountValue;
        private Decimal ParkingIncomeValue;
        private string SearchKeyWorkValue;

        public ParkingStatisticsPage()
        {
            this.Config = Configuration.FileConfig.FindConfig("GUI.cfg");
            this.ParkingStatisticsData = new DataTable();
            this.DataContext = this;
            this.InitializeComponent();
            this.PropertyChanged += ParkingStatisticsPage_PropertyChanged;
            this.CurrentPage = 1;
            this.ParkingCount = 0;
            this.ParkingIncome = 0;

        }



        private Configuration.Config Config { get; set; }

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

        public String SearchKeyWork
        {
            get { return this.SearchKeyWorkValue; }
            set
            {
                this.SearchKeyWorkValue = value;
                this.OnPropertyChanged("SearchKeyWork");
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

        public DataTable ParkingStatisticsData { get; set; }

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

        public void RefreshData(DataTable data)
        {
            this.ParkingStatisticsData = data;
            this.CurrentPage = 1;
            this.MaxPage = (UInt16)(((double)data.Rows.Count / 10) + 1);
            Decimal val;
            this.ParkingIncome = Decimal.TryParse(data.Compute("Sum(实收停车费)", String.Empty).ToString(), out val)
                ? val
                : 0;
            this.ParkingCount = (UInt64)data.Rows.Count;
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
        void ParkingStatisticsPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CurrentPage"))
            {
                var start = (int)this.CurrentPage * 10 - 10;


                var select = this.ParkingStatisticsData.Select().Skip(start).Take(10);
                if (@select.Any())
                {
                    var ds = select.CopyToDataTable();
                    this.StatisticsData.ItemsSource = ds.DefaultView;
                }
                else
                {
                    this.StatisticsData.ItemsSource = null;
                }

              
            }
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
    
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.SearchKeyWork = String.Empty;
        }
    }
}
