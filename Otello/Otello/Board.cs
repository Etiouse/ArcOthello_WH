﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Othello_HW
{
    class Board : IPlayable.IPlayable, INotifyPropertyChanged
    {
        public const int COLUMNS_NUMBER = 9;
        public const int LINES_NUMBER = 7;
        
        public int[,] CurrentBoard { get; set; }

        public Player PlayerBlack { get; private set; }
        public Player PlayerWhite { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public int WhiteScore
        {
            get { return PlayerWhite.Score; }
            set { PlayerWhite.Score = value; RaisePropertyChanged("WhiteScore"); }
        }
        public int BlackScore
        {
            get { return PlayerBlack.Score; }
            set { PlayerBlack.Score = value; RaisePropertyChanged("BlackScore"); }
        }

        public string WhiteTimeString
        {
            get { return whiteTimeString; }
            set { whiteTimeString = value; RaisePropertyChanged("WhiteTimeString"); }
        }
        public string BlackTimeString
        {
            get { return blackTimeString; }
            set { blackTimeString = value; RaisePropertyChanged("BlackTimeString"); }
        }

        public TimeSpan WhiteTime
        {
            get { return PlayerWhite.Time; }
            set
            {
                PlayerWhite.Time = value;
                WhiteTimeString = new TimeSpan(PlayerWhite.Time.Hours, PlayerWhite.Time.Minutes, PlayerWhite.Time.Seconds).ToString();
            }
        }
        public TimeSpan BlackTime
        {
            get { return PlayerBlack.Time; }
            set
            {
                PlayerBlack.Time = value;
                BlackTimeString = new TimeSpan(PlayerBlack.Time.Hours, PlayerBlack.Time.Minutes, PlayerBlack.Time.Seconds).ToString();
            }
        }

        private const int EMPTY_CASE_ID = -1;
        private const int PLAYER_BLACK_CASE_ID = 1;
        private const int PLAYER_WHITE_CASE_ID = 0;
        private string whiteTimeString;
        private string blackTimeString;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Default constructor.
        /// Init the board.
        /// </summary>
        public Board()
        {
            PlayerBlack = new Player(PLAYER_BLACK_CASE_ID, "bleu");
            PlayerWhite = new Player(PLAYER_WHITE_CASE_ID, "rouge");

            CurrentBoard = new int[COLUMNS_NUMBER, LINES_NUMBER];
            whiteTimeString = new TimeSpan(0, 0, 0).ToString();
            blackTimeString = new TimeSpan(0, 0, 0).ToString();

            InitBoard();
        }

        /// <summary>
        /// Deep copy an array of int and return it
        /// </summary>
        /// <param name="original">The original array of int that will be copied</param>
        /// <returns>The copied array of int</returns>
        public int[,] DeepCopyIntArray(int[,] original)
        {
            int[,] copy = new int[original.GetLength(0), original.GetLength(1)];
            Array.Copy(original, copy, original.Length);

            return copy;
        }

        public Board DeepCopyBoard()
        {
            Board board = new Board
            {
                CurrentBoard = DeepCopyIntArray(CurrentBoard),
                WhiteScore = WhiteScore,
                BlackScore = BlackScore
            };

            return board;
        }

        /// <summary>
        /// Returns the number of black tiles
        /// </summary>
        /// <returns></returns>
        public int GetBlackScore()
        {
            return PlayerBlack.Score;
        }

        /// <summary>
        /// Returns the number of white tiles on the board
        /// </summary>
        /// <returns></returns>
        public int GetWhiteScore()
        {
            return PlayerWhite.Score;
        }

        /// <summary>
        /// Returns a reference to a 2D array with the board status
        /// </summary>
        /// <returns>The 7x9 tiles status</returns>
        public int[,] GetBoard()
        {
            return CurrentBoard;
        }

        /// <summary>
        /// Returns the IA's name
        /// </summary>
        /// <returns>true or false</returns>
        public string GetName()
        {
            return "IAWH";
        }

        /// <summary>
        /// Asks the game engine next (valid) move given a game position
        /// The board assumes following standard move notation:
        /// 
        ///             A B C D E F G H I
        ///         [ ][0 1 2 3 4 5 6 7 8]     (first index)
        ///        1 0
        ///        2 1
        ///        3 2        X
        ///        4 3            X
        ///        5 4
        ///        6 5
        ///        7 6
        ///       
        ///          Column Line
        ///  E.g.:    D3, F4 game notation will map to {3,2} resp. {5,3}
        /// </summary>
        /// <param name="game">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="level">an integer value to set the level of the IA, 5 normally</param>
        /// <param name="whiteTurn">true if white players turn, false otherwise</param>
        /// <returns>The column and line indices. Will return {-1,-1} as PASS if no possible move </returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            List<Tuple<int, int>> pos = GetNextPossibleMoves(whiteTurn);

            Board board = new Board()
            {
                CurrentBoard = game,
                WhiteScore = WhiteScore,
                BlackScore = BlackScore
            };

            Tuple<int, int> nextMove = AlphaBeta(new IANode(board, null, pos), level, 1, whiteTurn).Item2;

            return nextMove;
        }

        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 8</param>
        /// <param name="line">value between 0 and 6</param>
        /// <param name="isWhite"></param>
        /// <returns>true or false</returns>
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

        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 8</param>
        /// <param name="line">value between 0 and 6</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        /// <returns></returns>
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
                    if (CheckUsableCase(col, line) &&
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
        /// Check if the board is full
        /// </summary>
        /// <returns>True if the board is full, false otherwise</returns>
        public bool IsBoardFull()
        {
            for (int i = 0; i < LINES_NUMBER; i++)
            {
                for (int j = 0; j < COLUMNS_NUMBER; j++)
                {
                    if (CurrentBoard[j, i] == EMPTY_CASE_ID)
                    {
                        return false;
                    }
                }
            }

            return true;
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
                    Console.Write(CurrentBoard[j, i] + "\t");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Find the next node to play for the IA with the alpha beta algorithme
        /// Based on the given pseudo code (IA course).
        /// </summary>
        /// <param name="node">The root node</param>
        /// <param name="depth">The depth of the algorithm in the tree</param>
        /// <param name="minOrMax">Maximize = 1, Minimize = -1</param>
        /// <param name="parentVal">The parent value, first call +infinity to maximize, -infinity to minimize</param>
        /// <returns>The winner node</returns>
        private Tuple<float, Tuple<int, int>> AlphaBeta(IANode node, int depth, int minOrMax, bool whiteTurn, float parentVal = float.MaxValue)
        {
            // If depth 0 is reached or if the game is finished (TODO)
            if(depth == 0 || node.Final())
            {
                return new Tuple<float, Tuple<int, int>>(node.Eval(whiteTurn, minOrMax), new Tuple<int, int>(-1, -1));
            }

            float currentVal = minOrMax * float.MinValue;
            Tuple<int, int> currentMove = new Tuple<int, int>(-1, -1);

            // Check for all possible move on the current board
            foreach (Tuple<int, int> move in node.Moves)
            {
                // Apply the current move and create a new node with it
                IANode newNode = node.Apply(move, whiteTurn);

                // Recursiv call of AlphaBeta with changes
                Tuple<float, Tuple<int, int>> res = AlphaBeta(newNode, depth - 1, -minOrMax, !whiteTurn, currentVal);
                
                // Test if the new node of the parent node has a best value
                if (res.Item1 * minOrMax > currentVal * minOrMax)
                {
                    currentVal = res.Item1;
                    currentMove = new Tuple<int, int>(move.Item1, move.Item2);

                    if (currentVal * minOrMax > parentVal * minOrMax)
                    {
                        break;
                    }
                }
            }

            return new Tuple<float, Tuple<int, int>>(currentVal, currentMove);
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
                CurrentBoard[column, line] != PlayerWhite.ID &&
                CurrentBoard[column, line] != PlayerBlack.ID)
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
                    if ((i != 0 || j != 0) &&
                        column + i >= 0 &&
                        column + i < COLUMNS_NUMBER &&
                        line + j >= 0 &&
                        line + j < LINES_NUMBER)
                    {
                        if (CurrentBoard[column + i, line + j] == PlayerWhite.ID ||
                            CurrentBoard[column + i, line + j] == PlayerBlack.ID)
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

            int playerID = PlayerWhite.ID;
            int otherPlayer = PlayerBlack.ID;

            if(!isWhite)
            {
                playerID = PlayerBlack.ID;
                otherPlayer = PlayerWhite.ID;
            }

            List<Tuple<int, int>> casesToChange = new List<Tuple<int, int>>();

            // For each direction
            for (int i = 0; i < incDirections.GetLength(0); i++)
            {
                List<Tuple<int, int>> casesToChangeInCurrentDirection = new List<Tuple<int, int>>();
                int currentCol = column + incDirections[i, 1], currentLine = line + incDirections[i, 0];
                bool isValidCase = true;

                // While the current column and the current line are in the board
                while (isValidCase &&
                    currentCol >= 0 &&
                    currentCol < COLUMNS_NUMBER && 
                    currentLine >= 0 &&
                    currentLine < LINES_NUMBER)
                {
                    // Don't apply change if their is an empty case
                    if (CurrentBoard[currentCol, currentLine] == EMPTY_CASE_ID)
                    {
                        isValidCase = false;
                    }
                    else if (CurrentBoard[currentCol, currentLine] == playerID)
                    {
                        // Doesn't apply changes if the cases to change is only the placed disc or less.
                        if (casesToChangeInCurrentDirection.Count > 0)
                        {
                            casesToChange.AddRange(casesToChangeInCurrentDirection);
                        }

                        isValidCase = false;
                    }
                    else if(CurrentBoard[currentCol, currentLine] == otherPlayer)
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

                casesToChangeInCurrentDirection.Clear();
            }

            if (casesToChange.Count > 0)
            {
                casesToChange.Add(new Tuple<int, int>(column, line));
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
                BlackScore += addedScore;
                WhiteScore -= (addedScore - 1);
            }
            else
            {
                WhiteScore += addedScore;
                BlackScore -= (addedScore - 1);
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
                CurrentBoard[item.Item1, item.Item2] = playerID;
            }
        }

        /// <summary>
        /// Fill the board with init discs (4 in the center).
        /// </summary>
        public void InitBoard()
        {
            for (int i = 0; i < COLUMNS_NUMBER; i++)
            {
                for (int j = 0; j < LINES_NUMBER; j++)
                {
                    CurrentBoard[i, j] = EMPTY_CASE_ID;
                }
            }

            int columnCenter = COLUMNS_NUMBER / 2;
            int lineCenter = LINES_NUMBER / 2;

            // init board with 4 discs (2 for each player) on the center of the board.
            CurrentBoard[columnCenter, lineCenter] = PlayerBlack.ID;
            CurrentBoard[columnCenter - 1, lineCenter + 1] = PlayerBlack.ID;
            CurrentBoard[columnCenter, lineCenter + 1] = PlayerWhite.ID;
            CurrentBoard[columnCenter - 1, lineCenter] = PlayerWhite.ID;

            WhiteScore = 2;
            BlackScore = 2;
        }
    }
}
