using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otello
{
    [Serializable]
    class GameModel
    {
        public int[,] Board { get; private set; }
        public int WhiteScore { get; private set; }
        public int BlackScore { get; private set; }
        public TimeSpan WhiteTime { get; private set; }
        public TimeSpan BlackTime { get; private set; }
        public bool WhiteTurn { get; private set; }
        public Stack<Tuple<int[,], Tuple<TimeSpan, TimeSpan>, Tuple<int, int>>> PreviousTurns { get; private set; }

        public GameModel(int[,] board, int whiteScore, int blackScore, TimeSpan whiteTime, TimeSpan blackTime, bool whiteTurn,
            Stack<Tuple<int[,], Tuple<TimeSpan, TimeSpan>, Tuple<int, int>>> previousTurns)
        {
            Board = board;
            WhiteScore = whiteScore;
            BlackScore = blackScore;
            WhiteTime = whiteTime;
            BlackTime = blackTime;
            WhiteTurn = whiteTurn;
            PreviousTurns = previousTurns;
        }
    }
}
