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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reflector_WorldCreator
{
    /// <summary>
    /// CheckButton.xaml 的交互逻辑
    /// </summary>
    public partial class ImageCheckButton : UserControl
    {
        public static DependencyProperty IsCheckedProperty = DependencyProperty.Register
            ("IsChecked", typeof(bool), typeof(ImageCheckButton), new PropertyMetadata(false));
        public bool IsChecked
        {
            get { return (bool)this.GetValue(IsCheckedProperty); }
            set 
            {
                if (!CanCheck) return;
                this.SetValue(IsCheckedProperty, value);
                if (value == true) border.Visibility = System.Windows.Visibility.Visible;
                else border.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public static DependencyProperty SourceProperty = DependencyProperty.Register
            ("Source", typeof(ImageSource), typeof(ImageCheckButton));
        public ImageSource Source
        {
            get { return (ImageSource)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public bool CanCheck = false;
        bool isDown;
        public ImageCheckButton()
        {
            InitializeComponent();
            this.MouseLeave += (s, e) => { isDown = false; };
            this.MouseLeftButtonDown += (s, e) =>
            {
                isDown = true;
            };
            this.MouseLeftButtonUp += (s, e) =>
            {
                if (!isDown || !CanCheck) return;
                this.IsChecked = !this.IsChecked;
                isDown = false;
            };
            animation = new DoubleAnimation(0,0,TimeSpan.FromSeconds(0.5));
        }

        DoubleAnimation animation;
        public void ImageRotate(double angle)
        {
            RotateTransform trans = image.RenderTransform as RotateTransform;

            animation.From = animation.To;
            animation.To = animation.From + angle;

            trans.BeginAnimation(RotateTransform.AngleProperty, animation);
            trans.Angle %= 360;
        }

        public double GetCurrentAngle()
        {
            return (int)animation.To % 360;
        }
    }
}
