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
    /// Interaction logic for Text.xaml
    /// </summary>
    public partial class Text : Window
    {
        static Point p;
        static Label label;
        public Text(Point point, Label l)
        {
            InitializeComponent();
            comboBoxColor.ItemsSource = typeof(Colors).GetProperties();
            //comboBoxColor.SelectedItem = typeof(Colors).GetProperties()[9];
            p = point;
            label = l;
            if (label!=null)
            {
                labelText.Visibility = Visibility.Hidden;
                greskaText.Visibility = Visibility.Hidden;
                textBoxText.Visibility = Visibility.Hidden;
                
            }
           
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void draw_Click(object sender, RoutedEventArgs e)
        {
            if(label!=null)
            {
                if (Validate2())
                {
                    var selectedItemFill = (PropertyInfo)comboBoxColor.SelectedItem;
                    var color = (Color)selectedItemFill.GetValue(null, null);

                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString(color.ToString());

                    Label label = new Label();
                    label.Content = Text.label.Content;
                    label.FontSize = double.Parse(textBoxSize.Text);
                    label.Foreground = brush;
                    label.Margin = Text.label.Margin;
                    label.Uid = Text.label.Uid;
                    //label.AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler(Edit_Text));
                    MainWindow.textDrawing = label;
                    this.Close();
                }
            }
            else
            {
                if (Validate())
                {
                    var selectedItemFill = (PropertyInfo)comboBoxColor.SelectedItem;
                    var color = (Color)selectedItemFill.GetValue(null, null);

                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString(color.ToString());

                    Label label = new Label();
                    label.Content =   textBoxText.Text;
                    label.FontSize = double.Parse(textBoxSize.Text);
                    label.Foreground = brush;
                    label.Margin = new Thickness(p.X, p.Y, 0, 0);
                    label.Uid = "text" + (p.X + p.Y);
                    MainWindow.textDrawing = label;
                    this.Close();
                }
            }
        }
        private bool Validate()
        {
            bool ret = true;
            greskaText.Content = "";
            greskaSize.Content = "";
            greskaColor.Content = "";
            if (textBoxText.Text=="")
            {
                greskaText.Content = "Unesite text.";
                ret = false;
            }
            if (!double.TryParse(textBoxSize.Text, out double res))
            {
                greskaSize.Content = "Velicina mora sadrzati samo brojeve.";
                ret = false;
                
            }
            else if(res < 0)
                {
                greskaSize.Content = "Velicina mora sadrzati samo pozitivne brojeve.";
                ret = false;
            }

            if (textBoxSize.Text=="")
            {
                greskaSize.Content = "Unesite velicinu teksta.";
                ret = false;
            }
           
            if(comboBoxColor.SelectedItem==null)
            {
                greskaColor.Content = "Odaberite boju.";
                ret = false;
            }
            return ret;
        }
        private bool Validate2()
        {
            bool ret = true;
            greskaText.Content = "";
            greskaSize.Content = "";
            greskaColor.Content = "";
           
            if (!double.TryParse(textBoxSize.Text, out double res))
            {
                greskaSize.Content = "Velicina mora sadrzati samo brojeve.";
                ret = false;
            }
            else if (res < 0)
            {
                greskaSize.Content = "Velicina mora sadrzati samo pozitivne brojeve.";
                ret = false;
            }
            if (textBoxSize.Text == "")
            {
                greskaSize.Content = "Unesite velicinu teksta.";
                ret = false;
            }

            if (comboBoxColor.SelectedItem == null)
            {
                greskaColor.Content = "Odaberite boju.";
                ret = false;
            }
            return ret;
        }
    }
}
