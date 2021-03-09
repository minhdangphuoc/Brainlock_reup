using System;
using System.Collections.Generic;
using System.Linq;
using NeuroSky.ThinkGear;
using System.Windows.Forms;

namespace Brainlocknetv1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
       {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Homepage());
        }
    }
}
