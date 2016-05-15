using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// \file Area.cs
namespace PCG_Dungeon {
    class Area {
        // Grid coordinates for the corners of the area
        public short x1, y1,
                     x2, y2;

        // Width and height of the area in terms of the grid
        public short w,
                     h;

        // Center point of the area
        public Point center;

        // Constructor for creating a new area
        public Area( short x, short y, short w, short h ) {
            x1 = x;
            y1 = y;

            x2 = (short)(x + w);
            y2 = (short)(y + h);

            this.w = w;
            this.h = h;

            center = new Point();

            center.x = (short)Math.Floor( (double)((x1 + x2) / 2) );
            center.y = (short)Math.Floor( (double)((y1 + y2) / 2) );
        }

        // Returns true if this area intersects provided area
        public bool Intersects(Area area) {
            return (x1 <= area.x2 && x2 >= area.x1 &&
                    y1 <= area.y2 && y2 >= area.y1);
        }
    }
}
