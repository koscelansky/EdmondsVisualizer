using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace Edmonds
{
    public partial class Visualizer : Form
    {

        public Visualizer()
        {
            InitializeComponent();
        }

        private EdmondsVisualizer _v = null;
        private List<Point> _points = new List<Point>();

        private void Visualizer_Load(object sender, EventArgs e)
        {
            Timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((_v != null) && (!_v.NextIter()))
            { 
                foreach (Edge ee in _v.Edges)
                {
                    MainTextBox.AppendText(ee.ToString());
                }
                if (!_v.IsMatching)
                {
                    MainTextBox.AppendText("FAIL");
                }
                Timer.Stop();
            }
            MainPictureBox.Invalidate();
        }

        private void MainPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (_v != null)
            {
                _v.Draw(e.Graphics);
            }
            else
            {
                foreach (Point p in _points)
                {
                    e.Graphics.DrawEllipse(Pens.Black, p.X - 1, p.Y - 1, 3, 3);
                }
            }

            if (ShowVertces.Checked)
            {
                for (int i = 0; i< _points.Count; ++i)
                {
                    Point p = _points[i];
                    e.Graphics.DrawString(i.ToString(), new Font("Arial", 10), Brushes.Black, new Point(p.X + 3, p.Y + 3));
                }
            }
        }


        private void Go_Click(object sender, EventArgs e)
        {
            if (_v == null)
            {
                if ((_points.Count > 0) && (_points.Count % 2 == 0))
                {
                    _v = new EdmondsVisualizer(MainPictureBox.Width, MainPictureBox.Height, _points);
                    MainTextBox.AppendText("Graph created, n = " + _points.Count.ToString() + "\n");
                }
                else
                {
                    MainTextBox.AppendText("Number of points must be even.\n");
                }
                Timer.Interval = (int)Pause.Value;
                Timer.Start();
            }
        }

        private void InitRandom_Click(object sender, EventArgs e)
        {
            if (_v == null)
            {
                Random rnd = new Random();
                _points = new List<Point>();
                for (int i = 0; i < (int)Size.Value; i++)
                {
                    _points.Add(new Point(rnd.Next() % MainPictureBox.Width, rnd.Next() % MainPictureBox.Height));
                }
                MainPictureBox.Invalidate();
            }
        }

        private void MainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (_v == null)
            {
                _points.Add(new Point(e.X, e.Y));
                MainPictureBox.Invalidate();
            }
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            _v = null;
            _points = new List<Point>();

            MainPictureBox.Invalidate();
            MainTextBox.Clear();
        }

        private void MainPictureBox_Click(object sender, EventArgs e)
        {

        }
    }
}
