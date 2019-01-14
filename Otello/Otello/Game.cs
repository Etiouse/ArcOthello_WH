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
        public SolidColorBrush PreviewColor { get; private set; }

        public Game(bool whiteBegins)
        {
            Board = new Board();

            WhiteTurn = whiteBegins;

            GameStart = true;
            WhiteColor = Brushes.Red;
            BlackColor = Brushes.Blue;
            PreviewColor = Brushes.White;

            TurnSkipped = false;
        }

        public List<Tuple<int, int>> FindNextPossibleMoves()
        {
            List<Tuple<int, int>> nextPossibleMoves = Board.GetNextPossibleMoves(WhiteTurn);

            return nextPossibleMoves;
        }

        public bool PlayMove(int col, int line)
        {
            bool validMove = false;

            if (Board.IsPlayable(col, line, WhiteTurn))
            {
                validMove = Board.PlayMove(col, line, WhiteTurn);
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
