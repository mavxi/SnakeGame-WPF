using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private const int SquareSize = 20;
        private const int GameSpeed = 100;

        private List<SnakePart> snake;
        private Direction direction;
        private DispatcherTimer gameTimer;
        private Rectangle food;
        private Random random = new Random();
        private bool isPaused = false;
        private int score = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Create initial snake
            snake = new List<SnakePart>();
            snake.Add(new SnakePart { X = 10, Y = 10 });

            direction = Direction.Right;

            CreateFood();

            DrawSnake();

            // Set up game timer
            gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = TimeSpan.FromMilliseconds(GameSpeed);
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (isPaused) return;

            MoveSnake();
            CheckCollisions();
        }

        private void MoveSnake()
        {
            SnakePart head = snake[0];
            SnakePart newHead = new SnakePart();

            switch (direction)
            {
                case Direction.Up:
                    newHead.X = head.X;
                    newHead.Y = head.Y - 1;
                    break;
                case Direction.Down:
                    newHead.X = head.X;
                    newHead.Y = head.Y + 1;
                    break;
                case Direction.Left:
                    newHead.X = head.X - 1;
                    newHead.Y = head.Y;
                    break;
                case Direction.Right:
                    newHead.X = head.X + 1;
                    newHead.Y = head.Y;
                    break;
            }

            snake.Insert(0, newHead);

            // Check if food was eaten
            if (newHead.X * SquareSize == Canvas.GetLeft(food) &&
                newHead.Y * SquareSize == Canvas.GetTop(food))
            {
                // Food was eaten
                score++;
                ScoreText.Text = score.ToString();
                CreateFood();
            }
            else
            {
                // Remove tail if no food was eaten
                SnakePart tail = snake[snake.Count - 1];
                snake.RemoveAt(snake.Count - 1);
                GameCanvas.Children.Remove(tail.UIElement);
            }

            // Draw new head
            DrawSnakeHead(newHead);
        }

        private void DrawSnake()
        {
            foreach (var part in snake)
            {
                DrawSnakeHead(part);
            }
        }

        private void DrawSnakeHead(SnakePart part)
        {
            Rectangle rect = new Rectangle
            {
                Width = SquareSize,
                Height = SquareSize,
                Fill = Brushes.Green
            };

            Canvas.SetLeft(rect, part.X * SquareSize);
            Canvas.SetTop(rect, part.Y * SquareSize);

            part.UIElement = rect;
            GameCanvas.Children.Add(rect);
        }

        private void CreateFood()
        {
            if (food != null)
            {
                GameCanvas.Children.Remove(food);
            }

            food = new Rectangle
            {
                Width = SquareSize,
                Height = SquareSize,
                Fill = Brushes.Red
            };

            // Position food randomly
            int maxX = (int)(GameCanvas.ActualWidth / SquareSize);
            int maxY = (int)(GameCanvas.ActualHeight / SquareSize);

            Canvas.SetLeft(food, random.Next(0, maxX) * SquareSize);
            Canvas.SetTop(food, random.Next(0, maxY) * SquareSize);

            GameCanvas.Children.Add(food);
        }

        private void CheckCollisions()
        {
            SnakePart head = snake[0];

            // Check wall collision
            if (head.X < 0 || head.X >= GameCanvas.ActualWidth / SquareSize ||
                head.Y < 0 || head.Y >= GameCanvas.ActualHeight / SquareSize)
            {
                GameOver();
                return;
            }

            // Check self collision
            for (int i = 1; i < snake.Count; i++)
            {
                if (head.X == snake[i].X && head.Y == snake[i].Y)
                {
                    GameOver();
                    return;
                }
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show($"Game Over! Your score: {score}", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (direction != Direction.Down)
                        direction = Direction.Up;
                    break;
                case Key.Down:
                    if (direction != Direction.Up)
                        direction = Direction.Down;
                    break;
                case Key.Left:
                    if (direction != Direction.Right)
                        direction = Direction.Left;
                    break;
                case Key.Right:
                    if (direction != Direction.Left)
                        direction = Direction.Right;
                    break;
                case Key.Space:
                    isPaused = !isPaused;
                    PauseText.Visibility = isPaused ? Visibility.Visible : Visibility.Collapsed;
                    break;
            }
        }
    }
}