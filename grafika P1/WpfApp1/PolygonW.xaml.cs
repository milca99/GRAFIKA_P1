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
    /// Interaction logic for Polygon.xaml
    /// </summary>
    public partial class PolygonW : Window
    {
        Point p;
        Canvas canvas;
        List<System.Windows.Point> points;
        public PolygonW(Point point,Canvas c,List<System.Windows.Point> pointss)
        {
            
            InitializeComponent();
            comboBoxFillColor.ItemsSource = typeof(Colors).GetProperties();
           // comboBoxFillColor.SelectedItem = typeof(Colors).GetProperties()[2];
            comboBoxBorderColor.ItemsSource = typeof(Colors).GetProperties();
            //comboBoxBorderColor.SelectedItem = typeof(Colors).GetProperties()[2];
            comboBoxContentColor.ItemsSource = typeof(Colors).GetProperties();
           // comboBoxContentColor.SelectedItem = typeof(Colors).GetProperties()[2];
            p = point;
            canvas = c;
            points = pointss;
            if(canvas!=null)
            {
                textBoxContent.Visibility = Visibility.Hidden;
                comboBoxContentColor.Visibility = Visibility.Hidden;
                labelContentColor.Visibility = Visibility.Hidden;
                labelContent.Visibility = Visibility.Hidden;
            }
        }

        private void draw_Click(object sender, RoutedEventArgs e)
        {
            if(Validate())
            {
                if(canvas == null)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text =textBoxContent.Text;
                    
                    textBlock.FontSize = 10;
                    var selectedItemFill = (PropertyInfo)comboBoxFillColor.SelectedItem;
                    var color = (Color)selectedItemFill.GetValue(null, null);

                    var converter = new System.Windows.Media.BrushConverter();
                    var brushFill = (Brush)converter.ConvertFromString(color.ToString());

                    var selectedItemBorder = (PropertyInfo)comboBoxBorderColor.SelectedItem;
                    var color2 = (Color)selectedItemBorder.GetValue(null, null);
                    var selectedItemText = (PropertyInfo)comboBoxContentColor.SelectedItem;
                    if(selectedItemText!=null)
                    {
                        var color3 = (Color)selectedItemText.GetValue(null, null);
                        var brushText = (Brush)converter.ConvertFromString(color3.ToString());
                        textBlock.Foreground = brushText;
                    }
                    else
                    {
                        textBlock.Foreground = System.Windows.Media.Brushes.Green ;
                    }
                   
                    var brushBorder = (Brush)converter.ConvertFromString(color2.ToString());

                    Polygon polygon = new Polygon();
                    polygon.Stroke = brushBorder;
                    polygon.StrokeThickness = Int32.Parse(textBoxBorderThickness.Text);
                    polygon.Fill = brushFill ;
                    //polygon.Margin = new Thickness(p.X, p.Y, 0, 0);
                    //polygon.Uid = "polygon"+(p.X+p.Y);
                    //maksimumX = networkModel.Substations.Max(x => x.X);
                    double minx = points.Min(x => x.X);
                    double maxx = points.Max(x => x.X);
                    double miny = points.Min(x => x.Y);
                    double maxy = points.Max(x => x.Y);

                    foreach (Point poin in points)
                    {
                        
                        polygon.Points.Add(new System.Windows.Point(poin.X-minx,poin.Y-miny));
                    }
                   
                    textBlock.Margin = new Thickness(((double)((maxx - minx) / 2))-10, ((double)((maxy - miny) / 2))-10, 0, 0);

                    Canvas canvass = new Canvas();
                    canvass.Margin = new Thickness(minx, miny, 0, 0);
                    canvass.Uid = "polygon" + (minx + miny);
                    canvass.Children.Add(polygon);
                    canvass.Children.Add(textBlock);
                    canvass.Height = maxy - miny;
                    canvass.Width = maxx-minx;
                    MainWindow.polygonDrawing = canvass;
                    this.Close();
                }
                else
                {
                    Polygon pom = (Polygon)canvas.Children[0];
                    TextBlock tbpom = (TextBlock)canvas.Children[1];


                    var selectedItemFill = (PropertyInfo)comboBoxFillColor.SelectedItem;
                    var color = (Color)selectedItemFill.GetValue(null, null);

                    var converter = new System.Windows.Media.BrushConverter();
                    var brushFill = (Brush)converter.ConvertFromString(color.ToString());

                    var selectedItemBorder = (PropertyInfo)comboBoxBorderColor.SelectedItem;
                    var color2 = (Color)selectedItemBorder.GetValue(null, null);
                    var selectedItemText = (PropertyInfo)comboBoxContentColor.SelectedItem;
                    

                    var brushBorder = (Brush)converter.ConvertFromString(color2.ToString());

                    Polygon polygon = new Polygon();
                    polygon.Stroke =  brushBorder;
                    polygon.StrokeThickness = Int32.Parse(textBoxBorderThickness.Text);
                    polygon.Fill = brushFill;
                    polygon.Points = pom.Points;
                    //polygon.Margin = new Thickness(p.X, p.Y, 0, 0);
                    //polygon.Uid = "polygon"+(p.X+p.Y);
                    //maksimumX = networkModel.Substations.Max(x => x.X);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = tbpom.Text;
                    textBlock.Foreground = tbpom.Foreground; 
                    textBlock.FontSize = 10;
                    textBlock.Margin = tbpom.Margin;

                    Canvas canvass = new Canvas();
                    canvass.Margin = pom.Margin;
                    canvass.Uid = pom.Uid;
                    canvass.Children.Add(polygon);
                    canvass.Children.Add(textBlock);
                    canvass.Height = canvas.Height;
                    canvass.Width = canvas.Width;
                    MainWindow.polygonDrawing = canvass;
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

            if (!double.TryParse(textBoxBorderThickness.Text, out double res3))
            {
                greskaBorderThickness.Content = "Sirina oblika mora sadrzati samo brojeve.";
                ret = false;
            }
            else if (res3<0)
            {
                greskaBorderThickness.Content = "Sirina oblika mora biti pozitivan broj.";
                ret = false;
            }
            if (textBoxBorderThickness.Text == "")
            {
                greskaBorderThickness.Content = "Unesite debljinu okvira.";
                ret = false;
            }

            if (comboBoxFillColor.SelectedItem == null)
            {
                greskaFillColor.Content = "Odaberite boju oblika.";
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
