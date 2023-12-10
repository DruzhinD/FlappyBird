using FlappyBird.Engine;

namespace FlappyBird.Game
{
    /// <summary>отдельно взятая пара колонн</summary>
    class PipePair
    {
        private readonly Renderer _renderer;
        public Rectangle RectangleBottom { get; }
        public Rectangle RectangleTop {get; }

        /// <summary>Координата по оси X <br/>
        /// Причем правая часть окна = 0f, левая = -2f</summary>
        public float MovePosition { get; set; }

        /// <summary>Отступ по оси Y относительно центра <br/>
        /// как только колонна уходит за левую часть окна, это свойство меняется</summary>
        public float OffsetY { get; set; }

        /// <summary>
        /// расстояние между верхней и нижней частью колонны
        /// </summary>
        public float ConstOffsetY { get; }

        public int Group { get; }

        /// <param name="renderer">экземпляр рендера</param>
        /// <param name="offsetX">расстояние между разными парами колонн</param>
        /// <param name="offsetY">расстояние между верхней и нижней частью колонны <br/>
        /// должно быть меньше 0.9f</param>
        public PipePair(Renderer renderer, float offsetX, float offsetY)
        {
            _renderer = renderer;
            ConstOffsetY = offsetY;
            //чтобы растянуть текстуру, нужно увеличить второй и четвертый аргумент
            //дробная часть второго аргумента - расстояние (высота) между парой колонн
            //однако нужно еще подумать как это реализовать грамотно
            //уменьшить - уменьшить расстояние; увеличить - увеличить расстояние
            //однако коллизии работают по-другому
            RectangleTop = new Rectangle(1f, 2+ConstOffsetY, 0.15f, -2f, 12f, Rectangle.RectMode.Left);
            RectangleBottom = new Rectangle(1f , -2-ConstOffsetY, 0.15f, 2f, 12f, Rectangle.RectMode.Left);

            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, RectangleBottom);
            _renderer.AddRectangleToGroup(Group, RectangleTop);

            //Задаем стартовую точку для колонны
            //при этом это свойство будет с каждым кадром меняться
            MovePosition = offsetX;
        }
    }
}
