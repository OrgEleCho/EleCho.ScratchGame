using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyWar
{
    internal class AcceleratingBullet : Bullet
    {
        public SizeF Acceleration { get; set; }

        private bool stop = false;
        public async override void Update()
        {
            base.Update();

            if (stop)
                return;

            var originSpeed = Speed;
            Speed += Acceleration * Game.DeltaTime;

            if (NumberOpposite(originSpeed.Width, Speed.Width) && NumberOpposite(originSpeed.Height, Speed.Height))
            {
                stop = true;
                await Task.Delay(1500);
                Game.RemoveObject(this);
            }
        }

        bool NumberOpposite(float a, float b)
        {
            return a switch
            {
                0 => b switch
                {
                    > 0 or < 0 => true,
                    _ => false,
                },
                > 0 => b switch
                {
                    0 or < 0 => true,
                    _ => false,
                },
                < 0 => b switch
                {
                    0 or > 0 => true,
                    _ => false,
                },
                float.NaN => false
            };
        }
    }
}
