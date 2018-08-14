using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDI_Beehive_Simulator
{
    using System.Drawing;

    [Serializable]
    public class Hive
    {
        public double Honey { get; private set; }

        private Dictionary<string, Point> locations;
        private int beeCount = 0;

        private const int InitialBees = 6;
        private const int MaximumBees = 8;
        private const double InitialHoney = 3.2;
        private const double MaximumHoney = 15.0;
        private const double NectarHoneyRatio = 0.25; // 4 units of nectar needs to produse 1 unit of honey
        private const double MinimumHoneyToBirthBee = 4;

        private World world;

        [NonSerialized]
        public BeeMessage MessageSender;

        public Hive(World world, BeeMessage MessageSender)
        {
            this.world = world;
            Honey = InitialHoney;
            this.MessageSender = MessageSender;
            InitializeLocations();
            Random random = new Random();
            for (int i = 0; i < InitialBees; i++)
                AddBee(random);
            
        }

        public void InitializeLocations()
        {
            locations = new Dictionary<string, Point>();
            locations.Add("Entrance", new Point(615, 102));
            locations.Add("Nursery", new Point(135, 230));
            locations.Add("HoneyFactory", new Point(244, 119));
            locations.Add("Exit", new Point(274, 281));
        }

        public bool AddHoney(double nectar)
        {
            //throw new NotImplementedException();
            double honeyToAdd = nectar * NectarHoneyRatio;
            if (honeyToAdd + Honey > MaximumHoney)
                return false;
            Honey += honeyToAdd;
            return true;
        }

        public bool ConsumeHoney(double amount)
        {
            //throw new NotImplementedException();
            if (amount > Honey)
                return false;
            else
            {
                Honey -= amount;
                return true;
            }
        }

        public void AddBee(Random random)
        {
            //throw new NotImplementedException();
            beeCount++;
            
            int r1 = random.Next(100) - 50;
            int r2 = random.Next(100) - 50;
            Point startPoint = new Point(locations["Nursery"].X + r1,
                                            locations["Nursery"].Y + r2);
            Bee newBee = new Bee(beeCount, startPoint, this, world);
            newBee.MessageSender += this.MessageSender;
            world.Bees.Add(newBee);
            
            
        }

        public void Go(Random random)
        {
            //throw new NotImplementedException();
            if (world.Bees.Count < MaximumBees && 
                Honey > MinimumHoneyToBirthBee && 
                random.Next(10) == 1)
                AddBee(random);
        }

        public Point GetLocation(string location)
        {
            if (locations.Keys.Contains(location))
                return locations[location];
            else
                throw new ArgumentException("Unknown location: " + location);
        }
    }
}
