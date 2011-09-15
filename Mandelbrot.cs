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
            Complex c = Point;
            while (iter < Max && (Point.Real * Point.Real + Point.Imag * Point.Imag) < 2.0)
            {
                Complex p = Point;
                Point.Real = p.Real * p.Real - p.Imag * p.Imag + c.Real;
                Point.Imag = p.Imag * p.Real + p.Real * p.Imag + c.Imag;
                iter++;
            }
            return iter;
        }
    }
}
