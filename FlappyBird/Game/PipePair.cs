using FlappyBird.Engine;

namespace FlappyBird.Game
{
    class PipePair
    {
        private readonly Renderer _renderer;
        public Rectangle RectangleBottom { get; }
        public Rectangle RectangleTop {get; }

        /// <summary>Расстояние между разными парами колонн</summary>
        public float MovePosition { get; set; }

        /// <summary>Расстояние между частями ОДНОЙ колонны</summary>
        public float VerticalOffset { get; set; }

        //нужна ли? если все равно расстояние меняется в верхней переменной
        /// <summary>
        /// =VerticalOffset, только не изменяется при работе программы
        /// </summary>
        public float ConstOffsetY { get; }

        public int Group { get; }

        /// <param name="renderer">экземпляр рендера</param>
        /// <param name="offsetX">расстояние между разными парами колонн</param>
        /// <param name="offsetY">дробная часть расстояния между верхней и нижней частями ОДНОЙ колонны <br/>
        /// Напр. у числа 1,5 дробная часть = 5, т.е. вводим 5</param>
        public PipePair(Renderer renderer, float offsetX, float offsetY)
        {
            _renderer = renderer;
            ConstOffsetY = offsetY;
            //чтобы растянуть текстуру, нужно увеличить второй и четвертый аргумент
            //дробная часть второго аргумента - расстояние (высота) между парой колонн
            //однако нужно еще подумать как это реализовать грамотно
            //уменьшить - уменьшить расстояние; увеличить - увеличить расстояние
            //однако коллизии работают по-другому
            RectangleTop = new Rectangle(1f, 1+ConstOffsetY, 0.25f, -1f, 2f, Rectangle.RectMode.Left);
            RectangleBottom = new Rectangle(1f , -1-ConstOffsetY, 0.25f, 1f, 2f, Rectangle.RectMode.Left);

            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, RectangleBottom);
            _renderer.AddRectangleToGroup(Group, RectangleTop);

            //Задаем стартовую точку для колонны
            //при этом это свойство будет с каждым кадром меняться
            MovePosition = offsetX;
        }
    }
}
