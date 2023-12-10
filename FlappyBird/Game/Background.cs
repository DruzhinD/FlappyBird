using FlappyBird.Engine;

namespace FlappyBird.Game
{
    /// <summary>класс, представляющий статическую текстуру <br/>
    /// будь то задний фон, стартовый экран или экран завершения игры</summary>
    class Background
    {
        private Renderer _renderer;
        private Rectangle _rectangle;

        public int Group { get; }

        public Background(Renderer renderer, float textureIndex)
        {
            _renderer = renderer;
            _rectangle = new Rectangle(0f, 0f, 2f, 2f, textureIndex);
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, _rectangle);
        }
    }
}