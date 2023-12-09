using FlappyBird.Engine;

namespace FlappyBird.Game
{
    class Background
    {
        private Renderer _renderer;
        private Rectangle _rectangle;

        public int Group { get; }

        public Background(Renderer renderer)
        {
            _renderer = renderer;
            _rectangle = new Rectangle(0f, 0f, 2f, 2f, 10f);
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, _rectangle);
        }
    }
}