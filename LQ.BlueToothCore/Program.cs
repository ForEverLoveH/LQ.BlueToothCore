 
using LQ.BlueToothCore.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
using System.Windows.Forms;
using LQ.BlueToothCore.Views.Windows;

namespace LQ.BlueToothCore
{

    static class Program
    {
      
        [STAThread]
        static void Main()
        {
           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow()); // 启动主窗体
        }
    }
}
