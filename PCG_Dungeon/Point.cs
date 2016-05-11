using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCG_Dungeon {
    class Point {
        public short x { get; set; }
        public short y { get; set; }

        public Point() {
            x = y = 0;
        }

        public Point( short x, short y ) {
            this.x = x;
            this.y = y;
        }
    }
}
