using FlappyBird.Engine;
using OpenTK;
using System;
using System.ComponentModel.Design;

namespace FlappyBird.Game
{
    class Player
    {
        private Renderer _renderer;
        private Rectangle _rectangle;

        private float _velocity;
        private float _position;
        private float _angle;
        private float _height = 0.3f;
        private float _width = 0.2f;

        public int Group { get; }
        public int Score { get; private set; }
        public bool Alive { get; private set; }

        public Player(Renderer renderer)
        {
            _renderer = renderer;
            _rectangle = new Rectangle(0f, 0f, _width, _height, 1f);
            //сохраняем индекс созданного буфера
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, _rectangle);

            Alive = true;
        }

        //etime - время одного кадра, при 75гц - 0,0134с
        public void MovePlayer(float eTime)
        {
            if (_velocity < 3f)
                 _velocity -= 0.07f * eTime; //для 75гц = 0,0094

            _position += _velocity;
            Console.WriteLine($"position: {Math.Round(_position, 2)}    velocity: {_velocity}");
            Console.WriteLine($"angle: {_angle}");
            //птица опускает клюв, если она начинает падать 
            if (_velocity < 0 && _angle > -5)
                _angle -= 1;
            //птица поднимает клюв, когда начинает взлетать
            if (_velocity > 0.02f && _angle < 9)
                _angle += 1;
            Matrix4 transform = Matrix4.Identity;
            transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_angle));
            transform *= Matrix4.CreateTranslation(0f, _position, 0f);

            //передаем через Group индекс буфера и матрицу "состояния"
            _renderer.SetTransformRenderGroup(Group, transform);
        }

        public void DetectCollision(Pipes pipes)
        {
            //top/bottom colision
            if (this._position > 1f - _height / 3 || this._position < -1f + _height / 3)
                Alive = false;

            foreach (PipePair pair in pipes.PipePairs)
            {
                //top pipe collision
                if (_position > pair.HorizontalOffset + 0.4f - _height / 3 && pair.MovePosition < -1f + _width / 3 && pair.MovePosition > -1f + _width / 3 - 0.25f)
                {
                    Alive = false;
                    break;
                }

                //bottom pipe collision
                if (_position < pair.HorizontalOffset - 0.4f + _height / 3 && pair.MovePosition < -1f + _width / 3 && pair.MovePosition > -1f + _width / 3 - 0.25f)
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

        public void Jump()
        {
            if (_position == -1f)
                _position = -0.999f;
            _velocity = 0.03f;
            Console.WriteLine("--ПРОБЕЛ--");
            //_angle = 6;
        }
    }
}
