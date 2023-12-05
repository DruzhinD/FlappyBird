using FlappyBird.Engine;

namespace FlappyBird.Game
{
    class PipePair
    {
        private Renderer _renderer;
        private Rectangle _rectangleBottom;
        private Rectangle _rectangleTop;

        public float MovePosition { get; set; }
        public float HorizontalOffset { get; set; }
        public float Offset { get; }

        public int Group { get; }

        public PipePair(Renderer renderer, float offset)
        {
            _renderer = renderer;
            //чтобы растянуть текстуру, нужно увеличить второй и четвертый аргумент
            //дробная часть второго аргумента - расстояние (высота) между парой колонн
            //однако нужно еще подумать как это реализовать грамотно
            //уменьшить - уменьшить расстояние; увеличить - увеличить расстояние
            //однако коллизии работают по-другому
            _rectangleTop = new Rectangle(1f, 1.4f, 0.25f, -1f, 2f, Rectangle.RectMode.Left);
            _rectangleBottom = new Rectangle(1f , -1.4f, 0.25f, 1f, 2f, Rectangle.RectMode.Left);

            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, _rectangleBottom);
            _renderer.AddRectangleToGroup(Group, _rectangleTop);

            Offset = offset;
            MovePosition = offset;
        }
    }
}
