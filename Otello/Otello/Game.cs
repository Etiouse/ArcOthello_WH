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

        private const int EMPTY_CASE_ID = -1;
        private const int PLAYER_BLACK_CASE_ID = 1;
        private const int PLAYER_WHITE_CASE_ID = 0;

        private const int COLUMNS_NUMBER = 9;
        private const int LINES_NUMBER = 7;

        public Game()
        {
            Player playerBlack = new Player(PLAYER_BLACK_CASE_ID, "Black player");
            Player playerWhite = new Player(PLAYER_WHITE_CASE_ID, "White player");

            Board = new Board(LINES_NUMBER, COLUMNS_NUMBER, EMPTY_CASE_ID, playerBlack, playerWhite);
        }
    }
}
