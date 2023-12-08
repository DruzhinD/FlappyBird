using FlappyBird.Engine;
using OpenTK;
using System;

namespace FlappyBird.Game
{
    class Player
    {
        private readonly Renderer _renderer;
        public Rectangle Rect { get; private set; }

        private float _velocity;
        private float _position;
        private float _angle;
        private float _height = 0.2f;
        private float _width = 0.1f;

        public int Group { get; }
        public int Score { get; private set; }
        public bool Alive { get; private set; }

        public Player(Renderer renderer)
        {
            _renderer = renderer;
            Rect = new Rectangle(0f, 0f, _width, _height, 8f); //Rectmode = center
            //сохраняем индекс созданного буфера
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, Rect);

            Alive = true;
        }

        //time - время одного кадра, при 75гц - 0,0134с
        /// <summary>
        /// Движение игрока (птицы)
        /// </summary>
        /// <param name="frameTime">длительность одного кадра</param>
        public void MovePlayer(float frameTime)
        {
            if (_velocity < 3f)
                 _velocity -= 0.07f * frameTime; //для 75гц = 0,0094

            _position += _velocity;
            //Console.WriteLine($"position: {Math.Round(_position, 2)}    velocity: {_velocity}");
            //Console.WriteLine($"angle: {_angle}");
            //птица опускает клюв, если она начинает падать 
            if (_velocity < 0 && _angle > -10)
                _angle -= 1;
            //птица поднимает клюв, когда начинает взлетать
            if (_velocity >= 0.01f && _angle < 9)
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
        public void DetectCollision(Pipes pipes)
        {
            //проверка коллизий с верхней и нижней частью окна
            if (this._position > 1f - _height / 3 || this._position < -1f + _height / 3)
                Alive = false;

            foreach (PipePair pair in pipes.PipePairs)
            {

                //проверка коллизий с верхней колонной
                if (_position > pair.VerticalOffset + pair.ConstOffsetY - _height / 3 && pair.MovePosition < -1f + _width / 3 && pair.MovePosition > -1f + _width / 3 - 0.25f)
                {
                    Alive = false;
                    break;
                }

                //проверка коллизий с нижней колонной
                if (_position < pair.VerticalOffset - pair.ConstOffsetY + _height / 3 && pair.MovePosition < -1f + _width / 3 && pair.MovePosition > -1f + _width / 3 - 0.25f)
                {
                    Alive = false;
                    break;
                }

                if (pair.MovePosition < -1f && pair.MovePosition > -1.005f)
                {
                    Score++;
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
            _velocity = 0.02f;
            _angle = 6;
        }
    }
}
