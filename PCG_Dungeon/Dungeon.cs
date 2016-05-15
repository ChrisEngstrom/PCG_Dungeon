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
            ROOM = 0,
            WALL = 1,
            HALL = 2,
            PLAYER = 3,
            START = 4,
            END = 5,
        }

        Random rand;
        Player player;
        short[,] board;
        List<Area> areaList;

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
                return NUM_ROOMS;
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
            areaList = new List<Area>();

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

            if ( areaList.Count > 0 &&
                 endRoomIsNewStartRoom ) {
                startRoom = (Room)areaList.Last();

                areaList.Clear();

                areaList.Add( startRoom );
            } else {
                areaList.Clear();
            }

            initializeBoard();

            while ( (areaList.Count - (NUM_ROOMS - 1) * 2) < NUM_ROOMS ) {
                short w = (short)rand.Next( MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1 ),
                      h = (short)rand.Next( MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1 ),
                      x = (short)(rand.Next( (DUNGEON_WIDTH - w + 1 - 2) ) + 1),    // The (- 2) and (+ 1) keeps the rooms
                      y = (short)(rand.Next( (DUNGEON_HEIGHT - h + 1 - 2) ) + 1);   //  off of the edges by one tile
                bool intersection = false;

                Room newRoom = new Room( x, y, w, h );

                foreach ( Room existingRoom in areaList.OfType<Room>() ) {
                    if ( newRoom.Intersects( existingRoom ) ) {
                        intersection = true;
                        break;
                    }
                }

                if ( !intersection ) {
                    areaList.Add( newRoom );

                    // Save the center point of this new room
                    Point newCenter = new Point( newRoom.center.x, newRoom.center.y );

                    if ( areaList.Count > 1 ) {
                        HorizontalHallway hHall = null;
                        VerticalHallway vHall = null;

                        Point prevCenter = new Point( areaList[areaList.Count - 2].center.x,
                                                      areaList[areaList.Count - 2].center.y );

                        // Randomly choose which hall type to do first
                        if ( rand.Next( 2 ) == 1 ) {
                            hHall = new HorizontalHallway( Math.Min( prevCenter.x, newCenter.x ), prevCenter.y, (short)(Math.Abs( prevCenter.x - newCenter.x ) + 1) );
                            vHall = new VerticalHallway( newCenter.x, Math.Min( prevCenter.y, newCenter.y ), (short)(Math.Abs( prevCenter.y - newCenter.y ) + 1) );
                        } else {
                            vHall = new VerticalHallway( prevCenter.x, Math.Min( prevCenter.y, newCenter.y ), (short)(Math.Abs( prevCenter.y - newCenter.y ) + 1) );
                            hHall = new HorizontalHallway( Math.Min( prevCenter.x, newCenter.x ), newCenter.y, (short)(Math.Abs( prevCenter.x - newCenter.x ) + 1) );
                        }

                        areaList.Insert( areaList.Count - 1, hHall );
                        areaList.Insert( areaList.Count - 1, vHall );
                    } else if ( areaList.Count == 1 ) {
                        player = new Player( new Point( newRoom.center.x, newRoom.center.y ) );
                    }
                }
            }

            updateBoard();
        }

        /// <summary>
        ///     Goes through the Area%s that have been added to the Dungeon and
        ///         sets the appropriate tiles to the correct state. Also sets
        ///         the tile state for the Player%'s location
        /// </summary>
        private void updateBoard() {
            // Draw the hallways
            foreach ( Hallway hallway in areaList.OfType<Hallway>() ) {
                for ( int col = hallway.x1; col < hallway.x2; col++ ) {
                    for ( int row = hallway.y1; row < hallway.y2; row++ ) {
                        // Use the hall TileState
                        board[col, row] = (short)TileState.HALL;
                    }
                }
            }

            // Draw the rooms
            foreach ( Room room in areaList.OfType<Room>() ) {
                for ( int col = room.x1; col < room.x2; col++ ) {
                    for ( int row = room.y1; row < room.y2; row++ ) {
                        if ( room == areaList.First() ) {
                            board[col, row] = (short)TileState.START;
                        } else if ( room == areaList.Last() ) {
                            board[col, row] = (short)TileState.END;
                        } else {
                            board[col, row] = (short)TileState.ROOM;
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
                //board[player.Position.x, player.Position.y] = (short)TileState.ROOM;

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
                foreach ( Area area in areaList ) {
                    file.Write( area.x1 + "," + area.y1 + "," + area.w + "," + area.h );
                    file.WriteLine();
                }
            }
        }

        /// <summary>
        ///     Loads a Dungeon state from a set of text files.
        /// </summary>
        public void LoadFromFile() {
            // \todo Add a check to make sure a set of save files exist

            // Wipe the current state of the board
            initializeBoard();
            areaList.Clear();

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
                int lineNum = 0;

                while ( !file.EndOfStream ) {
                    line = file.ReadLine();
                    values = line.Split( ',' );

                    if ( lineNum % 3 == 0 ) {
                        areaList.Add( new Room( short.Parse( values[0] ),
                                                short.Parse( values[1] ),
                                                short.Parse( values[2] ),
                                                short.Parse( values[3] ) ) );
                    } else {
                        areaList.Add( new Hallway( short.Parse( values[0] ),
                                                   short.Parse( values[1] ),
                                                   short.Parse( values[2] ),
                                                   short.Parse( values[3] ) ) );
                    }

                    lineNum++;
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
            if ( player.Position.x == areaList.Last().center.x &&
                 player.Position.y == areaList.Last().center.y ) {
                return true;
            }

            return false;
        }
    }
}
