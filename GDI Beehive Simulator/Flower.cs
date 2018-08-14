using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDI_Beehive_Simulator
{
    using System.Drawing;

    [Serializable]
    public class Flower
    {
        public Point Location { get; private set; }
        public int Age { get; private set; }
        public bool Alive { get; private set; }
        public  double Nectar { get; private set; }

        public double NectarHarvested { get; set; }

        //constants
        private int lifespan;
        public const int LifeSpanMax = 30000;
        public const int LifeSpanMin = 15000;
        public const double InitialNectar = 1.5;
        public const double MaxNectar = 5;
        public const double NectarAddedPerTurn = 0.01;
        public const double NectarGatheredPerTurn = 0.3;

        public Flower(Point location, Random random)
        {
            Location = location;
            Age = 0;
            Alive = true;
            Nectar = InitialNectar;
            NectarHarvested = 0;
            lifespan = random.Next(LifeSpanMin, LifeSpanMax + 1);
        }

        public double HarvestNectar()
        {
            if (NectarGatheredPerTurn > Nectar) return 0;
            else
            {
                Nectar -= NectarGatheredPerTurn;
                NectarHarvested += NectarGatheredPerTurn;
                return NectarGatheredPerTurn;
            }
        }

        public void Go()
        {
            Age++;
            if (Age > lifespan)
                Alive = false;
            else
            {
                Nectar += NectarAddedPerTurn;
                if (Nectar > MaxNectar)
                    Nectar = MaxNectar;
            }
        }
    }
}
