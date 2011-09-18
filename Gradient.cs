using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MandelZoom
{
    /// <summary>
    /// Represents a color gradient for an iterative fractal.
    /// </summary>
    public class Gradient
    {
        public Gradient(double Period, Color Initial, Stop[] Stops, Color Final)
        {
            this.Period = Period;
            this.Initial = Initial;
            this.Stops = Stops;
            this.Final = Final;
        }

        /// <summary>
        /// Creates a cache of the given size for this gradient.
        /// </summary>
        public void CreateCache(int Size)
        {
            this._Cache = new Color[Size];
            double d = 1.0 / (double)Size;
            for (int t = 0; t < Size; t++)
            {
                this._Cache[t] = this._GetPeriodColor(t * d);
            }
        }

        /// <summary>
        /// Gets the color for a fragment with the given iteration count.
        /// </summary>
        public Color GetColor(double Iterations)
        {
            if (this._Cache != null)
            {
                double scale = this._Cache.Length / this.Period;
                double index = (Iterations % this.Period) * scale;
                return this._Cache[(int)index];
            }
            return this._GetPeriodColor(Iterations % this.Period / this.Period);
        }

        /// <summary>
        /// Gets a color for a given offset (between 0.0 and 1.0) in a period.
        /// </summary>
        private Color _GetPeriodColor(double Offset)
        {
            Color prev = this.Initial;
            double prevoffset = 0.0;
            for (int t = 0; t < this.Stops.Length; t++)
            {
                Stop stop = this.Stops[t];
                Color next = stop.Color;
                double nextoffset = stop.Offset;
                if (Offset < nextoffset)
                    return Color.Mix(prev, next, (Offset - prevoffset) / (nextoffset - prevoffset));
                prev = next;
                prevoffset = nextoffset;
            }
            return Color.Mix(prev, this.Initial, (Offset - prevoffset) / (1.0 - prevoffset));
        }

        /// <summary>
        /// Represents a defined point in a gradient.
        /// </summary>
        public struct Stop
        {
            public Stop(double Offset, Color Color)
            {
                this.Offset = Offset;
                this.Color = Color;
            }

            /// <summary>
            /// The offset (relative to a period) at which this stop occurs.
            /// </summary>
            public double Offset;

            /// <summary>
            /// The color of the stop.
            /// </summary>
            public Color Color;
        }

        /// <summary>
        /// The amount of iterations in a period for this gradient.
        /// </summary>
        public readonly double Period;

        /// <summary>
        /// The initial color in each period of the gradient.
        /// </summary>
        public readonly Color Initial;

        /// <summary>
        /// The stops for a period in this gradient.
        /// </summary>
        public readonly Stop[] Stops;

        /// <summary>
        /// The final color used when the iteration count is approaching the maximum iteration amount.
        /// </summary>
        public readonly Color Final;

        /// <summary>
        /// Cache for colors in the gradient.
        /// </summary>
        private Color[] _Cache;
    }

    /// <summary>
    /// Represents a color.
    /// </summary>
    public struct Color
    {
        public Color(double R, double G, double B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        /// <summary>
        /// Mixes color A with color B by the given amount between 0.0 and 1.0.
        /// </summary>
        public static Color Mix(Color A, Color B, double Amount)
        {
            double af = 1.0 - Amount;
            double bf = Amount;
            return new Color(
                A.R * af + B.R * bf,
                A.G * af + B.G * bf,
                A.B * af + B.B * bf);
        }

        /// <summary>
        /// Writes the color to the given memory location.
        /// </summary>
        public unsafe void Write(byte* Ptr)
        {
            byte r = (byte)(this.R * 255.0);
            byte g = (byte)(this.G * 255.0);
            byte b = (byte)(this.B * 255.0);
            Ptr[0] = b;
            Ptr[1] = g;
            Ptr[2] = r;
        }

        /// <summary>
        /// The red component of the color.
        /// </summary>
        public double R;

        /// <summary>
        /// The green component of the color.
        /// </summary>
        public double G;

        /// <summary>
        /// The blue component of the color.
        /// </summary>
        public double B;
    }
}
