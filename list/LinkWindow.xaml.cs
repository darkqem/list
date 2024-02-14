// LinkWindow.xaml.cs
using System.Windows;

namespace list
{
    public partial class LinkWindow : Window
    {
        public LinkWindow(string url)
        {
            InitializeComponent();
            webBrowser.Navigate(url);
        }
    }
}
