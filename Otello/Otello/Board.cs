using System;
using System.Collections.Generic;

namespace Otello
{
    class Board : IPlayable.IPlayable
    {
        public const int COLUMNS_NUMBER = 9;
        public const int LINES_NUMBER = 7;

        private const int EMPTY_CASE_ID = -1;
        private const int PLAYER_BLACK_CASE_ID = 1;
        private const int PLAYER_WHITE_CASE_ID = 0;

        public Player PlayerBlack { get; private set; }
        public Player PlayerWhite { get; private set; }

        private readonly int[,] board;

        public Board()
        {
            PlayerBlack = new Player(PLAYER_BLACK_CASE_ID, "Black player");
            PlayerWhite = new Player(PLAYER_WHITE_CASE_ID, "White player");

            board = new int[COLUMNS_NUMBER, LINES_NUMBER];

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
            return "IA_WH";
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            // Check if the move is not out of the board and
            // if their is not a disc on the case.
            if (!CheckUsableCase(column, line))
            {
                return false;
            }

            // Check if the case is isolated
            if (CheckIsolation(column, line))
            {
                return false;
            }

            // Check if their is change to make on the board.
            List<Tuple<int, int>> casesToChange = CheckChangesOnBoard(line, column, isWhite);

            if (casesToChange.Count <= 0)
            {
                return false;
            }

            return true;
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            List<Tuple<int, int>> casesToChange = CheckChangesOnBoard(line, column, isWhite);
                
            if(casesToChange.Count > 0)
            {
                int currentPlayerID;

                if (isWhite)
                {
                    currentPlayerID = PlayerWhite.ID;
                }
                else
                {
                    currentPlayerID = PlayerBlack.ID;
                }

                UpdatePlayerScore(currentPlayerID, casesToChange.Count);

                ApplyChangesOnBoard(casesToChange, currentPlayerID);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get all the next possible moves for a given player id.
        /// </summary>
        /// <param name="isWhite">Is true if white, flase if black</param>
        /// <returns>The next possible moves</returns>
        public List<Tuple<int, int>> GetNextPossibleMoves(bool isWhite)
        {
            List<Tuple<int, int>> nextPossibleCases = new List<Tuple<int, int>>();

            for (int col = 0; col < COLUMNS_NUMBER; col++)
            {
                for (int line = 0; line < LINES_NUMBER; line++)
                {
                    if (board[col, line] != PlayerWhite.ID &&
                        board[col, line] != PlayerBlack.ID &&
                        CheckUsableCase(col, line) &&
                        !CheckIsolation(col, line))
                    {
                        List<Tuple<int, int>> casesToChange = CheckChangesOnBoard(line, col, isWhite);

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

            for (int i = 0; i < COLUMNS_NUMBER; i++)
            {
                Console.Write(i + "\t");
            }

            Console.WriteLine();

            for (int i = 0; i < LINES_NUMBER; i++)
            {
                Console.Write(i + "\t");

                for (int j = 0; j < COLUMNS_NUMBER; j++)
                {
                    Console.Write(board[j, i] + "\t");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Check if the case is on the board and if the case is not occupied.
        /// </summary>
        /// <param name="column">The column of the case on the board</param>
        /// <param name="line">The line of the case on the board</param>
        /// <returns>True if case is on the board and if it's not occupied, false otherwise</returns>
        private bool CheckUsableCase(int column, int line)
        {
            if (column >= 0 &&
                column < COLUMNS_NUMBER &&
                line >= 0 &&
                line < LINES_NUMBER &&
                board[column, line] != PlayerWhite.ID &&
                board[column, line] != PlayerBlack.ID)
            {
                return true;
            }

            return false;
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
                    // Test if the case is in the board and
                    if (i != 0 &&
                        j != 0 &&
                        column + i >= 0 &&
                        column + i < COLUMNS_NUMBER &&
                        line + j >= 0 &&
                        line + j < LINES_NUMBER)
                    {
                        if (board[column + i, line + j] == PlayerWhite.ID ||
                            board[column + i, line + j] == PlayerBlack.ID)
                        {
                            isIsolated = false;
                        }
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
        /// <param name="isWhite">Is true if white, flase if black</param>
        /// <returns>The list of cases to change if this move is applied on the board</returns>
        private List<Tuple<int, int>> CheckChangesOnBoard(int line, int column, bool isWhite)
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

            int otherPlayer = PlayerBlack.ID;
            int playerID = PlayerWhite.ID;

            if(isWhite)
            {
                otherPlayer = PlayerWhite.ID;
                playerID = PlayerBlack.ID;
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
                    currentCol < COLUMNS_NUMBER && 
                    currentLine >= 0 &&
                    currentLine < LINES_NUMBER)
                {
                    // Don't apply change if their is an empty case
                    if (board[currentCol, currentLine] == EMPTY_CASE_ID)
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
            for (int i = 0; i < COLUMNS_NUMBER; i++)
            {
                for (int j = 0; j < LINES_NUMBER; j++)
                {
                    board[i, j] = EMPTY_CASE_ID;
                }
            }

            int columnCenter = COLUMNS_NUMBER / 2;
            int lineCenter = LINES_NUMBER / 2;

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
