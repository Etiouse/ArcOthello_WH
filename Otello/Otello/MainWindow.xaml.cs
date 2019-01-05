using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Otello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Game game = new Game();
            game.Board.DisplayBoardInConsole();
            game.Board.PlayMove(3, 3, false);
            Console.WriteLine();
            game.Board.DisplayBoardInConsole();
            game.Board.PlayMove(3, 2, true);
            Console.WriteLine();
            game.Board.DisplayBoardInConsole();
            game.Board.PlayMove(4, 2, false);
            Console.WriteLine();
            game.Board.DisplayBoardInConsole();
            game.Board.PlayMove(3, 4, true);
            Console.WriteLine();
            game.Board.DisplayBoardInConsole();
            game.Board.PlayMove(5, 2, true);
            Console.WriteLine();
            game.Board.DisplayBoardInConsole();
        }
    }
}
