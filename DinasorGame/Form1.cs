using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DinasorGame
{
    public partial class Form1 : Form
    {
        int speed = 200;
        spriteLoader sLoader;
        animeator a = new animeator();
        engine eng;
        int GAME_CENTER_X = 200;
        int GAME_CENTER_Y = 355;
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            System.Diagnostics.Debug.WriteLine("welcome..");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //  BackColor = Color.White;
            sLoader = new spriteLoader();
            sLoader.initlize();
            eng = new engine(this, sLoader);
            eng.Set_Center_coords(GAME_CENTER_X, GAME_CENTER_Y);
            eng.createInfinitRoad(0, 340);
            eng.createThePlayer();
            eng.StartBuildingTrees();
            eng.StartDrawing(100);
            eng.GameSpeed = 70;
            eng.RoadSpeed = 10;
            button1.Enabled = false;
            Timer x = new Timer() { Enabled = true, Interval = 1000 };
            x.Tick += delegate
            {
                Text = eng.Fps + " fps";
                eng.Fps = 0;
                button1.Enabled = eng.IsPaused;
            };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            eng.RoadSpeed += 2;
            eng.GameSpeed += 10;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            eng.RoadSpeed--;
            eng.GameSpeed -= 5;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            eng.ResumeDrawing();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            eng.PLAYER_CROUCH();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            eng.PLAYER_NORMAL();
        }
    }
}
