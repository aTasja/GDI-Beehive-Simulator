using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDI_Beehive_Simulator
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Click(object sender, EventArgs e)
        {
            using (Graphics g = CreateGraphics())
            {
                g.FillRectangle(Brushes.SkyBlue, ClientRectangle);
                g.DrawImage(Properties.Resources.Bee_animation_1, 50, 20, 75, 75);
                g.DrawImage(Properties.Resources.Flower, 10, 130, 100, 150);
                using (Pen thickBlackPen = new Pen(Brushes.Black, 3.0F))
                {
                    g.DrawLines(thickBlackPen, new Point[]
                    {
                        new Point (130,110), new Point(120,160), new Point(155,163)
                    });
                    g.DrawCurve(thickBlackPen, new Point[]
                    {
                        new Point(120,160), new Point(175,120), new Point(215,70)
                    });
                }
                using (Font font = new Font("Arial", 16, FontStyle.Italic))
                {
                    SizeF size = g.MeasureString("Nectar here", font);
                    g.DrawString("Nectar here", font, Brushes.Red, new Point(
                        215 - (int)size.Width / 2, 70 - (int)size.Height));
                }
            }

            
        }

        public void DrawBee(Graphics g, Rectangle rect)
        {
            g.DrawImage(Properties.Resources.Bee_animation_1, rect);
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            
                g.DrawImage(Properties.Resources.Hive__inside_,
                    -Width, -Height, Width * 2, Height * 2);
                Size size = new Size(Width / 5, Height / 5);
                DrawBee(g, new Rectangle(
                    new Point(Width / 2 - 50, Height / 2 - 40), size));
                DrawBee(g, new Rectangle(
                    new Point(Width / 2 - 20, Height / 2 - 60), size));
                DrawBee(g, new Rectangle(
                    new Point(Width / 2 - 80, Height / 2 - 30), size));
                DrawBee(g, new Rectangle(
                    new Point(Width / 2 - 90, Height / 2 - 80), size));
            
        }
    }
}
