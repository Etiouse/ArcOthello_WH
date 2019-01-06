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

        public const int COLUMNS_NUMBER = 9;
        public const int LINES_NUMBER = 7;

        private const int EMPTY_CASE_ID = -1;
        private const int PLAYER_BLACK_CASE_ID = 1;
        private const int PLAYER_WHITE_CASE_ID = 0;

        private bool whiteTurn;
        private bool drawInConsole;

        public Game(bool whiteBegins, bool drawInConsole)
        {
            Player playerBlack = new Player(PLAYER_BLACK_CASE_ID, "Black player");
            Player playerWhite = new Player(PLAYER_WHITE_CASE_ID, "White player");

            Board = new Board(LINES_NUMBER, COLUMNS_NUMBER, EMPTY_CASE_ID, playerBlack, playerWhite);

            whiteTurn = whiteBegins;
            this.drawInConsole = drawInConsole;
        }

        public bool PlayMove(int col, int line)
        {
            bool validMove = Board.PlayMove(col, line, whiteTurn);

            if (validMove)
            {
                if (drawInConsole)
                {
                    Console.WriteLine("New board state :");
                    Board.DisplayBoardInConsole();
                }

                whiteTurn = !whiteTurn;
                return true;
            }
            else
            {
                if (drawInConsole)
                {
                    Console.WriteLine("Not a valid case !");
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

            if (whiteTurn)
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
