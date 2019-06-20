using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
namespace DinasorGame
{
    class spriteLoader
    {
        public struct sprite
        {
            public List<Image> frames;
            public Size size;
            public int frameratetime;
            public bool animateable;
        }
        List<sprite> d1 = new List<sprite>();
        List<string> d2 = new List<string>();
        public void initlize()
        {
            string all = File.ReadAllText("sprites\\sprite.txt");
            all = all.Replace(System.Environment.NewLine, string.Empty);
            int semi = 0, oldsemi = 0;
            do
            {
                semi = all.IndexOf(";", semi + 1);
                if (semi > -1)
                {
                    string file;
                    if (oldsemi == 0) file = all.Substring(oldsemi, semi);
                    else file = all.Substring(oldsemi + 1, semi - (oldsemi + 1));
                    int n = 0, oldn = 0;
                    do
                    {
                        n = file.IndexOf(":", oldn + 1);
                        sprite x = new sprite();
                        if (n > -1)
                        {
                            if (oldn == 0) d2.Add(file.Substring(oldn, n - oldn));
                            else d2.Add(file.Substring(oldn + 1, n - (oldn + 1)));

                            x.frames = new List<Image>();
                            int n2 = file.IndexOf(":", n + 1);
                            if (n2 > -1)
                            {
                                string txt = file.Substring(n + 1, n2 - (n + 1));
                                int f = 0, fold = 0;
                                do
                                {
                                    f = txt.IndexOf(",", f + 1);
                                    if (f > -1)
                                    {
                                        string frame;
                                        if (fold == 0) frame = txt.Substring(fold, f);
                                        else frame = txt.Substring(fold + 1, f - (fold + 1));
                                        x.frames.Add(Bitmap.FromFile("sprites\\" + frame));
                                    }
                                    else
                                    {
                                        if (fold == 0) fold = -1;
                                        string frame = txt.Substring(fold + 1, txt.Length - (fold + 1));
                                        x.frames.Add(Bitmap.FromFile("sprites\\" + frame));
                                    }
                                    fold = f;
                                } while (f > -1);
                                int tm = 0;
                                int l = file.IndexOf("time");
                                if (l > -1)
                                {
                                    int e = file.IndexOf("=", l);
                                    if (e > -1)
                                    {
                                        tm = int.Parse(file.Substring(e + 1, file.Length - (e + 1)));
                                    }
                                }
                                if (tm > 0) x.frameratetime = tm;
                                else x.frameratetime = 1000;
                            }
                            oldn = n2;
                            x.size = x.frames[0].Size;
                            if (x.frames.Count == 1) x.animateable = false;
                            else x.animateable = true;
                            d1.Add(x);
                        }


                    } while (n > -1);
                }
                oldsemi = semi;
            } while (semi > -1);
        }
        public sprite GetSprite(string name)
        {
            for (int i = 0; i < d2.Count; i++)
            {
                if (name == d2[i]) return d1[i];
            }
            return new sprite();
        }
    }
}
