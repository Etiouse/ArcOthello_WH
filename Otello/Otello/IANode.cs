using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otello
{
    class IANode
    {
        public int[,] LocalBoard { get; set; }
        public Tuple<int, int> PreviousMove { get; set; }
        public List<Tuple<int, int>> Moves { get; set; }

        private readonly int[,] evaluationMatrix = new int[,] { { 30,-25, 10, 5 , 1 , 5 , 10,-25, 30},
                                                                {-25,-25, 1 , 1 , 1 , 1 , 1 ,-25,-25},
                                                                { 10, 1 , 5 , 2 , 2 , 2 , 5 , 1 , 10},
                                                                { 5 , 1 , 2 , 1 , 1 , 1 , 2 , 1 , 5 },
                                                                { 10, 1 , 5 , 2 , 2 , 2 , 5 , 1 , 10},
                                                                {-25,-25, 1 , 1 , 1 , 1 , 1 ,-25,-25},
                                                                { 30,-25, 10, 5 , 1 , 5 , 10,-25, 30} };

        public IANode(int[,] board, Tuple<int, int> previousNode, List<Tuple<int, int>> moves)
        {
            LocalBoard = Board.DeepCopyIntArray(board);
            PreviousMove = previousNode;
            Moves = moves;
        }

        /// <summary>
        /// Evaluation function
        /// </summary>
        /// <returns>The number that correspond to the score of the function</returns>
        public float Eval()
        {
            return evaluationMatrix[PreviousMove.Item2, PreviousMove.Item1];
        }

        /// <summary>
        /// Check if the game is over
        /// </summary>
        /// <returns>True if the game is over, false otherwise</returns>
        public bool Final()
        {
            return false;
        }

        /// <summary>
        /// Applies the board state to this node and return the new node
        /// </summary>
        /// <param name="move">The move to apply to the board</param>
        /// <returns>The new node if this board state</returns>
        public IANode Apply(Tuple<int, int> move, bool whiteTurn)
        {
            Board board = new Board();
            board.CurrentBoard = Board.DeepCopyIntArray(LocalBoard);

            bool isPlayable = board.IsPlayable(move.Item1, move.Item2, whiteTurn);
            IANode node = null;

            if (isPlayable)
            {
                board.PlayMove(move.Item1, move.Item2, whiteTurn);

                node = new IANode(board.CurrentBoard, move, board.GetNextPossibleMoves(!whiteTurn));
            }

            return node;
        }
    }
}
