﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCG_Dungeon {
    /// <summary>
    ///     The Dungeon class represents the whole playable grid that includes
    ///         all of the Room%s and Corridors as well as the Player.
    /// </summary>
    class Dungeon {
        public enum TileState {
            OPEN = 0,
            WALL = 1,
            PLAYER = 2,
            START = 3,
            END = 4,
        }

        Random rand;
        Player player;
        short[,] board;
        List<Room> roomList;

        short DUNGEON_WIDTH,
              DUNGEON_HEIGHT,
              MIN_ROOM_SIZE,
              MAX_ROOM_SIZE,
              NUM_ROOMS;

        public int Width {
            get {
                return DUNGEON_WIDTH;
            }

            private set { }
        }

        public int Height {
            get {
                return DUNGEON_HEIGHT;
            }

            private set { }
        }

        public int MinRoomSize {
            get {
                return MIN_ROOM_SIZE;
            }

            private set { }
        }

        public int MaxRoomSize {
            get {
                return MAX_ROOM_SIZE;
            }

            private set { }
        }

        public int NumRooms {
            get {
                return roomList.Count();
            }

            private set { }
        }

        /// <summary>
        ///     The Dungeon constructor takes the required parameters and
        ///         creates the necessary structures for later use.
        /// </summary>
        /// <param name="width">
        ///     The width of the entire Dungeon.
        /// </param>
        /// <param name="height">
        ///     The height of the entire Dungeon.
        /// </param>
        /// <param name="minRoomSize">
        ///     The minimum size for Room%s in the Dungeon.
        /// </param>
        /// <param name="maxRoomSize">
        ///     The maximum size for Room%s in the Dungeon.
        /// </param>
        /// <param name="numRooms">
        ///     The number of Room%s to attempt to place in the Dungeon.
        /// </param>
        public Dungeon( short width, short height, short minRoomSize, short maxRoomSize, short numRooms ) {
            DUNGEON_WIDTH = width;
            DUNGEON_HEIGHT = height;
            MIN_ROOM_SIZE = minRoomSize;
            MAX_ROOM_SIZE = maxRoomSize;
            NUM_ROOMS = numRooms;

            rand = new Random();
            board = new short[DUNGEON_WIDTH, DUNGEON_HEIGHT];
            roomList = new List<Room>();

            initializeBoard();
        }

        /// <summary>
        ///     initializeBoard() sets every tile in the Dungeon to the WALL
        ///         state.
        /// </summary>
        private void initializeBoard() {
            for ( int col = 0; col < DUNGEON_WIDTH; col++ ) {
                for ( int row = 0; row < DUNGEON_HEIGHT; row++ ) {
                    board[col, row] = (short)TileState.WALL;
                }
            }
        }

        /// <summary>
        ///     Generates Room%s of varying sizes using the limits passed in to
        ///         the constructor.
        /// </summary>
        /// <param name="endRoomIsNewStartRoom">
        ///     true = Use the current end Room as the next Dungeon%'s starting Room.<br/>
        ///     false = Start with a completely new set of Room%s for the next Dungeon.
        /// </param>
        public void PlaceRooms( bool endRoomIsNewStartRoom ) {
            Room startRoom = null;

            if ( roomList.Count > 0 &&
                 endRoomIsNewStartRoom ) {
                startRoom = roomList.Last();

                roomList.Clear();

                roomList.Add( startRoom );
            } else {
                roomList.Clear();
            }

            initializeBoard();

            while ( roomList.Count < NUM_ROOMS ) {
                short w = (short)rand.Next( MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1 ),
                      h = (short)rand.Next( MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1 ),
                      x = (short)(rand.Next( (DUNGEON_WIDTH - w + 1 - 2) ) + 1),    // The (- 2) and (+ 1) keeps the rooms
                      y = (short)(rand.Next( (DUNGEON_HEIGHT - h + 1 - 2) ) + 1);   //  off of the edges by one tile
                bool intersection = false;

                Room newRoom = new Room( x, y, w, h );

                foreach ( Room existingRoom in roomList ) {
                    if ( newRoom.Intersects( existingRoom ) ) {
                        intersection = true;
                        break;
                    }
                }

                if ( !intersection ) {
                    roomList.Add( newRoom );

                    // Save the center point of this new room
                    Point newCenter = new Point( newRoom.center.x, newRoom.center.y );

                    if ( roomList.Count > 1 ) {
                        Point prevCenter = new Point( roomList[roomList.Count - 2].center.x,
                                                      roomList[roomList.Count - 2].center.y );

                        // Randomly choose to either do the hCorridor first or the vCorridor first
                        if ( rand.Next( 2 ) == 1 ) {
                            hCorridor( (int)prevCenter.x, (int)newCenter.x, (int)prevCenter.y );
                            vCorridor( (int)prevCenter.y, (int)newCenter.y, (int)newCenter.x );
                        } else {
                            vCorridor( (int)prevCenter.y, (int)newCenter.y, (int)prevCenter.x );
                            hCorridor( (int)prevCenter.x, (int)newCenter.x, (int)newCenter.y );
                        }
                    } else if ( roomList.Count == 1 ) {
                        player = new Player( new Point( newRoom.center.x, newRoom.center.y ) );
                    }
                }
            }

            updateBoard();
        }

        /// <summary>
        ///     Creates a horizontal corridor from x1 to x2 using the
        ///         horizontal coordinate y.
        /// </summary>
        /// <param name="x1">
        ///     The x coordinate for the center of the first Room.
        /// </param>
        /// <param name="x2">
        ///     The x coordinate for the center of the second Room.
        /// </param>
        /// <param name="y">
        ///     The y coordinate that the corridor will run along.
        /// </param>
        private void hCorridor( int x1, int x2, int y ) {
            for ( int col = Math.Min( x1, x2 ); col < Math.Max( x1, x2 ) + 1; col++ ) {
                // Remove the wall present
                board[col, y] = (short)TileState.OPEN;
            }
        }

        /// <summary>
        ///     Creates a vertical corridor from y1 to y2 using the vertical
        ///         coordinate x.
        /// </summary>
        /// <param name="y1">
        ///     The y coordinate for the center of the first Room.
        /// </param>
        /// <param name="y2">
        ///     The y coordinate for the center of the second Room.
        /// </param>
        /// <param name="x">
        ///     The x coordinate that the corridor will run along.
        /// </param>
        private void vCorridor( int y1, int y2, int x ) {
            for ( int row = Math.Min( y1, y2 ); row < Math.Max( y1, y2 ) + 1; row++ ) {
                // Remove the wall present
                board[x, row] = (short)TileState.OPEN;
            }
        }

        /// <summary>
        ///     Goes through the Room%s that have been added to the Dungeon and
        ///         sets the appropriate tiles to the correct state. Also sets
        ///         the tile state for the Player%'s location
        /// </summary>
        private void updateBoard() {
            foreach ( Room room in roomList ) {
                for ( int col = room.x1; col < room.x2; col++ ) {
                    for ( int row = room.y1; row < room.y2; row++ ) {
                        if ( room == roomList.First() ) {
                            board[col, row] = (short)TileState.START;
                        } else if ( room == roomList.Last() ) {
                            board[col, row] = (short)TileState.END;
                        } else {
                            board[col, row] = (short)TileState.OPEN;
                        }
                    }
                }
            }

            board[player.Position.x, player.Position.y] = (short)TileState.PLAYER;
        }

        /// <summary>
        ///     Gets the TileState of the position indicated.
        /// </summary>
        /// <param name="col">
        ///     The x coordinate of the tile to get the TileState for.
        /// </param>
        /// <param name="row">
        ///     The y coordinate of the tile to get the TileState for.
        /// </param>
        /// <returns>
        ///     The numerical value that is associated with the corresponding
        ///         TileState.
        /// </returns>
        public int GetPosition( int col, int row ) {
            return board[col, row];
        }

        /// <summary>
        ///     Attempts to move the Player in a certain direction.
        /// </summary>
        /// <param name="x">
        ///     The amount to change the x coordinate of the player.
        /// </param>
        /// <param name="y">
        ///     The amount to change the y coordinate of the player.
        /// </param>
        /// <returns>
        ///     true = Successfully moved Player<br/>
        ///     false = Failed to move Player
        /// </returns>
        private bool movePlayer( int x, int y ) {
            short potentialMoveTile = board[player.Position.x + x, player.Position.y - y];

            if ( potentialMoveTile != (short)TileState.WALL ) {
                // Remove old player piece
                board[player.Position.x, player.Position.y] = (short)TileState.OPEN;

                // Update player piece
                player.Position.x += (short)x;
                player.Position.y -= (short)y;

                // Place updated player piece
                board[player.Position.x, player.Position.y] = (short)TileState.PLAYER;

                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        ///     Attemps to move the Player up one tile.
        /// </summary>
        public void MovePlayerUp() {
            if ( movePlayer( 0, 1 ) ) {
                updateBoard();
            }
        }

        /// <summary>
        ///     Attemps to move the Player right one tile.
        /// </summary>
        public void MovePlayerRight() {
            if ( movePlayer( 1, 0 ) ) {
                updateBoard();
            }
        }

        /// <summary>
        ///     Attemps to move the Player down one tile.
        /// </summary>
        public void MovePlayerDown() {
            if ( movePlayer( 0, -1 ) ) {
                updateBoard();
            }
        }

        /// <summary>
        ///     Attemps to move the Player left one tile.
        /// </summary>
        public void MovePlayerLeft() {
            if ( movePlayer( -1, 0 ) ) {
                updateBoard();
            }
        }

        /// <summary>
        ///     Saves the current Dungeon state out to a set of text files.
        /// </summary>
        public void SaveToFile() {
            // Save the board settings
            using ( System.IO.StreamWriter file = new System.IO.StreamWriter( @"..\..\saved_dungeonSettings.txt" ) ) {
                file.Write( DUNGEON_WIDTH + "," +
                            DUNGEON_HEIGHT + "," +
                            MIN_ROOM_SIZE + "," +
                            MAX_ROOM_SIZE + "," +
                            NUM_ROOMS );

                file.WriteLine();
            }

            // Save the board itself
            using ( System.IO.StreamWriter file = new System.IO.StreamWriter( @"..\..\saved_dungeonBoard.txt" ) ) {
                for ( int row = 0; row < DUNGEON_HEIGHT; row++ ) {
                    for ( int col = 0; col < DUNGEON_WIDTH; col++ ) {
                        file.Write( board[col, row] );
                    }

                    file.WriteLine();
                }
            }

            // Save the list of rooms
            using ( System.IO.StreamWriter file = new System.IO.StreamWriter( @"..\..\saved_dungeonRooms.txt" ) ) {
                foreach ( Room room in roomList ) {
                    file.Write( room.x1 + "," + room.y1 + "," + room.w + "," + room.h );
                    file.WriteLine();
                }
            }
        }

        /// <summary>
        ///     Loads a Dungeon state from a set of text files.
        /// </summary>
        public void LoadFromFile() {
            // Wipe the current state of the board
            initializeBoard();
            roomList.Clear();

            // Load the Dungeon's settings
            using ( System.IO.StreamReader file = new System.IO.StreamReader( @"..\..\saved_dungeonSettings.txt" ) ) {
                string line = file.ReadLine();
                string [] values = line.Split( ',' );

                DUNGEON_WIDTH = short.Parse( values[0] );
                DUNGEON_HEIGHT = short.Parse( values[1] );
                MIN_ROOM_SIZE = short.Parse( values[2] );
                MAX_ROOM_SIZE = short.Parse( values[3] );
                NUM_ROOMS = short.Parse( values[4] );
            }

            // Load the Dungeon board
            using ( System.IO.StreamReader file = new System.IO.StreamReader( @"..\..\saved_dungeonBoard.txt" ) ) {
                char[] buffer = new char[1];

                for ( int row = 0; row < DUNGEON_HEIGHT; row++ ) {
                    for ( int col = 0; col < DUNGEON_WIDTH; col++ ) {
                        file.Read( buffer, 0, 1 );
                        board[col, row] = short.Parse( buffer[0].ToString() );

                        if ( board[col, row] == (short)TileState.PLAYER ) {
                            player.Position.x = (short)col;
                            player.Position.y = (short)row;
                        }
                    }

                    file.ReadLine();
                }
            }

            // Load the Dungeon roomList
            using ( System.IO.StreamReader file = new System.IO.StreamReader( @"..\..\saved_dungeonRooms.txt" ) ) {
                string line;
                string [] values;

                while ( !file.EndOfStream ) {
                    line = file.ReadLine();
                    values = line.Split( ',' );

                    roomList.Add( new Room( short.Parse( values[0] ),
                                            short.Parse( values[1] ),
                                            short.Parse( values[2] ),
                                            short.Parse( values[3] ) ) );
                }
            }
        }

        /// <summary>
        ///     Checks to see if the Player has reached the end Room%'s center.
        /// </summary>
        /// <returns>
        ///     true = Dungeon has been completed<br/>
        ///     false = Dungeon has not been completed yet
        /// </returns>
        public bool CheckForLevelComplete() {
            if ( player.Position.x == roomList.Last().center.x &&
                 player.Position.y == roomList.Last().center.y ) {
                return true;
            }

            return false;
        }
    }
}
