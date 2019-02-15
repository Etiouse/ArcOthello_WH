using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello_HW
{
    class IANode
    {
        public Board LocalBoard { get; set; }
        public Tuple<int, int> PreviousMove { get; set; }
        public List<Tuple<int, int>> Moves { get; set; }

        public IANode(Board board, Tuple<int, int> previousNode, List<Tuple<int, int>> moves)
        {
            LocalBoard = board;
            PreviousMove = previousNode;
            Moves = moves;
        }

        private Tuple<int, int> CornersScore()
        {
            int whiteCorners = 0;
            int blackCorners = 0;

            int[,] grid = LocalBoard.GetBoard();

            if (grid[0, 0] == 0) whiteCorners++;
            else if (grid[0, 0] == 1) blackCorners++;

            if (grid[0, grid.GetLength(1) - 1] == 0) whiteCorners++;
            else if (grid[0, grid.GetLength(1) - 1] == 1) blackCorners++;

            if (grid[grid.GetLength(0) - 1, 0] == 0) whiteCorners++;
            else if (grid[grid.GetLength(0) - 1, 0] == 1) blackCorners++;

            if (grid[grid.GetLength(0) - 1, grid.GetLength(1) - 1] == 0) whiteCorners++;
            else if (grid[grid.GetLength(0) - 1, grid.GetLength(1) - 1] == 1) blackCorners++;


            return new Tuple<int, int>(whiteCorners, blackCorners);
        }

        /// <summary>
        /// Evaluation function
        /// </summary>
        /// <returns>The number that correspond to the score of the function</returns>
        public float Eval(bool whiteTurn, int minOrMax)
        {
            int maxPlayerScore;
            int minPlayerScore;
            int maxPlayerCorners;
            int minPlayerCorners;
            int maxPlayerMobility;
            int minPlayerMobility;

            float weightScore = 0.33f;
            float weightCorners = 0.33f;
            float weightMobility = 0.33f;

            Tuple<int, int> cornersScore = CornersScore();
            int whiteMobility = LocalBoard.GetNextPossibleMoves(true).Capacity;
            int blackMobility = LocalBoard.GetNextPossibleMoves(false).Capacity;

            if (minOrMax == 1 ^ whiteTurn)
            {
                maxPlayerScore = LocalBoard.BlackScore;
                minPlayerScore = LocalBoard.WhiteScore;
                maxPlayerCorners = cornersScore.Item2;
                minPlayerCorners = cornersScore.Item1;
                maxPlayerMobility = blackMobility;
                minPlayerMobility = whiteMobility;
            } 
            else
            {
                maxPlayerScore = LocalBoard.WhiteScore;
                minPlayerScore = LocalBoard.BlackScore;
                maxPlayerCorners = cornersScore.Item1;
                minPlayerCorners = cornersScore.Item2;
                maxPlayerMobility = whiteMobility;
                minPlayerMobility = blackMobility;
            }

            // Parity Score
            float scoreParity = 0f;
            if (maxPlayerScore + minPlayerScore != 0)
                scoreParity = (100f * (maxPlayerScore - minPlayerScore)) / (maxPlayerScore + minPlayerScore);

            // Mobility Score
            float scoreMobility = 0f;
            if (maxPlayerMobility + minPlayerMobility != 0)
                scoreMobility = (100f * (maxPlayerMobility - minPlayerMobility)) / (maxPlayerMobility + minPlayerMobility);

            // Corners Score
            float scoreCorners = 0f;
            if (maxPlayerCorners + minPlayerCorners != 0)
                scoreCorners = (100f * (maxPlayerCorners - minPlayerCorners)) / (maxPlayerCorners + minPlayerCorners);

            return weightScore * scoreParity + weightMobility * scoreMobility + weightCorners * scoreCorners;
        }

        /// <summary>
        /// Check if the game is over
        /// </summary>
        /// <returns>True if the game is over, false otherwise</returns>
        public bool Final()
        {
            return Moves.Count <= 0;
        }

        /// <summary>
        /// Applies the board state to this node and return the new node
        /// </summary>
        /// <param name="move">The move to apply to the board</param>
        /// <returns>The new node if this board state</returns>
        public IANode Apply(Tuple<int, int> move, bool whiteTurn)
        {
            Board board = LocalBoard.DeepCopyBoard();

            bool isPlayable = board.IsPlayable(move.Item1, move.Item2, whiteTurn);
            IANode node = null;

            if (isPlayable)
            {
                board.PlayMove(move.Item1, move.Item2, whiteTurn);

                node = new IANode(board, move, board.GetNextPossibleMoves(!whiteTurn));
            }

            return node;
        }
    }
}
