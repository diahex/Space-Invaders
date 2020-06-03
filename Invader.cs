using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    public class Invader : IDraw, IHitBox
    {
 
        private const int WIDTH = 32;
        private const int HEIGHT = 24;
        private static readonly Bitmap Bitmap = new Bitmap(@"C:\Users\diahex\Desktop\Invader.png");

        public Point Point = new Point(0,0);
        public int Direction = 0; // 0=> Right, 1 => Left
        public int InvaderSpeed = 5;

        public Invader(Point start)
        {
            this.Point = start;
        }

        public void Draw(Graphics g)
        {
            //g.FillRectangle(Brushes.Green, Point.X,Point.Y,WIDTH,HEIGHT);
            g.DrawImage(Bitmap, Point.X, Point.Y, WIDTH, HEIGHT);
        }

        public Rectangle GetHitbox()
        {
            return new Rectangle(Point, new Size(WIDTH, HEIGHT));
        }
    }
}
