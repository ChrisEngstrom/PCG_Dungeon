using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCG_Dungeon {
    class Dungeon {
        Random rand;

        public enum TileState {
            OPEN = 0,
            WALL = 1,
            PLAYER = 2,
            START = 3,
            END = 4,
        }

        Point player;

        // Class globals/consts that shouldn't be changed
        short DUNGEON_WIDTH,
              DUNGEON_HEIGHT,
              MIN_ROOM_SIZE,
              MAX_ROOM_SIZE,
              NUM_ROOMS;

        short[,] board;
        List<Room> roomList;

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

        private void initializeBoard() {
            for ( int col = 0; col < DUNGEON_WIDTH; col++ ) {
                for ( int row = 0; row < DUNGEON_HEIGHT; row++ ) {
                    board[col, row] = (short)TileState.WALL;
                }
            }
        }

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
                        player = new Point( newRoom.center.x, newRoom.center.y );
                    }
                }
            }

            updateBoard();
        }

        private void hCorridor( int x1, int x2, int y ) {
            for ( int col = Math.Min( x1, x2 ); col < Math.Max( x1, x2 ) + 1; col++ ) {
                // Remove the wall present
                board[col, y] = (short)TileState.OPEN;
            }
        }

        private void vCorridor( int y1, int y2, int x ) {
            for ( int row = Math.Min( y1, y2 ); row < Math.Max( y1, y2 ) + 1; row++ ) {
                // Remove the wall present
                board[x, row] = (short)TileState.OPEN;
            }
        }

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

            board[player.x, player.y] = (short)TileState.PLAYER;
        }

        // Get the TileState of the position indicated
        public int GetPosition( int col, int row ) {
            return board[col, row];
        }

        // Attempt to move the player's piece in a certain direction
        private bool movePlayer( int x, int y ) {
            short potentialMoveTile = board[player.x + x, player.y - y];

            if ( potentialMoveTile != (short)TileState.WALL ) {
                // Remove old player piece
                board[player.x, player.y] = (short)TileState.OPEN;

                // Update player piece
                player.x += (short)x;
                player.y -= (short)y;

                // Place updated player piece
                board[player.x, player.y] = (short)TileState.PLAYER;

                return true;
            } else {
                return false;
            }
        }

        public void MovePlayerUp() {
            if ( movePlayer( 0, 1 ) ) {
                updateBoard();
            }
        }

        public void MovePlayerRight() {
            if ( movePlayer( 1, 0 ) ) {
                updateBoard();
            }
        }

        public void MovePlayerDown() {
            if ( movePlayer( 0, -1 ) ) {
                updateBoard();
            }
        }

        public void MovePlayerLeft() {
            if ( movePlayer( -1, 0 ) ) {
                updateBoard();
            }
        }

        public void SaveToFile() {
            using ( System.IO.StreamWriter file = new System.IO.StreamWriter( @"..\..\dungeon.txt" ) ) {
                for ( int row = 0; row < DUNGEON_HEIGHT; row++ ) {
                    for ( int col = 0; col < DUNGEON_WIDTH; col++ ) {
                        file.Write( board[col, row] );
                    }

                    file.WriteLine();
                }
            }
        }

        public void LoadFromFile() {
            using ( System.IO.StreamReader file = new System.IO.StreamReader( @"..\..\dungeon.txt" ) ) {
                char[] buffer = new char[1];

                for ( int row = 0; row < DUNGEON_HEIGHT; row++ ) {
                    for ( int col = 0; col < DUNGEON_WIDTH; col++ ) {
                        file.Read( buffer, 0, 1 );
                        board[col, row] = short.Parse( buffer[0].ToString() );

                        if ( board[col, row] == (short)TileState.PLAYER ) {
                            player.x = (short)col;
                            player.y = (short)row;
                        }
                    }

                    file.ReadLine();
                }
            }
        }

        public bool CheckForLevelComplete() {
            if ( player.x == roomList.Last().center.x &&
                 player.y == roomList.Last().center.y ) {
                return true;
            }

            return false;
        }
    }
}
