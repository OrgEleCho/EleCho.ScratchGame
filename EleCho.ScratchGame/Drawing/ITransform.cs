namespace EleCho.ScratchGame.Drawing
{
    public interface ITransform : ICloneable
    {
        public void Translate(float dx, float dy);
        public void Scale(float scaleX, float scaleY);
        public void Rotate(float degree);

        public void ScaleAt(float centerX, float centerY, float scaleX, float scaleY);
        public void RotateAt(float cetnerX, float centerY, float degree);

        public void ScaleAndRotateAt(float centerX, float centerY, float scaleX, float scaleY, float degree);
    }
}
