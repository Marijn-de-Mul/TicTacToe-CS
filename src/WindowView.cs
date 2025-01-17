using System;
using System.Windows.Forms;

namespace TicTacToe
{
    public class WindowView : Form
    {
        public WindowView()
        {
            Text = "Tic Tac Toe";
            Width = 800;
            Height = 800;
        }

        [STAThread]
        public static WindowView Create()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WindowView windowView = new WindowView();

            GridComponent.Create(windowView);

            Application.Run(windowView);

            return windowView;
        }
    }
}