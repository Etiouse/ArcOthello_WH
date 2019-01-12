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
        private int sizeCells;
        private Game game;

        private const int COLUMNS = 9;
        private const int ROWS = 7;

        public MainWindow()
        {
            game = new Game(false, false);
        }

        public void UpdateSize(object sender, RoutedEventArgs e)
        {
            sizeCells = (int)Math.Min(layout.ColumnDefinitions[0].ActualWidth / (COLUMNS + 2),
                                       layout.RowDefinitions[1].ActualHeight / (ROWS + 2));
            InitGridPosition();

            for (int i = 0; i < gameGrid.ColumnDefinitions.Count; i++)
            {
                gameGrid.ColumnDefinitions[i].Width = new GridLength(sizeCells);
            }
            for (int j = 0; j < gameGrid.RowDefinitions.Count; j++)
            {
                gameGrid.RowDefinitions[j].Height = new GridLength(sizeCells);
            }

            foreach (UIElement child in gameGrid.Children.OfType<Rectangle>())
            {
                Rectangle rect = (Rectangle) child;
                rect.Width = sizeCells - 1;
                rect.Height = sizeCells - 1;
            }
        }

        override
        protected void OnSourceInitialized(EventArgs args)
        {
            sizeCells = (int)Math.Min(layout.ColumnDefinitions[0].ActualWidth / (COLUMNS + 2),
                                       layout.RowDefinitions[1].ActualHeight / (ROWS + 2));
            //PlayGameInInterface();
            PlayGameInConsole();
        }

        private void PlayGameInInterface()
        {
            InitGridPosition();
            InitGridLabels();
            InitGrid();
            InitGridDisplay();
        }

        private void InitGridPosition()
        {
            int shiftLeft = (int)(layout.ColumnDefinitions[0].ActualWidth - (COLUMNS + 2) * sizeCells) / 2;
            int shiftTop = (int)(layout.RowDefinitions[1].ActualHeight - (ROWS + 2) * sizeCells) / 2;
            gameGrid.Margin = new Thickness(shiftLeft, shiftTop, 0, 0);
        }

        private void InitGridLabels()
        {
            // Colonnes
            char letter = 'A';
            for (int i = 1; i <= COLUMNS; i++)
            {
                ColumnDefinition column = new ColumnDefinition
                {
                    Width = new GridLength(sizeCells)
                };
                gameGrid.ColumnDefinitions.Add(column);

                Label label = new Label
                {
                    Content = letter,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 25
                };

                Grid.SetColumn(label, i);
                Grid.SetRow(label, 0);
                gameGrid.Children.Add(label);

                letter++;
            }

            // Lignes
            for (int i = 1; i <= ROWS; i++)
            {
                RowDefinition row = new RowDefinition
                {
                    Height = new GridLength(sizeCells)
                };
                gameGrid.RowDefinitions.Add(row);

                Label label = new Label
                {
                    Content = i,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 25
                };

                Grid.SetColumn(label, 0);
                Grid.SetRow(label, i);
                gameGrid.Children.Add(label);
            }
        }

        private void InitGrid()
        {
            for (int i = 1; i <= COLUMNS; i++)
            {
                ColumnDefinition column = new ColumnDefinition
                {
                    Width = new GridLength(sizeCells)
                };
                gameGrid.ColumnDefinitions.Add(column);

                for (int j = 1; j <= COLUMNS; j++)
                {
                    RowDefinition row = new RowDefinition
                    {
                        Height = new GridLength(sizeCells)
                    };
                    gameGrid.RowDefinitions.Add(row);
                }
            }
        }

        private void InitGridDisplay()
        {
            Rectangle rect;
            for (int i = 1; i <= COLUMNS; i++)
            {
                for (int j = 1; j <= ROWS; j++)
                {
                    rect = new Rectangle
                    {
                        Height = sizeCells - 1,
                        Width = sizeCells - 1,
                        Fill = Brushes.Black
                    };

                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, j);

                    gameGrid.Children.Add(rect);
                }
            }
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