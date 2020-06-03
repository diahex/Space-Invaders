using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    public class Tank : IDraw, IHitBox
    {
        private const int WIDTH = 52;
        private const int HEIGHT = 32;
        private static readonly Bitmap Bitmap = new Bitmap(Properties.Resources.Ship);

        public Point Point = new Point(0, 0);

        public Tank(Point start)
        {
            this.Point = start;
        }

        public void Draw(Graphics g)
        {
            //g.FillRectangle(Brushes.HotPink, new Rectangle(Point, new Size(WIDTH, HEIGHT)));
            g.DrawImage(Bitmap, Point.X, Point.Y, WIDTH, HEIGHT);
        }

        public Rectangle GetHitbox()
        {
            return new Rectangle(Point, new Size(WIDTH, HEIGHT));
        }
    }
}
