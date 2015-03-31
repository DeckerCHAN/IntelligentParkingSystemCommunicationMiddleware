using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using IPSCM.UI.Annotations;

namespace IPSCM.UI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private string ResultStringValue;
        private bool IsLoginEnableValue;
        private bool PerserverAccountValue;

        public LoginWindow()
        {
            this.ResultStringValue = String.Empty;
            this.IsLoginEnableValue = true;
            this.PerserverAccount = true;
            InitializeComponent();
        }

        public Boolean IsLoginEnable
        {
            get { return this.IsLoginEnableValue; }
            set { this.IsLoginEnableValue = value; }
        }

        public String ResultString
        {
            get { return this.ResultStringValue; }
            set
            {
                this.ResultStringValue = value;
                this.OnPropertyChanged("ResultString");
            }
        }


        public Boolean PerserverAccount
        {
            get { return this.PerserverAccountValue; }
            set
            {
                this.PerserverAccountValue = value;
                this.OnPropertyChanged("PerserverAccount");
            }
        }

        private void Header_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
