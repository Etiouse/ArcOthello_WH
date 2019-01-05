using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otello
{
    class Game
    {
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }

        public Board Board { get; private set; }

        private const int EMPTY_CASE_ID = 0;
        private const int PLAYER_1_CASE_ID = 1;
        private const int PLAYER_2_CASE_ID = 2;

        private const int COLUMNS_NUMBER = 9;
        private const int LINES_NUMBER = 7;

        public Game()
        {
            Player1 = new Player(PLAYER_1_CASE_ID, "Player1");
            Player2 = new Player(PLAYER_2_CASE_ID, "Player2");

            Board = new Board(LINES_NUMBER, COLUMNS_NUMBER, EMPTY_CASE_ID, PLAYER_1_CASE_ID, PLAYER_2_CASE_ID);
        }
    }
}
