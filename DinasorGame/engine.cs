using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace DinasorGame
{
    class engine
    {
        Form mainfrm;
        bool paused = false;
        spriteLoader mainLoader;
        animeator mainAnim;
        public PictureBox[] roads;
        PictureBox player;
        int road_speed = 1;
        int game_speed = 1;
        int trees_count = 3;
        int TREES_MINIMUM_DISTANCE = 300;
        int TREES_MAXIMUM_DISTANCE = 800;
        Random rand;
        int CENTER_X;
        int CENTER_Y;
        List<PictureBox> trees;
        Timer road_timer;
        List<PictureBox> drawList = new List<PictureBox>();
        public int Fps = 0;
        Timer drawingTimer;
        public engine(Form mainForm, spriteLoader sp)
        {
            mainfrm = mainForm;
            mainLoader = sp;
            mainAnim = new animeator();
            trees = new List<PictureBox>();
            rand = new Random();
            System.Diagnostics.Debug.WriteLine("[EVENT] new engine class instance created!");
        }
        public void Set_Center_coords(int x, int y) { CENTER_X = x; CENTER_Y = y; }
        public void createThePlayer()
        {

            spriteLoader.sprite s = mainLoader.GetSprite("player");
            player = AddDrawObject_Animated(mainLoader.GetSprite("player"), new Point(CENTER_X - s.size.Width, CENTER_Y - s.size.Height));
            System.Diagnostics.Debug.WriteLine("[EVENT] player created!");
        }
        public void PLAYER_CROUCH()
        {
            spriteLoader.sprite s = mainLoader.GetSprite("player_crouch");
            player.Image = s.frames[0];
            player.Location = new Point(CENTER_X - s.size.Width, CENTER_Y - s.size.Height);
            mainAnim.ChangeSprite(player, s);

        }
        public void PLAYER_NORMAL()
        {
            spriteLoader.sprite s = mainLoader.GetSprite("player");
            player.Image = s.frames[0];
            player.Location = new Point(CENTER_X - s.size.Width, CENTER_Y - s.size.Height);
            mainAnim.ChangeSprite(player, s);

        }
        public void createInfinitRoad(int x, int y)
        {
            roads = new PictureBox[2];
            roads[0] = new PictureBox();
            roads[1] = new PictureBox();
            spriteLoader.sprite s = mainLoader.GetSprite("road");
            roads[0].Size = roads[1].Size = s.size;
            roads[0].Location = new Point(x, y);
            roads[1].Location = new Point(roads[0].Location.X + roads[0].Width, y);
            roads[0].Visible = roads[1].Visible = false;
            roads[0].Image = roads[1].Image = s.frames[0];
            mainfrm.Controls.Add(roads[0]);
            mainfrm.Controls.Add(roads[1]);
            drawList.Add(roads[0]);
            drawList.Add(roads[1]);
            road_timer = new Timer()
            {
                Interval = 10,
                Enabled = true
            };
            road_timer.Tick += new EventHandler(road_move);
            System.Diagnostics.Debug.WriteLine("[EVENT] road created!");
        }
        public int RoadSpeed
        {
            set => road_speed = value;
            get => road_speed;
        }
        public int GameSpeed
        {
            get
            {
                return game_speed;
            }
            set
            {
                game_speed = value;
                mainAnim.ChangeSpeed(200 - value);
            }
        }
        public bool IsPaused { get => paused; }
        public void StartDrawing(int FPS)
        {
            int interval = (int)Math.Round((double)(1000.0 / (double)FPS));
            System.Diagnostics.Debug.WriteLine("[EVENT] StartDrawing : TIMER intreval set to " + interval);
            drawingTimer = new Timer();
            drawingTimer.Interval = interval;
            drawingTimer.Enabled = true;
            drawingTimer.Tick += new EventHandler(DrawingAll);
        }
        private void road_move(object sender, EventArgs e)
        {
            if (paused) return;
            for (int i = 0; i < 2; i++)
            {
                roads[i].Location = new Point(roads[i].Location.X - road_speed, roads[i].Location.Y);
                if (roads[i].Location.X + roads[i].Width <= 0)
                {
                    if (i < 1)
                    {
                        roads[i].Location = new Point(roads[i + 1].Location.X + roads[i + 1].Width, roads[i].Location.Y);
                    }
                    else
                    {
                        roads[i].Location = new Point(roads[i - 1].Location.X + roads[i - 1].Width, roads[i].Location.Y);
                    }
                }
            }
        }
        public void AddDrawObject_nonAnimated(PictureBox obj)
        {
            drawList.Add(obj);
        }
        public PictureBox AddDrawObject_nonAnimated(spriteLoader.sprite obj, Point xy)
        {
            PictureBox x = new PictureBox();
            x.Image = obj.frames[0];
            x.Size = obj.size;
            x.Location = xy;
            drawList.Add(x);
            return x;
        }
        public PictureBox AddDrawObject_Animated(spriteLoader.sprite obj, Point xy)
        {
            PictureBox x = new PictureBox();
            x.Image = obj.frames[0];
            x.Size = obj.size;
            x.Location = xy;
            drawList.Add(x);
            mainAnim.NewAnimator(x, obj);
            return x;
        }
        public void ResumeDrawing()
        {
            drawingTimer.Enabled = true;
            paused = false;
        }
        private void DrawingAll(object sender, EventArgs e)
        {
            if (mainfrm.WindowState == FormWindowState.Minimized)
            {
                drawingTimer.Enabled = false; mainfrm.BackgroundImage = null;
                paused = true;
                System.Diagnostics.Debug.WriteLine("[EVENT] Drawing paused because the window is minimized!");
                return;
            }
            Fps++;
            Bitmap frameBuffer = new Bitmap(mainfrm.Size.Width, mainfrm.Size.Height);
            Graphics g = Graphics.FromImage(frameBuffer);
            g.Clear(mainfrm.BackColor);
            for (int i = 0; i < drawList.Count; i++)
            {
                g.DrawImage(drawList[i].Image, drawList[i].Location);
            }
            if (mainfrm.BackgroundImage != null) mainfrm.BackgroundImage.Dispose();
            g.Dispose();
            mainfrm.BackgroundImage = frameBuffer;
            Application.DoEvents();

            frameBuffer.Dispose();
            //frameBuffer = null;
        }

        public void StartBuildingTrees()
        {
            PictureBox[] t = new PictureBox[trees_count];
            spriteLoader.sprite x = mainLoader.GetSprite("tree1");
            int starting_x = mainfrm.Width;
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = new PictureBox();
                t[i].Image = x.frames[0];
                t[i].Location = new Point((starting_x + (RandomTreeDistance() * i)) - x.size.Width, CENTER_Y - x.size.Height);
                t[i].Size = x.size;
                drawList.Add(t[i]);
                trees.Add(t[i]);
            }
            Timer ti = new Timer()
            {
                Interval = road_timer.Interval,
                Enabled = true
            };
            ti.Tick += new EventHandler(trees_timeelapse);
        }
        int RandomTreeDistance()
        {
            return rand.Next(TREES_MINIMUM_DISTANCE, TREES_MAXIMUM_DISTANCE);
        }
        private void trees_timeelapse(object sender, EventArgs e)
        {
            if (paused) return;
            for (int i = 0; i < trees.Count; i++)
            {
                trees[i].Location = new Point(trees[i].Location.X - road_speed, trees[i].Location.Y);
                if (trees[i].Location.X + trees[i].Width < 0) { trees[i].Location = new Point(mainfrm.Width + (RandomTreeDistance() + 100), trees[i].Location.Y); UpdateTree(RandomTreeSprite(), i); }
            }
        }
        void UpdateTree(spriteLoader.sprite sprite, int index)
        {
            trees[index].Size = sprite.size;
            trees[index].Location = new Point(trees[index].Location.X, CENTER_Y - sprite.size.Height);
            trees[index].Image = sprite.frames[0];
        }
        spriteLoader.sprite RandomTreeSprite()
        {
            int r = rand.Next(1, 3);
            System.Diagnostics.Debug.WriteLine("[EVENT] random return a sprite (tree" + r + ")");
            return mainLoader.GetSprite("tree" + r);

        }

    }
}
