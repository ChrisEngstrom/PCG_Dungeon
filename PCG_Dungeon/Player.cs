using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// \file Player.cs
namespace PCG_Dungeon {
    class Player{
        public Point Position { get; set; }

        /// <summary>
        ///     The Player constructor initializes the instance.
        /// </summary>
        /// <param name="pos">
        ///     The Point position that the Player is currently at.
        /// </param>
        public Player( Point pos ) {
            Position = pos;
        }
    }
}
