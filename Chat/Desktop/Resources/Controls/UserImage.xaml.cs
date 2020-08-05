using System;
using System.Windows;
using System.Windows.Controls;

namespace ChatDesktop.Resources.Controls
{
    /// <summary>
    /// Логика взаимодействия для UserImage.xaml
    /// </summary>
    public partial class UserImage : UserControl
    {
        public UserImage()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(UserImage), new FrameworkPropertyMetadata(double.NaN));
        public double ImageWidth
        {
            get => (double)base.GetValue(ImageWidthProperty);
            set { base.SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(UserImage), new FrameworkPropertyMetadata(double.NaN));
        public double ImageHeight
        {
            get => (double)GetValue(ImageHeightProperty); 
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty PhotoPathProperty = DependencyProperty.Register("PhotoPath", typeof(String), typeof(UserImage));
        public string PhotoPath
        {
            get => (String)this.GetValue(PhotoPathProperty); 
            set { this.SetValue(PhotoPathProperty, value); }
        }

        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register("FillColor", typeof(String), typeof(UserImage));
        public string FillColor
        {
            get=> (String)this.GetValue(FillColorProperty);
            set { this.SetValue(FillColorProperty, value); }
        }

        public static readonly DependencyProperty ShortNameProperty = DependencyProperty.Register("ShortName", typeof(String), typeof(UserImage));
        public string ShortName
        {
            get => (String)this.GetValue(ShortNameProperty);
            set { this.SetValue(ShortNameProperty, value); }
        }

        public static readonly DependencyProperty ShortNameMarginProperty = DependencyProperty.Register("ShortNameMargin", typeof(Thickness), typeof(UserImage));
        public Thickness ShortNameMargin
        {
            get => (Thickness)this.GetValue(ShortNameMarginProperty);
            set { this.SetValue(ShortNameMarginProperty, value); }
        }
    }
}
