using System;
using OpenTK;
using FlappyBird.Engine;
using OpenTK.Platform.Windows;

namespace FlappyBird.Game
{
    class Pipes
    {
        private Renderer _renderer;
        public PipePair[] PipePairs;

        private float _offset;

        //offset - расстояние между разными парами блоков по оси X
        public Pipes(Renderer renderer, int count, float offset)
        {
            _renderer = renderer;
            PipePairs = new PipePair[count];
            _offset = offset;

            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                PipePairs[i] = new PipePair(_renderer, i * offset);
                PipePairs[i].HorizontalOffset = (float)rand.Next(-4, 4) / 10;
            }
        }

        public void MovePipes(float frameTime)
        {
            for (int i = 0; i < PipePairs.Length; i++)
            {
                PipePairs[i].MovePosition -= 0.6f * frameTime; //0.5 по умолч

                if (PipePairs[i].MovePosition < -2f - 0.25f)
                {
                    PipePairs[i].MovePosition = 0f;
                    Random rand = new Random();
                    PipePairs[i].HorizontalOffset = (float)rand.Next(-4, 4) / 10;
                }

                _renderer.SetTransformRenderGroup(PipePairs[i].Group, Matrix4.CreateTranslation(PipePairs[i].MovePosition, PipePairs[i].HorizontalOffset, 0f));
            }
        }
    }
}
