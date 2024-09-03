using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolCelsiusChk
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length <= 0)
            {
                MessageBox.Show("缺少必要參數");
                return;
            }

            Form1 f = new Form1();
            f.Id = args[0];
            //f.Password = args[1];

            if (f.Id.Length != 5)
            {
                MessageBox.Show("ID 不足5碼");
                return;
            }

            //if (f.Password.Trim().Length <= 0)
            //{
            //    MessageBox.Show("密碼不得為空");
            //    return;
            //}

            
            Application.Run(f);
        }
    }
}
