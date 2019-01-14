using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace Otello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int sizeCells;
        private const int TIME_BEFORE_CLEAR_MESSAGE_INFO = 2;

        private bool playerSkipingTurn;

        private Game game;

        private SolidColorBrush colorWhitePlayer = Brushes.Red;
        private SolidColorBrush colorBlackPlayer = Brushes.Blue;
        private DispatcherTimer dispatcherTimer;
        private DateTime lastTime;
        private DateTime nextTimeClearMessageInfo;

        private BinaryFormatter formatter;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            game = new Game(false);
            DataContext = game.Board;

            formatter = new BinaryFormatter();

            nextTimeClearMessageInfo = DateTime.Now;
            lastTime = DateTime.Now;

            playerSkipingTurn = false;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dispatcherTimer.Start();
        }

        // Custom overflow for the toolbar
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        private void CommandBinding_New(object sender, ExecutedRoutedEventArgs e)
        {
            game.WhiteTurn = false;

            Board board = game.Board;
            board.InitBoard();
            board.WhiteScore = 2;
            board.BlackScore = 2;
            board.WhiteTime = new TimeSpan(0, 0, 0);
            board.BlackTime = new TimeSpan(0, 0, 0);

            ResetGrid();
            DrawTokens();
            DisplayPossibilites();

            dispatcherTimer.Start();
        }

        private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "otello board files (*.owh)|*.owh|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            
            if(openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                var stream = openFileDialog.OpenFile();

                GameModel gameModel = (GameModel)formatter.Deserialize(stream);
                stream.Close();

                game.WhiteTurn = gameModel.WhiteTurn;

                Board board = game.Board;
                board.SetBoard(gameModel.Board);
                board.WhiteScore = gameModel.WhiteScore;
                board.BlackScore = gameModel.BlackScore;
                board.WhiteTime = gameModel.WhiteTime;
                board.BlackTime = gameModel.BlackTime;

                ResetGrid();
                DrawTokens();
                DisplayPossibilites();

                dispatcherTimer.Start();
            }
        }

        private void CommandBinding_Save(object sender, ExecutedRoutedEventArgs e)
        {
            Board board = game.Board;
            GameModel gameModel = new GameModel(board.GetBoard(), board.WhiteScore, board.BlackScore, board.WhiteTime, board.BlackTime, game.WhiteTurn);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "otello board files (*.owh)|*.owh|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            Stream myStream;
            if (saveFileDialog.ShowDialog() == true)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    formatter.Serialize(myStream, gameModel);
                    myStream.Close();
                }
            }
        }

        /// <summary>
        /// Undo a turn if possible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Undo(object sender, ExecutedRoutedEventArgs e)
        {
            bool isFill = game.PopAndApplyLastPushedGridForUndo();

            if(isFill)
            {
                game.WhiteTurn = !game.WhiteTurn;

                DrawTokens();
                DisplayPossibilites();
            }
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
                ellipse.Width = sizeCells - 10;
                ellipse.Height = sizeCells - 10;
            }
        }

        override
        protected void OnSourceInitialized(EventArgs args)
        {
            sizeCells = (int)Math.Min(layout.ColumnDefinitions[0].ActualWidth / (Board.COLUMNS_NUMBER + 2),
                                       layout.RowDefinitions[1].ActualHeight / (Board.LINES_NUMBER + 2));
            PlayGameInInterface();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - lastTime;
            lastTime = DateTime.Now;

            if (DateTime.Now >= nextTimeClearMessageInfo)
            {
                if (playerSkipingTurn)
                {
                    messageInfo.Content = "";
                    playerSkipingTurn = false;
                    game.TurnSkipped = true;
                    game.WhiteTurn = !game.WhiteTurn;
                    DisplayPossibilites();
                }
                else
                {
                    if (game.WhiteTurn)
                    {
                        game.Board.WhiteTime += elapsedTime;
                    }
                    else
                    {
                        game.Board.BlackTime += elapsedTime;
                    }
                }
            }
        }

        private void ClickEvent(object sender, MouseButtonEventArgs e)
        {
            if (!playerSkipingTurn)
            {
                int x = Grid.GetColumn(sender as UIElement) - 1;
                int y = Grid.GetRow(sender as UIElement) - 1;

                List<Tuple<int, int>> possibilities = game.FindNextPossibleMoves();
                foreach (Tuple<int, int> possibility in possibilities)
                {
                    if (possibility.Item1 == x && possibility.Item2 == y)
                    {
                        game.PushCurrentGridForUndo();
                        game.PlayMove(possibility.Item1, possibility.Item2);
                        DrawTokens();
                        DisplayPossibilites();
                    }
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
                Height = dataGrid.RowDefinitions[1].ActualHeight - 20,
                Width = dataGrid.RowDefinitions[1].ActualHeight - 20,
                Fill = colorWhitePlayer
            };
            Grid.SetColumn(ellipse, 2);
            Grid.SetRow(ellipse, 1);
            dataGrid.Children.Add(ellipse);
        }

        private void ResetGrid()
        {
            for (int i = gameGrid.Children.Count - 1; i >= 0; i--)
            {
                if (gameGrid.Children[i] is Ellipse)
                {
                    gameGrid.Children.RemoveAt(i);
                }
            }
        }

        private void DrawTokens()
        {
            int[,] board = game.Board.GetBoard();

            // Delete all previous tokens
            for (int i = 0; i < gameGrid.Children.Count; i++)
            {
                UIElement child = gameGrid.Children[i];
                if (child is Ellipse)
                {
                    gameGrid.Children.Remove(child);
                }
            }

            // Display all tokens
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
                if (((Ellipse)child).Fill == game.WhiteColorPreview || ((Ellipse)child).Fill == game.BlackColorPreview)
                {
                    elementsToBeDeleted.Add(child);
                }
            }
            foreach (UIElement element in elementsToBeDeleted)
            {
                gameGrid.Children.Remove(element);
            }

            // Update the current player information
            for (int i = 0; i < dataGrid.Children.Count; i++)
            {
                UIElement element = dataGrid.Children[i];
                if (Grid.GetRow(element) == 1 && Grid.GetColumn(element) == 2)
                {
                    Ellipse rect = (Ellipse)element;
                    rect.Fill = game.CurrentPlayerColor();
                }
            }

            if (possibilities.Count <= 0)
            {
                if (game.Board.IsBoardFull())
                {
                    EndGame();
                }
                else
                {
                    if (game.TurnSkipped)
                    {
                        EndGame();
                    }
                    else
                    {
                        messageInfo.Content = "Le joueur passe son tour, aucun coup n'est valide !";

                        UpdateTimeClearMessageInfo();
                        
                        playerSkipingTurn = true;
                    }
                }
            }
            else
            {
                game.TurnSkipped = false;

                // Display possible moves
                foreach (Tuple<int, int> possibility in possibilities)
                {
                    Ellipse ellipse = new Ellipse
                    {
                        Height = sizeCells - 10,
                        Width = sizeCells - 10,
                        Stroke = Brushes.White,
                        StrokeThickness = 5
                    };

                    if (game.WhiteTurn)
                    {
                        ellipse.Fill = game.WhiteColorPreview;
                    }
                    else
                    {
                        ellipse.Fill = game.BlackColorPreview;
                    }

                    Grid.SetColumn(ellipse, possibility.Item1 + 1);
                    Grid.SetRow(ellipse, possibility.Item2 + 1);
                    gameGrid.Children.Add(ellipse);
                }
            }
        }

        /// <summary>
        /// Update the next time when the message info label will be clear.
        /// </summary>
        private void UpdateTimeClearMessageInfo()
        {
            nextTimeClearMessageInfo = DateTime.Now + new TimeSpan(0, 0, TIME_BEFORE_CLEAR_MESSAGE_INFO);
        }

        /// <summary>
        /// Display the end message and stop the time.
        /// </summary>
        private void EndGame()
        {
            messageInfo.Content = "Fin de la partie!";

            dispatcherTimer.Stop();
        }
    }
}