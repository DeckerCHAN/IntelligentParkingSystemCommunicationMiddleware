using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
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

        public TicketUsageStatisticsPage()
        {
            this.TicketUseStatisticsData = new DataTable();
            this.DataContext = this;
            this.InitializeComponent();
            this.PropertyChanged += TicketUsageStatisticsPage_PropertyChanged;
            this.MaxPage = 1;
            this.CurrentPage = 1;
            this.TicketUsedSummary = 0;

        }

        void TicketUsageStatisticsPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CurrentPage"))
            {
                var start = (int)this.CurrentPage * 10 - 10;


                var select = this.TicketUseStatisticsData.Select().Skip(start).Take(10);
                if (@select.Any())
                {
                    var ds = select.CopyToDataTable();
                    this.StatisticsData.ItemsSource = ds.DefaultView;
                }
            }
        }

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

        public DataTable TicketUseStatisticsData { get; set; }

        public UInt32 MaxPage
        {
            get { return this.MaxPageValue; }
            set
            {
                this.MaxPageValue = value;
                this.OnPropertyChanged("MaxPage");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        public void RefreshData(DataTable data)
        {
            this.TicketUseStatisticsData = data;
            this.CurrentPage = 1;
            this.MaxPage = (UInt16)(((double)data.Rows.Count / 10) + 1);
            this.TicketUsedSummary = (UInt32)data.Rows.Count;
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

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            new PopupWindow(Window.GetWindow(this), "not support", "not support");
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
