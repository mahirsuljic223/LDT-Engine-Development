using System.Drawing;
using System.Drawing.Text;
using System.Text.Json.Serialization.Metadata;

namespace LDT___Engine_Development
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        #region Constants
        const short GRAVITY = 9;
        const short CLOCK_INTERVAL = 10;
        const short PLAYER_JUMP_SPEED = 35;
        const short DEFAULT_PLAYER_HEIGHT = 120;
        const short DEFAULT_PLAYER_WIDTH = 50;
        const short DEFAULT_PLAYER_SPEED = 10;
        //const short MAX_PLAYER_SPEED = 50;
        static Color DEFAULT_PLAYER_COLOR = Color.Blue;
        static Color DEFAULT_BACKGROUND_COLOR = Color.White;
        #endregion

        #region Global Variables
        private bool onGround = false;
        private static Image? display = null;
        private static Player? player = null;
        private List<SolidObject> solidObjects = new List<SolidObject>();
        private Actions actions;
        #endregion

        #region Classes, enums
        class Player
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Height { get; }
            public int Width { get; }
            public int SpeedX { get; set; }
            public int SpeedY { get; set; }
            public Rectangle Rect { get; set; }
            public Color color;   // privremena
            //Image image;

            public Player(int x, int y, int h, int w, Color color)
            {
                this.Height = h;
                this.Width = w;
                this.X = x;
                this.Y = display.Height - y - h;
                this.color = color;
                this.Rect = new Rectangle(x, y, w, h);
            }

            public Player(int x = 0, int y = 0, int h = DEFAULT_PLAYER_HEIGHT, int w = DEFAULT_PLAYER_WIDTH)
            {
                this.X = x;
                this.Y = display.Height - y - h;
                this.Height = h;
                this.Width = w;
                this.color = DEFAULT_PLAYER_COLOR;
                this.Rect = new Rectangle(x, y, w, h);
            }

            public void Move()
            {
                this.X += this.SpeedX;
                this.Y += this.SpeedY;
                this.Rect = new Rectangle(this.X, this.Y, this.Width, this.Height);
            }

            public void Move(Point p)
            {
                this.X = p.X;
                this.Y = p.Y;
                this.Rect = new Rectangle(this.X, this.Y, this.Width, this.Height);
            }

            public void Move(int x, int y)
            {
                this.X = x;
                this.Y = y;
                this.Rect = new Rectangle(this.X, this.Y, this.Width, this.Height);
            }
        }

        class SolidObject { }

        class Box : SolidObject
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
            public Rectangle Rect { get; set; }
            public Color color;   // privremena
            //Image image;

            public Box(int x, int y, int h, int w, Color color)
            {
                this.X = x;
                this.Y = display.Height - y - h;
                this.Height = h;
                this.Width = w;
                this.color = color;
                this.Rect = new Rectangle(x, y, w, h);
            }

            public void Move(Point p)
            {
                this.X = p.X;
                this.Y = p.Y;
                this.Rect = new Rectangle(this.X, this.Y, this.Width, this.Height);
            }

            public void Move(int x, int y)
            {
                this.X = x;
                this.Y = y;
                this.Rect = new Rectangle(this.X, this.Y, this.Width, this.Height);
            }
        }

        class Polygon : SolidObject
        {
            public Point[] points;
            public Color color;
            public Rectangle Rect { get; set; }

            public Polygon(Point[] points, Color color)
            {
                this.points = points;
                this.color = color;
                this.Rect = MakeRect();
            }

            // pravi pravougaonik u kojem se nalazi cijeli objekat
            private Rectangle MakeRect()
            {
                int minX = points[0].X;
                int minY = points[0].Y;
                int maxX = points[0].X;
                int maxY = points[0].Y;

                for (int i = 1; i < points.Length; i++)
                {
                    if (points[i].X > maxX) maxX = points[i].X;
                    else if (points[i].X < minX) minX = points[i].X;

                    if (points[i].Y > maxY) maxY = points[i].Y;
                    else if (points[i].Y < minY) minY = points[i].Y;
                }

                return new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }
        }


        [Flags]
        public enum Actions
        {
            None = 0,
            Up = 0b00000001,
            Down = 0b00000010,
            Right = 0b00000100,
            Left = 0b00001000
        }
        #endregion

        #region Functions
        private void Draw()
        {
            if (display == null) return;

            using (Graphics g = Graphics.FromImage(display))
            {
                g.Clear(DEFAULT_BACKGROUND_COLOR);

                g.FillRectangle(new SolidBrush(player.color), player.X, player.Y, player.Width, player.Height); // player

                foreach (SolidObject obj in solidObjects)
                {
                    if (obj is Box box)
                        g.FillRectangle(new SolidBrush(box.color), box.X, box.Y, box.Width, box.Height);
                    else if (obj is Polygon polygon)
                        g.FillPolygon(new SolidBrush(polygon.color), polygon.points);
                    //g.DrawLine(new Pen(polygon.color, 2), polygon.points[0], polygon.points[1]);
                }

                pb_display.Image = display;
                this.Invalidate();
            }
        }

        private void Init()
        {
            display = new Bitmap(pb_display.Width, pb_display.Height);
            player = new Player();
            timer_main.Interval = CLOCK_INTERVAL;
            timer_main.Start();
        }

        public void GetLineKN(Point p1, Point p2, ref float k, ref float n)
        {
            k = (float)(p2.Y - p1.Y) / (p2.X - p1.X);
            n = p1.Y - k * p1.X;
        }

        public int GetLineY(int x, float k, float n)
        {
            return (int)(k * x + n);
        }

        public int GetLineX(int y, float k, float n)
        {
            return (int)((y - n) / k);
        }
        #endregion

        #region Events
        private void Main_Load(object sender, EventArgs e)
        {
            Init();

            solidObjects.Add(new Box(500, 0, 100, 100, Color.Red));
            solidObjects.Add(new Box(550, 350, 100, 100, Color.Red));

            solidObjects.Add(new Polygon(new Point[] { new Point(100, display.Height - 250), new Point(150, display.Height - 300), new Point(350, display.Height - 200), new Point(200, display.Height - 150) }, Color.Green));
            solidObjects.Add(new Polygon(new Point[] { new Point(700, display.Height - 200), new Point(900, display.Height - 250), new Point(1000, display.Height - 150) }, Color.Green));
            solidObjects.Add(new Polygon(new Point[] { new Point(250, display.Height - 400), new Point(450, display.Height - 550), new Point(450, display.Height - 400) }, Color.Green));
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (!actions.HasFlag(Actions.Up) && onGround)
                    {
                        actions |= Actions.Up;
                        player.SpeedY = -PLAYER_JUMP_SPEED;
                    }
                    break;
                //case Keys.Down:
                //    actions |= Actions.Down;
                //break;
                case Keys.Right:
                    actions |= Actions.Right;
                    break;
                case Keys.Left:
                    actions |= Actions.Left;
                    break;
            }
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    actions &= ~Actions.Up;
                    break;
                //case Keys.Down:
                //    actions &= ~Actions.Down;
                //    break;
                case Keys.Right:
                    actions &= ~Actions.Right;
                    break;
                case Keys.Left:
                    actions &= ~Actions.Left;
                    break;
            }
        }

        private void Main_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Char.ToLower(e.KeyChar))
            {
                case 'r':   // give the player a random color
                    Random rnd = new Random();
                    player.color = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));
                    break;
            }
        }

        //int tempN = 0;
        short jumpCounter = 0;
        private void timer_main_Tick(object sender, EventArgs e)
        {
            onGround = false;
            player.SpeedX = 0;
            player.SpeedY = GRAVITY;

            // jump movement
            if (actions.HasFlag(Actions.Up) && jumpCounter <= 6)
            {
                jumpCounter++;
                player.SpeedY -= PLAYER_JUMP_SPEED;
            }
            else if (!actions.HasFlag(Actions.Up))
                jumpCounter = 0;

            // left-right movement
            if (actions.HasFlag(Actions.Right)) player.SpeedX = DEFAULT_PLAYER_SPEED;
            if (actions.HasFlag(Actions.Left)) player.SpeedX = -DEFAULT_PLAYER_SPEED;

            Point futurePos = new Point(player.X + player.SpeedX, player.Y + player.SpeedY);
            Rectangle futureRect = new Rectangle(futurePos, new Size(player.Width, player.Height));

            // screen border collision
            if (futurePos.X < 0) player.X = 0;
            if (futurePos.Y < 0) player.Y = 0;
            if (futurePos.X + player.Width > pb_display.Width) player.X = pb_display.Width - player.Width;
            if (futurePos.Y + player.Height > pb_display.Height) { player.Y = pb_display.Height - player.Height; onGround = true; }

            foreach (SolidObject obj in solidObjects)
            {
                // player-box collision
                if (obj is Box box)
                {
                    Point newPos = new Point(player.X, player.Y);

                    if (player.X < box.X &&
                        futurePos.X + player.Width > box.X &&
                        player.Y + player.Height > box.Y &&
                        player.Y < box.Y + box.Height &&
                        player.SpeedX > 0)
                    {
                        newPos.X = box.X - player.Width;
                        player.SpeedX = 0;
                    }
                    else if (player.X + player.Width > box.X + box.Width &&
                        futurePos.X < box.X + box.Width &&
                        player.Y + player.Height > box.Y &&
                        player.Y < box.Y + box.Height &&
                        player.SpeedX < 0)
                    {
                        newPos.X = box.X + box.Width;
                        player.SpeedX = 0;
                    }

                    if (player.Y < box.Y &&
                        futurePos.Y + player.Height > box.Y &&
                        player.X + player.Width > box.X &&
                        player.X < box.X + box.Width &&
                        player.SpeedY > 0)
                    {
                        newPos.Y = box.Y - player.Height;
                        player.SpeedY = 0;
                        onGround = true;
                    }
                    else if (player.Y + player.Height > box.Y + box.Height &&
                        futurePos.Y < box.Y + box.Height &&
                        player.X + player.Width > box.X &&
                        player.X < box.X + box.Width &&
                        player.SpeedY < 0)
                    {
                        newPos.Y = box.Y + box.Height;
                        player.SpeedY = 0;
                    }

                    if (player.X != newPos.X || player.Y != newPos.Y)
                    {
                        player.X = newPos.X;
                        player.Y = newPos.Y;

                        break;
                    }
                }

                // player-polygon collision
                if (obj is Polygon polygon)
                {
                    if (futureRect.IntersectsWith(polygon.Rect))
                    {
                        Point newPos = new Point(player.X, player.Y);
                        bool adjusted = false;

                        for (int i = 0; i < polygon.points.Length; i++)
                        {
                            Point p = polygon.points[i];

                            // POINT

                            if (futurePos.Y < p.Y && futurePos.Y + player.Height - GRAVITY > p.Y)
                            {
                                // left
                                if (futurePos.X + player.Width > p.X && player.X + player.Width <= p.X)
                                {
                                    newPos.X = p.X - player.Width;
                                    player.SpeedX = 0;
                                    //lb_display.Text = i + "\nP_L";
                                    adjusted = true;
                                    break;
                                }
                                // right
                                else if (futurePos.X < p.X && player.X >= p.X)
                                {
                                    newPos.X = p.X;
                                    player.SpeedX = 0;
                                    //lb_display.Text = i + "\nP_R";
                                    adjusted = true;
                                    break;
                                }
                            }

                            if (futurePos.X < p.X && futurePos.X + player.Width > p.X && false)
                            {
                                // top
                                if (futurePos.Y + player.Height > p.Y && player.Y + player.Height <= p.Y)
                                {
                                    newPos.Y = p.Y - player.Height;
                                    onGround = true;
                                    //lb_display.Text = i + "\nP_T";
                                    adjusted = true;
                                    break;
                                }
                                // bottom
                                else if (futurePos.Y < p.Y && player.Y >= p.Y)
                                {
                                    newPos.Y = p.Y;
                                    player.SpeedY = 0;
                                    //lb_display.Text = i + "\nP_B";
                                    adjusted = true;
                                    break;
                                }
                            }
                        }

                        for (int i = 0; i < polygon.points.Length && !adjusted; i++)
                        {
                            Point p1 = polygon.points[i];
                            Point p2 = (i != polygon.points.Length - 1) ? polygon.points[i + 1] : polygon.points[0];

                            // VERTICAL LINE
                            if (p1.X == p2.X)
                            {
                                if (p2.Y < p1.Y) (p2, p1) = (p1, p2);

                                if (futurePos.Y + player.Height > p1.Y &&
                                    futurePos.Y < p2.Y &&
                                    futurePos.X < p1.X &&
                                    futurePos.X + player.Width > p1.X)
                                {
                                    //lb_display.Text = String.Format("VL\n{0}\n{1}\n\n{2}\n{3}", p1, p2, new Point(player.X, player.Y), futurePos);

                                    if (player.SpeedX < 0)        // right
                                    {
                                        newPos.X = p1.X;
                                        player.SpeedX = 0;
                                        //lb_display.Text = i + "\nVL_R";
                                        break;
                                    }
                                    else if (player.SpeedX > 0)   // left
                                    {
                                        newPos.X = p1.X - player.Width;
                                        player.SpeedX = 0;
                                        //lb_display.Text = i + "\nVL_L";
                                        break;
                                    }
                                }
                            }

                            // HORIZONTAL LINE
                            else if (p1.Y == p2.Y)
                            {
                                if (p1.X > p2.X) (p2, p1) = (p1, p2);

                                if (futurePos.X + player.Width >= p1.X && futurePos.X <= p2.X)
                                {
                                    if (player.Y > futurePos.Y && futurePos.Y < p1.Y && futurePos.Y + player.Height > p1.Y)          // bottom
                                    {
                                        newPos.Y = p1.Y;
                                        player.SpeedY = GRAVITY;
                                        //lb_display.Text = i + "\nVH_B";
                                        break;
                                    }
                                    else if (player.Y < futurePos.Y && futurePos.Y + player.Height > p1.Y && futurePos.Y < p1.Y)    // top
                                    {
                                        newPos.Y = p1.Y - player.Height;
                                        onGround = true;
                                        //lb_display.Text = i + "\nVH_T";
                                        break;
                                    }
                                }
                            }

                            // ANGLED LINE
                            else
                            {
                                float k = 0f;
                                float n = 0f;

                                GetLineKN(p1, p2, ref k, ref n);

                                int yL = GetLineY(futurePos.X, k, n);
                                int yR = GetLineY(futurePos.X + player.Width, k, n);

                                if (p1.X > p2.X) (p2, p1) = (p1, p2);

                                ////lb_display.Text = String.Format("X: {0}\nyL: {1}", futurePos.X, yL);

                                // angle-ground
                                if (k < 0 && futurePos.Y > pb_display.Height - player.Height && yL > pb_display.Height - player.Height)
                                {
                                    newPos.Y = pb_display.Height - player.Height;
                                    newPos.X = GetLineX(pb_display.Height - player.Height, k, n);
                                    onGround = true;
                                    break;
                                }
                                else if (k > 0 && futurePos.Y > pb_display.Height - player.Height && yR > pb_display.Height - player.Height)
                                {
                                    newPos.Y = pb_display.Height - player.Height;
                                    newPos.X = GetLineX(pb_display.Height - player.Height, k, n) - player.Width;
                                    onGround = true;
                                    break;
                                }

                                // edge cases
                                if (k < 0)
                                {
                                    if (player.Y > p1.Y &&
                                        player.Y > futurePos.Y &&
                                        futureRect.Contains(p1))
                                    {
                                        newPos.Y = p1.Y;
                                        player.SpeedY = GRAVITY;
                                        //tempN = newPos.Y;
                                        //lb_display.Text = i + "\nL_P1";
                                        break;
                                    }
                                    else if (player.Y + player.Height - GRAVITY < p2.Y &&
                                        player.Y < futurePos.Y &&
                                        futureRect.Contains(p2))
                                    {
                                        newPos.Y = p2.Y - player.Height;
                                        onGround = true;
                                        //tempN = newPos.Y;
                                        //lb_display.Text = i + "\nL_P2";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (player.Y > p2.Y &&
                                        player.Y > futurePos.Y &&
                                        futureRect.Contains(p2))
                                    {
                                        newPos.Y = p2.Y;
                                        player.SpeedY = GRAVITY;
                                        //tempN = newPos.Y;
                                        //lb_display.Text = i + "\nL_P2";
                                        break;
                                    }
                                    else if (player.Y + player.Height - GRAVITY < p1.Y &&
                                        player.Y < futurePos.Y &&
                                        futureRect.Contains(p1))
                                    {
                                        newPos.Y = p1.Y - player.Height;
                                        onGround = true;
                                        //tempN = newPos.Y;
                                        //lb_display.Text = i + "\nL_P1";
                                        break;
                                    }
                                }

                                // from bottom
                                if (k < 0 &&
                                    player.X + player.Width >= p1.X &&
                                    player.X <= p2.X &&
                                    futurePos.Y < yL &&
                                    player.Y + GRAVITY > yL)
                                {
                                    newPos.Y = yL;
                                    player.SpeedY = GRAVITY;
                                    //tempN = newPos.Y;
                                    //lb_display.Text = i + "\nL_B1\nyL: " + yL + "\nfPy: " + futurePos.Y;
                                    break;
                                }
                                else if (k > 0 &&
                                    player.X + player.Width >= p1.X &&
                                    player.X <= p2.X &&
                                    futurePos.Y < yR &&
                                    player.Y + GRAVITY > yR)
                                {
                                    newPos.Y = yR;
                                    player.SpeedY = GRAVITY;
                                    //tempN = newPos.Y;
                                    //lb_display.Text = i + "\nL_B2";

                                    if (newPos.Y > pb_display.Height - player.Height)
                                    {
                                        newPos.Y = pb_display.Height - player.Height;

                                        if (newPos.X > player.X && k > 0) newPos.X = player.X;
                                    }

                                    break;
                                }

                                // from top
                                if (k < 0 &&
                                    player.X + player.Width >= p1.X &&
                                    player.X <= p2.X &&
                                    futurePos.Y + player.Height > yR &&
                                    player.Y < yR)
                                {
                                    newPos.Y = yR - player.Height;
                                    onGround = true;
                                    //lb_display.Text = i + "\nL_T1";
                                    //tempN = newPos.Y;
                                    break;
                                }
                                else if (k > 0 &&
                                    player.X + player.Width >= p1.X &&
                                    player.X <= p2.X &&
                                    futurePos.Y + player.Height > yL &&
                                    player.Y < yL)
                                {
                                    //lb_display.Text = i + "\nL_T2";
                                    newPos.Y = yL - player.Height;
                                    //tempN = newPos.Y;
                                    onGround = true;
                                    break;
                                }

                                //lb_display.Text += "\n{X: " + player.X + ", Y: " + player.Y + "}\n" + futurePos.ToString() + "\nk: " + k + "\nn: " + n + "\n" + tempN.ToString() + "\nsX: " + player.SpeedX + "\nsY: " + player.SpeedY;
                            }
                        }

                        if (player.X != newPos.X || player.Y != newPos.Y)
                        {
                            player.X = newPos.X;
                            player.Y = newPos.Y;
                            break;
                        }
                    }
                }
            }

            if (onGround) player.SpeedY = 0;

            player.Move();
            Draw();

            /*
            // debug draw
            using (Graphics g = Graphics.FromImage(display))
            {
                // player-line interaction new position
                g.DrawLine(new Pen(Color.Purple, 1), 0, tempN, this.Width, tempN);

                // horizontal display grid
                // for (int i = 0; i < pb_display.Height; i += 100)
                //     g.DrawLine(new Pen(Color.Black, 1), 0, i, this.Width, i);

                pb_display.Image = display;
                this.Invalidate();
            }
            */
        }
        #endregion
    }
}