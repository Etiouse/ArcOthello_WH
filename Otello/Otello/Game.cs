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

        public Game()
        {
            Player1 = new Player(1, "Player1");
            Player2 = new Player(2, "Player2");

            Board = new Board();
        }
    }
}
