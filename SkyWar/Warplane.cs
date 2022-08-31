using NAudio.Wave;
using EleCho.ScratchGame;
using SkyWar.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SkyWar
{
    internal class Warplane : GameSprite
    {
        static Bitmap me1 = Resources.me1;
        static Bitmap me2 = Resources.me2;

        static Bitmap[] deathFrames = new Bitmap[]
        {
            Resources.me_destroy_1,
            Resources.me_destroy_2,
            Resources.me_destroy_3,
            Resources.me_destroy_4,
        };

        static WaveFileReader bulletSound = new WaveFileReader(Resources.bullet);
        static WaveFileReader deathSound = new WaveFileReader(Resources.me_down);

        public Warplane()
        {
            SpriteChangeLoop();
            SuperLoop();

            bulletSoundPlayer = new WaveOut();
            bulletSoundPlayer.Init(bulletSound);

            deathSoundPlayer = new WaveOut();
            deathSoundPlayer.Init(deathSound);

            Pivot = new PointF(0.5f, 0);
        }

        WaveOut bulletSoundPlayer;
        WaveOut deathSoundPlayer;

        private async void SpriteChangeLoop()
        {
            Bitmap[] mes = new Bitmap[] { me1, me2 };
            while (true)
            {
                foreach (var me in mes)
                {
                    await Task.Delay(100);
                    Sprite = me;
                }
            }
        }

        public async void SuperLoop()
        {
            float degreeAngle = 0;
            while (true)
            {
                await Task.Delay(50);
                degreeAngle += 10;
                degreeAngle %= 360;

                if (is_died)
                    continue;

                while (Game.GetMouseState(MouseButton.Left))
                {
                    PlayBulletSound();
                    for (int i = 0; i < 6; i++)
                    {
                        var _missile = new Bullet()
                        {
                            Position = Position,
                        };

                        float radian = (degreeAngle + 60 * i) * MathF.PI / 180;
                        float speed = MathF.Sqrt(_missile.Speed.Width * _missile.Speed.Width + _missile.Speed.Height * _missile.Speed.Height);
                        SizeF newSpeed = new SizeF(MathF.Cos(radian) * speed, MathF.Sin(radian) * speed);
                        _missile.Speed = newSpeed;

                        Game.AddObject(_missile);
                    }

                    await Task.Delay(50);
                    degreeAngle += 10;
                    degreeAngle %= 360;
                }

                while (Game.GetMouseState(MouseButton.Right))
                {
                    PlayBulletSound();
                    for (int i = 0; i < 6; i++)
                    {
                        var _missile = new DeceleratingBullet()
                        {
                            Position = Position,
                            DecelerationRatio = 0.05f
                        };

                        float radian = (degreeAngle + 60 * i) * MathF.PI / 180;
                        float speed = MathF.Sqrt(_missile.Speed.Width * _missile.Speed.Width + _missile.Speed.Height * _missile.Speed.Height);
                        SizeF newSpeed = new SizeF(MathF.Cos(radian) * speed, MathF.Sin(radian) * speed);
                        _missile.Speed = newSpeed;

                        Game.AddObject(_missile);
                    }

                    await Task.Delay(50);
                    degreeAngle += 13;
                    degreeAngle %= 360;
                }
            }
        }

        void PlayBulletSound()
        {
            bulletSoundPlayer.Stop();
            bulletSound.Seek(0, SeekOrigin.Begin);
            bulletSoundPlayer.Play();
        }

        void PlayDeathSound()
        {
            deathSoundPlayer.Stop();
            deathSound.Seek(0, SeekOrigin.Begin);
            deathSoundPlayer.Play();
        }

        public override void InvokeMouse()
        {
            base.InvokeMouse();
        }

        public override void InvokeKeyboard(KeyboardKey key)
        {
            base.InvokeKeyboard(key);

            if (is_died)
                return;

            PlayBulletSound();
            Game.AddObject(new Bullet()
            {
                Position = Position,
            });
        }

        bool pause = false;

        public override void Update()
        {
            if (pause)
                return;

            if (is_died)
                return;

            var offset = new SizeF(Game.MousePosition) - new SizeF(Position);
            SizeF sizeF = offset / 2;
            float offsetX = sizeF.Width;
            Rotation = (int)(Math.Sign(offsetX) * Math.Log(Math.Abs(offsetX) + 1) * 3);
            Position += sizeF;

            foreach (var enermy in Game.OfType<Enermy>())
            {
                if (Game.IsCollided(enermy, this))
                {
                    DieAndRespawn();
                }
            }
        }

        bool is_died;

        public async void DieAndRespawn()
        {
            is_died = true;

            PlayDeathSound();
            foreach (var frame in deathFrames)
            {
                await Task.Delay(100);
                Sprite = frame;
            }

            Visible = false;

            await Task.Delay(3000);

            Position = new PointF(0, -Game.Height);
            Visible = true;

            is_died = false;
        }
    }
}
