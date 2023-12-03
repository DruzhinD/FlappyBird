using FlappyBird.Engine;

namespace FlappyBird.Game
{
    class PipePair
    {
        private Renderer _renderer;
        private Rectangle _rectangle0;
        private Rectangle _rectangle1;

        public float MovePosition { get; set; }
        public float HorizontalOffset { get; set; }
        public float Offset { get; }

        public int Group { get; }

        public PipePair(Renderer renderer, float offset)
        {
            _renderer = renderer;
            _rectangle0 = new Rectangle(1f , -3.4f, 0.25f, 3f, 2f, Rectangle.RectMode.Left);
            _rectangle1 = new Rectangle(1f, 3.4f, 0.25f, -3f, 2f, Rectangle.RectMode.Left);

            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, _rectangle0);
            _renderer.AddRectangleToGroup(Group, _rectangle1);

            Offset = offset;
            MovePosition = offset;
        }
    }
}
