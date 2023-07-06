using System.Drawing;

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
        const short PLAYER_JUMP_SPEED = 60;
        const short DEFAULT_PLAYER_HEIGHT = 100;
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
            public int Height { get; set; }
            public int Width { get; set; }
            public int SpeedX { get; set; }
            public int SpeedY { get; set; }
            public Color color;   // privremena
            //Image image;

            public Player(int x, int y, int h, int w, Color color)
            {
                this.Height = h;
                this.Width = w;
                this.X = x;
                this.Y = display.Height - y - h;
                this.color = color;
            }

            public Player(int x = 0, int y = 0, int h = DEFAULT_PLAYER_HEIGHT, int w = DEFAULT_PLAYER_WIDTH)
            {
                this.X = x;
                this.Y = display.Height - y - h;
                this.Height = h;
                this.Width = w;
                this.color = DEFAULT_PLAYER_COLOR;
            }

            public void Move()
            {
                this.X += this.SpeedX;
                this.Y += this.SpeedY;
            }

            public void Move(Point p)
            {
                this.X = p.X;
                this.Y = p.Y;
            }

            public void Move(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        class SolidObject { }

        class Box : SolidObject
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
            public Color color;   // privremena
            //Image image;

            public Box(int x, int y, int h, int w, Color color)
            {
                this.X = x;
                this.Y = display.Height - y - h;
                this.Height = h;
                this.Width = w;
                this.color = color;
            }

            public void Move(Point p)
            {
                this.X = p.X;
                this.Y = p.Y;
            }

            public void Move(int x, int y)
            {
                this.X = x;
                this.Y = y;
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

                foreach (SolidObject obj in solidObjects)
                {
                    if (obj is Box)
                    {
                        Box box = (Box)obj;
                        g.FillRectangle(new SolidBrush(box.color), box.X, box.Y, box.Width, box.Height);
                    }
                }

                g.FillRectangle(new SolidBrush(player.color), player.X, player.Y, player.Width, player.Height); // player

                pb_display.Image = display;
                this.Invalidate();
            }
        }

        private void Init()
        {
            display = new Bitmap(pb_display.Width, pb_display.Height);
            player = new Player();
            timer_main.Start();
        }
        #endregion

        #region Events
        private void Main_Load(object sender, EventArgs e)
        {
            Init();

            solidObjects.Add(new Box(500, 0, 100, 100, Color.Red));
            solidObjects.Add(new Box(550, 350, 100, 100, Color.Red));
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
                case 'r':
                    break;
            }
        }

        short jumpCounter = 0;
        private void timer_main_Tick(object sender, EventArgs e)
        {
            onGround = false;
            player.SpeedX = 0;
            player.SpeedY = GRAVITY;

            if (actions.HasFlag(Actions.Up) && jumpCounter <= 5)
            {
                jumpCounter++;
                player.SpeedY -= PLAYER_JUMP_SPEED;
            }
            else if (!actions.HasFlag(Actions.Up))
                jumpCounter = 0;

            if (actions.HasFlag(Actions.Right)) player.SpeedX = DEFAULT_PLAYER_SPEED;
            if (actions.HasFlag(Actions.Left)) player.SpeedX = -DEFAULT_PLAYER_SPEED;

            Point futurePos = new Point(player.X + player.SpeedX, player.Y + player.SpeedY);

            // screen border collision
            if (futurePos.X < 0) player.X = 0;
            if (futurePos.Y < 0) player.Y = 0;
            if (futurePos.X + player.Width > pb_display.Width) player.X = pb_display.Width - player.Width;
            if (futurePos.Y + player.Height > pb_display.Height) { player.Y = pb_display.Height - player.Height; onGround = true; }

            foreach (SolidObject obj in solidObjects)
            {
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
            }

            player.Move();

            lb_display.Text = onGround.ToString();

            Draw();
        }
        #endregion
    }
}