﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                if (this.StatisticsData!=null) this.StatisticsData.ItemsSource = this.TicketUseStatistics.AsDataView();
                this.OnPropertyChanged("TicketUseStatistics");
            }
        }

        public TicketUsageStatisticsPage()
        {
            this.TicketUseStatistics = new DataTable();
            this.DataContext = this;
            this.InitializeComponent();
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
    }
}