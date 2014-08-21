using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Reflector_WorldCreator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        int[,] ChildrenPositions = new int[8, 10];
        int holeCount = 0;
        string helpMsg = "左键点击道具改变颜色\r\n右键拖动道具改变位置\r\n选中道具后按Left和Right键旋转\r\n如果添加黑洞，必须且只能添加两个\r\n把道具拖动到游戏区之外或其他道具之上会删除道具\r\n改变编辑框的数字来指定可用道具数量\r\n最后，别忘了保存~~";
        public MainWindow()
        {
            InitializeComponent();
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((obj)=>
            {
                //System.Threading.Thread.Sleep(1000);
                MessageBox.Show(helpMsg, "提示");
            }));
        }

        ImageCheckButton image;
        bool drag = false;
        Point ControlPosition;
        private void Control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is MyButton)
            {
                MyButton ct = e.Source as MyButton;
                drag = true;

                ControlPosition = e.GetPosition(workingArea);

                image = new ImageCheckButton();

                image.Width = 45;
                image.Height = 45;
                if (ct.Name == "CT_Emi") image.CanCheck = true;

                image.Source = ct.SelectedItem.Source;

                image.SetValue(Canvas.LeftProperty, Canvas.GetLeft(ct));
                image.SetValue(Canvas.TopProperty, Canvas.GetTop(ct));

                workingArea.Children.Add(image);

            }
            else if (e.Source is ImageCheckButton)
            {
                drag = true;
                image = e.Source as ImageCheckButton;
                Point pt = e.GetPosition(workingArea);

                ChildrenPositions[(int)pt.X / 45, (int)pt.Y / 45] = 0;
            }
            else if (e.Source == CT_Hole && holeCount < 2)
            {
                holeCount++;

                drag = true;

                ControlPosition = e.GetPosition(workingArea);

                image = new ImageCheckButton();
                image.Width = 45;
                image.Height = 45;
                image.Source = CT_Hole.Source;

                image.SetValue(Canvas.LeftProperty, Canvas.GetLeft(CT_Hole));
                image.SetValue(Canvas.TopProperty, Canvas.GetTop(CT_Hole));

                workingArea.Children.Add(image);
            }
            
        }

        private void Control_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (drag)
            {
                drag = false;
                Point pt = e.GetPosition(workingArea);
                //image.IsChecked = true;
                if(pt.X >= workingArea.Width 
                || pt.Y >= workingArea.Height
                || ChildrenPositions[(int)pt.X / 45, (int)pt.Y / 45] == 1)//remove
                {
                    workingArea.Children.Remove(image);
                    if (image.Source == CT_Hole.Source)
                    {
                        holeCount--;
                    }
                    image = null;
                    return;
                }

                ChildrenPositions[(int)pt.X / 45, (int)pt.Y / 45] = 1;
            }            
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if(drag)
            {
                Point newPoint = e.GetPosition(workingArea);

                double x, y;

                if (newPoint.X < workingArea.Width && newPoint.Y < workingArea.Height)
                {
                    x = (int)newPoint.X / 45 * 45;
                    y = (int)newPoint.Y / 45 * 45;
                }
                else
                {
                    x = newPoint.X - ControlPosition.X + Canvas.GetLeft(image);
                    y = newPoint.Y - ControlPosition.Y + Canvas.GetTop(image);
                }

                image.SetValue(Canvas.LeftProperty, x);
                image.SetValue(Canvas.TopProperty, y);

                ControlPosition = newPoint;
            }
         
        }

        private void Rotate_Left()
        {
            foreach (var child in workingArea.Children)
            {
                ImageCheckButton img = child as ImageCheckButton;
                if (img == null || !img.CanCheck || !img.IsChecked) continue;
                img.ImageRotate(-45);
            }
        }

        private void Rotate_Right()
        {
            foreach (var child in workingArea.Children)
            {
                ImageCheckButton img = child as ImageCheckButton;
                if (img == null || !img.CanCheck || !img.IsChecked) continue;
                img.ImageRotate(45);
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "Reflector关卡文件|*.rf";
            dialog.FileName = "Reflector关卡.rf";
            dialog.AddExtension = true;
            dialog.AutoUpgradeEnabled = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = dialog.FileName;
                StreamWriter writer = new StreamWriter(filename, false);
                try
                {
                    writer.Write(MirrorCount.Text.Trim() + "," + PrismCount.Text.Trim());
                    writer.Write(";");

                    //文件格式：
                    //镜子数量，棱镜数量；（坐标x，坐标y，类型，角度，颜色（可能无）；）*若干个
                    //其中
                    //角度是以水平向右为0度，逆时针参考的角度值，位于0~360之间
                    //类型：3，接收器；4，开关；5，门；6，黑洞；7，发射器；8，墙壁
                    //颜色：1，蓝色；2，黄色；3，红色；4，绿色；5，橘色；6，紫色；

                    foreach (ImageCheckButton img in workingArea.Children)
                    {
                        string[] splits = ((BitmapImage)img.Source).UriSource.OriginalString.Split('/');

                        int x = (int)Canvas.GetLeft(img) / 45 + 1;
                        int y = (int)Canvas.GetTop(img) / 45 + 1;
                        int angle = 0;
                        int color = 0, type = 0;
                        switch (splits.Last().Replace(".png", ""))
                        {
                            case "blue": color = 1; break;
                            case "yellow": color = 2; break;
                            case "red": color = 3; break;
                            case "green": color = 4; break;
                            case "orange": color = 5; break;
                            case "purple": color = 6; break;
                            case "hole": type = 6; break;
                        }
                        switch (splits[splits.Length - 2])
                        {
                            case "Emi": type = 7; angle = 90 - (int)img.GetCurrentAngle();
                                if (angle < 0) angle += 360;
                                break;
                            case "Rec": type = 3; break;
                            case "Wall": type = (color == 0) ? 8 : 5; break;
                            case "Swi": type = 4; break;
                        }

                        writer.Write(x + "," + y + "," + type + "," + angle);
                        if (color != 0) writer.Write("," + color);
                        writer.Write(";");
                    }
                    
                    writer.Close();
                    MessageBox.Show("关卡生成成功，存在文件" + filename + "中。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存失败，原因是：" + ex.Message);
                    writer.Close();
                    File.Delete(filename);
                }
            }
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Reflector关卡文件|*.rf";
            dialog.FileName = "Reflector关卡.rf";
            dialog.AddExtension = true;
            dialog.AutoUpgradeEnabled = true;
            dialog.CheckFileExists = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = dialog.FileName;
                StreamReader reader = new StreamReader(filename);
                try
                {
                    string[] data = reader.ReadToEnd().Split(new char[]{ ';'}, StringSplitOptions.RemoveEmptyEntries);
                    string[] splits = data[0].Split(',');
                    MirrorCount.Text = splits[0];
                    PrismCount.Text = splits[1];

                    List<ImageCheckButton> imgs = new List<ImageCheckButton>();
                    for (int i = 1; i < data.Length; i++)
                    {
                        splits = data[i].Split(',');
                        ImageCheckButton icb = new ImageCheckButton();
                        icb.Width = 45;
                        icb.Height = 45;
                        icb.SetValue(Canvas.LeftProperty, (double.Parse(splits[0]) - 1) * 45);
                        icb.SetValue(Canvas.TopProperty, (double.Parse(splits[1]) - 1) * 45);
                        if (splits.Length < 5)
                        {
                            if(splits[2] == "8")
                                icb.Source = new BitmapImage(new Uri("/Images/Wall/" + "black.png", UriKind.Relative));
                            else if (splits[2] == "6")
                            {
                                icb.Source = new BitmapImage(new Uri("/Images/hole.png", UriKind.Relative));
                            }
                        }
                        else
                        {
                            string color = string.Empty;
                            switch (splits[4])
                            {
                                case "1": color = "blue.png"; break;
                                case "2": color = "yellow.png"; break;
                                case "3": color = "red.png"; break;
                                case "4": color = "green.png"; break;
                                case "5": color = "orange.png"; break;
                                case "6": color = "purple.png"; break;
                            }

                            Uri source = null;
                            switch (splits[2])
                            {
                                case "7":
                                    int angle = int.Parse(splits[3]) - 90;
                                    icb.ImageRotate(-angle);
                                    icb.CanCheck = true;
                                    source = new Uri("/Images/Emi/" + color, UriKind.Relative);
                                    break;
                                case "3":
                                    source = new Uri("/Images/Rec/" + color, UriKind.Relative);
                                    break;
                                case "4":
                                    source = new Uri("/Images/Swi/" + color, UriKind.Relative);
                                    break;
                                case "5":
                                    source = new Uri("/Images/Wall/" + color, UriKind.Relative);
                                    break;
                            }
                            icb.Source = new BitmapImage(source);
                        }
                        imgs.Add(icb);
                    }

                    this.workingArea.Children.Clear();
                    foreach (var img in imgs)
                        this.workingArea.Children.Add(img);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("读取失败，原因是：" + ex.Message);
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("任何未保存的内容都将丢失。确定要退出么？", "温馨提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void Hole_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Hole_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left: Rotate_Left(); break;
                case Key.Right: Rotate_Right(); break;
            }
        }
    }
}
