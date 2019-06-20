using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace DinasorGame
{
    class animeator : IDisposable
    {
        List<PictureBox> pB = new List<PictureBox>();
        List<spriteLoader.sprite> S = new List<spriteLoader.sprite>();
        List<Timer> dispose_timer = new List<Timer>();
        public void NewAnimator(PictureBox box, spriteLoader.sprite sprite)
        {
            pB.Add(box);
            pB[pB.Count - 1].Tag = 0;
            box.Size = sprite.size;
            S.Add(sprite);
            if (sprite.frames.Count > 1)
            {
                Timer x = new Timer();
                x.Tick += new EventHandler(timeelapse);
                x.Tag = S.Count - 1;
                x.Interval = sprite.frameratetime;
                x.Enabled = true;
                dispose_timer.Add(x);
            }
            else
            {
                box.Image = sprite.frames[0];
            }
        }

        void timeelapse(object sender, EventArgs e)
        {
            Timer x = (Timer)sender;
            int index = (int)x.Tag;
            int frame = (int)pB[index].Tag;
            if (S[index].animateable == false) { pB[index].Image = S[index].frames[0]; return; }
            if (frame == S[index].frames.Count - 1)
            {
                pB[index].Image = S[index].frames[frame];
                pB[index].Tag = 0;
                pB[index].BackColor = Color.Transparent;
            }
            else
            {
                pB[index].Image = S[index].frames[frame];
                pB[index].Tag = (int)pB[index].Tag + 1;
                pB[index].BackColor = Color.Transparent;
            }
        }
        public void ChangeSpeed(PictureBox box, int newTime)
        {
            for (int i = 0; i < pB.Count; i++)
            {
                if (pB[i] == box)
                {
                    spriteLoader.sprite x = S[i];
                    x.frameratetime = newTime;
                    dispose_timer[i].Interval = newTime;
                    S[i] = x;
                    return;
                }
            }
        }
        public void ChangeSpeed(int newTime)
        {
            for (int i = 0; i < pB.Count; i++)
            {
                    spriteLoader.sprite x = S[i];
                    x.frameratetime = newTime;
                    dispose_timer[i].Interval = newTime;
                    S[i] = x;
                    return;
            }
        }
        public void ChangeSprite(PictureBox box,spriteLoader.sprite newSprite)
        {
            for (int i = 0; i < pB.Count; i++)
            {
                if (pB[i] == box)
                {
                    S[i] = newSprite;
                    box.Size = newSprite.size;
                    return;
                }
            }
        }
        public void Dispose()
        {
            for (int i = 0; i < dispose_timer.Count; i++)
            {
                dispose_timer[i].Dispose();
            }
            pB = null;
            S = null;
            dispose_timer = null;
        }
    }
}
