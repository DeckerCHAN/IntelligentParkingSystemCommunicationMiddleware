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
        private string UserNameValue;
        private string PasswordValue;
        private bool IsLoginEnableValue;
        private bool PerserverAccountValue;

        public LoginWindow()
        {
            this.ResultStringValue = String.Empty;
            this.UserNameValue = String.Empty;
            this.PasswordValue = String.Empty;
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

        public String UserName
        {
            get { return this.UserNameValue; }
            set
            {
                this.UserNameValue = value;
                this.OnPropertyChanged("UserName");
            }
        }

        public String Password
        {
            get { return this.PasswordValue; }
            set
            {
                this.PasswordValue = value;
                this.OnPropertyChanged("Password");
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
