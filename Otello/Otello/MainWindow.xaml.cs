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

            // Comment line bellow to use graphic console
            PlayGameInConsole();
        }

        private void PlayGameInConsole()
        {
            Game game = new Game(true, true);

            ConsoleManager.Show();

            Console.WriteLine("Welcome in Otello WH !");
            Console.WriteLine("Board size : " + Game.COLUMNS_NUMBER + "x" + Game.LINES_NUMBER);
            game.Board.DisplayBoardInConsole();
            Console.WriteLine("White begins !");

            while (game.GameStart)
            {
                Console.WriteLine(game.CurrentPlayerColor() + " turn !");
                Console.WriteLine("Case you want to play in (format : column line)");

                string[] newCase = Console.ReadLine().Split(' ');
                int col = int.Parse(newCase[0]);
                int line = int.Parse(newCase[1]);

                game.PlayMove(col, line);
            }
        }
    }
}
