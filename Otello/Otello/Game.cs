using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otello
{
    class Game
    {
        public Board Board { get; private set; }

        public bool GameStart { get; private set; }
        public bool WhiteTurn { get; private set; }

        public const int COLUMNS_NUMBER = 9;
        public const int LINES_NUMBER = 7;

        private const int EMPTY_CASE_ID = -1;
        private const int PLAYER_BLACK_CASE_ID = 1;
        private const int PLAYER_WHITE_CASE_ID = 0;

        private bool writeInConsole;

        public Game(bool whiteBegins, bool writeInConsole)
        {
            Player playerBlack = new Player(PLAYER_BLACK_CASE_ID, "Black player");
            Player playerWhite = new Player(PLAYER_WHITE_CASE_ID, "White player");

            Board = new Board(LINES_NUMBER, COLUMNS_NUMBER, EMPTY_CASE_ID, playerBlack, playerWhite);

            WhiteTurn = whiteBegins;
            this.writeInConsole = writeInConsole;

            GameStart = true;
        }

        public List<Tuple<int, int>> FindNextPossibleMoves()
        {
            List<Tuple<int, int>> nextPossibleMoves;

            if (WhiteTurn)
            {
                nextPossibleMoves = Board.GetNextPossibleMoves(Board.PlayerWhite.ID);
            }
            else
            {
                nextPossibleMoves = Board.GetNextPossibleMoves(Board.PlayerBlack.ID);
            }

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
            bool validMove = Board.PlayMove(col, line, WhiteTurn);

            if (validMove)
            {
                WhiteTurn = !WhiteTurn;

                if (writeInConsole)
                {
                    Console.WriteLine("White score : " + Board.PlayerWhite.Score + " | Black score : " + Board.PlayerBlack.Score);
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
        public string CurrentPlayerColor()
        {
            string currentPlayerColor;

            if (WhiteTurn)
            {
                currentPlayerColor = "White";
            }
            else
            {
                currentPlayerColor = "Black";
            }

            return currentPlayerColor;
        }
    }
}
