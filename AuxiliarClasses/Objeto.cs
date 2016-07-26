using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuxiliarClasses
{
    public class Objeto
    {
        public Rectangle Posicao;
        public Texture2D Texture;

        public int SpeedX;
        public int SpeedY;
        public int MaxSpeedX;
        public int MaxSpeedY;

        private Objeto _Initialized;

        public Objeto Clone()
        {
            Objeto novo = new Objeto();

            novo.SpeedX = this.SpeedX;
            novo.SpeedY = this.SpeedY;
            novo.Posicao = new Rectangle(Posicao.X, Posicao.Y, Posicao.Width, Posicao.Height);
            novo.MaxSpeedX = this.MaxSpeedX;
            novo.MaxSpeedY = this.MaxSpeedY;
            novo.Texture = this.Texture;

            return novo;
        }
        public void SaveState()
        {
            _Initialized = this.Clone();
        }
        public void Reset()
        {
            this.SpeedX = _Initialized.SpeedX;
            this.SpeedY = _Initialized.SpeedY;
            this.Posicao = new Rectangle(_Initialized.Posicao.X, _Initialized.Posicao.Y, _Initialized.Posicao.Width, _Initialized.Posicao.Height);
            this.MaxSpeedX = _Initialized.MaxSpeedX;
            this.MaxSpeedY = _Initialized.MaxSpeedY;
            this.Texture = _Initialized.Texture;
        }
        public bool Collides(Objeto obj)
        {
            return this.Posicao.Intersects(obj.Posicao);
        }
        public void CheckInsideWindow(GameWindow Window)
        {
            int temp = 0;
            this.CheckInsideWindow(Window, false, ref temp);
        }
        public void CheckInsideWindow(GameWindow Window, bool bounce, ref int placar)
        {
            bool xChanged = false;
            bool yChanged = false;

            if (this.Posicao.Right > Window.ClientBounds.Width)
            {
                this.Posicao.X = Window.ClientBounds.Width - Posicao.Width;
                xChanged = true;
            }
            else if (this.Posicao.Left < 0)
            {
                this.Posicao.X = 0;
                xChanged = true;
            }
            if (this.Posicao.Bottom > Window.ClientBounds.Height)
            {
                this.Posicao.Y = Window.ClientBounds.Height - Posicao.Height;
                placar -= 1;
                Reset();
            }
            else if (this.Posicao.Top < 0)
            {
                this.Posicao.Y = 0;
                placar += 1;
                Reset();
            }

            if (xChanged)
            {
                if (!bounce)
                    this.SpeedX = 0;
                else
                    this.SpeedX *= -1;
            }
            if (yChanged)
            {
                if (!bounce)
                    this.SpeedY = 0;
                else
                    this.SpeedY *= -1;
            }
        }
        public void Update()
        {
            Posicao.X += SpeedX;
            Posicao.Y += SpeedY;
        }
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(this.Texture, Posicao, Color.White);
        }
        public void AccelerateLeft()
        {
            if (SpeedX > (MaxSpeedX * (-1)))
                SpeedX -= 1;
        }
        public void AccelerateRight()
        {
            if (SpeedX < MaxSpeedX )
                SpeedX += 1;
        }
        public void AccelerateTop()
        {
            if (SpeedY > (MaxSpeedY * (-1)))
                SpeedY -= 1;
        }
        public void AccelerateBottom()
        {
            if (SpeedY < MaxSpeedY)
                SpeedY += 1;
        }
        public void Stop()
        {
            SpeedX = SpeedY = 0;
        }
    }
}
