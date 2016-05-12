using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_Dungeon {
    /// <summary>
    ///     The Player class is a Point that may hold more functionality in the
    ///         future.
    /// </summary>
    class Player : Point {
        /// <summary>
        ///     The Player constructor takes and passes the required fields to
        ///         the Point constructor.
        /// </summary>
        /// <param name="x">
        ///     The x coordinate for the Player in the Dungeon.
        /// </param>
        /// <param name="y">
        ///     The y coordinate for the Player in the Dungeon.
        /// </param>
        public Player(short x, short y)
            : base( x, y ) {

        }
    }
}
