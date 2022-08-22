namespace EleCho.ScratchGame
{
    public struct GameObjectCollider
    {
        public readonly PointF[] Vertexes;

        public GameObjectCollider()
        {
            Vertexes = new PointF[]
            {
                new PointF(0, 0),
                new PointF(1, 0),
                new PointF(1, 1),
                new PointF(0, 1),
            };
        }

        public GameObjectCollider(params PointF[] vertexes)
        {
            Vertexes = vertexes;
        }
    }
}