using FlappyBird.Engine;
using System.Collections.Generic;

namespace FlappyBird.Game
{
    /// <summary>
    /// класс, необходимый для вывода количества очков на экран
    /// </summary>
    class ScoreTable
    {
        private readonly Renderer _renderer;

        /// <summary>
        /// Key = прямоугольник, содержащий в себе информацию о координатах и индексах текстур <br/>
        /// value = ID буфера, в котором хранится информация о прямоугольнике (Group)
        /// </summary>
        public Dictionary<Rectangle, int> Rectangles { get; private set; }

        private float _height = 0.16f * 1f;
        private float _width = 0.09f * 1f;

        /// <summary>
        /// текущая отступ от края экрана по оси X, на месте которого можно сгенерировать новый объект
        /// </summary>
        private float currentOffsetX = 0f;

        public ScoreTable(Renderer renderer)
        {
            _renderer = renderer;

            KeyValuePair<Rectangle, int> currentRect = GenNewRectangle();
            Rectangles = new Dictionary<Rectangle, int>()
            {
                { currentRect.Key, currentRect.Value },
            };
        }

        /// <summary>
        /// Генератор нового прямоугольника, который будет храниться в буфере с последующей отрисовкой
        /// </summary>
        /// <returns>Key - сгенерированный прямоугольник <br/>
        /// Value - индекс буфера, хранящего указанный прямоугольник</returns>
        private KeyValuePair<Rectangle, int> GenNewRectangle()
        {
            //сохраняем индекс сгенерированного буфера, в котором будет храниться информация о прямоугольнике
            int Group = _renderer.CreateRenderGroup();

            //непосредственно создание нового прямоугольника
            KeyValuePair<Rectangle, int> currentRect = new KeyValuePair<Rectangle, int>(
                new Rectangle(-1f + currentOffsetX, -1f, _width, _height, 0f, Rectangle.RectMode.Left), Group);
            currentOffsetX += _width;

            //добавляем текущий прямоугольник в объекты для отрисовки
            _renderer.AddRectangleToGroup(currentRect.Value, currentRect.Key);
            return currentRect;
        }

        /// <summary>
        /// Смена текстуры (цифр в числе), которые будут отрисованы на месте количества очков
        /// </summary>
        /// <param name="score">количество очков</param>
        public void ChangeScoreTable(int score)
        {
            string stringScore = score.ToString();
            //проверяем, если число, допустим, было однозначным, а стало двузначным,
            //то добавляем еще один объект отрисовки
            if (Rectangles.Count != stringScore.Length)
            {
                KeyValuePair<Rectangle, int> currentRect = GenNewRectangle();
                Rectangles.Add(currentRect.Key, currentRect.Value);
            }

            //меняем текстуру (цифру)
            int numPosition = 0;
            foreach (var RectPair in Rectangles)
            {
                for (int i = 5; i <= 23; i += 6)
                    RectPair.Key.Verticies[i] = float.Parse(stringScore[numPosition].ToString());
                numPosition++;
            }
        }
    }
}
