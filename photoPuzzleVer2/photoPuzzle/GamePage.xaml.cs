using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace photoPuzzle
{
    public partial class GamePage : PhoneApplicationPage
    {
        private const double doubleTapSpeed = 500;
        private const int imageSize = 450;
        private Canvas[] puzzlePieces;
        private Stream imageStream;
        private PuzzleGame game;

        private long lastTapTicks;
        private int movingPieceId = -1;
        private int movingPieceDir;
        private double movingPieceStartPos;
        private int level=0;

        Popup popupCS = new Popup();
        

        public Stream ImageStream
        {
            get
            {
                return this.imageStream;
            }

            set
            {
                this.imageStream = value;
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(value);
                this.PreviewImage.Source = bitmap;
                int i = 0;
                int pieceSize = imageSize / this.game.ColsAndRows;
                for (int ix = 0; ix < this.game.ColsAndRows; ix++)
                {
                    for (int iy = 0; iy < this.game.ColsAndRows; iy++)
                    {
                        Image pieceImage = this.puzzlePieces[i].Children[0] as Image;
                        pieceImage.Source = bitmap;
                        i++;
                    }
                }
            }
        }

        

        public GamePage()
        {
           
            if (!popupCS.IsOpen)
            {
                displayCSpopup();
            }
            
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.Portrait;
            
        }

        // Puzzle Game

        public void startGame()
        {
            if (radioBtn3x3.IsChecked == true)
            {
                level = 3;
            }
            else if (radioBtn4x4.IsChecked == true)
            {
                level = 4;
            }
            else if (radioBtn5x5.IsChecked == true)
            {
                level = 5;
            }
            this.game = new PuzzleGame(level, tbCongrats);

            this.game.GameStarted += delegate
            {
                this.StatusPanel.Visibility = Visibility.Visible;
                //this.cvsLevel.Visibility = Visibility.Collapsed;
                this.tbTapToStart.Opacity = 0;
                this.tbTotalMoves.Text = this.game.TotalMoves.ToString();
                this.tbCongrats.Opacity = 0;
            };

            this.game.GameOver += delegate
            {
                //this.TapToContinueTextBlock.Opacity = 1;
                this.tbCongrats.Opacity = 1;
                this.borCongrats.Opacity = 1;
                //this.CongratsBorder.Visibility = Visibility.Visible;
                //this.Congo.Visibility = Visibility.Visible;
                this.StatusPanel.Visibility = Visibility.Visible;
                this.tbTotalMoves.Text = this.game.TotalMoves.ToString();
            };

            this.game.PieceUpdated += delegate(object sender, PieceUpdatedEventArgs args)
            {
                int pieceSize = imageSize / this.game.ColsAndRows;
                this.AnimatePiece(this.puzzlePieces[args.PieceId], Canvas.LeftProperty, (int)args.NewPosition.X * pieceSize);
                this.AnimatePiece(this.puzzlePieces[args.PieceId], Canvas.TopProperty, (int)args.NewPosition.Y * pieceSize);
                this.tbTotalMoves.Text = this.game.TotalMoves.ToString();
            };

            //Start game
            if (IsolatedStorageSettings.ApplicationSettings.Contains("MyPhoto"))
            {
                var imageResource = IsolatedStorageSettings.ApplicationSettings["MyPhoto"];
                if (imageResource == null)
                {
                    InitBoard();
                }
                else
                {
                    InitBoardNew();
                }
            }

            this.game.PieceUpdated += delegate(object sender, PieceUpdatedEventArgs args)
            {
                int pieceSize = imageSize / this.game.ColsAndRows;
                this.AnimatePiece(this.puzzlePieces[args.PieceId], Canvas.LeftProperty, (int)args.NewPosition.X * pieceSize);
                this.AnimatePiece(this.puzzlePieces[args.PieceId], Canvas.TopProperty, (int)args.NewPosition.Y * pieceSize);
                this.tbTotalMoves.Text = this.game.TotalMoves.ToString();
            };
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            popupCS.IsOpen = false;
            startGame();
            
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (popupCS.IsOpen)
            {
                popupCS.IsOpen = false;
            }
        }

        private void InitBoard()
        {
            int totalPieces = this.game.ColsAndRows * this.game.ColsAndRows;
            int pieceSize = imageSize / this.game.ColsAndRows;
            this.puzzlePieces = new Canvas[totalPieces];
            int nx = 0;
            for (int ix = 0; ix < this.game.ColsAndRows; ix++)
            {
                for (int iy = 0; iy < this.game.ColsAndRows; iy++)
                {
                    nx = (ix * this.game.ColsAndRows) + iy;
                    Image image = new Image();
                    image.SetValue(FrameworkElement.NameProperty, "PuzzleImage_" + nx);
                    image.Height = imageSize;
                    image.Width = imageSize;
                    image.Stretch = Stretch.UniformToFill;
                    RectangleGeometry r = new RectangleGeometry();
                    r.Rect = new Rect((ix * pieceSize), (iy * pieceSize), pieceSize, pieceSize);
                    image.Clip = r;
                    image.SetValue(Canvas.TopProperty, Convert.ToDouble(iy * pieceSize * -1));
                    image.SetValue(Canvas.LeftProperty, Convert.ToDouble(ix * pieceSize * -1));

                    this.puzzlePieces[nx] = new Canvas();
                    this.puzzlePieces[nx].SetValue(FrameworkElement.NameProperty, "PuzzlePiece_" + nx);
                    this.puzzlePieces[nx].Width = pieceSize;
                    this.puzzlePieces[nx].Height = pieceSize;
                    this.puzzlePieces[nx].Children.Add(image);
                    this.puzzlePieces[nx].MouseLeftButtonDown += this.PuzzlePiece_MouseLeftButtonDown;
                    if (nx < totalPieces - 1)
                    {
                        this.GameContainer.Children.Add(this.puzzlePieces[nx]);
                    }
                }
            }

            // Retrieve image
            List<Uri> Pictures = new List<Uri>()
            {
                new Uri ("Assets/animals/cats.jpg", UriKind.Relative),
                new Uri ("Assets/animals/delphins.jpg", UriKind.Relative),
                new Uri ("Assets/animals/dog.jpg", UriKind.Relative),
                new Uri ("Assets/animals/dog2.jpg", UriKind.Relative),
                new Uri ("Assets/animals/horse.jpg", UriKind.Relative),
            };
            Random rand = new Random();
            int index = rand.Next(0, Pictures.Count);
            StreamResourceInfo imageResource = Application.GetResourceStream(Pictures[index]);
            //StreamResourceInfo imageResource = Application.GetResourceStream(new Uri("Puzzle1.png", UriKind.Relative)); 
            this.ImageStream = imageResource.Stream;

            this.game.Reset();
        }

        private void InitBoardNew()
        {
            int totalPieces = this.game.ColsAndRows * this.game.ColsAndRows;
            int pieceSize = imageSize / this.game.ColsAndRows;
            this.puzzlePieces = new Canvas[totalPieces];
            int nx = 0;
            for (int ix = 0; ix < this.game.ColsAndRows; ix++)
            {
                for (int iy = 0; iy < this.game.ColsAndRows; iy++)
                {
                    nx = (ix * this.game.ColsAndRows) + iy;
                    Image image = new Image();
                    image.SetValue(FrameworkElement.NameProperty, "PuzzleImage_" + nx);
                    image.Height = imageSize;
                    image.Width = imageSize;
                    image.Stretch = Stretch.UniformToFill;
                    RectangleGeometry r = new RectangleGeometry();
                    r.Rect = new Rect((ix * pieceSize), (iy * pieceSize), pieceSize, pieceSize);
                    image.Clip = r;
                    image.SetValue(Canvas.TopProperty, Convert.ToDouble(iy * pieceSize * -1));
                    image.SetValue(Canvas.LeftProperty, Convert.ToDouble(ix * pieceSize * -1));

                    this.puzzlePieces[nx] = new Canvas();
                    this.puzzlePieces[nx].SetValue(FrameworkElement.NameProperty, "PuzzlePiece_" + nx);
                    this.puzzlePieces[nx].Width = pieceSize;
                    this.puzzlePieces[nx].Height = pieceSize;
                    this.puzzlePieces[nx].Children.Add(image);
                    this.puzzlePieces[nx].MouseLeftButtonDown += this.PuzzlePiece_MouseLeftButtonDown;
                    if (nx < totalPieces - 1)
                    {
                        this.GameContainer.Children.Add(this.puzzlePieces[nx]);
                    }
                }
            }

            // Retrieve image

            Stream imageStream = (Stream)IsolatedStorageSettings.ApplicationSettings["MyPhoto"];
            this.ImageStream = imageStream;
            //
            this.game.Reset();
        }

        private void AnimatePiece(DependencyObject piece, DependencyProperty dp, double newValue)
        {
            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTarget(storyBoard, piece);
            Storyboard.SetTargetProperty(storyBoard, new PropertyPath(dp));
            storyBoard.Children.Add(new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                From = Convert.ToInt32(piece.GetValue(dp)),
                To = Convert.ToDouble(newValue),
                EasingFunction = new SineEase()
            });
            storyBoard.Begin();
        }

        private void PuzzlePiece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.game.IsPlaying)
            {
                this.game.NewGame();
            }
        }

        private void Restart_Tapped(object sender, GestureEventArgs e)
        {
            //   this.game.Reset();
            this.game.NewGame();
            this.borCongrats.Opacity = 0;
            //  this.game.CheckWinner(Congratulations);
        }

        private void PhoneApplicationPage_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (this.game.IsPlaying && e.ManipulationContainer is Image && e.ManipulationContainer.GetValue(FrameworkElement.NameProperty).ToString().StartsWith("PuzzleImage_"))
            {
                int pieceIx = Convert.ToInt32(e.ManipulationContainer.GetValue(FrameworkElement.NameProperty).ToString().Substring(12));
                Canvas piece = this.FindName("PuzzlePiece_" + pieceIx) as Canvas;
                if (piece != null)
                {
                    int totalPieces = this.game.ColsAndRows * this.game.ColsAndRows;
                    for (int i = 0; i < totalPieces; i++)
                    {
                        if (piece == this.puzzlePieces[i] && this.game.CanMovePiece(i) > 0)
                        {
                            int direction = this.game.CanMovePiece(i);
                            DependencyProperty axisProperty = (direction % 2 == 0) ? Canvas.LeftProperty : Canvas.TopProperty;
                            this.movingPieceDir = direction;
                            this.movingPieceStartPos = Convert.ToDouble(piece.GetValue(axisProperty));
                            this.movingPieceId = i;
                            break;
                        }
                    }
                }
            }
        }

        private void PhoneApplicationPage_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (this.movingPieceId > -1)
            {
                int pieceSize = imageSize / this.game.ColsAndRows;
                Canvas movingPiece = this.puzzlePieces[this.movingPieceId];

                // validate direction
                DependencyProperty axisProperty;
                double normalizedValue;

                if (this.movingPieceDir % 2 == 0)
                {
                    axisProperty = Canvas.LeftProperty;
                    normalizedValue = e.CumulativeManipulation.Translation.X;
                }
                else
                {
                    axisProperty = Canvas.TopProperty;
                    normalizedValue = e.CumulativeManipulation.Translation.Y;
                }

                // enforce drag constraints
                // (top or left)
                if (this.movingPieceDir == 1 || this.movingPieceDir == 4)
                {
                    if (normalizedValue < -pieceSize)
                    {
                        normalizedValue = -pieceSize;
                    }
                    else if (normalizedValue > 0)
                    {
                        normalizedValue = 0;
                    }
                }
                // (bottom or right)
                else if (this.movingPieceDir == 3 || this.movingPieceDir == 2)
                {
                    if (normalizedValue > pieceSize)
                    {
                        normalizedValue = pieceSize;
                    }
                    else if (normalizedValue < 0)
                    {
                        normalizedValue = 0;
                    }
                }

                // set position
                movingPiece.SetValue(axisProperty, normalizedValue + this.movingPieceStartPos);
            }
        }

        private void PhoneApplicationPage_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (this.movingPieceId > -1)
            {
                int pieceSize = imageSize / this.game.ColsAndRows;
                Canvas piece = this.puzzlePieces[this.movingPieceId];

                // check for double tapping
                if (TimeSpan.FromTicks(DateTime.Now.Ticks - this.lastTapTicks).TotalMilliseconds < doubleTapSpeed)
                {
                    // force move
                    this.game.MovePiece(this.movingPieceId);
                    this.lastTapTicks = int.MinValue;
                }
                else
                {
                    // calculate moved distance
                    DependencyProperty axisProperty = (this.movingPieceDir % 2 == 0) ? Canvas.LeftProperty : Canvas.TopProperty;
                    double minRequiredDisplacement = pieceSize / 3;
                    double diff = Math.Abs(Convert.ToDouble(piece.GetValue(axisProperty)) - this.movingPieceStartPos);

                    // did it get halfway across?
                    if (diff > minRequiredDisplacement)
                    {
                        // move piece
                        this.game.MovePiece(this.movingPieceId);
                    }
                    else
                    {
                        // restore piece
                        this.AnimatePiece(piece, axisProperty, this.movingPieceStartPos);
                    }
                }

                this.movingPieceId = -1;
                this.movingPieceStartPos = 0;
                this.movingPieceDir = 0;
                this.lastTapTicks = DateTime.Now.Ticks;
            }
        }

        RadioButton radioBtn5x5 = new RadioButton(); 
        RadioButton radioBtn3x3 = new RadioButton();
        RadioButton radioBtn4x4 = new RadioButton();

        public void displayCSpopup()
        {
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Green);
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(10, 10, 10, 10);
            border.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            StackPanel spOutter = new StackPanel();
            spOutter.Background = new SolidColorBrush(Colors.Orange);
            spOutter.Orientation = System.Windows.Controls.Orientation.Vertical;


            TextBlock tb1 = new TextBlock();
            tb1.Text = "Select your level";
            tb1.TextAlignment = TextAlignment.Center;
            tb1.FontSize = 25;
            tb1.Margin = new Thickness(10, 20, 10, 20);
            tb1.Foreground = new SolidColorBrush(Colors.White);

            spOutter.Children.Add(tb1);
            
            
            StackPanel spInner = new StackPanel();
            spInner.Orientation = System.Windows.Controls.Orientation.Horizontal;
            spInner.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          
            radioBtn3x3.Content = "3x3";
            radioBtn3x3.Margin = new Thickness(10, 0, 0, 0);
            radioBtn3x3.Width = 140;
            radioBtn3x3.Height = 100;
        
            radioBtn4x4.Content = "4x4";
            radioBtn3x3.Margin = new Thickness(10, 0, 0, 0);
            radioBtn4x4.Width = 140;
            radioBtn3x3.Height = 100;
   
            radioBtn5x5.Content = "5x5";
            radioBtn3x3.Margin = new Thickness(10, 0, 0, 0);
            radioBtn5x5.Width = 140;
            radioBtn3x3.Height = 100;

            spInner.Children.Add(radioBtn3x3);
            spInner.Children.Add(radioBtn4x4);
            spInner.Children.Add(radioBtn5x5);
 
            spOutter.Children.Add(spInner);

            StackPanel spInner2 = new StackPanel();
            spInner2.Orientation = System.Windows.Controls.Orientation.Horizontal;

            Button btnContinue = new Button();
            btnContinue.Content = "Continue";
            btnContinue.Width = 225;
            btnContinue.Height = 100;
            btnContinue.Click += new RoutedEventHandler(btnContinue_Click);

            Button btnCancel = new Button();
            btnCancel.Content = "Cancel";
            btnCancel.Width = 225;
            btnCancel.Height = 100;
            btnCancel.Click += new RoutedEventHandler(btnCancel_Click);

            spInner2.Children.Add(btnContinue);
            spInner2.Children.Add(btnCancel);
            
            spOutter.Children.Add(spInner2);

            border.Child = spOutter;
            popupCS.Child = border;

            popupCS.VerticalOffset = 300;
            popupCS.HorizontalOffset = 3;
           
            popupCS.IsOpen = true;
        }
    }
}