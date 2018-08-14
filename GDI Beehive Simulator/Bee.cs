using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDI_Beehive_Simulator
{
    using System.Drawing;

    [Serializable]
    public class Bee
    {
        private const double HoneyConsumed = 0.5;
        private const int MoveRate = 3;
        private const double MinimunFlowerNectar = 1.5;
        private const int CarreerSpan = 1000;

        public int Age { get; private set; }
        public bool InsideHive { get; private set; }
        public double NectarCollected { get; private set; }

        private Point location;
        public Point Location { get { return location; } }

        private int ID;
        private Flower destinationFlower;
        public BeeState CurrentState;

        private Hive hive;
        private World world;

        [NonSerialized]
        public BeeMessage MessageSender;

        public Bee(int id, Point location, Hive hive, World world)
        {
            this.ID = id;
            Age = 0;
            this.location = location;
            InsideHive = true;
            CurrentState = BeeState.Idle;
            destinationFlower = null;
            NectarCollected = 0;

            this.hive = hive;
            this.world = world;
        }

        public void Go(Random random)
        {
            Age++;
            BeeState oldState = CurrentState;
            switch (CurrentState)
            {
                case BeeState.Idle:
                    if (Age > CarreerSpan)
                    {
                        CurrentState = BeeState.Retired;
                    }
                    else if(world.Flowers.Count >0 &&
                        hive.ConsumeHoney(HoneyConsumed))
                    {
                        Flower flower = world.Flowers[random.Next(world.Flowers.Count)];
                        if(flower.Nectar >= MinimunFlowerNectar && flower.Alive)
                        {
                            destinationFlower = flower;
                            CurrentState = BeeState.FlyingToFlower;
                            


                        }
                    }
                    break;
                case BeeState.FlyingToFlower:
                    if (!world.Flowers.Contains(destinationFlower))
                        CurrentState = BeeState.ReturningToHive;
                    else if (InsideHive)
                    {
                        if (MoveTowardsLocation(hive.GetLocation("Exit")))
                        {
                            InsideHive = false;
                            location = hive.GetLocation("Entrance");
                        }
                    }
                    else
                    {
                        if (MoveTowardsLocation(destinationFlower.Location))
                            CurrentState = BeeState.GatheringNectar;
                        
                    }
                    break;
                case BeeState.GatheringNectar:
                    double nectar = destinationFlower.HarvestNectar();
                    if (nectar > 0)
                        NectarCollected += nectar;
                    else CurrentState = BeeState.ReturningToHive;
                    break;
                case BeeState.ReturningToHive:
                    if (!InsideHive)
                    {
                        if (MoveTowardsLocation(hive.GetLocation("Entrance")))
                        {
                            InsideHive = true;
                            location = hive.GetLocation("Exit");
                        }
                    }
                    else
                    {
                        if (MoveTowardsLocation(hive.GetLocation("HoneyFactory")))
                            CurrentState = BeeState.MakingHoney;
                    }
                    break;
                case BeeState.MakingHoney:
                    if(NectarCollected < HoneyConsumed)
                    {
                        NectarCollected = 0;
                        CurrentState = BeeState.Idle;
                    }
                    else
                    {
                        if (hive.AddHoney(HoneyConsumed))
                            NectarCollected -= HoneyConsumed;
                        else
                            NectarCollected = 0;
                    }
                    break;
                case BeeState.Retired:
                    // Do Nothing!
                    break;
            }
            if (oldState != CurrentState && MessageSender != null)
                MessageSender(ID, CurrentState.ToString());
        }

        public bool MoveTowardsLocation(Point destination)
        {
            if (Math.Abs(destination.X - location.X) <= MoveRate &&
                Math.Abs(destination.Y - location.Y) <= MoveRate)
                return true;

            if (destination.X > location.X)
                location.X += MoveRate;
            else if (destination.X < location.X)
                location.X -= MoveRate;

            if (destination.Y > location.Y)
                location.Y += MoveRate;
            else if (destination.Y < location.Y)
                location.Y -= MoveRate;

            return false;
        }
    }
}
