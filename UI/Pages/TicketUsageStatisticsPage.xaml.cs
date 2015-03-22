using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using IPSCM.UI.Annotations;

namespace IPSCM.UI.Pages
{
    /// <summary>
    /// Interaction logic for TicketUsageStatisticsPage.xaml
    /// </summary>
    public partial class TicketUsageStatisticsPage : Page, INotifyPropertyChanged
    {
        private UInt32 CurrentPageValue;
        private uint TicketUsedSummaryValue;
        private UInt32 MaxPageValue;
        private DataTable TicketUseStatisticsValue;

        public String PageTitle
        {
            get
            {
                return String.Format(Properties.Resources.TodayTicketUseSummaryFormater, this.TicketUsedSummary);
            }
        }

        public UInt32 TicketUsedSummary
        {
            get { return this.TicketUsedSummaryValue; }
            set
            {
                this.TicketUsedSummaryValue = value;
                this.OnPropertyChanged("TicketUsedSummary");
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

        public DataTable TicketUseStatistics
        {
            get { return this.TicketUseStatisticsValue; }
            set
            {
                this.TicketUseStatisticsValue = value;
                if (this.StatisticsData != null) this.StatisticsData.ItemsSource = this.TicketUseStatistics.AsDataView();
                this.OnPropertyChanged("TicketUseStatistics");
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
        public TicketUsageStatisticsPage()
        {
            this.TicketUseStatistics = new DataTable();
            this.DataContext = this;
            this.InitializeComponent();
            this.MaxPage = 1;
            this.CurrentPage = 1;
            this.TicketUsedSummary = 0;

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {

        }
        public void RefreshDataView(UInt32 ticketUsedSummary,UInt32 maxPage,DataView gridDataView)
        {
            this.TicketUsedSummary = ticketUsedSummary;
            this.MaxPage = maxPage;
            this.StatisticsData.ItemsSource = gridDataView;
        }
    }
}
