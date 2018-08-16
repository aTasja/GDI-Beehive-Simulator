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

    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Drawing.Printing;

    public partial class Form1 : Form
    {
        World world;
        private Random random = new Random();
        private DateTime start = DateTime.Now;
        private DateTime end;

        private int framesRun = 0;

        private HiveForm hiveForm = new HiveForm();
        private FieldForm fieldForm = new FieldForm();
        private Renderer renderer;
        //private Form2 form2 = new Form2();

        public Form1()
        {
            InitializeComponent();
            world = new World(new BeeMessage(SendMessage));

            timer1.Interval = 50;
            timer1.Tick += new EventHandler(RunFrame);
            timer1.Enabled = false;
            UpdateStats(new TimeSpan());

            MoveChildForms();
            hiveForm.Show(this);
            fieldForm.Show(this);
            ResetSimulator();

            //form2.Show(this);

        }

        private void MoveChildForms()
        {
            hiveForm.Location = new Point(Location.X + Width + 10, Location.Y);
            fieldForm.Location = new Point(Location.X, Location.Y + Math.Max(Height, hiveForm.Height) + 10);
        }

        private void UpdateStats(TimeSpan frameDutation)
        {
            Bees.Text = world.Bees.Count.ToString();
            Flowers.Text = world.Flowers.Count.ToString();
            HoneyInHive.Text = String.Format("{0:f3}", world.Hive.Honey);
            double nectar = 0;
            foreach (Flower flower in world.Flowers)
                nectar += flower.Nectar;
            NectarInFlowers.Text = String.Format("{0:f3}", nectar);
            FramesRun.Text = framesRun.ToString();
            double milliSeconds = frameDutation.TotalMilliseconds;
            if (milliSeconds != 0.0)
                FrameRate.Text = string.Format("{0:f0} ({1:f1}ms)",
                    1000 / milliSeconds, milliSeconds);
            else
                FrameRate.Text = "N/A";
        }

        public void RunFrame(object sender, EventArgs e)
        {
            framesRun++;
            world.Go(random);
            end = DateTime.Now;
            TimeSpan frameDuration = end - start;
            start = end;
            UpdateStats(frameDuration);
            hiveForm.Invalidate();
            fieldForm.Invalidate();

        }

        private void Form1_Move(object sender, EventArgs e)
        {
            MoveChildForms();
        }

        private void ResetSimulator()
        {
            framesRun = 0;
            world = new World(new BeeMessage(SendMessage));
            renderer = new Renderer(world, hiveForm, fieldForm);
        }

        private void StartButton_Click_1(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                toolStrip1.Items[0].Text = "Resume similation";
                timer1.Stop();
                statusStrip1.Items[0].Text = "Simulation paused";
            }
            else
            {
                toolStrip1.Items[0].Text = "Pause simulation";
                timer1.Start();
                statusStrip1.Items[0].Text = "Simulation run";
            }
        }

        private void ResetButton_Click_1(object sender, EventArgs e)
        {
            framesRun = 0;
            world = new World(new BeeMessage(SendMessage));
            UpdateStats(new TimeSpan());
            //renderer.Reset();
            ResetSimulator();
            if (!timer1.Enabled)
                toolStrip1.Items[0].Text = "Start simulation";
        }


        private void SendMessage(int ID, string Message)
        {
            statusStrip1.Items[0].Text = "Bee #" + ID + ": " + Message;



            var beeGroups =
                            from bee in world.Bees
                            group bee by bee.CurrentState
                            into beeGroup
                            orderby beeGroup.Key
                            select beeGroup;
            listBox1.Items.Clear();
            
            foreach (var group in beeGroups) {
                string s;
                if (group.Count() == 1)
                    s = "";
                else
                    s = "s";
                listBox1.Items.Add(group.Key.ToString() + ": " +
                    group.Count() + " bee" + s);
                if(group.Key == BeeState.Idle &&
                    group.Count() == world.Bees.Count() &&
                    framesRun > 0)
                {
                    listBox1.Items.Add("Simulation ended: all bees are idle");
                    toolStrip1.Items[0].Text = "Simulation ended";
                    statusStrip1.Items[0].Text = "Simulation ended";
                    timer1.Enabled = false;
                }
            }

        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            World currentWorld = world;
            int currenrFramesRun = framesRun;

            //renderer.Reset();
            //renderer = new Renderer(world, hiveForm, fieldForm);

            bool enabled = timer1.Enabled;
            if (enabled)
                timer1.Stop();

            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Simulator File (*.bees)|*.bees";
            openDialog.CheckPathExists = true;
            openDialog.CheckFileExists = true;
            openDialog.Title = "Choose a file with a simulation to load";
            if(openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (Stream input = File.OpenRead(openDialog.FileName))
                    {
                        world = (World)bf.Deserialize(input);
                        framesRun = (int)bf.Deserialize(input);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to read the somulator file\r\n" + ex.Message,
                        "Bee Simulator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    world = currentWorld;
                    framesRun = currenrFramesRun;
                }
            }

            world.Hive.MessageSender = new BeeMessage(SendMessage);
            foreach (Bee bee in world.Bees)
                bee.MessageSender = new BeeMessage(SendMessage);
            if (enabled)
                timer1.Start();           
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            bool enabled = timer1.Enabled;
            if (enabled)
                timer1.Stop();

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Simulator File (*.bees)|*.bees";
            saveDialog.CheckPathExists = true;
            saveDialog.Title = "Choose a file to seve the current simulation";
            if(saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (Stream output = File.OpenWrite(saveDialog.FileName))
                    {
                        bf.Serialize(output, world);
                        bf.Serialize(output, framesRun);
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show("Unable to save the simulator file\r\n" + ex.Message,
                        "Bee Simulator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (enabled)
                timer1.Start();
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            renderer.AnimateBees();
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            bool stoppedTimer = false;
            if (timer1.Enabled)
            {
                timer1.Stop();
                stoppedTimer = true;
            }
            PrintDocument document = new PrintDocument();
            document.PrintPage += Document_PrintPage; ;
            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = document;
            preview.ShowDialog(this);
            if (stoppedTimer)
                timer1.Start();
        }

        private void Document_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Size stringSize;
            using (Font arial24bold = new Font("Arial", 24, FontStyle.Bold))
            {
                stringSize = Size.Ceiling(g.MeasureString("Bee Simulator", arial24bold));
                g.FillEllipse(Brushes.Gray, new Rectangle(e.MarginBounds.X + 2, e.MarginBounds.Y + 2,
                    stringSize.Width + 30, stringSize.Height + 30));
                g.FillEllipse(Brushes.Black, new Rectangle(e.MarginBounds.X, e.MarginBounds.Y,
                    stringSize.Width + 30, stringSize.Height + 30));
                g.DrawString("Bee Simulator", arial24bold, Brushes.White,
                    e.MarginBounds.X + 17, e.MarginBounds.Y + 17);
                g.DrawString("Bee Simulator", arial24bold, Brushes.White,
                    e.MarginBounds.X + 15, e.MarginBounds.Y + 15);
            }
            int tableX = e.MarginBounds.X + (int)stringSize.Width + 50;
            int tableWidth = e.MarginBounds.X + e.MarginBounds.Width - tableX - 20;
            int firstColumnX = tableX + 2;
            int secondColumlnX = tableX + (tableWidth / 2) + 5;
            int tableY = e.MarginBounds.Y;
            //PrintTableRow(g, tableX, tableWidth, firstColumnX, secondColumlnX,
            //    tableY, "Bees", Bees.Text);

            tableY = PrintTableRow(g, tableX, tableWidth, firstColumnX, secondColumlnX,
                tableY, "Bees", Bees.Text);
            tableY = PrintTableRow(g, tableX, tableWidth, firstColumnX, secondColumlnX,
                tableY, "Flowers", Flowers.Text);
            tableY = PrintTableRow(g, tableX, tableWidth, firstColumnX, secondColumlnX,
                tableY, "Honey in Hive", HoneyInHive.Text);
            tableY = PrintTableRow(g, tableX, tableWidth, firstColumnX, secondColumlnX,
                tableY, "Nectar in Flowers", NectarInFlowers.Text);
            tableY = PrintTableRow(g, tableX, tableWidth, firstColumnX, secondColumlnX,
                tableY, "Frames Run", FramesRun.Text);
            tableY = PrintTableRow(g, tableX, tableWidth, firstColumnX, secondColumlnX,
                tableY, "Frame Rate", FrameRate.Text);

            g.DrawRectangle(Pens.Black, tableX, e.MarginBounds.Y,
                tableWidth, tableY - e.MarginBounds.Y);
           
            g.DrawLine(Pens.Black, secondColumlnX, e.MarginBounds.Y, secondColumlnX, tableY);

            using (Pen blackPen = new Pen(Brushes.Black, 2))
            using (Bitmap hiveBitmap = new Bitmap(hiveForm.ClientSize.Width, hiveForm.ClientSize.Height))
            using (Bitmap fieldBitmap = new Bitmap(fieldForm.ClientSize.Width, fieldForm.ClientSize.Height))
            {
                using (Graphics hiveGraphics = Graphics.FromImage(hiveBitmap))
                {
                    renderer.PaintHive(hiveGraphics);
                }
                int hiveWidth = e.MarginBounds.Width / 2;
                float ratio = (float)hiveBitmap.Height / (float)hiveBitmap.Width;
                int hiveHeight = (int)(hiveWidth * ratio);
                int hiveX = e.MarginBounds.X + (e.MarginBounds.Width - hiveWidth) / 2;
                int hiveY = e.MarginBounds.Height / 3;
                g.DrawImage(hiveBitmap, hiveX, hiveY, hiveWidth, hiveHeight);
                g.DrawRectangle(blackPen, hiveX, hiveY, hiveWidth, hiveHeight);

                using (Graphics fieldGraphics = Graphics.FromImage(fieldBitmap))
                {
                    renderer.PaintField(fieldGraphics);
                }
                int fieldWidth = e.MarginBounds.Width;
                ratio = (float)fieldBitmap.Height / (float)fieldBitmap.Width;
                int fieldHeight = (int)(fieldWidth * ratio);
                int fieldX = e.MarginBounds.X;
                int fieldY = e.MarginBounds.Y + e.MarginBounds.Height - fieldHeight;
                g.DrawImage(fieldBitmap, fieldX, fieldY, fieldWidth, fieldHeight);
                g.DrawRectangle(blackPen, fieldX, fieldY, fieldWidth, fieldHeight);
            }

        }

        private int PrintTableRow(Graphics printGraphics, 
            int tableX, int tableWidth, int firstColumnX, int secondColumnX,
            int tableY, string firstColumn, string secondColumn)
        {
            Font arial2 = new Font("Arial", 12);
            Size stringSize = Size.Ceiling(printGraphics.MeasureString(firstColumn, arial2));
            tableY += 2;
            printGraphics.DrawString(firstColumn, arial2, Brushes.Black, firstColumnX, tableY);
            printGraphics.DrawString(secondColumn, arial2, Brushes.Black, secondColumnX, tableY);
            tableY += (int)stringSize.Height + 2;
            printGraphics.DrawLine(Pens.Black, tableX, tableY, tableX + tableWidth, tableY);
            arial2.Dispose();
            return tableY;
        }
    }
}
