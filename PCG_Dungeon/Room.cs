using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// \file Room.cs
namespace PCG_Dungeon {
    class Room : Area {
        public short Area { get; private set; }

        public Room(short x, short y, short w, short h)
            : base( x, y, w, h ) {
            this.Area = (short)(w * h);
        }
    }
}
