using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphic
{
    public partial class Form1 : Form
    {
        double R = 0;
        double r = 0;
        double arm = 0;

        int scale = 10;

        PointF[] epicycle;
        PointF[] circle1;
        PointF[] circle2;
        PointF[] traectory;

        displayedValues values;

        public Form1()
        {
            InitializeComponent();
            trackBar1.TickFrequency = 10;
            trackBar1.Minimum = 1;
            trackBar1.Maximum = 100;
        }

        private PointF[] getEpiCycle(double R, double r, double arm)
        {
            if (arm != 0)
            {
                R = (float)((r + R) - arm);
                r = (float)arm;
            }

            int size = 360 * (int)r;
            if(R/r >= 1)
            {
                size = 360 * (int)(R / r);
            }

            PointF[] points = new PointF[0];
            try
            {
                points = new PointF[size];
            }
            catch
            {
                MessageBox.Show("Ты б ещё 100 нулей наквасил?!");
            }

            double k = R / r;
            double a = r * (k + 1);
            for (int i = 0; i < points.Length; i++)
            {
                    double t = i * Math.PI / 180.0;

                    //Эпициклоида
                    double x = (a * (Math.Cos(t) - Math.Cos((k + 1.0) * t) / (k + 1.0)));
                    double y = (a * (Math.Sin(t) - Math.Sin((k + 1.0) * t) / (k + 1.0)));

                    //Гипоциклоида
                    //double x = (a * (Math.Cos(t) + Math.Cos((k - 1.0) * t) / (k - 1.0)));
                    //double y = (a * (Math.Sin(t) - Math.Sin((k - 1.0) * t) / (k - 1.0)));
                    points[i] = new PointF((float)x, (float)y);
                
            }
            return points;
        }

        private PointF[] getCircle(double r, double shift)
        {
            PointF[] circle = new PointF[360];

            for (int i = 0; i < circle.Length; i++)
            {
                double t = i * Math.PI / 180.0;

                double x = (r * Math.Cos(t)) + shift;
                double y = r * Math.Sin(t);

                circle[i] = new PointF((float)x, (float)y);
            }
            return circle;
        }

        private void reduceScale()
        {
            double min;
            if (R < r) min = R;
            else min = r;
            if (arm < min) min = arm;

            int order = 1;
            while (min > 10)
            {
                min /= 10;
                order++;
            }

            R = (R/Math.Pow(10, order)) * scale;
            r = (r/Math.Pow(10, order)) * scale;
            arm = (arm/Math.Pow(10, order)) * scale;

            //return Convert.ToString(R) + " " + Convert.ToString(r) + " " + Convert.ToString(arm);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.R = Convert.ToDouble(textBox1.Text);
                this.r = Convert.ToDouble(textBox2.Text);
                if (textBox3.Text != "")
                {
                    this.arm = Convert.ToDouble(textBox3.Text);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат ввода!");
            }

            values = new displayedValues(R, r, arm);

            reduceScale();
            
            epicycle = getEpiCycle(this.R, this.r, this.arm);
            circle1 = getCircle(this.R, 0);
            circle2 = getCircle(this.r, this.R + this.r);
            traectory = getCircle(this.R + this.r, 0);

            //MessageBox.Show(Convert.ToString(reduceScale()));

            pictureBox1.Refresh();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "R - радиус стационарной окружности\n r - радиус катящейся окружности\n x - расстояние до точки Х",
            "Инструкция",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics grafen = e.Graphics;

            Pen pendefault = new Pen(Color.Gray, 1);
            Pen penCrcle = new Pen(Color.DarkRed, 1);
            Pen penTraectory = new Pen(Color.Yellow, 1);
            Pen penEpicycle = new Pen(Color.DeepSkyBlue, 1);

            grafen.DrawLine(pendefault, new Point(0, pictureBox1.Height / 2), new Point(pictureBox1.Height, pictureBox1.Height / 2));
            grafen.DrawLine(pendefault, new Point(pictureBox1.Width / 2, 0), new Point(pictureBox1.Width / 2, pictureBox1.Width));

            try
            {
                for (int i = 0; i < circle1.Length; i++)
                {
                    grafen.DrawRectangle(penCrcle, (float)(circle1[i].X + pictureBox1.Height / 2), (float)(-1 * (circle1[i].Y - pictureBox1.Width / 2)), 1, 1);
                    grafen.DrawRectangle(penCrcle, (float)(circle2[i].X + pictureBox1.Height / 2), (float)(-1 * (circle2[i].Y - pictureBox1.Width / 2)), 1, 1);
                    grafen.DrawRectangle(penTraectory, (float)(traectory[i].X + pictureBox1.Height / 2), (float)(-1 * (traectory[i].Y - pictureBox1.Width / 2)), 1, 1);
                }
                for (int i = 0; i < epicycle.Length; i++)
                {
                    grafen.DrawRectangle(penEpicycle, (float)(epicycle[i].X + pictureBox1.Height / 2), (float)(-1 * (epicycle[i].Y - pictureBox1.Width / 2)), 1, 1);

                }
                grafen.DrawString(values.radiusR, new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular),
                    new SolidBrush(Color.Black), (float)this.R + (pictureBox1.Height / 2), pictureBox1.Width / 2);
                grafen.DrawString(values.radiusr, new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular),
                    new SolidBrush(Color.Black), (float)this.R + 2*(float)this.r + pictureBox1.Height / 2, (pictureBox1.Width / 2)-10);
            }
            catch
            {
                grafen.DrawRectangle(penCrcle, 0 + pictureBox1.Height / 2, (-1 * (0 - pictureBox1.Width / 2)), 1, 1);
            }
        }

        struct displayedValues
        {
            public String radiusR;
            public String radiusr;

            public displayedValues(double R, double r, double arm)
            {
                radiusR = Convert.ToString(R);
                radiusr = Convert.ToString(r + R);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            scale = trackBar1.Value;
            label4.Text = "Масштаб - " + Convert.ToString(scale);

            reduceScale();
            epicycle = getEpiCycle(this.R, this.r, this.arm);
            circle1 = getCircle(this.R, 0);
            circle2 = getCircle(this.r, this.R + this.r);
            traectory = getCircle(this.R + this.r, 0);
            pictureBox1.Refresh();
        }
    }
}
