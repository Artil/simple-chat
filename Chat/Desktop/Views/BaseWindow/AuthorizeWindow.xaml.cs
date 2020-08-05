using ChatDesktop.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatDesktop.Views.BaseWindow
{
    /// <summary>
    /// Логика взаимодействия для AuthorizeWindow.xaml
    /// </summary>
    public partial class AuthorizeWindow : Window
    {
        public AuthorizeWindow()
        {
            InitializeComponent();
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
