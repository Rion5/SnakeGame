using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace snakeGame
{
    public partial class Form1 : Form
    {
        //The Snake is just a chain or list of circles you eat.
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        //Each time the program starts, we need to reset the settings and start the game
        public Form1()
        {
            InitializeComponent();
            //Set settings to default
            new Settings();

            //Set timer and game speed
            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            //start new game
            StartGame();
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;

            //Set settings to default
            new Settings();

            //Create snake object
            Snake.Clear();
            Circle head = new Circle(); //or can write as = new Circles{x=10, y=5};
            head.x = 10;
            head.y = 5;
            Snake.Add(head);
            
            //Sets label to display score
            labelScore.Text = Settings.Score.ToString();

            //Need to create function to create food
            CreateFood();
        }

        //CreateFood Function.
        private void CreateFood()
        {
            //Sets X and Y axis boundaries
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Width / Settings.Height;

            //Creating new random object
            //Place food randomly within the boundaries
            Random random = new Random();
            food = new Circle();
            food.x = random.Next(0, maxXPos);
            food.y = random.Next(0, maxYPos);
        }

        //Update Screen Function
        private void UpdateScreen(object sender, EventArgs e)
        {
            //Checks if game is over
            if(Settings.GameOver == true)
            {
                //Hit Enter to start a new game
                if (Input.KeyPressed(Keys.Enter) || Input.KeyPressed(Keys.Space))
                {
                    StartGame();
                }
            }
            //if Game is not over, perform these checks
            else
            {
                /*Need to make sure the user is making legal moves.
                I.E If the snake is moving to the left, we can not move to the right
                otherwise there would be a collision. */
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;

                //If player isn't making an illegal move, then we need to create the function to move the snake
                MovePlayer();
            }
            //Make sure that all data on the canvas gets refreshed
            pbCanvas.Invalidate();
            
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if(!Settings.GameOver)
            {
                //Give color to snake
                Brush SnakeColor;

                //Draw Snake
                for (int i = 0; i < Snake.Count; i++)
                {
                   
                    //Draw head
                    if (i == 0)
                        SnakeColor = Brushes.Black;
                    else
                        SnakeColor = Brushes.Green; // rest of body

                    //Draw snake
                    canvas.FillEllipse(SnakeColor,
                        new Rectangle(Snake[i].x * Settings.Width,
                                      Snake[i].y * Settings.Height,
                                      Settings.Width, Settings.Height));

                    //Draw Food
                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.x * Settings.Width,
                        food.y * Settings.Height, Settings.Width, Settings.Height));

                    
                }
            }
            else
            {
                string gameOver = "Game over \nYour final score is: " + Settings.Score + "\nPress Enter to try again";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
                
                
            }
        }

        private void MovePlayer()
        {

            for(int i = Snake.Count -1; i>=0; i--)
            {
                //Move head
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].x++;
                            break;
                        case Direction.Left:
                            Snake[i].x--;
                            break;
                        case Direction.Up:
                            Snake[i].y--;
                            break;
                        case Direction.Down:
                            Snake[i].y++;
                            break;
                    }
                    //Get max x and y pos
                    int maxXpos = pbCanvas.Size.Width / Settings.Width;
                    int maxYpos = pbCanvas.Size.Height / Settings.Height;

                    //Detect Collisions with game borders
                    if (Snake[i].x < 0 || Snake[i].y < 0 || Snake[i].x >= maxXpos || Snake[i].y >= maxYpos)
                    {
                       Die();
                    }

                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if(Snake[i].x == Snake[j].x && Snake[i].y == Snake[j].y)
                        {
                            Die();
                        }
                    }

                    //Detect Collision with Food (Gain point + size)
                    if (Snake[i].x == food.x && Snake[i].y == food.y)
                    {
                        Eat();
                    }
                }
                else
                {
                    //move body 
                    Snake[i].x = Snake[i - 1].x;
                    Snake[i].y = Snake[i - 1].y;
                }
                
            }
        }
        //GAME OVER (Dead)
        private void Die()
        {
            Settings.GameOver = true;
        }

        //Eat Food -> Add Food to Snake Body -> Update Game Score
        private void Eat()
        {
            //Add Circle to body after eating
            Circle food = new Circle();
            food.x = Snake[Snake.Count - 1].x;
            food.y = Snake[Snake.Count - 1].y;

            Snake.Add(food);

            //Update Score
            Settings.Score += Settings.Points;
            labelScore.Text = Settings.Score.ToString();

            CreateFood(); //Create new food in random position
            SpeedUpGame(); //Speed up game every 300 points
        }

        //Button is being pressed down
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        //Button is being released
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void SpeedUpGame()
        {
            //Every 300 points, Increase speed by +5
            if (Settings.Score % 3 == 0) //Need to figure out why its not working
            {
                Settings.Speed += 20;
            }

        }
    }
}

/*
 * Room for Improvement: Different Speed Settings on the side
 * Difficultiy Settings
 * Better Graphics and Displays
 * Ability to Save High Score
 * Hit Space for new game (instead of just enter)
 */