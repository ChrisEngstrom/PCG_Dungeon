using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PCG_Dungeon {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window {
        const short WIDTH = 96,
                    HEIGHT = 54;

        Dungeon dungeon;

        public MainWindow() {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler( MainWindow_Loaded );
            this.KeyDown += new KeyEventHandler( MainWindow_KeyDown );

            dungeon = new Dungeon( WIDTH, HEIGHT,
                                   short.Parse( tbMinRoomSize.Text ),
                                   short.Parse( tbMaxRoomSize.Text ),
                                   short.Parse( tbNumRooms.Text ) );
        }

        void MainWindow_Loaded( object sender, RoutedEventArgs e ) {
            btnNewDungeon_Click( this, new RoutedEventArgs() );
        }

        void MainWindow_KeyDown( object sender, KeyEventArgs e ) {
            switch ( e.Key ) {
                case Key.W:
                    canvDungeon.Focus();
                    dungeon.MovePlayerUp();
                    break;
                case Key.D:
                    canvDungeon.Focus();
                    dungeon.MovePlayerRight();
                    break;
                case Key.S:
                    canvDungeon.Focus();
                    dungeon.MovePlayerDown();
                    break;
                case Key.A:
                    canvDungeon.Focus();
                    dungeon.MovePlayerLeft();
                    break;
            }

            if ( dungeon.CheckForLevelComplete() ) {
                dungeon.PlaceRooms( true );
            }

            canvDungeon.Children.Clear();
            canvDungeon.InvalidateVisual();
            DrawDungeon();

            e.Handled = false;
        }

        void DrawDungeon() {
            for ( int col = 0; col < WIDTH; col++ ) {
                for ( int row = 0; row < HEIGHT; row++ ) {
                    if ( dungeon.GetPosition( col, row ) != (int)Dungeon.TileState.WALL ) {
                        Rectangle rect = new Rectangle();

                        if ( dungeon.GetPosition( col, row ) == (int)Dungeon.TileState.OPEN ) {
                            rect.Fill = new SolidColorBrush( Colors.LightGray );
                        } else if ( dungeon.GetPosition( col, row ) == (int)Dungeon.TileState.START ) {
                            rect.Fill = new SolidColorBrush( Colors.LightGreen );
                        } else if ( dungeon.GetPosition( col, row ) == (int)Dungeon.TileState.END ) {
                            rect.Fill = new SolidColorBrush( Colors.LightCoral );
                        } else if ( dungeon.GetPosition( col, row ) == (int)Dungeon.TileState.PLAYER ) {
                            rect.Fill = new SolidColorBrush( Colors.LightSeaGreen );
                        }
                        //else {
                        //    rect.Fill = new SolidColorBrush( Colors.DarkGray );
                        //}

                        rect.Width = (canvDungeon.ActualWidth / WIDTH);
                        rect.Height = (canvDungeon.ActualHeight / HEIGHT);

                        Canvas.SetTop( rect, row * (canvDungeon.ActualWidth / WIDTH) );
                        Canvas.SetLeft( rect, col * (canvDungeon.ActualHeight / HEIGHT) );

                        canvDungeon.Children.Add( rect );
                    }
                }
            }
        }

        private void btnNewDungeon_Click( object sender, RoutedEventArgs e ) {
            canvDungeon.Children.Clear();
            dungeon = new Dungeon( WIDTH, HEIGHT,
                                   short.Parse( tbMinRoomSize.Text ),
                                   short.Parse( tbMaxRoomSize.Text ),
                                   short.Parse( tbNumRooms.Text ) );
            dungeon.PlaceRooms( false );
            DrawDungeon();

        }

        private void btnSaveDungeonToFile_Click( object sender, RoutedEventArgs e ) {
            dungeon.SaveToFile();
        }

        private void btnLoadDungeonFromFile_Click( object sender, RoutedEventArgs e ) {
            canvDungeon.Children.Clear();

            dungeon.LoadFromFile();

            DrawDungeon();
        }
    }
}
