using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MandelZoom
{
    /// <summary>
    /// Gives parameters for a view of the complex plane.
    /// </summary>
    public struct Camera
    {
        /// <summary>
        /// The point in the center of the camera view.
        /// </summary>
        public Complex Center;

        /// <summary>
        /// The zoom level of the camera such that the edge length of the camera view is proportional to 2 ^ (-Zoom).
        /// </summary>
        public double Zoom;

        /// <summary>
        /// The change in position over time for the camera.
        /// </summary>
        public Complex Velocity;

        /// <summary>
        /// The change in zoom level over time for the camera.
        /// </summary>
        public double ZoomVelocity;

        /// <summary>
        /// Transforms a point from a discrete coordinate space using this camera.
        /// </summary>
        public Complex Transform(int X, int Y, int Width, int Height)
        {
            double x = ((double)X + 0.5) / (double)Width - 0.5;
            double y = ((double)Y + 0.5) / (double)Height - 0.5;
            double z = Math.Pow(0.5, this.Zoom);
            double ar = (double)Width / (double)Height;
            if (ar > 1.0)
            {
                return this.Center + new Complex(2.0 * x * z, 2.0 * y * z / ar);
            }
            else
            {
                return this.Center + new Complex(2.0 * x * z * ar, 2.0 * y * z);
            }
        }

        /// <summary>
        /// Finds the center of the topleft corner and the pixel size for a discrete coordinate space using this camera.
        /// </summary>
        public void Transform(int Width, int Height, out Complex TopLeft, out double PixelSize)
        {
            double z = Math.Pow(0.5, this.Zoom);
            if (Width > Height)
            {
                PixelSize = z * 2.0 / Width;
                TopLeft = this.Center + new Complex(PixelSize * 0.5 - z, (Width - Height) * PixelSize * 0.5 + PixelSize * 0.5 - z);
            }
            else
            {
                PixelSize = z * 2.0 / Height;
                TopLeft = this.Center + new Complex((Height - Width) * PixelSize * 0.5 + PixelSize * 0.5 - z, PixelSize * 0.5 - z);
            }
        }

        /// <summary>
        /// Updates the state of the camera by the given amount of time in seconds.
        /// </summary>
        public void Update(double Time, double Damping, double ZoomDamping)
        {
            this.Center += this.Velocity * Time;
            this.Zoom += this.ZoomVelocity * Time;

            double d = Math.Pow(Damping, Time);
            double dz = Math.Pow(ZoomDamping, Time);
            this.Velocity *= d;
            this.ZoomVelocity *= dz;
        }
    }
}
