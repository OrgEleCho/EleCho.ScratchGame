namespace SkyWar
{
    internal class DeceleratingBullet : Bullet
    {

        public float DecelerationRatio { get; set; } = 0.5f;

        public override void Update()
        {
            base.Update();

            Speed -= Speed * DecelerationRatio;
            if (Speed.Width * Speed.Width + Speed.Height * Speed.Height < 1f)
            {
                Game.RemoveObject(this);
            }
        }
    }
}
