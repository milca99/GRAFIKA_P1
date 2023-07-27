using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class Div
    {
        double x1;
        double x2;
        double y1;
        double y2;
        bool free;
        int hasLineJ;
        int hasLineI;

        public Div(double x1, double x2, double y1, double y2, bool free)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            this.free = free;
            this.HasLineJ = 0;
            this.HasLineI = 0;
        }

        public double X1 { get => x1; set => x1 = value; }
        public double X2 { get => x2; set => x2 = value; }
        public double Y1 { get => y1; set => y1 = value; }
        public double Y2 { get => y2; set => y2 = value; }
        public bool Free { get => free; set => free = value; }
        public int HasLineJ { get => hasLineJ; set => hasLineJ = value; }
        public int HasLineI { get => hasLineI; set => hasLineI = value; }

        public bool ChechIfFits(double x,double y)
        {
            if (x <= X2 && x >= X1 && y <= Y2 && y >= Y1)
                return true;
            return false;
        }
        public bool Occupy()
        {
            if(Free)
            {
                Free = false;
                return true;
            }
            return false;
        }
    }
}
