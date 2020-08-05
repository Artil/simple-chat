using ChatDesktop.Helpers;
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

namespace ChatDesktop.Views.BaseWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        protected override void OnActivated(EventArgs e)
        {
            FlashWindowHelper.StopFlashingWindow(this);
            base.OnActivated(e);
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
