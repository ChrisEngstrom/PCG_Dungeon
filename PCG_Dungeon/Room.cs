using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCG_Dungeon {
    class Room {
        // Grid coordinates for corners of the room
        public short x1, y1,
                     x2, y2;

        // Width and height of the room in terms of the grid
        public short w,
                     h;

        // Center point of the room
        public Point center;

        // Constructor for creating a new room
        public Room( short x, short y, short w, short h ) {
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

        // Returns true if this room intersects provided room
        public bool Intersects(Room room) {
            return (x1 <= room.x2 && x2 >= room.x1 &&
                    y1 <= room.y2 && y2 >= room.y1);
        }
    }
}
