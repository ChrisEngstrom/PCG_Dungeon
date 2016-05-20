using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// \file Hallway.cs
namespace PCG_Dungeon {
    [Serializable()]
    class Hallway : Area {
        public Hallway( short x, short y, short w, short h )
            : base( x, y, w, h ) {

        }
    }
}
