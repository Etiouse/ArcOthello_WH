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
using System.Windows.Media.Animation;

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
        private double lastStarXShift;

        private bool playingTurn;

        private Game game;
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
            game = new Game(false, true);
            DataContext = game.BoardGame;

            formatter = new BinaryFormatter();

            nextTimeClearMessageInfo = DateTime.Now;
            lastTime = DateTime.Now;

            playerSkipingTurn = false;
            lastStarXShift = 0;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dispatcherTimer.Start();
        }
        
        /// <summary>
        /// Custom overflow for the toolbar
        /// </summary>
        /// <param name="sender">Sender of the call</param>
        /// <param name="e">Arguments of the events</param>
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

        /// <summary>
        /// Called to reset the content of the game grid and adapt all of it's displayed informations
        /// </summary>
        private void ResetGame()
        {
            ResetGrid();
            DrawTokens();
            DisplayPossibilites();
            MoveStar();
        }

        /// <summary>
        /// Binded method called when the new button is pressed
        /// </summary>
        /// <param name="sender">Sender of the call</param>
        /// <param name="e">Arguments of the events</param>
        private void CommandBinding_New(object sender, ExecutedRoutedEventArgs e)
        {
            game.WhiteTurn = false;

            Board board = game.BoardGame;
            board.InitBoard();
            board.WhiteScore = 2;
            board.BlackScore = 2;
            board.WhiteTime = new TimeSpan(0, 0, 0);
            board.BlackTime = new TimeSpan(0, 0, 0);

            ResetGame();

            dispatcherTimer.Start();

            game.PreviousTurns.Clear();

            messageInfo.Content = "";
        }

        /// <summary>
        /// Binded method called when the open / load button is pressed
        /// </summary>
        /// <param name="sender">Sender of the call</param>
        /// <param name="e">Arguments of the events</param>
        private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "othello board files (*.owh)|*.owh|All files (*.*)|*.*",
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
                game.PreviousTurns = gameModel.PreviousTurns;

                Board board = game.BoardGame;
                board.CurrentBoard = gameModel.Board;
                board.WhiteScore = gameModel.WhiteScore;
                board.BlackScore = gameModel.BlackScore;
                board.WhiteTime = gameModel.WhiteTime;
                board.BlackTime = gameModel.BlackTime;

                ResetGame();

                dispatcherTimer.Start();

                messageInfo.Content = "";
            }
        }

        /// <summary>
        /// Binded method called when the save button is pressed
        /// </summary>
        /// <param name="sender">Sender of the call</param>
        /// <param name="e">Arguments of the events</param>
        private void CommandBinding_Save(object sender, ExecutedRoutedEventArgs e)
        {
            Board board = game.BoardGame;
            GameModel gameModel = new GameModel(board.GetBoard(), board.WhiteScore, board.BlackScore,
                board.WhiteTime, board.BlackTime, game.WhiteTurn, game.PreviousTurns);

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
        /// Binded method called when the undo button is pressed
        /// </summary>
        /// <param name="sender">Sender of the call</param>
        /// <param name="e">Arguments of the events</param>
        private void CommandBinding_Undo(object sender, ExecutedRoutedEventArgs e)
        {
            bool isFill = game.PopAndApplyLastPushedTurnForUndo();

            if(isFill)
            {
                messageInfo.Content = "";
                playerSkipingTurn = false;
                nextTimeClearMessageInfo = DateTime.Now;

                ResetGame();

                dispatcherTimer.Start();
            }
        }

        /// <summary>
        /// Called on resize event to secure the responsivity of the application
        /// </summary>
        /// <param name="sender">Sender of the call</param>
        /// <param name="e">Arguments of the events</param>
        public void UpdateSize(object sender, RoutedEventArgs e)
        {
            sizeCells = (int)Math.Min(Layout.ColumnDefinitions[0].ActualWidth / (Board.COLUMNS_NUMBER + 2),
                                       Layout.RowDefinitions[1].ActualHeight / (Board.LINES_NUMBER + 2));
            InitGridPosition();

            // Update the dimension of the grid definitions
            for (int i = 0; i < gameGrid.ColumnDefinitions.Count; i++)
            {
                gameGrid.ColumnDefinitions[i].Width = new GridLength(sizeCells);
            }
            for (int j = 0; j < gameGrid.RowDefinitions.Count; j++)
            {
                gameGrid.RowDefinitions[j].Height = new GridLength(sizeCells);
            }

            // Update the dimensions of all grid cells visually
            foreach (UIElement child in gameGrid.Children.OfType<Rectangle>())
            {
                Rectangle rect = (Rectangle) child;
                rect.Width = sizeCells - 1;
                rect.Height = sizeCells - 1;
            }

            // Update the dimensions of al ltokens visually
            foreach (UIElement child in gameGrid.Children.OfType<Ellipse>())
            {
                Ellipse ellipse = (Ellipse) child;
                ellipse.Width = sizeCells - 10;
                ellipse.Height = sizeCells - 10;
            }
        }

        /// <summary>
        /// Called when all ressources are initialized, entry point of the program
        /// </summary>
        /// <param name="args">Arguments</param>
        override
        protected void OnSourceInitialized(EventArgs args)
        {
            // Compute the size of a grid cell
            sizeCells = (int)Math.Min(Layout.ColumnDefinitions[0].ActualWidth / (Board.COLUMNS_NUMBER + 2),
                                       Layout.RowDefinitions[1].ActualHeight / (Board.LINES_NUMBER + 2));

            // Initialize the game
            InitGridPosition();
            InitGridLabels();
            InitGrid();
            InitGridDisplay();

            // Draw it's content
            DrawTokens();
            DisplayPossibilites();
        }

        /// <summary>
        /// Called every millisecond to update the timers and refresh the UI 
        /// </summary>
        /// <param name="sender">Sender of the call</param>
        /// <param name="e">Arguments of the events</param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!playingTurn)
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

                        if (game.PlayAgainsIA && game.WhiteTurn)
                        {
                            game.PlayMoveIA();
                            elapsedTime = DateTime.Now - lastTime;
                            lastTime = DateTime.Now;
                            game.BoardGame.WhiteTime += elapsedTime;
                            DrawTokens();
                            DisplayPossibilites();
                            MoveStar();
                        }
                        else
                        {
                            game.WhiteTurn = !game.WhiteTurn;
                        }
                    }
                    else
                    {
                        if (game.WhiteTurn)
                        {
                            if (game.PlayAgainsIA)
                            {
                                game.PlayMoveIA();
                                elapsedTime = DateTime.Now - lastTime;
                                lastTime = DateTime.Now;
                                game.BoardGame.WhiteTime += elapsedTime;
                                DrawTokens();
                                DisplayPossibilites();
                                MoveStar();
                            }
                            else
                            {
                                game.BoardGame.WhiteTime += elapsedTime;
                            }
                        }
                        else
                        {
                            game.BoardGame.BlackTime += elapsedTime;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Click event
        /// </summary>
        /// <param name="sender">Sender of the call</param>
        /// <param name="e">Arguments of the events</param>
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
                        playingTurn = true;
                        game.PushCurrentTurnForUndo();
                        game.PlayMove(possibility.Item1, possibility.Item2);
                        DrawTokens();
                        DisplayPossibilites();
                        MoveStar();
                        playingTurn = false;
                    }
                }
            }
        }

        /// <summary>
        /// Update the XProperty of the star image depending on the score of both players
        /// </summary>
        private void MoveStar()
        {
            // Informations
            double ratio = (double) game.BoardGame.BlackScore / (game.BoardGame.WhiteScore + game.BoardGame.BlackScore);
            double width = Layout.ColumnDefinitions[0].ActualWidth;

            // Shift
            double newShiftX = (ratio * width) - (Layout.ColumnDefinitions[0].ActualWidth / 2);

            // Transform
            TranslateTransform transform = new TranslateTransform();
            StarImage.RenderTransform = transform;
            DoubleAnimation translate = new DoubleAnimation(lastStarXShift, newShiftX, TimeSpan.FromSeconds(0.4));
            transform.BeginAnimation(TranslateTransform.XProperty, translate);

            // Persistence
            lastStarXShift = newShiftX;
        }

        /// <summary>
        /// Setup the grid position depending on the window dimensions
        /// </summary>
        private void InitGridPosition()
        {
            int shiftLeft = (int)(Layout.ColumnDefinitions[0].ActualWidth - (Board.COLUMNS_NUMBER + 2) * sizeCells) / 2;
            int shiftTop = (int)(Layout.RowDefinitions[1].ActualHeight - (Board.LINES_NUMBER + 2) * sizeCells) / 2;
            gameGrid.Margin = new Thickness(shiftLeft, shiftTop, 0, 0);
        }

        /// <summary>
        /// Compute and draw the labels of the game grid
        /// </summary>
        private void InitGridLabels()
        {
            // Columns
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
                    FontSize = 25,
                    Foreground = Brushes.White
                };

                Grid.SetColumn(label, i);
                Grid.SetRow(label, 0);
                gameGrid.Children.Add(label);

                letter++;
            }

            // Rows
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
                    FontSize = 25,
                    Foreground = Brushes.White
                };

                Grid.SetColumn(label, 0);
                Grid.SetRow(label, i);
                gameGrid.Children.Add(label);
            }
        }

        /// <summary>
        /// Compute the definitions for the grid columns and rows
        /// </summary>
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

        /// <summary>
        /// Construct the game grid
        /// </summary>
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

                    // Interactive cell to handle clicks
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

            // Display who's turn it is
            Ellipse ellipse = new Ellipse
            {
                Height = dataGrid.RowDefinitions[1].ActualHeight - 20,
                Width = dataGrid.RowDefinitions[1].ActualHeight - 20,
                Fill = game.WhiteColor,
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            Grid.SetColumn(ellipse, 2);
            Grid.SetRow(ellipse, 1);
            dataGrid.Children.Add(ellipse);
        }

        /// <summary>
        /// Delete all tokens in the grid
        /// </summary>
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

        /// <summary>
        /// Draw all tokens and delete previous ones
        /// </summary>
        private void DrawTokens()
        {
            int[,] board = game.BoardGame.GetBoard();

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
                            Width = sizeCells - 10,
                            Stroke = Brushes.White,
                            StrokeThickness = 1
                        };
                        if (board[i, j] == 0)
                        {
                            ellipse.Fill = game.WhiteColor;
                        } else
                        {
                            ellipse.Fill = game.BlackColor;
                        }
                        Grid.SetColumn(ellipse, i + 1);
                        Grid.SetRow(ellipse, j + 1);

                        gameGrid.Children.Add(ellipse);
                    }
                }
            }
        }

        /// <summary>
        /// Display the cells that accepts a new token
        /// </summary>
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

            // Update the turn display
            for (int i = 0; i < dataGrid.Children.Count; i++)
            {
                UIElement element = dataGrid.Children[i];
                if (Grid.GetRow(element) == 1 && Grid.GetColumn(element) == 2)
                {
                    Ellipse rect = (Ellipse)element;
                    rect.Fill = game.CurrentPlayerColor();
                }
            }

            // Check whenever a player has to skip his turn and the end of a game
            if (possibilities.Count <= 0)
            {
                if (game.BoardGame.IsBoardFull())
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
                        string playerName;
                        if (game.WhiteTurn)
                        {
                            playerName = game.BoardGame.PlayerWhite.Name;
                        }
                        else
                        {
                            playerName = game.BoardGame.PlayerBlack.Name;
                        }

                        messageInfo.Content = "Le joueur " + playerName + " passe son tour, aucun coup possible !";

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
            if (game.BoardGame.PlayerWhite.Score > game.BoardGame.PlayerBlack.Score)
            {
                messageInfo.Content = "Le joueur " + game.BoardGame.PlayerWhite.Name + " à gagné !";
            }
            else if (game.BoardGame.PlayerWhite.Score < game.BoardGame.PlayerBlack.Score)
            {
                messageInfo.Content = "Le joueur " + game.BoardGame.PlayerBlack.Name + " à gagné !";
            }
            else
            {
                messageInfo.Content = "Égalité !";
            }

            dispatcherTimer.Stop();
        }
    }
}