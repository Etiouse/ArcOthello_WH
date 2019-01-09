using System;
using System.Collections.Generic;

namespace Otello
{
    class Board : IPlayable.IPlayable
    {
        public Player PlayerBlack { get; private set; }
        public Player PlayerWhite { get; private set; }

        private readonly int[,] board;

        private readonly int linesNumber;
        private readonly int columnsNumber;

        private readonly int emptyCaseID;

        public Board(int linesNumber, int columnsNumber, int emptyCaseID,
            Player playerBlack, Player playerWhite)
        {
            this.linesNumber = linesNumber;
            this.columnsNumber = columnsNumber;
            this.emptyCaseID = emptyCaseID;
            this.PlayerBlack = playerBlack;
            this.PlayerWhite = playerWhite;

            board = new int[columnsNumber, linesNumber];

            InitBoard();
        }

        public int GetBlackScore()
        {
            return PlayerBlack.Score;
        }

        public int GetWhiteScore()
        {
            return PlayerWhite.Score;
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
            if (column < 0 ||
                column >= columnsNumber ||
                line < 0 ||
                line >= linesNumber ||
                board[column, line] == PlayerWhite.ID ||
                board[column, line] == PlayerBlack.ID)
            {
                return false;
            }

            return !CheckIsolation(column, line);
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            if (IsPlayable(column, line, isWhite))
            {
                int currentPlayerID;

                if(isWhite)
                {
                    currentPlayerID = PlayerWhite.ID;
                }
                else
                {
                    currentPlayerID = PlayerBlack.ID;
                }

                List<Tuple<int, int>> casesToChange = CheckChangesOnBoard(line, column, currentPlayerID);
                
                if(casesToChange.Count > 0)
                {
                    UpdatePlayerScore(currentPlayerID, casesToChange.Count);

                    ApplyChangesOnBoard(casesToChange, currentPlayerID);
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Get all the next possible moves for a given player id.
        /// </summary>
        /// <param name="playerID">The current player id</param>
        /// <returns>The next possible moves</returns>
        public List<Tuple<int, int>> GetNextPossibleMoves(int playerID)
        {
            List<Tuple<int, int>> nextPossibleCases = new List<Tuple<int, int>>();

            for (int col = 0; col < columnsNumber; col++)
            {
                for (int line = 0; line < linesNumber; line++)
                {
                    if (board[col, line] != PlayerWhite.ID &&
                        board[col, line] != PlayerBlack.ID &&
                        !CheckIsolation(col, line))
                    {
                        List<Tuple<int, int>> casesToChange = CheckChangesOnBoard(line, col, playerID);

                        if (casesToChange.Count > 0)
                        {
                            nextPossibleCases.Add(new Tuple<int, int>(col, line));
                        }
                    }
                }
            }

            return nextPossibleCases;
        }

        /// <summary>
        /// Display the current board in console
        /// </summary>
        public void DisplayBoardInConsole()
        {
            Console.Write("\t");

            for (int i = 0; i < columnsNumber; i++)
            {
                Console.Write(i + "\t");
            }

            Console.WriteLine();

            for (int i = 0; i < linesNumber; i++)
            {
                Console.Write(i + "\t");

                for (int j = 0; j < columnsNumber; j++)
                {
                    Console.Write(board[j, i] + "\t");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Check if a case is isolated (surounded by empty case).
        /// </summary>
        /// <param name="column">The column of the case to check</param>
        /// <param name="line">The line of the case to check</param>
        /// <returns>true if case is isolated, false otherwise</returns>
        private bool CheckIsolation(int column, int line)
        {
            bool isIsolated = true;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    try
                    {
                        if (board[column + i, line + j] == PlayerWhite.ID ||
                            board[column + i, line + j] == PlayerBlack.ID)
                        {
                            isIsolated = false;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            return isIsolated;
        }
        
        /// <summary>
        /// Check if their are change on all the diagonals of the given disc
        /// </summary>
        /// <param name="line">The indicated line</param>
        /// <param name="column">The indicated column</param>
        /// <param name="playerID">The player ID</param>
        /// <returns>The list of cases to change if this move is applied on the board</returns>
        private List<Tuple<int, int>> CheckChangesOnBoard(int line, int column, int playerID)
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

            int otherPlayer = PlayerWhite.ID;

            if(playerID == PlayerWhite.ID)
            {
                otherPlayer = PlayerBlack.ID;
            }

            List<Tuple<int, int>> casesToChange = new List<Tuple<int, int>>();

            // For each direction
            for (int i = 0; i < incDirections.GetLength(0); i++)
            {
                List<Tuple<int, int>> casesToChangeInCurrentDirection = new List<Tuple<int, int>>();
                int currentCol = column + incDirections[i, 1], currentLine = line + incDirections[i, 0];
                bool isValidCase = true;
                
                casesToChangeInCurrentDirection.Add(new Tuple<int, int>(column, line));

                // While the current column and the current line are in the board
                while (isValidCase &&
                    currentCol >= 0 &&
                    currentCol < columnsNumber && 
                    currentLine >= 0 &&
                    currentLine < linesNumber)
                {
                    // Don't apply change if their is an empty case
                    if (board[currentCol, currentLine] == emptyCaseID)
                    {
                        isValidCase = false;
                    }
                    else if (board[currentCol, currentLine] == playerID)
                    {
                        // Doesn't apply changes if the cases to change is only the placed disc or less.
                        if (casesToChangeInCurrentDirection.Count <= 1)
                        {
                            isValidCase = false;
                        }
                        else
                        {
                            casesToChange.AddRange(casesToChangeInCurrentDirection);
                        }
                    }
                    else if(board[currentCol, currentLine] == otherPlayer)
                    {
                        casesToChangeInCurrentDirection.Add(new Tuple<int, int>(currentCol, currentLine));
                    }
                    else
                    {
                        isValidCase = false;
                    }

                    currentCol += incDirections[i, 1];
                    currentLine += incDirections[i, 0];
                }
            }

            return casesToChange;
        }

        /// <summary>
        /// Update the score.
        /// </summary>
        /// <param name="addToPlayerId"></param>
        /// <param name="addedScore"></param>
        private void UpdatePlayerScore(int addToPlayerId, int addedScore)
        {
            if(addToPlayerId == PlayerBlack.ID)
            {
                PlayerBlack.Score += addedScore;
                PlayerWhite.Score -= (addedScore - 1);
            }
            else
            {
                PlayerWhite.Score += addedScore;
                PlayerBlack.Score -= (addedScore - 1);
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
            board[columnCenter, lineCenter] = PlayerWhite.ID;
            board[columnCenter + 1, lineCenter + 1] = PlayerWhite.ID;
            board[columnCenter, lineCenter + 1] = PlayerBlack.ID;
            board[columnCenter + 1, lineCenter] = PlayerBlack.ID;

            PlayerWhite.Score = 2;
            PlayerBlack.Score = 2;
        }
    }
}
