using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Otello
{
    class Game
    {
        public Board Board { get; private set; }

        public bool GameStart { get; private set; }
        public bool WhiteTurn { get; set; }
        public bool TurnSkipped { get; set; }

        public SolidColorBrush WhiteColor { get; private set; }
        public SolidColorBrush BlackColor { get; private set; }
        public SolidColorBrush WhiteColorPreview { get; private set; }
        public SolidColorBrush BlackColorPreview { get; private set; }

        public Stack<int[,]> PreviousGrid { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="whiteBegins">True if the white player begins, false if it's the black one</param>
        public Game(bool whiteBegins)
        {
            Board = new Board();

            WhiteTurn = whiteBegins;

            GameStart = true;
            WhiteColor = Brushes.Red;
            BlackColor = Brushes.Blue;
            WhiteColorPreview = new SolidColorBrush(Color.FromArgb(255, 255, 65, 105));
            BlackColorPreview = new SolidColorBrush(Color.FromArgb(255, 0, 170, 255));

            TurnSkipped = false;

            PreviousGrid = new Stack<int[,]>();
        }

        /// <summary>
        /// Returns the next possible moves for the current player
        /// </summary>
        /// <returns>The next possible moves for the current player</returns>
        public List<Tuple<int, int>> FindNextPossibleMoves()
        {
            return Board.GetNextPossibleMoves(WhiteTurn);
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
            bool validMove = false;

            if (Board.IsPlayable(column, line, WhiteTurn))
            {
                validMove = Board.PlayMove(column, line, WhiteTurn);
            }

            if (validMove)
            {
                WhiteTurn = !WhiteTurn;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the color of the current player turn
        /// </summary>
        /// <returns>The color of the current player turn</returns>
        public SolidColorBrush CurrentPlayerColor()
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
    }
}
