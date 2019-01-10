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
        private bool inConsole = false;
        private Grid grid;
        private const int COLUMNS = 9;
        private const int ROWS = 7;

        public MainWindow()
        {
            InitializeComponent();

            if (inConsole)
            {
                PlayGameInConsole();
            }
            else
            {
                PlayGameInInterface();
            }
        }

        private void PlayGameInInterface()
        {
            grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 450,
                Height = 350,
                Background = Brushes.ForestGreen,
                ShowGridLines = true
            };

            for (int i = 0; i < COLUMNS; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < ROWS; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            Content = grid;

            //Game game = new Game(true, inConsole);
            //while (game.GameStart)
            //{
                //game.FindNextPossibleMoves();
                //game.PlayMove(col, line);
            //}
        }

        private void PlayGameInConsole()
        {
            Game game = new Game(false, true);

            ConsoleManager.Show();

            Console.WriteLine("Welcome in Otello WH !");
            Console.WriteLine("Board size : " + Board.COLUMNS_NUMBER + "x" + Board.LINES_NUMBER);
            game.Board.DisplayBoardInConsole();

            while (game.GameStart)
            {
                Console.WriteLine(game.CurrentPlayerColor() + " turn !");
                Console.WriteLine("Case you want to play in (format : column line)");
                game.FindNextPossibleMoves();

                string[] newCase = Console.ReadLine().Split(' ');
                int col = int.Parse(newCase[0]);
                int line = int.Parse(newCase[1]);

                game.PlayMove(col, line);
            }
        }
    }
}
