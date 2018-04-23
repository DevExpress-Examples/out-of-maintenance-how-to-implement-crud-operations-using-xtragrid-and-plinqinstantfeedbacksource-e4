// Developer Express Code Central Example:
// How to implement CRUD operations using XtraGrid and PLinqInstantFeedbackSource
// 
// This example demonstrates how to implement the Create, Update and Delete
// operations using PLinqInstantFeedbackSource.
// This example works with the
// standard SQL Northwind database.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4501

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;

namespace PLinqServerMode
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

            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.UserSkins.BonusSkins.Register();
            UserLookAndFeel.Default.SetSkinStyle("London Liquid Sky");

            Application.Run(new Form1());
        }
    }
}