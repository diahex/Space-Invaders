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
        private const int WIDTH = 32;
        private const int HEIGHT = 24;
        private static readonly Bitmap Bitmap = new Bitmap(@"C:\Users\diahex\Desktop\Bum.png");
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
            Drawables.Add(new Tank(new Point(100,400))); // Must be first in list
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
                    
                    if(invader.Point.X > 600)
                    {
                        invader.Direction = 1;
                        invader.Point.Y += 5;
                    }
                    else if(invader.Point.X < 80)
                    {
                        invader.Direction = 0;
                        invader.Point.Y += 5;
                    }
                    Exists = true;
                    invader.Point.X += invader.InvaderSpeed + (-invader.InvaderSpeed * 2 * invader.Direction);
                }
            }

            if (!Exists) 
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.ResourceManager.GetStream("Win"));
                player.Play();
                Thread.Sleep(1000);
                Environment.Exit(0); 
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
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.ResourceManager.GetStream("Boom"));
                        player.Play();
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
                        tank.Point.X = Math.Min(tank.Point.X + 5, 700);
                        break;
                    case Keys.Left:
                        tank.Point.X = Math.Max(tank.Point.X - 5, 50);
                        break;
                    case Keys.Space:
                        if (Shot == false)
                        {
                            var bullet = new Bullet(new Point(tank.Point.X + 16, tank.Point.Y - 20), tank);
                            Drawables.Add(bullet);
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.ResourceManager.GetStream("Pew"));
                            player.Play();
                        }
                        break;
                }
            }

            pictureBox.Invalidate();
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
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

        /*private void test()
        {
            var g = pictureBox.CreateGraphics();
            
            pictureBox.Invalidate();
        }*/

    }
}
