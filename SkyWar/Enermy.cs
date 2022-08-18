using NAudio.Wave;
using EleCho.ScratchGame;
using SkyWar.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyWar
{
    internal class Enermy : GameSprite
    {
        static Bitmap enermy = Resources.enemy1;
        static Bitmap[] deathSequence = new Bitmap[]
        {
            Resources.enemy1_down1,
            Resources.enemy1_down2,
            Resources.enemy1_down3,
            Resources.enemy1_down4,
        };

        WaveFileReader deathSound = new WaveFileReader(Resources.enemy1_down);

        public Enermy()
        {
            Sprite = enermy;

            deathSoundPlayer = new WaveOut();
            deathSoundPlayer.Init(deathSound);
        }

        public float Speed = 150;

        WaveOut deathSoundPlayer;

        void PlayDownSound()
        {
            deathSoundPlayer.Stop();
            deathSound.Seek(0, SeekOrigin.Begin);
            deathSoundPlayer.Play();
        }

        bool down;
        private async void StartDestroySelf()
        {
            down = true;
            Speed /= 2;

            PlayDownSound();

            foreach (var frame in deathSequence)
            {
                await Task.Delay(100);
                Sprite = frame;
            }

            Game.RemoveObject(this);
        }

        public override void Update()
        {
            if (Sprite == null)
                return;

            float move = Speed * Game.DeltaTime;
            float y = Position.Y - move;

            Position = new PointF(Position.X, y);

            if (down)
                return;

            foreach (var bullet in Game.OfType<Bullet>())
            {
                if (Game.IsCollided(bullet, this))
                {
                    Game.RemoveObject(bullet);
                    StartDestroySelf();
                    break;
                }
            }

            if (!Game.Bounds.Contains(Point.Truncate(Position)))
            {
                Game.RemoveObject(this);
            }
        }
    }
}
