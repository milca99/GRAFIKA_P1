using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using WpfApp1.Model;
using Brushes = System.Drawing.Brushes;
using Pen = System.Drawing.Pen;
using Point = WpfApp1.Model.Point;
using Size = System.Drawing.Size;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string checkedType;
        public static PointCollection points = new PointCollection();
        Dictionary<string, object> drawings = new Dictionary<string, object>();
        public double noviX, noviY;
        Dictionary<long, NodeEntity> nodeEntities = new Dictionary<long, NodeEntity>();
        Dictionary<long, SubstationEntity> substationEntities = new Dictionary<long, SubstationEntity>();
        Dictionary<long, SwitchEntity> switchEntites = new Dictionary<long, SwitchEntity>();
        Dictionary<long, LineEntity> lineEntities = new Dictionary<long, LineEntity>();
        Dictionary<long, Node> nodes = new Dictionary<long, Node>();
        NetworkModel networkModel;
        List<UIElement> itemstoremove = new List<UIElement>();
        bool undo = false;
        bool redo = true;
        Div[,] divs;
        double divSize = 3;
        int divCountX;
        int divCountY;
        double stepX;
        double stepY;
        double minimumX;
        double maksimumX;
        double minimumY;
        double maksimumY;
        public static UIElement ellipseDrawing = null;
        public static UIElement textDrawing = null;
        public static UIElement polygonDrawing = null;
        UIElement undoedElement = null;
        string redFlag1 = null;
        string redFlag2 = null;
        string redFlagType1 = "";
        string redFlagType2 = "";
        private List<System.Windows.Point> polygonPoints = new List<System.Windows.Point>();
        public MainWindow()
        {
            InitializeComponent();
            divCountX = (int)(Math.Floor(canvasMap.Width / divSize));
            divCountY = (int)(Math.Floor(canvasMap.Height / divSize));
            divs = new Div[divCountX,divCountY];
        }

		private void LoadButton_Click(object sender, RoutedEventArgs e)
		{
            LoadFromXml();
            InitializeDivMatrix();
            DrawNodeEntities();
            DrawSwitchEntities();
            DrawSubstationEntities();
            DrawLines();
           
        }
        public void InitializeDivMatrix()
        {
            double x = 0;
            double y = 0;
            for(int i=0;i<divCountX;i++)
            {
                for(int j=0;j<divCountY;j++)
                {
                    divs[i, j] = new Div(x, x + divSize, y, y + divSize, true);
                    
                    y += divSize;
                }
                x += divSize;
                y = 0;
            }
        }
		public void LoadFromXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NetworkModel));
            StreamReader reader = new StreamReader("Geographic.xml");
             networkModel = (NetworkModel)serializer.Deserialize(reader);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeList;



            foreach (SubstationEntity ss in networkModel.Substations)
            {
                ToLatLon(ss.X, ss.Y, 34, out noviX, out noviY);
                ss.X = noviX;
                ss.Y = noviY;
            }
            maksimumX = networkModel.Substations.Max(x => x.X);
            minimumX = networkModel.Substations.Min(x => x.X);
            maksimumY = networkModel.Substations.Max(x => x.Y);
            minimumY = networkModel.Substations.Min(x => x.Y);
            foreach (NodeEntity n in networkModel.Nodes)
            {
                ToLatLon(n.X, n.Y, 34, out noviX, out noviY);
                n.X = noviX;
                n.Y = noviY;
            }
            double pom = networkModel.Nodes.Min(x => x.X);
            minimumX = (pom < minimumX) ? pom : minimumX;
            pom = networkModel.Nodes.Max(x => x.X);
            maksimumX = (pom > maksimumX) ? pom : maksimumX;
            pom = networkModel.Nodes.Max(x => x.Y);
            maksimumY = (pom > maksimumY) ? pom : maksimumY;
            pom = networkModel.Nodes.Min(x => x.Y);
            minimumY = (pom < minimumY) ? pom : minimumY;
            foreach (SwitchEntity s in networkModel.Switches)
            {
                ToLatLon(s.X, s.Y, 34, out noviX, out noviY);
                s.X = noviX;
                s.Y = noviY;
            }
            pom = networkModel.Switches.Min(x => x.X);
            minimumX = (pom < minimumX) ? pom : minimumX;
            pom = networkModel.Switches.Max(x => x.X);
            maksimumX = (pom > maksimumX) ? pom : maksimumX;
            pom = networkModel.Switches.Max(x => x.Y);
            maksimumY = (pom > maksimumY) ? pom : maksimumY;
            pom = networkModel.Switches.Min(x => x.Y);
            minimumY = (pom < minimumY) ? pom : minimumY;
            stepX = (maksimumX - minimumX) / canvasMap.Width;
            stepY = (maksimumY - minimumY) / canvasMap.Height;

            
        }
        public void DrawNodeEntities()
        {
            bool flag;
            foreach (NodeEntity n in networkModel.Nodes)
            {
                flag = false;
                double x = (double)((n.X - minimumX) * canvasMap.Width / (maksimumX - minimumX));
                double y =(double) ((n.Y - minimumY) * (canvasMap.Height) / (maksimumY - minimumY));
              
                int k = (int)Math.Ceiling(x / divSize)-1; 
                int h = (int)Math.Ceiling(y / divSize)-1;
                for (int i = k - 2; i < k + 2; i++)
                {
                    for (int j = h - 2; j < h + 2; j++)
                    {
                        if (i < 0 || i >= divCountX || j < 0 || j >= divCountY)
                        {
                            continue;
                        }
                        if (divs[i, j].ChechIfFits(x,y) )
                        {
                            if(divs[i,j].Occupy())
                            {
                                Node node = new Node(x, y, divs[i, j].X1+(double)(divSize/2), divs[i, j].Y1 + (double)(divSize / 2), n.Name, n.Id);
                                nodes.Add(n.Id,node);
                                System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                r.Uid = node.Id.ToString();
                                r.Width = divSize;
                                r.Height = divSize;
                                r.Stroke = System.Windows.Media.Brushes.Green;
                                r.Fill = System.Windows.Media.Brushes.Green;
                                //r.AddHandler(IsMouseOver, new RoutedEventHandler(Over_Node));
                                r.ToolTip = $"{n.Id}\n{n.Name}";
                                r.SetValue(Canvas.LeftProperty,node.Y2);
                                r.SetValue(Canvas.BottomProperty,node.X2 );
                                canvasMap.Children.Add(r);
                                flag = true;
                            }
                            else
                            {
                                int cnt = 0;
                                while (!flag)
                                {
                                    cnt++;
                                    for (int i2 = i - cnt; i2 <= i + cnt; i2++)
                                    {
                                        for (int j2 = j - cnt; j2 <= j + cnt; j2++)
                                        {
                                            if (i2 < 0 || i2 >= divCountX || j2 < 0 || j2 >= divCountY)
                                            {
                                                continue;
                                            }
                                            if (divs[i2, j2].Occupy())
                                            {
                                                Node node = new Node(x, y, divs[i2, j2].X1 + (double)(divSize / 2), divs[i2, j2].Y1 + (double)(divSize / 2), n.Name, n.Id);
                                                nodes.Add(n.Id, node);
                                                System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                                r.Uid = node.Id.ToString();
                                                r.Width = divSize;
                                                r.Height = divSize;
                                                r.ToolTip = $"{n.Id}\n{n.Name}";
                                                r.Stroke = System.Windows.Media.Brushes.Green;
                                                r.Fill = System.Windows.Media.Brushes.Green;
                                                r.SetValue(Canvas.LeftProperty,node.Y2 );
                                                r.SetValue(Canvas.BottomProperty,node.X2);
                                                canvasMap.Children.Add(r);
                                                flag = true;
                                            }
                                            if (flag)
                                                break;
                                        }
                                        if (flag)
                                            break;
                                    }
                                }
                            }
                        }
                        if (flag)
                            break;
                    }
                    if (flag)
                        break;
                }
              
            }
        }
        public void DrawSwitchEntities()
        {
            bool flag;
            foreach (SwitchEntity n in networkModel.Switches)
            {
                flag = false;
                double x = (double)((n.X - minimumX) * canvasMap.Width / (maksimumX - minimumX));
                double y = (double)((n.Y - minimumY) * (canvasMap.Height) / (maksimumY - minimumY));

                int k = (int)Math.Ceiling(x / divSize) - 1;
                int h = (int)Math.Ceiling(y / divSize) - 1;
                for (int i = k - 2; i < k + 2; i++)
                {
                    for (int j = h - 2; j < h + 2; j++)
                    {
                        if (i < 0 || i >= divCountX || j < 0 || j >= divCountY)
                        {
                            continue;
                        }
                        if (divs[i, j].ChechIfFits(x, y))
                        {
                            if (divs[i, j].Occupy())
                            {
                                if (n.Name == "loadbreaker_352187350969")
                                {
                                    int a = 5;
                                }
                                Node node = new Node(x,y, divs[i, j].X1 + (double)(divSize / 2), divs[i, j].Y1 + (double)(divSize / 2), n.Name, n.Id);
                                nodes.Add(n.Id, node);
                                System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                r.Uid = node.Id.ToString();
                                r.Width = divSize;
                                r.Height = divSize;
                                r.ToolTip = $"{n.Id}\n{n.Name}";
                                r.Stroke = System.Windows.Media.Brushes.Pink;
                                r.Fill = System.Windows.Media.Brushes.Pink;
                                r.SetValue(Canvas.LeftProperty, node.Y2);
                                r.SetValue(Canvas.BottomProperty, node.X2);
                                canvasMap.Children.Add(r);
                                flag = true;
                            }
                            else
                            {
                                int cnt = 0;
                                while (!flag)
                                {
                                    cnt++;
                                    for (int i2 = i - cnt; i2 <= i + cnt; i2++)
                                    {
                                        for (int j2 = j - cnt; j2 <= j + cnt; j2++)
                                        {
                                            if (i2 < 0 || i2 >= divCountX || j2 < 0 || j2 >= divCountY)
                                            {
                                                continue;
                                            }
                                            if (divs[i2, j2].Occupy())
                                            {
                                                if (n.Name == "loadbreaker_352187350969")
                                                {
                                                    int a = 5;
                                                }
                                                Node node = new Node(x, y, divs[i2, j2].X1 + (double)(divSize / 2), divs[i2, j2].Y1 + (double)(divSize / 2), n.Name, n.Id);
                                                nodes.Add(n.Id, node);
                                                System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                                r.Uid = node.Id.ToString();
                                                r.Width = divSize;
                                                r.Height = divSize;
                                                r.ToolTip = $"{n.Id}\n{n.Name}";
                                                r.Stroke = System.Windows.Media.Brushes.Pink;
                                                r.Fill = System.Windows.Media.Brushes.Pink;
                                                r.SetValue(Canvas.LeftProperty, node.Y2);
                                                r.SetValue(Canvas.BottomProperty, node.X2);
                                                canvasMap.Children.Add(r);
                                                flag = true;
                                            }
                                            if (flag)
                                                break;
                                        }
                                        if (flag)
                                            break;
                                    }
                                }
                            }
                        }
                        if (flag)
                            break;
                    }
                    if (flag)
                        break;
                }

            }
        }
        public void DrawSubstationEntities()
        {
            bool flag;
            foreach (SubstationEntity n in networkModel.Substations)
            {
                flag = false;
                double x = (double)((n.X - minimumX) * canvasMap.Width / (maksimumX - minimumX));
                double y = (double)((n.Y - minimumY) * (canvasMap.Height) / (maksimumY - minimumY));

                int k = (int)Math.Ceiling(x / divSize) - 1;
                int h = (int)Math.Ceiling(y / divSize) - 1;
                for (int i = k - 2; i < k + 2; i++)
                {
                    for (int j = h - 2; j < h + 2; j++)
                    {
                        if (i < 0 || i >= divCountX || j < 0 || j >= divCountY)
                        {
                            continue;
                        }
                        if (divs[i, j].ChechIfFits(x, y))
                        {
                            if (divs[i, j].Occupy())
                            {
                                Node node = new Node(x, y, divs[i, j].X1 + (double)(divSize / 2), divs[i, j].Y1 + (double)(divSize / 2), n.Name, n.Id);
                                nodes.Add(n.Id, node);
                                System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                r.Uid = node.Id.ToString();
                                r.Width = divSize;
                                r.Height = divSize;
                                r.ToolTip = $"{n.Id}\n{n.Name}";
                                r.Stroke = System.Windows.Media.Brushes.Blue;
                                r.Fill = System.Windows.Media.Brushes.Blue;
                                r.SetValue(Canvas.LeftProperty, node.Y2);
                                r.SetValue(Canvas.BottomProperty, node.X2);
                                canvasMap.Children.Add(r);
                                flag = true;
                            }
                            else
                            {
                                int cnt = 0;
                                while (!flag)
                                {
                                    cnt++;
                                    for (int i2 = i - cnt; i2 <= i + cnt; i2++)
                                    {
                                        for (int j2 = j - cnt; j2 <= j + cnt; j2++)
                                        {
                                            if (i2 < 0 || i2 >= divCountX || j2 < 0 || j2 >= divCountY)
                                            {
                                                continue;
                                            }
                                            if (divs[i2, j2].Occupy())
                                            {
                                                Node node = new Node(x, y, divs[i2, j2].X1 + (double)(divSize / 2), divs[i2, j2].Y1 + (double)(divSize / 2), n.Name, n.Id);
                                                nodes.Add(n.Id, node);
                                                System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                                r.Uid = node.Id.ToString();
                                                r.Width = divSize;
                                                r.Height = divSize;
                                                r.ToolTip = $"{n.Id}\n{n.Name}";
                                                r.Stroke = System.Windows.Media.Brushes.Blue;
                                                r.Fill = System.Windows.Media.Brushes.Blue;
                                                r.SetValue(Canvas.LeftProperty, node.Y2);
                                                r.SetValue(Canvas.BottomProperty, node.X2);
                                                canvasMap.Children.Add(r);
                                                flag = true;
                                            }
                                            if (flag)
                                                break;
                                        }
                                        if (flag)
                                            break;
                                    }
                                }
                            }
                        }
                        if (flag)
                            break;
                    }
                    if (flag)
                        break;
                }

            }
        }
        public void DrawLines()
        {
            bool flag;
            foreach(LineEntity l in networkModel.Lines)
            {
                flag = false;
                Node pe1 = ((nodes.Values).ToList()).Find(q => q.Id == l.FirstEnd);
                Node pe2 = ((nodes.Values).ToList()).Find(q => q.Id == l.SecondEnd);
                if (pe1!=null && pe2!=null )
                {
                   
                    int ii1 = (int)Math.Ceiling(pe1.X2 / divSize);
                    int jj1 = (int)Math.Ceiling(pe1.Y2 / divSize);
                    
                    
                    Polyline polyline = new Polyline();
                    polyline.Stroke = System.Windows.Media.Brushes.Gray;
                    polyline.AddHandler(MouseRightButtonDownEvent,new RoutedEventHandler(Line_Click));
                    polyline.FillRule = FillRule.EvenOdd;
                    polyline.Uid = l.Id.ToString();
                    polyline.ToolTip = $"{l.Id}\n{l.Name}";
                    PointCollection myPointCollection = new PointCollection();
                    //Line line = new Line();
                    if (pe1.X2==pe2.X2 || pe1.Y2 == pe2.Y2)
                    {
                        myPointCollection.Add(new System.Windows.Point(pe1.Y2 + (double)(divSize / 2), canvasMap.Height - (pe1.X2 + (double)(divSize / 2))));
                        myPointCollection.Add(new System.Windows.Point(pe2.Y2 + (double)(divSize / 2), canvasMap.Height - (pe2.X2 + (double)(divSize / 2))));
                        polyline.Points = myPointCollection;
                        canvasMap.Children.Add(polyline);
                        //line.X1 = (pe1.Y2 + (double)(divSize / 2));//class Node has properties x2,y2 which presents its new position refering to the division it is in
                        //line.Y1 = canvasMap.Height - (pe1.X2 + (double)(divSize / 2)); //
                        //line.X2 = (pe2.Y2 + (double)(divSize / 2));
                        //line.Y2 = canvasMap.Height - (pe2.X2 + (double)(divSize / 2)); //
                        //line.Stroke = System.Windows.Media.Brushes.Gray;

                    }
                    else
                    {
                        myPointCollection.Add(new System.Windows.Point(pe1.Y2 + (double)(divSize / 2), canvasMap.Height - (pe1.X2 + (double)(divSize / 2))));
                        myPointCollection.Add(new System.Windows.Point(pe2.Y2 + (double)(divSize / 2), canvasMap.Height - (pe1.X2 +(double)(divSize / 2))));
                        myPointCollection.Add(new System.Windows.Point(pe2.Y2 + (double)(divSize / 2), canvasMap.Height - (pe2.X2 + (double)(divSize / 2))));
                        polyline.Points = myPointCollection;
                        canvasMap.Children.Add(polyline);
                    }
                    #region mark intersection
                    //1. desno
                    if (pe1.X2 == pe2.X2 && pe1.Y2 < pe2.Y2)
                    {
                        for (int i = ii1 - 2; i < ii1 + 2; i++)
                        {
                            for (int j = jj1 - 2; j < jj1 + 2; j++)
                            {
                                if (i < 0 || i > divCountX - 1 || j < 0 || j > divCountY - 1)
                                    continue;
                                if (divs[i, j].X1 + (double)(divSize / 2) == pe1.X2 && divs[i, j].Y1 + (double)(divSize / 2) < pe1.Y2)
                                {
                                    j++;
                                    while (divs[i, j].Y1 + (double)(divSize / 2) <= pe2.Y2)
                                    {
                                        divs[i, j].HasLineJ++;
                                        if (divs[i, j].Free && divs[i, j].HasLineI >= 1 /*&& divs[i,j-1].Free &&  && divs[i,j-1].HasLine<2*/)
                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);


                                        }
                                        j++;
                                    }

                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                    // 2. levo
                    else if (pe1.X2 == pe2.X2 && pe1.Y2 > pe2.Y2)
                    {
                        for (int i = ii1 - 2; i < ii1 + 2; i++)
                        {
                            for (int j = jj1 - 2; j < jj1 + 2; j++)
                            {
                                if (i < 0 || i > divCountX - 1 || j < 0 || j > divCountY - 1)
                                    continue;
                                if (divs[i, j].X1 + (double)(divSize / 2) == pe1.X2 && divs[i, j].Y1 + (double)(divSize / 2) < pe1.Y2)
                                {
                                    //j--;
                                    while (divs[i, j].Y1 >= pe2.Y2)
                                    {
                                        divs[i, j].HasLineJ++;
                                        if (divs[i, j].Free && divs[i, j].HasLineI >= 1 /*&& divs[i, j + 1].Free && && divs[i, j + 1].HasLine < 2*/)

                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        j--;
                                    }

                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                    // 3. desno gore
                    else if (pe1.X2 < pe2.X2 && pe1.Y2 < pe2.Y2)
                    {
                        for (int i = ii1 - 2; i < ii1 + 2; i++)
                        {
                            for (int j = jj1 - 2; j < jj1 + 2; j++)
                            {
                                if (i < 0 || i > divCountX - 1 || j < 0 || j > divCountY - 1)
                                    continue;
                                if (divs[i, j].X1 + (double)(divSize / 2) == pe1.X2 && divs[i, j].Y1 + (double)(divSize / 2) < pe1.Y2)
                                {
                                    j++;
                                    while (divs[i, j].Y1 + (double)(divSize / 2) <= pe2.Y2)
                                    {
                                        divs[i, j].HasLineJ++;
                                        if (divs[i, j].Free && divs[i, j].HasLineI >= 1 /*&& divs[i, j - 1].Free && && divs[i, j - 1].HasLine < 2*/)
                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        j++;
                                    }
                                    divs[i, j-1].HasLineJ--;
                                    j--;
                                    while (divs[i, j].X1 + (double)(divSize / 2) <= pe2.X2)
                                    {
                                        
                                        if (divs[i, j].Free && divs[i, j].HasLineJ >= 1 /*&& divs[i-1, j].Free && divs[i-1, j ].HasLine < 2*/)
                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        divs[i, j].HasLineI++;
                                        i++;
                                    }
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }

                    // 4.levo dole  !!!!!!!!!!!!!!!!!
                    else if (pe1.X2 > pe2.X2 && pe1.Y2 > pe2.Y2)
                    {
                        for (int i = ii1 - 2; i < ii1 + 2; i++)
                        {
                            for (int j = jj1 - 2; j < jj1 + 2; j++)
                            {
                                if (i < 0 || i > divCountX - 1 || j < 0 || j > divCountY - 1)
                                    continue;
                                if (divs[i, j].X1 + (double)(divSize / 2) == pe1.X2 && divs[i, j].Y1 + (double)(divSize / 2) < pe1.Y2)
                                {
                                    while (divs[i, j].Y1 - (double)(divSize / 2) >= pe2.Y2)
                                    {
                                        divs[i, j].HasLineJ++;
                                        //if (divs[i, j].Free)
                                        if (divs[i, j].Free && divs[i, j].HasLineI >= 1 /*&& divs[i, j + 1].Free && && divs[i, j + 1].HasLine < 2*/)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        j--;
                                    }
                                    //divs[i, j].HasLineJ++;
                                    //i--;
                                    while (divs[i, j].X1 >= pe2.X2)
                                    {
                                        
                                        if (divs[i, j].Free && (divs[i, j].HasLineJ >= 1 || (divs[i, j].HasLineI >= 1 && divs[i,j+1].HasLineJ>=1)) /*&& divs[i+1, j ].Free && &&divs[i+1, j ].HasLine < 2*/)
                                       // if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        divs[i, j].HasLineI++;
                                        i--;
                                    }

                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                    // 5. dole
                    else if (pe1.X2 > pe2.X2 && pe1.Y2 == pe2.Y2)
                    {
                        for (int i = ii1 - 2; i < ii1 + 2; i++)
                        {
                            for (int j = jj1 - 2; j < jj1 + 2; j++)
                            {
                                if (i < 0 || i > divCountX - 1 || j < 0 || j > divCountY - 1)
                                    continue;
                                if (divs[i, j].X1 + (double)(divSize / 2) == pe1.X2 && divs[i, j].Y1 + (double)(divSize / 2) < pe1.Y2)
                                {

                                    //nz dal radi
                                    j++;
                                    while (divs[i, j].X1 >= pe2.X2)
                                    {
                                        divs[i, j].HasLineI++;
                                        if (divs[i, j].Free && divs[i, j].HasLineJ >= 1 /*&& divs[i+1, j ].Free && divs[i+1, j ].HasLine < 2*/)
                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        i--;
                                    }

                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                    //6. gore
                    else if (pe1.X2 < pe2.X2 && pe1.Y2 == pe2.Y2)
                    {
                        for (int i = ii1 - 2; i < ii1 + 2; i++)
                        {
                            for (int j = jj1 - 2; j < jj1 + 2; j++)
                            {
                                if (i < 0 || i > divCountX - 1 || j < 0 || j > divCountY - 1)
                                    continue;
                                if (divs[i, j].X1 + (double)(divSize / 2) == pe1.X2 && divs[i, j].Y1 + (double)(divSize / 2) < pe1.Y2)
                                {

                                    j++;
                                    while (divs[i, j].X1 + (double)(divSize / 2) <= pe2.X2)
                                    {
                                        divs[i, j].HasLineI++;
                                        if (divs[i, j].Free && (divs[i, j].HasLineJ >= 1 || (divs[i, j].HasLineI >= 1 && divs[i, j+1].HasLineJ >= 1))/* && divs[i+1, j ].Free && divs[i+1, j ].HasLine < 2*/)
                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        i++;
                                    }

                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                    // 7. levo gore
                    else if (pe1.X2 < pe2.X2 && pe1.Y2 > pe2.Y2)
                    {
                        for (int i = ii1 - 2; i < ii1 + 2; i++)
                        {
                            for (int j = jj1 - 2; j < jj1 + 2; j++)
                            {
                                if (i < 0 || i > divCountX - 1 || j < 0 || j > divCountY - 1)
                                    continue;
                                if (divs[i, j].X1 + (double)(divSize / 2) == pe1.X2 && divs[i, j].Y1 + (double)(divSize / 2) < pe1.Y2)
                                {
                                    while (divs[i, j].Y1 >= pe2.Y2)
                                    {
                                        divs[i, j].HasLineJ++;
                                        if (divs[i, j].Free && divs[i, j].HasLineI >= 1 /*&& divs[i, j + 1].Free && divs[i, j + 1].HasLine < 2*/)
                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        j--;
                                    }
                                    //j++;
                                    while (divs[i, j].X2 <= pe2.X2)
                                    {

                                        divs[i, j].HasLineI++;
                                        if (divs[i, j].Free && divs[i, j].HasLineJ >= 1 /*&& divs[i-1, j ].Free && divs[i-1, j].HasLineJ < 1*/)
                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        i++;
                                    }

                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                    //8. gore levo
                    else if (pe1.X2 > pe2.X2 && pe1.Y2 < pe2.Y2)
                    {
                        for (int i = ii1 - 2; i < ii1 + 2; i++)
                        {
                            for (int j = jj1 - 2; j < jj1 + 2; j++)
                            {
                                if (i < 0 || i > divCountX - 1 || j < 0 || j > divCountY - 1)
                                    continue;
                                if (divs[i, j].X1 + (double)(divSize / 2) == pe1.X2 && divs[i, j].Y1 + (double)(divSize / 2) < pe1.Y2)
                                {
                                    j++;
                                    while (divs[i, j].Y2 <= pe2.Y2)
                                    {
                                        divs[i, j].HasLineJ++;
                                        if (divs[i, j].Free && divs[i, j].HasLineI >= 1 /*&& divs[i, j - 1].Free && divs[i, j - 1].HasLine < 2*/)
                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);

                                        }
                                        j++;
                                    }
                                    //j--;
                                    // i++; //ovde mi ode zqa jedan vise u levo i ne znam kako da popravim al kontam da to nije toliko strasno
                                    while (divs[i, j].X1 > pe2.X2)
                                    {
                                        divs[i, j].HasLineI++;
                                        if (divs[i, j].Free && divs[i, j].HasLineJ >= 1  /*&& divs[i+1, j ].Free && divs[i+1, j].HasLine < 2*/)
                                        //if (divs[i, j].Free)
                                        {
                                            System.Windows.Shapes.Ellipse r = new System.Windows.Shapes.Ellipse();
                                            r.Width = divSize;
                                            r.Height = divSize;
                                            r.Stroke = System.Windows.Media.Brushes.Yellow;
                                            r.StrokeThickness = 0.5;
                                            r.Fill = System.Windows.Media.Brushes.Transparent;
                                            r.SetValue(Canvas.LeftProperty, divs[i, j].Y1 + (double)(divSize / 2));
                                            r.SetValue(Canvas.BottomProperty, divs[i, j].X1 + (double)(divSize / 2));
                                            canvasMap.Children.Add(r);
                                        }
                                        i--;
                                    }
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                    else 
                    {
                    }
                    #endregion
                }
            }
        }
        private void Line_Click(object sender, RoutedEventArgs e)
        {
            if (redFlag1 != null && redFlag2 != null)
            {
                foreach (UIElement ui in canvasMap.Children)
                {
                    if (ui.Uid == redFlag1)
                    {
                        ((Ellipse)ui).Height = 3;
                        ((Ellipse)ui).Width = 3;
                        if (redFlagType1 == "green")
                        {
                            ((Ellipse)ui).Stroke = System.Windows.Media.Brushes.Green;
                            ((Ellipse)ui).Fill = System.Windows.Media.Brushes.Green;
                        }

                        else if (redFlagType1 == "pink")
                        {
                            ((Ellipse)ui).Stroke = System.Windows.Media.Brushes.Pink;
                            ((Ellipse)ui).Fill = System.Windows.Media.Brushes.Pink;
                        }

                        else if (redFlagType1 == "blue")
                        {
                            ((Ellipse)ui).Stroke = System.Windows.Media.Brushes.Blue;
                            ((Ellipse)ui).Fill = System.Windows.Media.Brushes.Blue;
                        }
                        redFlagType1 = "";
                        redFlag1 = null;

                    }
                    if (ui.Uid == redFlag2)
                    {
                        ((Ellipse)ui).Height = 3;
                        ((Ellipse)ui).Width = 3;
                        if (redFlagType2 == "green")
                        {
                            ((Ellipse)ui).Stroke = System.Windows.Media.Brushes.Green;
                            ((Ellipse)ui).Fill = System.Windows.Media.Brushes.Green;
                        }

                        else if (redFlagType2 == "pink")
                        {
                            ((Ellipse)ui).Stroke = System.Windows.Media.Brushes.Pink;
                            ((Ellipse)ui).Fill = System.Windows.Media.Brushes.Pink;
                        }

                        else if (redFlagType2 == "blue")
                        {
                            ((Ellipse)ui).Stroke = System.Windows.Media.Brushes.Blue;
                            ((Ellipse)ui).Fill = System.Windows.Media.Brushes.Blue;
                        }
                        redFlag2 = null;
                        redFlagType2 = "";
                    }
                }
            }
            else
            {
                Polyline pl = (Polyline)e.OriginalSource;
                //LineEntity line = ((List<LineEntity>)(lineEntities.Values.ToList<LineEntity>())).Find(x => x.Id.ToString() == pl.Uid);
                LineEntity line = null;
                foreach (LineEntity le in networkModel.Lines)
                {
                    if (le.Id.ToString() == pl.Uid)
                    {
                        line = le;
                        break;
                    }
                }
                if (line != null)
                {
                    Node pe1 = ((nodes.Values).ToList()).Find(q => q.Id == line.FirstEnd);
                    Node pe2 = ((nodes.Values).ToList()).Find(q => q.Id == line.SecondEnd);
                    foreach (UIElement ui in canvasMap.Children)
                    {
                        if (ui.Uid == pe1.Id.ToString())
                        {

                            if ((((Ellipse)ui).Fill) == System.Windows.Media.Brushes.Pink)
                                redFlagType1 = "pink";
                            else if ((((Ellipse)ui).Fill) == System.Windows.Media.Brushes.Green)
                                redFlagType1 = "green";
                            else if ((((Ellipse)ui).Fill) == System.Windows.Media.Brushes.Blue)
                                redFlagType1 = "blue";
                            ((Ellipse)ui).Fill = System.Windows.Media.Brushes.Red;
                            ((Ellipse)ui).Stroke = System.Windows.Media.Brushes.Red;
                            ((Ellipse)ui).Height = 6;
                            ((Ellipse)ui).Width = 6;
                            redFlag1 = ui.Uid;
                        }
                        else if (ui.Uid == pe2.Id.ToString())
                        {

                            if ((((Ellipse)ui).Fill) == System.Windows.Media.Brushes.Pink)
                                redFlagType2 = "pink";
                            else if ((((Ellipse)ui).Fill) == System.Windows.Media.Brushes.Green)
                                redFlagType2 = "green";
                            else if ((((Ellipse)ui).Fill) == System.Windows.Media.Brushes.Blue)
                                redFlagType2 = "blue";
                            ((Ellipse)ui).Height = 6;
                            ((Ellipse)ui).Width = 6;
                            ((Ellipse)ui).Fill = System.Windows.Media.Brushes.Red;
                            ((Ellipse)ui).Stroke = System.Windows.Media.Brushes.Red;
                            redFlag2 = ui.Uid;
                        }
                    }
                }
            }
            
        }
        public void Edit_Ellipse(object sender, RoutedEventArgs e)
        {
            Canvas c = (Canvas)(((Ellipse)e.OriginalSource).Parent);
           
            Elipse window = new Elipse(new System.Windows.Point(-100,-100), c);
            window.ShowDialog();
            if(ellipseDrawing!=null)
            {
                ellipseDrawing.AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler(Edit_Ellipse));
               // c = (Canvas)ellipseDrawing;
               ((Ellipse)(c.Children[0])).Fill= ((Ellipse)((Canvas)ellipseDrawing).Children[0]).Fill;
                ((Ellipse)(c.Children[0])).Stroke = ((Ellipse)((Canvas)ellipseDrawing).Children[0]).Stroke;
                ((Ellipse)(c.Children[0])).StrokeThickness = ((Ellipse)((Canvas)ellipseDrawing).Children[0]).StrokeThickness;
                for (int i=0;i<canvasMap.Children.Count;i++)
                {
                    if (canvasMap.Children[i].Uid == ellipseDrawing.Uid)
                    {
                        //((Ellipse)(((Canvas)((Canvas)canvasMap.Children[i])).Children[0])).Fill = ((Ellipse)((Canvas)ellipseDrawing).Children[0]).Fill;
                        //((Ellipse)(((Canvas)((Canvas)canvasMap.Children[i])).Children[0])).Stroke = ((Ellipse)((Canvas)ellipseDrawing).Children[0]).Stroke;
                        //((Ellipse)(((Canvas)((Canvas)canvasMap.Children[i])).Children[0])).StrokeThickness = ((Ellipse)((Canvas)ellipseDrawing).Children[0]).StrokeThickness;
                        //canvasMap.Children[i] = c;
                        break;
                    }
                }
                ellipseDrawing = null;
            }
            
        }
        public void Edit_Text(object sender,RoutedEventArgs e)
        {
            TextBlock tb =(TextBlock)e.OriginalSource;
            Label c = new Label();
            c.Uid = tb.Uid;
            c.Margin = tb.Margin;
            c.Content = tb.Text;
            c.Foreground = tb.Foreground;
            c.FontSize = tb.FontSize;
            Text window = new Text(new System.Windows.Point(-100, -100), c);
            window.ShowDialog();
            if (textDrawing != null)
            {
                textDrawing.AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler(Edit_Text));
                for (int i = 0; i < canvasMap.Children.Count; i++)
                {
                    if (canvasMap.Children[i].Uid == textDrawing.Uid)
                    {
                        ((Label)(((Canvas)canvasMap).Children[i])).Foreground = ((Label)textDrawing).Foreground;/////Nece da kastuje u label jer je tipa border 
                        ((Label)(((Canvas)canvasMap).Children[i])).FontSize = ((Label)textDrawing).FontSize;
                        break;
                    }
                }
                textDrawing = null;
            }
        }
        public void Edit_Polygon(object sender, RoutedEventArgs e)
        {
            Canvas c = (Canvas)(((Polygon)e.OriginalSource).Parent);

            PolygonW window = new PolygonW(new System.Windows.Point(-100, -100), c,new System.Collections.Generic.List<System.Windows.Point>());
            window.ShowDialog();
            if (polygonDrawing != null)
            {
                ((Polygon)(c.Children[0])).Fill = ((Polygon)((Canvas)polygonDrawing).Children[0]).Fill;
                ((Polygon)(c.Children[0])).Stroke = ((Polygon)((Canvas)polygonDrawing).Children[0]).Stroke;
                ((Polygon)(c.Children[0])).StrokeThickness = ((Polygon)((Canvas)polygonDrawing).Children[0]).StrokeThickness;
                polygonDrawing.AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler(Edit_Polygon));
                for (int i = 0; i < canvasMap.Children.Count; i++)
                {
                    if (canvasMap.Children[i].Uid == polygonDrawing.Uid)
                    {
                        //((Polygon)((((Canvas)((canvasMap.Children[i])))).Children[0])).Fill = ((Polygon)((Canvas)polygonDrawing).Children[0]).Fill;
                        //((Polygon)(((Canvas)(canvasMap.Children[i])).Children[0])).Stroke = ((Polygon)((Canvas)polygonDrawing).Children[0]).Stroke;
                        //((Polygon)(((Canvas)(canvasMap.Children[i])).Children[0])).StrokeThickness = ((Polygon)((Canvas)polygonDrawing).Children[0]).StrokeThickness;
                        //canvasMap.Children[i] = c;
                        break;
                    }
                }
                polygonDrawing = null;
            }
        }
        private void Ellipse_Checked(object sender, RoutedEventArgs e)
        {
            switch (checkedType)
            {
                case "polygon":
                    polygon.IsChecked = false;
                    break;
                case "text":
                    text.IsChecked = false;
                    break;
            }
            checkedType = "ellipse";
        }
        private void Polygon_Checked(object sender, RoutedEventArgs e)
        {
            switch (checkedType)
            {
                case "ellipse":
                    ellipse.IsChecked = false;
                    break;
                case "text":
                    text.IsChecked = false;
                    break;
            }
            checkedType = "polygon";
        }
        private void Text_Checked(object sender, RoutedEventArgs e)
        {
            switch (checkedType)
            {
                case "polygon":
                    polygon.IsChecked = false;
                    break;
                case "ellipse":
                    ellipse.IsChecked = false;
                    break;
            }
            checkedType = "text";
        }
        private void Ellipse_Unchecked(object sender, RoutedEventArgs e)
        {
            checkedType = "";
        }
        private void Polygon_Unchecked(object sender, RoutedEventArgs e)
        {
            checkedType = "";
        }
        private void Text_Unchecked(object sender, RoutedEventArgs e)
        {
            checkedType = "";
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            itemstoremove = new List<UIElement>();
            foreach (UIElement ui in canvasMap.Children)
            {
                if (ui.Uid.StartsWith("text") || ui.Uid.StartsWith("ellipse") || ui.Uid.StartsWith("polygon"))
                {
                    itemstoremove.Add(ui);
                }
            }
            foreach (UIElement ui in itemstoremove)
            {
                canvasMap.Children.Remove(ui);
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
           if(!redo)
            {
                if (itemstoremove.Count != 0)
                {
                    foreach (UIElement uie in itemstoremove)
                    {
                        canvasMap.Children.Remove(uie);
                    }

                    itemstoremove = new List<UIElement>();
                }
                else if(undoedElement!=null)
                {
                    canvasMap.Children.Add(undoedElement);
                    undoedElement = null;
                }
                redo = true;
                undo = false;
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if(!undo)
            {
                if (itemstoremove.Count != 0)
                {
                    foreach (UIElement uie in itemstoremove)
                    {
                        canvasMap.Children.Add(uie);
                    }


                }
                else
                {
                    UIElement uie = canvasMap.Children[canvasMap.Children.Count - 1];
                    if (uie.Uid.StartsWith("text") || uie.Uid.StartsWith("ellipse") || uie.Uid.StartsWith("polygon"))
                    {
                        canvasMap.Children.Remove(uie);
                    }
                    undoedElement = uie;
                }
               // itemstoremove = new List<UIElement>();
                undo = true;
                redo = false;
            }
            
        }

        private void canvasMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch(checkedType)
            {
                case "ellipse":
                    Elipse ellipseWindow = new Elipse((System.Windows.Point)(e.GetPosition(canvasMap)),null);
                    ellipse.IsChecked = false;
                    ellipseWindow.ShowDialog();
                    if(ellipseDrawing!=null)
                    {
                        UIElement el = ellipseDrawing;
                        el.AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler(Edit_Ellipse));
                        canvasMap.Children.Add(el);
                        ellipseDrawing = null;
                        undo = false;
                        redo = true;
                    }
                    checkedType = "";
                    break;
                case "polygon":
                    System.Windows.Point p = (System.Windows.Point)(e.GetPosition(canvasMap));
                    polygonPoints.Add(p);
                    //PolygonW polygonWindow=new PolygonW((System.Windows.Point)(e.GetPosition(canvasMap)));
                    //polygon.IsChecked = false;
                    //polygonWindow.ShowDialog();
                    //if(polygonDrawing!=null)
                    //{
                    //    UIElement pol = polygonDrawing;
                    //    canvasMap.Children.Add(pol);
                    //    polygonDrawing = null;
                    //    undo = false;
                    //    redo = true;
                    //}
                    break;
                case "text":
                    Text textWindow = new Text((System.Windows.Point)(e.GetPosition(canvasMap)),null);
                    text.IsChecked = false;
                    textWindow.ShowDialog();
                    if(textDrawing!=null)
                    {
                        
                        UIElement l = textDrawing;
                        l.AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler(Edit_Text));
                        canvasMap.Children.Add(l);
                        textDrawing = null;
                        undo = false;
                        redo = true;

                    }
                    checkedType = "";
                    break;
            }
            
        }

        private void canvasMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(checkedType=="polygon" && polygonPoints.Count>=3)
            {
                PolygonW polygonWindow = new PolygonW((System.Windows.Point)(e.GetPosition(canvasMap)), null, polygonPoints);
                polygon.IsChecked = false;

                polygonWindow.ShowDialog();
                if (polygonDrawing != null)
                {

                    UIElement pol = polygonDrawing;
                    pol.AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler(Edit_Polygon));
                    canvasMap.Children.Add(pol);
                    polygonDrawing = null;
                    undo = false;
                    redo = true;
                }
                polygonPoints.Clear();
            }
        }

        //From UTM to Latitude and longitude in decimal

        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
		{
			bool isNorthHemisphere = true;

			var diflat = -0.00066286966871111111111111111111111111;
			var diflon = -0.0003868060578;

			var zone = zoneUTM;
			var c_sa = 6378137.000000;
			var c_sb = 6356752.314245;
			var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
			var e2cuadrada = Math.Pow(e2, 2);
			var c = Math.Pow(c_sa, 2) / c_sb;
			var x = utmX - 500000;
			var y = isNorthHemisphere ? utmY : utmY - 10000000;

			var s = ((zone * 6.0) - 183.0);
			var lat = y / (c_sa * 0.9996);
			var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
			var a = x / v;
			var a1 = Math.Sin(2 * lat);
			var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
			var j2 = lat + (a1 / 2.0);
			var j4 = ((3 * j2) + a2) / 4.0;
			var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
			var alfa = (3.0 / 4.0) * e2cuadrada;
			var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
			var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
			var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
			var b = (y - bm) / v;
			var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
			var eps = a * (1 - (epsi / 3.0));
			var nab = (b * (1 - epsi)) + lat;
			var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
			var delt = Math.Atan(senoheps / (Math.Cos(nab)));
			var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

			longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
			latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
		}
	}
}
