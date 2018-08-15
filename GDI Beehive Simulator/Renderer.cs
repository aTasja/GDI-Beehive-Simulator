using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDI_Beehive_Simulator
{
    using System.Drawing;
    using System.Windows.Forms;

    public class Renderer
    {
        private World world;
        private HiveForm hiveForm;
        private FieldForm fieldForm;

        private Bitmap HiveInside; 
        private Bitmap HiveOutside; 
        private Bitmap Flower; 

        private Bitmap[] BeeAnimationLarge ;
        private Bitmap[] BeeAnimationSmall ;

        //private List<Bee> retiredBees = new List<Bee>();

        public Renderer(World world, HiveForm hiveForm, FieldForm fieldForm)
        {
            this.world = world;
            this.hiveForm = hiveForm;
            this.fieldForm = fieldForm;

            hiveForm.Renderer = this;
            fieldForm.Renderer = this;

            InitializeImages();

        }

        void InitializeImages()
        {
            HiveInside = ResizeImage(Properties.Resources.Hive__inside_, 
                hiveForm.ClientRectangle.Width, hiveForm.ClientRectangle.Height);
            HiveOutside = ResizeImage(Properties.Resources.Hive__outside_, 85,100);
            Flower = ResizeImage(Properties.Resources.Flower, 75,75);

            BeeAnimationSmall = new Bitmap[4];
            BeeAnimationSmall[0] = ResizeImage(Properties.Resources.Bee_animation_1, 20, 20);
            BeeAnimationSmall[1] = ResizeImage(Properties.Resources.Bee_animation_2, 20, 20);
            BeeAnimationSmall[2] = ResizeImage(Properties.Resources.Bee_animation_3, 20, 20);
            BeeAnimationSmall[3] = ResizeImage(Properties.Resources.Bee_animation_4, 20, 20);

            BeeAnimationLarge = new Bitmap[4];
            BeeAnimationLarge[0] = ResizeImage(Properties.Resources.Bee_animation_1, 40, 40);
            BeeAnimationLarge[1] = ResizeImage(Properties.Resources.Bee_animation_2, 40, 40);
            BeeAnimationLarge[2] = ResizeImage(Properties.Resources.Bee_animation_3, 40, 40);
            BeeAnimationLarge[3] = ResizeImage(Properties.Resources.Bee_animation_4, 40, 40);

        }

        public static Bitmap ResizeImage(Bitmap picture, int width, int height)
        {
            Bitmap resizedPicture = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedPicture))
            {
                graphics.DrawImage(picture, 0, 0, width, height);
            }
            return resizedPicture;
        }
        
        private int cell = 0;
        private int frame = 0;

        public void AnimateBees()
        {
            frame++;
            if (frame >= 6)
                frame = 0;
            switch (frame)
            {
                case 0: cell = 0; break;
                case 1: cell = 1; break;
                case 2: cell = 2; break;
                case 3: cell = 3; break;
                case 4: cell = 2; break;
                case 5: cell = 1; break;
                default: cell = 0; break;
            }
            hiveForm.Invalidate();
            fieldForm.Invalidate();
        }

        public void PaintHive(Graphics g)
        {
            g.FillRectangle(Brushes.SkyBlue, new Rectangle(0, 0, hiveForm.Width, hiveForm.Height));
            g.DrawImageUnscaled(HiveInside, 0, 0);

            foreach (Bee bee in world.Bees)
            {
                if (bee.InsideHive)
                    g.DrawImageUnscaled(BeeAnimationLarge[cell], 
                        bee.Location.X, bee.Location.Y);
            }
                
        }

        public void PaintField(Graphics g)
        {
            using(Pen brownPen = new Pen(Color.Brown, 6.0F))
            {
                g.FillRectangle(Brushes.SkyBlue, 0, 0, fieldForm.ClientSize.Width, fieldForm.ClientSize.Height / 2);
                g.FillRectangle(Brushes.Green, 0, fieldForm.ClientSize.Height / 2, fieldForm.ClientSize.Width, fieldForm.ClientSize.Height);
                g.FillEllipse(Brushes.Yellow, 50,15,70,70);
                g.DrawLine(brownPen, new Point(593,0), new Point(593,30));
                g.DrawImageUnscaled(HiveOutside, 550,20);
                foreach (Flower flower in world.Flowers)
                    g.DrawImageUnscaled(Flower, flower.Location.X, flower.Location.Y);
                foreach(Bee bee in world.Bees)
                {
                    if (!bee.InsideHive)
                        g.DrawImageUnscaled(BeeAnimationSmall[cell], bee.Location.X, bee.Location.Y);
                }

            }
            
            
            
            
            
        }


        
    }
}
