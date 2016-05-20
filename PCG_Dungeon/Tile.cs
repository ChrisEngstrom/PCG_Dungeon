using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// \file Tile.cs
namespace PCG_Dungeon {
    public enum TileState {
        EMPTY = 0,
        ROOM = 1,
        START = 2,
        END = 3,
        HALL = 4,
        WALL = 5,
        PLAYER = 6,
    }

    /// <summary>
    ///     The Tile class represents a location on the Dungeon board
    /// </summary>
    [Serializable()]
    class Tile {
        public TileState TileState { get; set; }

        /// <summary>
        ///     The default constructor sets the TileState to EMPTY
        /// </summary>
        public Tile() {
            TileState = TileState.EMPTY;
        }

        /// <summary>
        ///     This constructor sets the desired TileState on creation
        /// </summary>
        /// <param name="tileState">
        ///     The desired TileState
        /// </param>
        public Tile(TileState tileState) {
            TileState = tileState;
        }

        public override string ToString() {
            return ((short)TileState).ToString();
        }
    }
}
