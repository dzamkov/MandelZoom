using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace MandelZoom
{
    /// <summary>
    /// A view form for a mandelbrot.
    /// </summary>
    public class View : Form
    {
        public View()
        {
            this.Text = "MandelZoom";
            this.SPF = 1.0 / 60.0;
            this.Width = 400;
            this.Height = 400;

            this.Camera = new Camera
            {
                Center = new Complex(0.0, 0.0),
                Velocity = new Complex(0.0, 0.0),
                Zoom = 0.5,
                ZoomVelocity = 0.0
            };

            this.Gradient = new Gradient(100.0, new Color(0.0, 0.0, 1.0), new Gradient.Stop[]
            {
                new Gradient.Stop(0.3, new Color(1.0, 0.0, 0.0)),
                new Gradient.Stop(0.5, new Color(1.0, 1.0, 0.0)),
                new Gradient.Stop(0.7, new Color(0.0, 1.0, 0.0)),
                new Gradient.Stop(0.9, new Color(0.0, 1.0, 1.0)),
            }, new Color(0.0, 0.0, 0.0), 10.0);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// The camera for this view.
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// The gradient for this view.
        /// </summary>
        public Gradient Gradient;

        /// <summary>
        /// An estimate of the SPF (seconds per frame) of the view based on update times.
        /// </summary>
        public double SPF;

        /// <summary>
        /// Updates the state of this view by the given amount of time in seconds.
        /// </summary>
        public void Update(double Time)
        {
            const double spfi = 20.0;
            this.SPF = (this.SPF * spfi + Time) / (spfi + 1.0);
            this.Text = "MandelZoom (" + (1.0 / this.SPF).ToString("#####") + " fps)";

            this.Camera.Update(Time, 0.1, 0.1);
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Insure buffer has been created and has the right size.
            Size clientsize = this.ClientSize;
            if (this._Buffer == null)
            {
                this._Buffer = new Bitmap(clientsize.Width, clientsize.Height);
            }
            else
            {
                if (this._Buffer.Size != clientsize)
                {
                    this._Buffer.Dispose();
                    this._Buffer = new Bitmap(clientsize.Width, clientsize.Height);
                }
            }

            // Draw to bitmap
            unsafe
            {
                BitmapData bd = this._Buffer.LockBits(new Rectangle(new Point(0, 0), clientsize), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                byte* ptr = (byte*)bd.Scan0.ToPointer();

                int width = bd.Width;
                int height = bd.Height;
                Complex tl; double d;
                this.Camera.Transform(width, height, out tl, out d);
                for (int x = 0; x < width; x++)
                {
                    double r = tl.Real + d * x;
                    for (int y = 0; y < height; y++)
                    {
                        double i = tl.Imag + d * y;
                        int iter = Mandelbrot.Evaluate(new Complex(r, i) * 4.0, 100);

                        byte* colptr = ptr + 4 * (x + (y * width));
                        Color col = this.Gradient.GetColor(iter, 100);
                        col.Write(colptr);
                    }
                }

                this._Buffer.UnlockBits(bd);
            }

            // Draw bitmap
            e.Graphics.DrawImageUnscaled(this._Buffer, 0, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Refresh();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Size size = this.ClientSize;
            Complex center = this.Camera.Transform(e.X, e.Y, size.Width, size.Height);
            double amount = (double)e.Delta / 500.0;

            Complex dif = center - this.Camera.Center;

            this.Camera.ZoomVelocity += amount;

            if (this._DragPoint == null)
            {
                this.Camera.Velocity += dif * amount;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Size size = this.ClientSize;
            this._DragPoint = this.Camera.Transform(e.X, e.Y, size.Width, size.Height);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this._DragPoint = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this._DragPoint != null)
            {
                Size size = this.ClientSize;
                Complex point = this.Camera.Transform(e.X, e.Y, size.Width, size.Height);
                Complex diff = point - this._DragPoint.Value;
                this.Camera.Center -= diff;
                this.Camera.Velocity = -diff;
            }
        }

        private Complex? _DragPoint;
        private Bitmap _Buffer;
    }
}
