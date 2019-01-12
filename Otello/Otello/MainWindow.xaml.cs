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
        private SolidColorBrush colorWhitePlayer = Brushes.Red;
        private SolidColorBrush colorBlackPlayer = Brushes.Blue;

        public MainWindow()
        {
            game = new Game(true, false);
            DataContext = game.Board;
        }

        public void UpdateSize(object sender, RoutedEventArgs e)
        {
            sizeCells = (int)Math.Min(layout.ColumnDefinitions[0].ActualWidth / (Board.COLUMNS_NUMBER + 2),
                                       layout.RowDefinitions[1].ActualHeight / (Board.LINES_NUMBER + 2));
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

            foreach (UIElement child in gameGrid.Children.OfType<Ellipse>())
            {
                Ellipse ellipse = (Ellipse) child;
                ellipse.Width = sizeCells - 1;
                ellipse.Height = sizeCells - 1;
            }
        }

        override
        protected void OnSourceInitialized(EventArgs args)
        {
            sizeCells = (int)Math.Min(layout.ColumnDefinitions[0].ActualWidth / (Board.COLUMNS_NUMBER + 2),
                                       layout.RowDefinitions[1].ActualHeight / (Board.LINES_NUMBER + 2));
            PlayGameInInterface();
        }

        private void ClickEvent(object sender, MouseButtonEventArgs e)
        {
            int x = Grid.GetColumn(sender as UIElement) - 1;
            int y = Grid.GetRow(sender as UIElement) - 1;

            List<Tuple<int, int>> possibilities = game.FindNextPossibleMoves();
            foreach (Tuple<int, int> possibility in possibilities)
            {
                if (possibility.Item1 == x && possibility.Item2 == y)
                {
                    game.PlayMove(possibility.Item1, possibility.Item2);
                    DrawTokens();
                    DisplayPossibilites();
                }
            }
        }

        private void PlayGameInInterface()
        {
            InitGridPosition();
            InitGridLabels();
            InitGrid();
            InitGridDisplay();

            DrawTokens();
            DisplayPossibilites();
        }

        private void InitGridPosition()
        {
            int shiftLeft = (int)(layout.ColumnDefinitions[0].ActualWidth - (Board.COLUMNS_NUMBER + 2) * sizeCells) / 2;
            int shiftTop = (int)(layout.RowDefinitions[1].ActualHeight - (Board.LINES_NUMBER + 2) * sizeCells) / 2;
            gameGrid.Margin = new Thickness(shiftLeft, shiftTop, 0, 0);
        }

        private void InitGridLabels()
        {
            // Colonnes
            char letter = 'A';
            for (int i = 1; i <= Board.COLUMNS_NUMBER; i++)
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
            for (int i = 1; i <= Board.LINES_NUMBER; i++)
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
            for (int i = 1; i <= Board.COLUMNS_NUMBER; i++)
            {
                ColumnDefinition column = new ColumnDefinition
                {
                    Width = new GridLength(sizeCells)
                };
                gameGrid.ColumnDefinitions.Add(column);

                for (int j = 1; j <= Board.LINES_NUMBER; j++)
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
            for (int i = 1; i <= Board.COLUMNS_NUMBER; i++)
            {
                for (int j = 1; j <= Board.LINES_NUMBER; j++)
                {
                    // Display cell
                    rect = new Rectangle
                    {
                        Height = sizeCells - 1,
                        Width = sizeCells - 1,
                        Fill = Brushes.Black
                    };
                    
                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, j);

                    gameGrid.Children.Add(rect);

                    // Interactive cell
                    rect = new Rectangle
                    {
                        Height = sizeCells - 1,
                        Width = sizeCells - 1,
                        Fill = Brushes.Transparent
                    };
                    rect.MouseLeftButtonUp += new MouseButtonEventHandler(ClickEvent);

                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, j);
                    Panel.SetZIndex(rect, 4);

                    gameGrid.Children.Add(rect);
                }
            }

            Ellipse ellipse = new Ellipse
            {
                Height = dataGrid.RowDefinitions[0].ActualHeight - 20,
                Width = dataGrid.RowDefinitions[0].ActualHeight - 20,
                Fill = colorWhitePlayer
            };
            Grid.SetColumn(ellipse, 2);
            Grid.SetRow(ellipse, 0);
            dataGrid.Children.Add(ellipse);
        }

        private void DrawTokens()
        {
            int[,] board = game.Board.GetBoard(); 
            
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] >= 0)
                    {
                        Ellipse ellipse = new Ellipse
                        {
                            Height = sizeCells - 10,
                            Width = sizeCells - 10
                        };
                        if (board[i, j] == 0)
                        {
                            ellipse.Fill = colorWhitePlayer;
                        } else
                        {
                            ellipse.Fill = colorBlackPlayer;
                        }
                        Grid.SetColumn(ellipse, i + 1);
                        Grid.SetRow(ellipse, j + 1);

                        gameGrid.Children.Add(ellipse);
                    }
                }
            }
        }

        private void DisplayPossibilites()
        {
            List<Tuple<int, int>> possibilities = game.FindNextPossibleMoves();

            // Delete previous possible moves
            List<UIElement> elementsToBeDeleted = new List<UIElement>();
            foreach (UIElement child in gameGrid.Children.OfType<Ellipse>())
            {
                if (((Ellipse) child).Fill == game.PreviewColor)
                {
                    elementsToBeDeleted.Add(child);
                }
            }
            foreach (UIElement element in elementsToBeDeleted)
            {
                gameGrid.Children.Remove(element);
            }

            // Display possible moves
            foreach (Tuple<int, int> possibility in possibilities)
            {
                Ellipse ellipse = new Ellipse
                {
                    Height = sizeCells - 10,
                    Width = sizeCells - 10,
                    Fill = game.PreviewColor
                };
                
                Grid.SetColumn(ellipse, possibility.Item1 + 1);
                Grid.SetRow(ellipse, possibility.Item2 + 1);
                gameGrid.Children.Add(ellipse);
            }
            
            // Update the current player information
            for (int i = 0; i < dataGrid.Children.Count; i++)
            {
                UIElement element = dataGrid.Children[i];
                if (Grid.GetRow(element) == 0 && Grid.GetColumn(element) == 2)
                {
                    Ellipse rect = (Ellipse) element;
                    rect.Fill = game.CurrentPlayerColor();
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