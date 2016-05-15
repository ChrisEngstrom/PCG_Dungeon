using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_Dungeon {
    /// <summary>
    ///     The VerticalHallway class is a Hallway with a width of one unit
    ///         that is used in conjunction with a HorizontalHallway to
    ///         connect two Room%s.
    /// </summary>
    class VerticalHallway : Hallway {
        /// <summary>
        ///     The VerticalHallway constructor creates a Hallway with a width
        ///         of one unit.
        /// </summary>
        /// <param name="x">
        ///     The VerticalHallway%'s starting x coordinate.
        /// </param>
        /// <param name="y">
        ///     The VerticalHallway%'s starting y coordinate.
        /// </param>
        /// <param name="h">
        ///     The VerticalHallway%'s height.
        /// </param>
        public VerticalHallway( short x, short y, short h )
            : base( x, y, 1, h ) {
            
        }
    }
}
