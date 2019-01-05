﻿using System;
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
        public int Score { get; private set; }

        public Player(int id, string name)
        {
            ID = id;
            Name = name;
            Score = 0;
        }
    }
}