using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace list
{
    /// <summary>
    /// Логика взаимодействия для ButtonClose.xaml
    /// </summary>
    public partial class ButtonClose : UserControl
    {
        public ButtonClose()
        {
            InitializeComponent();

            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Closing += ParentWindow_Closing;
            }

            
            CloseButton.Click += CloseButton_Click;
        }
        private void ParentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
