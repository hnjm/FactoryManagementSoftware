﻿using FactoryManagementSoftware.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactoryManagementSoftware
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()

        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

           //Application.Run(new frmSBBMould());

           //Application.Run(new frmNewPlanning());

           //Application.Run(new frmProductionRecordNewV2());

            Application.Run(new MainDashboard(1));

           //Application.Run(new frmLogIn());

        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
