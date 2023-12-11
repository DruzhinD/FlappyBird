using FlappyBird.Engine;
using OpenTK;
using System;
using System.Threading;

namespace FlappyBird.Game
{
    /// <summary>игрок</summary>
    class Player
    {
        private readonly Renderer _renderer;
        public Rectangle Rect { get; }

        /// <summary>скорость движения игрока в 1сек</summary>
        private float _velocity;
        /// <summary>позиция игрока по оси Y</summary>
        private float _position;
        /// <summary>угол наклона птицы</summary>
        private float _angle;
        private float _height = 0.16f*1.5f;
        private float _width = 0.09f*1.5f;

        public int Group { get; }
        /// <summary>количество очков</summary>
        public int Score { get; private set; }
        /// <summary>true - птица в игре, false - птица врезалась</summary>
        public bool Alive { get; private set; }

        public Player(Renderer renderer)
        {
            _renderer = renderer;
            Rect = new Rectangle(0f, 0f, _width, _height, 11f); //Rectmode = center
            //сохраняем индекс созданного буфера
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, Rect);

            Alive = true;
        }

        /// <summary>
        /// Движение игрока (птицы)
        /// </summary>
        /// <param name="frameTime">длительность одного кадра</param>
        public void MovePlayer(float frameTime)
        {
            if (_velocity < 3f)
                 _velocity -= 0.07f * frameTime;
            _position += _velocity;

            //птица опускает клюв, если она начинает падать 
            if (_velocity < 0 && _angle > -10)
                _angle -= 1;
            //птица поднимает клюв, когда начинает взлетать
            if (_velocity >= 0.001f && _angle < 9)
                _angle += 1;

            Matrix4 transform = Matrix4.Identity;
            //матрица вращения относительно оси Z
            transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_angle));

            //матрица сдвига относительно оси Y
            transform *= Matrix4.CreateTranslation(0f, _position, 0f);

            //передаем через Group индекс буфера и матрицу "состояния"
            _renderer.SetTransformRenderGroup(Group, transform);
        }

        /// <summary>
        /// Проверка на коллизии с колоннами
        /// </summary>
        /// <param name="pipes">Колонны</param>
        /// <param name="scoreTable">объект, необходимый для вывода количества очков на экран</param>
        public void DetectCollision(ref Pipes pipes, ref ScoreTable scoreTable, float frameTime)
        {
            //проверка коллизий с верхней и нижней частью окна
            if (this._position > 1f - _height / 3 || this._position < -1f + _height / 3)
            {
                Alive = false;
                ChangeTexture();
            }

            foreach (PipePair pair in pipes.PipePairs)
            {

                //проверка коллизий с верхней колонной
                if (_position > pair.OffsetY + pair.ConstOffsetY - this._height / 2.5f &&
                    pair.MovePosition< -1f + this._width / 2f && 
                    pair.MovePosition + 0.15f > -1f - this._width / 2f)
                {
                    Alive = false;
                    ChangeTexture();
                    break;
                }

                //проверка коллизий с нижней колонной
                if (_position < pair.OffsetY - pair.ConstOffsetY + this._height / 2.5f &&
                    pair.MovePosition < -1f + this._width / 2f &&
                    pair.MovePosition + 0.15f > -1f - this._width / 2f)
                {
                    Alive = false;
                    ChangeTexture();
                    break;
                }

                //проверка на проход между колоннами
                if (pair.MovePosition < -1f && pair.MovePosition > -1f - pipes.PipeSpeedFrequency * frameTime)
                {
                     Score++;
                    scoreTable.ChangeScoreTable(Score);
                }
            }

            //нужен для смены текстуры, после коллизии
            void ChangeTexture()
            {
                for (int i = 5; i <= 23; i+=6)
                {
                    this.Rect.Verticies[i] = 12f; 
                }
            }
        }
        
        /// <summary>
        /// реализация взмаха крыльев птички
        /// </summary>
        public void Jump()
        {
            if (_position == -1f)
                _position = -0.999f;
            _velocity = 0.025f;
            _angle += 1;
        }
    }
}
