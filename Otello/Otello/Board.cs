using System;
using System.Collections.Generic;

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

            board = new int[columnsNumber, linesNumber];

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
            if (column < 0 || column >= columnsNumber || line < 0 || line >= linesNumber)
            {
                return false;
            }



            return true;
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

                CheckChangesOnBoard(line, column, currentPlayerID);
            }

            return false;
        }

        /// <summary>
        /// Display the current board in console
        /// </summary>
        public void DisplayBoardInConsole()
        {
            for (int i = 0; i < linesNumber; i++)
            {
                for (int j = 0; j < columnsNumber; j++)
                {
                    Console.Write(board[j, i] + "\t");
                }
                Console.WriteLine();
            }
            Console.Read();
        }

        /// <summary>
        /// Check if their are change on all the diagonals of the given disc
        /// </summary>
        /// <param name="line">The indicated line</param>
        /// <param name="column">The indicated column</param>
        /// <param name="playerID">The player ID</param>
        private void CheckChangesOnBoard(int line, int column, int playerID)
        {
            int[,] incDirections = new int[,]
            {
                // down right
                {1, 1 },
                // down left
                {1, -1 },
                // up right
                {-1, 1 },
                // up left
                {-1, -1 },
                // right
                {0, 1 },
                // left
                {0, -1 },
                // up
                {-1, 0 },
                // down
                {1, 0 }
            };

            List<Tuple<int, int>> casesToChange = new List<Tuple<int, int>>();

            // For each direction
            for (int i = 0; i < incDirections.GetLength(0); i++)
            {
                int currentCol = column + incDirections[i, 1], currentLine = line + incDirections[i, 0];

                casesToChange.Clear();
                casesToChange.Add(new Tuple<int, int>(column, line));

                // While the current column and the current line are in the board
                while (currentCol >= 0 && currentCol < columnsNumber && currentLine >= 0 && currentLine < linesNumber)
                {
                    // Don't apply change if their is an empty case
                    if (board[currentCol, currentLine] == emptyCaseID)
                    {
                        break;
                    }
                    else if (board[currentCol, currentLine] == playerID)
                    {
                        ApplyChangesOnBoard(casesToChange, playerID);
                    }
                    else
                    {
                        casesToChange.Add(new Tuple<int, int>(currentCol, currentLine));
                    }

                    currentCol += incDirections[i, 1];
                    currentLine += incDirections[i, 0];
                }
            }
        }

        /// <summary>
        /// Change given cases on the board with the given player id.
        /// </summary>
        /// <param name="casesToChange">Given cases to change</param>
        /// <param name="playerID">Given player id</param>
        private void ApplyChangesOnBoard(List<Tuple<int, int>> casesToChange, int playerID)
        {
            foreach (Tuple<int, int> item in casesToChange)
            {
                board[item.Item1, item.Item2] = playerID;
            }
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
