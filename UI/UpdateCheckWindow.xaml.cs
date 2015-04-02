using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using IPSCM.UI.Annotations;

namespace IPSCM.UI
{
    /// <summary>
    /// Interaction logic for UpdateCheckWindow.xaml
    /// </summary>
    public partial class UpdateCheckWindow : Window,INotifyPropertyChanged
    {
        private string ContentStringValue;

        public UpdateCheckWindow()
            : base()
        {
            InitializeComponent();
            this.ConfirmUpdate = false;
        }

        public Boolean ConfirmUpdate { get; set; }

        public String ContentString
        {
            get { return this.ContentStringValue; }
            set
            {
                this.ContentStringValue = value;
                this.OnPropertyChanged("ContentString");
            }
        }

        public new void Show()
        {
            this.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TitleBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonConfim_OnClick(object sender, RoutedEventArgs e)
        {
            this.ConfirmUpdate = true;
            this.Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
