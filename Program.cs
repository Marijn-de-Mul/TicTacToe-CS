using TicTacToe; 

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            WindowView windowView = WindowView.Create();
            GridComponent.Create(windowView); 
        }
    }
}
