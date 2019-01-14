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

        private readonly bool writeInConsole;

        public Game(bool whiteBegins, bool writeInConsole)
        {
            Board = new Board();

            WhiteTurn = whiteBegins;
            this.writeInConsole = writeInConsole;

            GameStart = true;
            WhiteColor = Brushes.Red;
            BlackColor = Brushes.Blue;
            PreviewColor = Brushes.White;

            TurnSkipped = false;
        }

        public List<Tuple<int, int>> FindNextPossibleMoves()
        {
            List<Tuple<int, int>> nextPossibleMoves = Board.GetNextPossibleMoves(WhiteTurn);

            if(writeInConsole)
            {
                Console.WriteLine("Next possible moves : ");
                foreach (Tuple<int, int> nextPossibleMove in nextPossibleMoves)
                {
                    Console.WriteLine("( " + nextPossibleMove.Item1 + " ; " + nextPossibleMove.Item2 + " )");
                }
            }

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

                if (writeInConsole)
                {
                    Console.WriteLine(Board.PlayerWhite.Name + " : " + Board.PlayerWhite.Score + " | " + Board.PlayerBlack.Name + " : " + Board.PlayerBlack.Score);
                    Console.WriteLine("New board state :");
                    Board.DisplayBoardInConsole();
                }

                return true;
            }
            else
            {
                if (writeInConsole)
                {
                    Console.WriteLine("Not a valid case !");
                    Board.DisplayBoardInConsole();
                }
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
