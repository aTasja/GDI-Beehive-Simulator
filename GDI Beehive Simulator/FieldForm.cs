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
    public partial class FieldForm : Form
    {
        public Renderer Renderer { get; set; }

        public FieldForm()
        {
            InitializeComponent();
        }

        private void FieldForm_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show(e.Location.ToString());
        }


        private void FieldForm_Paint(object sender, PaintEventArgs e)
        {
            Renderer.PaintField(e.Graphics);
        }

        /*
        private void button1_Click(object sender, EventArgs e)
        {
            PictureBox beePicture = new PictureBox();
            beePicture.Location = new Point(10, 10);
            beePicture.Size = new Size(100, 100);
            beePicture.BorderStyle = BorderStyle.FixedSingle;
            beePicture.Image = Renderer.ResizeImage(Properties.Resources.Bee_animation_1, 80, 40);
            Controls.Add(beePicture);
        }
        */

    }
}
