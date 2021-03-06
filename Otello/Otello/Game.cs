﻿using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Othello_HW
{
    class Game
    {
        public Board BoardGame { get; private set; }

        public bool GameStart { get; private set; }
        public bool WhiteTurn { get; set; }
        public bool TurnSkipped { get; set; }
        public bool PlayAgainsIA { get; set; }

        public ImageBrush WhiteColor { get; private set; }
        public ImageBrush BlackColor { get; private set; }
        public ImageBrush WhiteColorPreview { get; private set; }
        public ImageBrush BlackColorPreview { get; private set; }

        public Stack<Tuple<int[,], bool, Tuple<TimeSpan, TimeSpan>, Tuple<int, int>>> PreviousTurns { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="whiteBegins">True if the white player begins, false if it's the black one</param>
        /// <param name="playAgainsIA">True if the player plays agains the IA</param>
        public Game(bool whiteBegins, bool playAgainsIA = false)
        {
            BoardGame = new Board();

            WhiteTurn = whiteBegins;
            PlayAgainsIA = playAgainsIA;

            ImageBrush whiteBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/WhiteColor.png", UriKind.Relative))
            };
            ImageBrush blackBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/BlackColor.png", UriKind.Relative))
            };
            ImageBrush whiteBrushPreview = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/WhiteColor.png", UriKind.Relative)),
                Opacity = 0.5
            };
            ImageBrush blackBrushPreview = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/BlackColor.png", UriKind.Relative)),
                Opacity = 0.5
            };

            GameStart = true;
            WhiteColor = whiteBrush;
            BlackColor = blackBrush;
            WhiteColorPreview = whiteBrushPreview;
            BlackColorPreview = blackBrushPreview;

            TurnSkipped = false;

            PreviousTurns = new Stack<Tuple<int[,], bool, Tuple<TimeSpan, TimeSpan>, Tuple<int, int>>>();
        }

        /// <summary>
        /// Returns the next possible moves for the current player
        /// </summary>
        /// <returns>The next possible moves for the current player</returns>
        public List<Tuple<int, int>> FindNextPossibleMoves()
        {
            return BoardGame.GetNextPossibleMoves(WhiteTurn);
        }

        /// <summary>
        /// Play the given move if it's playable.
        /// Change the turn if the move is valid
        /// </summary>
        /// <param name="column">The given column on the board</param>
        /// <param name="line">The given line on the board</param>
        /// <returns>True if the move is valide, false otherwise</returns>
        public bool PlayMove(int column, int line)
        {
            if (!PlayAgainsIA ||
                (PlayAgainsIA && !WhiteTurn))
            {
                bool validMove = false;

                if (BoardGame.IsPlayable(column, line, WhiteTurn))
                {
                    validMove = BoardGame.PlayMove(column, line, WhiteTurn);
                }

                if (validMove)
                {
                    WhiteTurn = !WhiteTurn;
                }

                return validMove;
            }

            return false;
        }

        /// <summary>
        /// Play the next move for the IA.
        /// </summary>
        /// <returns>True if the move is valide, false otherwise</returns>
        public bool PlayMoveIA()
        {
            if (PlayAgainsIA && WhiteTurn)
            {
                bool validMove = false;
                int column, line;

                // TODO needs to be cleaned and improved, meaby moved ? IA's turn
                Tuple<int, int> nextMove = BoardGame.GetNextMove(BoardGame.CurrentBoard, 5, WhiteTurn);

                // IA pass turn if no next possible move
                if (nextMove.Equals(new Tuple<int, int>(-1, -1)))
                {
                    Console.WriteLine("No next move for the IA");
                    Console.Read();
                    WhiteTurn = !WhiteTurn;
                    return false;
                }

                column = nextMove.Item1;
                line = nextMove.Item2;

                if (BoardGame.IsPlayable(column, line, WhiteTurn))
                {
                    validMove = BoardGame.PlayMove(column, line, WhiteTurn);
                }

                if (validMove)
                {
                    WhiteTurn = !WhiteTurn;
                    TurnSkipped = false;
                }

                return validMove;
            }

            return false;
        }

        /// <summary>
        /// Returns the color of the current player turn
        /// </summary>
        /// <returns>The color of the current player turn</returns>
        public ImageBrush CurrentPlayerColor()
        {
            if (WhiteTurn)
            {
                return WhiteColor;
            }
            else
            {
                return BlackColor;
            }
        }

        /// <summary>
        /// Push all the information for the undo functionnality.
        /// </summary>
        public void PushCurrentTurnForUndo()
        {
            // Deap copy of array
            int[,] copy = BoardGame.DeepCopyIntArray(BoardGame.CurrentBoard);

            // Players time
            Tuple<TimeSpan, TimeSpan> playersTime =
                new Tuple<TimeSpan, TimeSpan>(BoardGame.PlayerWhite.Time, BoardGame.PlayerBlack.Time);
            // Players score
            Tuple<int, int> playersScore =
                new Tuple<int, int>(BoardGame.PlayerWhite.Score, BoardGame.PlayerBlack.Score);

            // Store all infos
            PreviousTurns.Push(new Tuple<int[,], bool, Tuple<TimeSpan, TimeSpan>, Tuple<int, int>>(copy, WhiteTurn, playersTime, playersScore));
        }

        /// <summary>
        /// Pop and update all the information for the undo functionnality
        /// </summary>
        /// <returns>True if the storage contained info, false if empty</returns>
        public bool PopAndApplyLastPushedTurnForUndo()
        {
            if (PreviousTurns.Count > 0)
            {
                Tuple<int[,], bool, Tuple<TimeSpan, TimeSpan>, Tuple<int, int>> previousTurn = PreviousTurns.Pop();

                BoardGame.CurrentBoard = previousTurn.Item1;

                WhiteTurn = previousTurn.Item2;

                BoardGame.WhiteTime = previousTurn.Item3.Item1;
                BoardGame.BlackTime = previousTurn.Item3.Item2;

                BoardGame.WhiteScore = previousTurn.Item4.Item1;
                BoardGame.BlackScore = previousTurn.Item4.Item2;

                return true;
            }

            return false;
        }
    }
}
