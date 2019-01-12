using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otello
{
    class Player
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public int Score { get; set; }
        public TimeSpan Time { get; set; }

        public Player(int id, string name)
        {
            ID = id;
            Name = name;
            Score = 0;
        }
    }
}
