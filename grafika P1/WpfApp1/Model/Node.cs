using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class Node : PowerEntity
    {
        double x2;
        double y2;
        public Node(double x, double y,double x2,double y2,string name,long id) : base(id,name,x,y)
        {
            this.X2 = x2;
            this.Y2 = y2;
        }

        public double X2 { get => x2; set => x2 = value; }
        public double Y2 { get => y2; set => y2 = value; }
    }
}
