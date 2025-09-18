using System;
using System.Windows.Forms;

namespace ReconKitApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DataAccess.EnsureDatabase();
            Application.Run(new MainForm());
        }
    }
}
