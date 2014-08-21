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

namespace Reflector_WorldCreator
{
    /// <summary>
    /// MyButton.xaml 的交互逻辑
    /// </summary>
    public partial class MyButton : UserControl
    {
        public static DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(string), typeof(MyButton));
        public string Type
        {
            get
            {
                return this.GetValue(TypeProperty) as string;
            }
            set
            {
                this.SetValue(TypeProperty, value);
            }
        }

        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(Image), typeof(MyButton));
        public Image SelectedItem
        {
            get
            {
                return this.GetValue(SelectedItemProperty) as Image;
            }
            set
            {
                this.SetValue(SelectedItemProperty, value);

                button.Content = value;
            }
        }

        public MyButton()
        {
            InitializeComponent();
            this.MouseRightButtonDown += (s, e) =>
            {
                //e.Source = this;
            };
            this.Loaded += (s, e) =>
            {
                if (Type == "Wall")
                {
                    stackpanel.Children.Add(
                    new Image() { Source = new BitmapImage(new Uri("/Images/" + Type + "/" + "black.png", UriKind.Relative)), Stretch = Stretch.Fill });
                }
                stackpanel.Children.Add(
                new Image() { Source = new BitmapImage(new Uri("/Images/" + Type + "/" + "blue.png", UriKind.Relative)), Stretch = Stretch.Fill });
                stackpanel.Children.Add(
                    new Image() { Source = new BitmapImage(new Uri("/Images/" + Type + "/" + "red.png", UriKind.Relative)), Stretch = Stretch.Fill });
                stackpanel.Children.Add(
                    new Image() { Source = new BitmapImage(new Uri("/Images/" + Type + "/" + "green.png", UriKind.Relative)), Stretch = Stretch.Fill });
                stackpanel.Children.Add(
                    new Image() { Source = new BitmapImage(new Uri("/Images/" + Type + "/" + "yellow.png", UriKind.Relative)), Stretch = Stretch.Fill });
                stackpanel.Children.Add(
                    new Image() { Source = new BitmapImage(new Uri("/Images/" + Type + "/" + "orange.png", UriKind.Relative)), Stretch = Stretch.Fill });
                stackpanel.Children.Add(
                    new Image() { Source = new BitmapImage(new Uri("/Images/" + Type + "/" + "purple.png", UriKind.Relative)), Stretch = Stretch.Fill });
                this.SelectedItem = new Image();
                SelectedItem.Source = (stackpanel.Children[0] as Image).Source;
                stackpanel.Width = this.Width;
            };
        }

        private void stackpanel_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.SelectedItem.Source = (e.OriginalSource as Image).Source;
            popup.IsOpen = false;
        }

        public bool ContainsImage(ImageSource source)
        {
            foreach (Image img in stackpanel.Children)
            {
                if (img.Source == source) return true;
            }
            return false;
        }
    }
}
