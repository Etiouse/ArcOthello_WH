using System;

namespace Otello
{
    class Board : IPlayable.IPlayable
    {
        private readonly int[,] board;

        private readonly int linesNumber;
        private readonly int columnsNumber;

        private readonly int emptyCaseID;

        private Player playerBlack;
        private Player playerWhite;

        public Board(int linesNumber, int columnsNumber, int emptyCaseID,
            Player playerBlack, Player playerWhite)
        {
            this.linesNumber = linesNumber;
            this.columnsNumber = columnsNumber;
            this.emptyCaseID = emptyCaseID;
            this.playerBlack = playerBlack;
            this.playerWhite = playerWhite;

            board = new int[,] { };

            InitBoard();
        }

        public int GetBlackScore()
        {
            return playerBlack.Score;
        }

        public int GetWhiteScore()
        {
            return playerWhite.Score;
        }

        public int[,] GetBoard()
        {
            return board;
        }

        public string GetName()
        {
            return "WH_IA";
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            bool doesColumnExists = column >= 0 && column < columnsNumber;
            bool doesLineExists = line >= 0 && line < linesNumber;

            if (doesColumnExists &&
                doesLineExists &&
                IsPlayable(column, line, isWhite))
            {
                int currentPlayerID;

                if(isWhite)
                {
                    currentPlayerID = playerWhite.ID;
                }
                else
                {
                    currentPlayerID = playerBlack.ID;
                }

                ChangeLine(line, column, currentPlayerID);
                ChangeColumn(line, column, currentPlayerID);
                ChangeDiagonal(line, column, currentPlayerID);
            }

            return false;
        }

        private void ChangeLine(int line, int column, int playerID)
        {
            for (int i = column + 1; i < columnsNumber; i++)
            {
                // if(i > )
            }

            for (int i = column; i >= 0; i--)
            {

            }
        }

        private void ChangeColumn(int line, int column, int playerID)
        {

        }

        private void ChangeDiagonal(int line, int column, int playerID)
        {

        }

        /// <summary>
        /// Fill the board with init discs (4 in the center).
        /// </summary>
        private void InitBoard()
        {
            for (int i = 0; i < columnsNumber; i++)
            {
                for (int j = 0; j < linesNumber; j++)
                {
                    board[i, j] = emptyCaseID;
                }
            }

            int columnCenter = columnsNumber / 2;
            int lineCenter = linesNumber / 2;

            // init board with 4 discs (2 for each player) on the center of the board.
            board[columnCenter, lineCenter] = playerWhite.ID;
            board[columnCenter + 1, lineCenter + 1] = playerWhite.ID;
            board[columnCenter, lineCenter + 1] = playerBlack.ID;
            board[columnCenter + 1, lineCenter] = playerBlack.ID;
        }
    }
}
