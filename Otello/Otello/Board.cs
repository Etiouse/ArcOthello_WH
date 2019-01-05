using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otello
{
    class Board : IPlayable.IPlayable
    {
        private readonly int[,] board;

        private readonly int linesNumber;
        private readonly int columnsNumber;

        private readonly int emptyCaseID;
        private readonly int player1CaseID;
        private readonly int player2CaseID;

        public Board(int linesNumber, int columnsNumber, int emptyCaseID,
            int player1CaseID, int player2CaseID)
        {
            this.linesNumber = linesNumber;
            this.columnsNumber = columnsNumber;
            this.emptyCaseID = emptyCaseID;
            this.player1CaseID = player1CaseID;
            this.player2CaseID = player2CaseID;

            board = new int[,] { };

            for (int i = 0; i < columnsNumber; i++)
            {
                for (int j = 0; j < linesNumber; j++)
                {
                    board[i, j] = emptyCaseID;
                }
            }
        }

        public int GetBlackScore()
        {
            throw new NotImplementedException();
        }

        public int[,] GetBoard()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "WH_IA";
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public int GetWhiteScore()
        {
            throw new NotImplementedException();
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }
    }
}
