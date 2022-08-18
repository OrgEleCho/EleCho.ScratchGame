using EleCho.ScratchGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyWar
{
    internal class RotatingText : GameText
    {

        public float Speed { get; set; } = 90;

        private float rotation;
        public override void Start()
        {
            rotation = Rotation;
        }

        public override void Update()
        {
            rotation += Speed * Game.DeltaTime;
            Rotation = (int)rotation;
        }
    }
}
