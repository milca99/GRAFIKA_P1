using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Elipse.xaml
    /// </summary>
    public partial class Elipse : Window
    {
        static Point point;
        static Canvas c;
        public Elipse(Point startPoint, Canvas c)
        {
            InitializeComponent();
            if (startPoint.X == -100 && startPoint.Y == -100)
            {
                textBoxHeight.Visibility = Visibility.Hidden;
                textBoxWidth.Visibility = Visibility.Hidden;
                textBoxContent.Visibility = Visibility.Hidden;
                comboBoxContentColor.Visibility = Visibility.Hidden;
                greskaHeight.Visibility = Visibility.Hidden;
                greskaWidth.Visibility = Visibility.Hidden;
                labelContentColor.Visibility = Visibility.Hidden;
                labelContent.Visibility = Visibility.Hidden;
                labelHeight.Visibility = Visibility.Hidden;
                labelWidth.Visibility = Visibility.Hidden;
                Elipse.c = c;

            }
            else
            {
                
               
                comboBoxContentColor.ItemsSource = typeof(Colors).GetProperties();
                //comboBoxContentColor.SelectedItem = typeof(Colors).GetProperties()[1];
                point = startPoint;
                Elipse.c = null;
            }
            comboBoxFillColor.ItemsSource = typeof(Colors).GetProperties();
            //comboBoxFillColor.SelectedItem = typeof(Colors).GetProperties()[1];

            comboBoxBorderColor.ItemsSource = typeof(Colors).GetProperties();
            //comboBoxBorderColor.SelectedItem = typeof(Colors).GetProperties()[1];
        }



        private void draw_Click(object sender, RoutedEventArgs e)
        {
            if (c == null)
            {
                if (Validate())
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = textBoxContent.Text;
                    textBlock.FontSize = 10;
                    
                    var selectedItemFill = (PropertyInfo)comboBoxFillColor.SelectedItem;
                    var color = (Color)selectedItemFill.GetValue(null, null);

                    var converter = new System.Windows.Media.BrushConverter();
                    var brushFill = (Brush)converter.ConvertFromString(color.ToString());

                    var selectedItemText = (PropertyInfo)comboBoxContentColor.SelectedItem;
                    //var brushText = System.Windows.Media.Brushes.Green;
                    if (selectedItemText != null)
                    {
                        var color3 = (Color)selectedItemText.GetValue(null, null);

                         var brushText = (Brush)converter.ConvertFromString(color3.ToString());
                        textBlock.Foreground = brushText;
                    }
                    else
                    {
                        textBlock.Foreground = System.Windows.Media.Brushes.Green;
                    }
                   

                    var selectedItemBorder = (PropertyInfo)comboBoxBorderColor.SelectedItem;
                    var color2 = (Color)selectedItemBorder.GetValue(null, null);

                    var brushBorder = (Brush)converter.ConvertFromString(color2.ToString());
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = Int32.Parse(textBoxWidth.Text);
                    ellipse.Height = Int32.Parse(textBoxHeight.Text);
                    ellipse.Stroke = brushBorder;
                    ellipse.StrokeThickness = Int32.Parse(textBoxBorderThickness.Text);
                    ellipse.Fill = brushFill;

                    //ellipse.Margin = new Thickness(point.X, point.Y, 0, 0);
                    //ellipse.Uid = "ellipse"+(point.X+point.Y);

                    textBlock.Margin = new Thickness(((double)(ellipse.Width / 2))-10, ((double)(ellipse.Height / 2))-10, 0, 0);

                    Canvas canvas = new Canvas();
                    canvas.Margin = new Thickness(point.X, point.Y, 0, 0);
                    canvas.Children.Add(ellipse);
                    canvas.Children.Add(textBlock);
                    canvas.Height = ellipse.Height;
                    canvas.Width = ellipse.Width;
                    canvas.Uid = "ellipse" + (point.X + point.Y);
                    MainWindow.ellipseDrawing = canvas;
                    this.Close();
                }
            }
            else
            {
                if(Validate2())
                {

                    var selectedItemFill = (PropertyInfo)comboBoxFillColor.SelectedItem;
                    var color = (Color)selectedItemFill.GetValue(null, null);

                    var converter = new System.Windows.Media.BrushConverter();
                    var brushFill = (Brush)converter.ConvertFromString(color.ToString());

                    var selectedItemBorder = (PropertyInfo)comboBoxBorderColor.SelectedItem;
                    var color2 = (Color)selectedItemBorder.GetValue(null, null);

                    var brushBorder = (Brush)converter.ConvertFromString(color2.ToString());
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = ((Ellipse)c.Children[0]).Width;
                    ellipse.Height = ((Ellipse)c.Children[0]).Height;
                    ellipse.Stroke = brushBorder;
                    ellipse.StrokeThickness = Int32.Parse(textBoxBorderThickness.Text);
                    ellipse.Fill = brushFill ;
                    

                    //ellipse.Margin = new Thickness(point.X, point.Y, 0, 0);
                    //ellipse.Uid = "ellipse"+(point.X+point.Y);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = ((TextBlock)c.Children[1]).Text;
                    textBlock.Foreground = ((TextBlock)c.Children[1]).Foreground;
                    textBlock.FontSize = ((TextBlock)c.Children[1]).FontSize;

                    Canvas canvas = new Canvas();
                    canvas.Margin = c.Margin;
                    canvas.Children.Add(ellipse);
                    canvas.Children.Add(textBlock);
                    canvas.Height = ellipse.Height;
                    canvas.Width = ellipse.Width;
                    canvas.Uid = c.Uid;
                    MainWindow.ellipseDrawing = canvas;
                    this.Close();
                }
            }
        }
       
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private bool Validate()
        {
            bool ret = true;
            greskaBorderColor.Content = "";
            greskaBorderThickness.Content = "";
            greskaFillColor.Content = "";
            greskaHeight.Content = "";
            greskaWidth.Content = "";
           
            if (!double.TryParse(textBoxHeight.Text, out double res1))
            {
                greskaHeight.Content = "Visina elipse mora sadrzati samo brojeve.";
                ret = false;
            }
            if (!double.TryParse(textBoxWidth.Text, out double res2))
            {
                greskaWidth.Content = "Sirina elipse mora sadrzati samo brojeve.";
                ret = false;
            }
            if (!double.TryParse(textBoxBorderThickness.Text, out double res3))
            {
                greskaWidth.Content = "Sirina elipse mora sadrzati samo brojeve.";
                ret = false;
            }
            else if (res3<0)
            {
                greskaWidth.Content = "Sirina elipse mora sadrzati samo pozitivne brojeve.";
                ret = false;
            }
            if (textBoxWidth.Text=="")
            {
                greskaWidth.Content = "Unesite sirinu.";
                ret = false;
            }
            if(textBoxHeight.Text=="")
            {
                greskaHeight.Content = "Unesite visinu.";
                ret = false;
            }
            if (textBoxBorderThickness.Text == "")
            {
                greskaBorderThickness.Content = "Unesite debljinu okvira.";
                ret = false;
            }

            if (comboBoxFillColor.SelectedItem == null)
            {
                greskaFillColor.Content = "Odaberite boju elipse.";
                ret = false;
            }
            if (comboBoxBorderColor.SelectedItem == null)
            {
                greskaBorderColor.Content = "Odaberite boju okvira.";
                ret = false;
            }
            return ret;
        }
        private bool Validate2()
        {
            bool ret = true;
            greskaBorderColor.Content = "";
            greskaBorderThickness.Content = "";
            greskaFillColor.Content = "";
            greskaHeight.Content = "";
            greskaWidth.Content = "";

          
            if (!double.TryParse(textBoxBorderThickness.Text, out double res3))
            {
                greskaWidth.Content = "Sirina elipse mora sadrzati samo brojeve.";
                ret = false;
            }
            else if (res3 < 0)
            {
                greskaWidth.Content = "Sirina elipse mora sadrzati samo pozitivne brojeve.";
                ret = false;
            }
            if (textBoxBorderThickness.Text == "")
            {
                greskaBorderThickness.Content = "Unesite debljinu okvira.";
                ret = false;
            }

            if (comboBoxFillColor.SelectedItem == null)
            {
                greskaFillColor.Content = "Odaberite boju elipse.";
                ret = false;
            }
            if (comboBoxBorderColor.SelectedItem == null)
            {
                greskaBorderColor.Content = "Odaberite boju okvira.";
                ret = false;
            }
            return ret;
        }
    }
}
