using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    public partial class Form1 : Form
    {
        System.Media.SoundPlayer Win = new System.Media.SoundPlayer(Properties.Resources.ResourceManager.GetStream("Win"));
        System.Media.SoundPlayer Boom = new System.Media.SoundPlayer(Properties.Resources.ResourceManager.GetStream("Boom"));
        System.Media.SoundPlayer Pew = new System.Media.SoundPlayer(Properties.Resources.ResourceManager.GetStream("Pew"));
        int Score = 0;
        int Round = 1;
        bool NewRound = false;
        private System.Timers.Timer timer;
        const int TIMER_PERIOD = 1000 / 20;
        private List<IDraw> Drawables = new List<IDraw>();
        private HashSet<Keys> KeyPresses = new HashSet<Keys>();
        public Form1()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.ContainerControl |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.SupportsTransparentBackColor
                          , true);

            InitializeComponent();
            InitGame();
            InitTimer();
        }

        private Tank getTank() { return (Tank)Drawables[0]; }

        private void InitGame()
        {
            Drawables.Add(new Tank(new Point(100,625))); // Must be first in list
            Drawables.Add(new Invader(new Point(100, 100)));
            Drawables.Add(new Invader(new Point(150, 100)));
            Drawables.Add(new Invader(new Point(200, 100)));
            Drawables.Add(new Invader(new Point(250, 100)));
            Drawables.Add(new Invader(new Point(300, 100)));
            Drawables.Add(new Invader(new Point(100, 141)));
            Drawables.Add(new Invader(new Point(150, 141)));
            Drawables.Add(new Invader(new Point(200, 141)));
            Drawables.Add(new Invader(new Point(250, 141)));
            Drawables.Add(new Invader(new Point(300, 141)));
        }
        private void NextRound()
        {
            NewRound = false;
            Round++;
            Drawables.Add(new Invader(new Point(100, 100)));
            Drawables.Add(new Invader(new Point(150, 100)));
            Drawables.Add(new Invader(new Point(200, 100)));
            Drawables.Add(new Invader(new Point(250, 100)));
            Drawables.Add(new Invader(new Point(300, 100)));
            Drawables.Add(new Invader(new Point(100, 141)));
            Drawables.Add(new Invader(new Point(150, 141)));
            Drawables.Add(new Invader(new Point(200, 141)));
            Drawables.Add(new Invader(new Point(250, 141)));
            Drawables.Add(new Invader(new Point(300, 141)));
        }

        private void InitTimer()
        {
            timer = new System.Timers.Timer(TIMER_PERIOD);
            timer.Elapsed += GameLoop;
            timer.Start();
        }

        private bool DoesColide(Rectangle a, Rectangle b)
        {
            return a.IntersectsWith(b);
        }

        private void GameLoop(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool Shot = false;
            bool Exists = false;
            foreach (var idraw in Drawables) // Move loop
            {
                if (idraw is Bullet)
                {
                    var bullet = (Bullet)idraw;

                    bullet.Point.Y += -10;
                    Shot = true;
                }
                if (idraw is Invader)
                {
                    var invader = (Invader)idraw;
                    
                    if(invader.Point.X > 1020)
                    {
                        invader.Direction = 1;
                        invader.Point.Y += 40;
                    }
                    else if(invader.Point.X < 80)
                    {
                        invader.Direction = 0;
                        invader.Point.Y += 40;
                    }
                    Exists = true;
                    
                    if((Score-((Round-1)*100)) == 90) 
                    {
                        invader.InvaderSpeed = 10 + Score / 80;
                    }
                    else {
                        invader.InvaderSpeed = 5 + Score / 80;
                    }
                    invader.Point.X += invader.InvaderSpeed + (-invader.InvaderSpeed * 2 * invader.Direction);

                    if (invader.Point.Y == 625)
                    {
                        Thread.Sleep(1000);
                        Application.Exit();
                    }



                }
            }

            if (!Exists) 
            {
                Win.Play();
                NewRound = true;
                Thread.Sleep(1000);
            }
            if (Score % 100 == 0 && Score != 0 && NewRound == true)
            {
                NextRound();
            }

            var bullets = Drawables.Where(d => d is Bullet);

            List<IDraw> Delete = new List<IDraw>();
            foreach (Bullet bullet in bullets) {
                bool DoesHit = false;
                foreach(var idraw in Drawables)
                {
                    if (bullet == idraw) { continue; }
                    else if(bullet.Sender == idraw) { continue; }
                    
                    if(! (idraw is IHitBox)) { continue; }


                    if (DoesColide(bullet.GetHitbox(), ((IHitBox)idraw).GetHitbox()))
                    {
                        Delete.Add(idraw);
                        DoesHit = true;
                        Score += 10;
                        Boom.Play();
                        break;
                    }
                }

                if (bullet.Point.Y < 0 || DoesHit == true)
                {
                    Delete.Add(bullet);
                }
            }

            foreach (var delete in Delete)
            {
                Drawables.Remove(delete);
            }

            var tank = getTank();
            foreach (var key in new HashSet<Keys>(KeyPresses))
            {

                switch (key)
                {
                    case Keys.Right:
                        tank.Point.X = Math.Min(tank.Point.X + 7, 1020);
                        break;
                    case Keys.Left:
                        tank.Point.X = Math.Max(tank.Point.X - 7, 80);
                        break;
                    case Keys.Space:
                        if (Shot == false)
                        {
                            var bullet = new Bullet(new Point(tank.Point.X + 16, tank.Point.Y - 20), tank);
                            Drawables.Add(bullet);
                            Pew.Play();
                        }
                        break;
                }
            }

            pictureBox.Invalidate();
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
           e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
           Score1.Text = $"Score: {Score}";
           Round1.Text = $"Round: {Round}";
            lock (Drawables)
            {
                e.Graphics.Clear(Color.Black);

                foreach (var idraw in new List<IDraw>(Drawables))
                {
                    idraw.Draw(e.Graphics);
                }
            }
              
        }

        private void KeyReleased(object sender, KeyEventArgs e)
        {
            lock (KeyPresses)
            {
                KeyPresses.Remove(e.KeyCode);
            }   
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            lock (KeyPresses)
            {
                KeyPresses.Add(e.KeyCode);
            }
        }

        private void KeyJustPressed(object sender, KeyPressEventArgs e)
        {
            
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        /*private void test()
        {
            var g = pictureBox.CreateGraphics();
            
            pictureBox.Invalidate();
        }*/

    }
}
