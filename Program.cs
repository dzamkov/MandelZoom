using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MandelZoom
{
    /// <summary>
    /// Program main class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program main entry point.
        /// </summary>
        [STAThread]
        public static void Main(string[] Args)
        {
            Application.EnableVisualStyles();

            View view = new View();
            view.Show();

            DateTime last = DateTime.Now;
            while (view.Visible)
            {
                DateTime cur = DateTime.Now;
                double updatetime = (cur - last).Milliseconds / 1000.0;
                last = cur;

                view.Update(updatetime);
                Application.DoEvents();
            }
        }
    }
}
