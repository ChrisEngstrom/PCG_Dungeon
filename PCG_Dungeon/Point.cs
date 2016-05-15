using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// \file Point.cs
namespace PCG_Dungeon {
    class Point {
        public short x { get; set; }
        public short y { get; set; }

        /// <summary>
        ///     The default constructor creates a Point at (0,0)
        /// </summary>
        public Point() {
            x = y = 0;
        }

        /// <summary>
        ///     Creates a Point with the given x and y coordinates
        /// </summary>
        /// <param name="x">
        ///     The x coordinate for the Point
        /// </param>
        /// <param name="y">
        ///     The y coordinate for the Point
        /// </param>
        public Point( short x, short y ) {
            this.x = x;
            this.y = y;
        }
    }
}
