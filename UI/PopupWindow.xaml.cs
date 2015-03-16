using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using IPSCM.UI.Annotations;

namespace IPSCM.UI
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window, INotifyPropertyChanged
    {
        public String PopupTitleValue = String.Empty;
        private string ContentStringValue;

        public PopupWindow(Window owner, String title, String content)
            : base()
        {
            this.Owner = owner;
            InitializeComponent();
            this.PopupTitle = title;
            this.ContentString = content;
        }

        public string PopupTitle
        {
            get
            {
                return this.PopupTitleValue;
            }
            set
            {
                this.PopupTitleValue = value;
                this.OnPropertyChanged("PopupTitle");
            }
        }

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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
