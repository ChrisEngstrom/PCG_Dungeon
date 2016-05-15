using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_Dungeon {
    /// <summary>
    ///     The HorizontalHallway class is a Hallway with a height of one unit
    ///         that is used in conjunction with a VerticalHallway to connect
    ///         two Room%s.
    /// </summary>
    class HorizontalHallway : Hallway {
        /// <summary>
        ///     The HorizontalHallway constructor creates a Hallway with a
        ///         height of one unit.
        /// </summary>
        /// <param name="x">
        ///     The HorizontalHallway%'s starting x coordinate.
        /// </param>
        /// <param name="y">
        ///     The HorizontalHallway%'s starting y coordinate.
        /// </param>
        /// <param name="w">
        ///     The HorizontalHallway%'s width.
        /// </param>
        public HorizontalHallway( short x, short y, short w )
            : base( x, y, w, 1 ) {
            
        }
    }
}
