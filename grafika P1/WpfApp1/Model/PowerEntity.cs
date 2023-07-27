using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class PowerEntity
    {
        private long id;
        private string name;
        private double x;
        private double y;

        public PowerEntity()
        {

        }

        public PowerEntity(long id, string name, double x, double y)
        {
            this.id = id;
            this.name = name;
            this.x = x;
            this.y = y;
        }

        public long Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }
    }
}
