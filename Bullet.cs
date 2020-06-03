using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    public class Bullet : IDraw, IHitBox
    {
        private const int WIDTH = 8;
        private const int HEIGHT = 12;

        public Point Point = new Point();
        private object _sender;
        public object Sender { get => _sender; }

        public Bullet(Point start, object sender)
        {
            this.Point = start;
            this._sender = sender;
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.White, new Rectangle(Point, new Size(WIDTH, HEIGHT)));
        }

        public Rectangle GetHitbox()
        {
            return new Rectangle(Point, new Size(WIDTH, HEIGHT));
        }
    }
}
