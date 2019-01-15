using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otello
{
    class IANode
    {
        public List<int[,]> BoardStates { get; set; }
        public float Value { get; set; }

        public IANode(List<int[,]> boardStates, float value)
        {
            BoardStates = new List<int[,]>(boardStates);
            Value = value;
        }

        /// <summary>
        /// Evaluation function
        /// </summary>
        /// <returns>The number that correspond to the score of the function</returns>
        public float Eval()
        {
            return 0;
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
        /// <param name="boardState">The new board state</param>
        /// <returns>The new node if this board state</returns>
        public IANode Apply(int[,] boardState)
        {
            IANode node = new IANode(BoardStates, Value);
            node.BoardStates.Add(boardState);

            return node;
        }
    }
}
