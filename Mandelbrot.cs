using System;
using System.Collections.Generic;
using System.Linq;

namespace MandelZoom
{
    /// <summary>
    /// Contains functions for evaluating the mandelbrot set.
    /// </summary>
    public static class Mandelbrot
    {
        /// <summary>
        /// Evaluates the mandelbrot at the given point, returning the amount of iterations until the point
        /// escapes the bounds (points in the mandelbrot will never escape the bounds, and will always have a value of Max).
        /// </summary>
        public static int Evaluate(Complex Point, int Max)
        {
            int iter = 0;

            double cr = Point.Real;
            double ci = Point.Imag;
            double zr = cr;
            double zi = ci;

            while (iter < Max)
            {
                double ozr = zr;
                double zis = zi * zi;
                zr *= zr;

                if (zr + zis > 4.0)
                    break;

                zr -= zis;
                zr += cr;

                zi *= 2.0;
                zi *= ozr;
                zi += ci;

                iter++;
            }
            return iter;
        }
    }
}
