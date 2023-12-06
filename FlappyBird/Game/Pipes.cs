using System;
using OpenTK;
using FlappyBird.Engine;
using OpenTK.Platform.Windows;

namespace FlappyBird.Game
{
    class Pipes
    {
        private readonly Renderer _renderer;
        public PipePair[] PipePairs;

        //offset - расстояние между разными парами блоков по оси X
        /// <param name="renderer">экземпляр рендера</param>
        /// <param name="count">количество генерируемых колонн</param>
        /// <param name="offsetX">расстояние между разными парами колонн (ось X)</param>
        /// <param name="offsetY">дробная часть расстояния между верхней и нижней частями ОДНОЙ колонны (ось Y)<br/>
        /// Напр. у числа 1,5 дробная часть = 5, т.е. вводим 5</param>
        public Pipes(Renderer renderer, int count, float offsetX, int offsetY)
        {
            _renderer = renderer;
            PipePairs = new PipePair[count];
            this.offsetY = offsetY;

            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                PipePairs[i] = new PipePair(_renderer, i * offsetX, (float)this.offsetY/10);
                
                PipePairs[i].VerticalOffset = (float)rand.Next(-this.offsetY, this.offsetY) / 10;
            }
        }

        private int offsetY;
        public void MovePipes(float frameTime)
        {
            for (int i = 0; i < PipePairs.Length; i++)
            {
                PipePairs[i].MovePosition -= 0.6f * frameTime; //0.5 по умолч

                //блок кода, необходимый для респауна колонн, при этом -2f - это расстояние между боковыми краями экрана
                //-2 из-за особенностей трансформации матрицами, на деле координата будет -1 -[ширина блока]
                if (PipePairs[i].MovePosition < -2f - 0.25f)
                {
                    PipePairs[i].MovePosition = 0f; //ось x
                    Random rand = new Random();
                     PipePairs[i].VerticalOffset = (float)rand.Next(-offsetY, offsetY) / 10; //ось y
                }
                if (i % 3 == 0)
                {
                    //Console.WriteLine($"X---|{PipePairs[i].MovePosition}----");
                    //Console.WriteLine($"Y---{PipePairs[i].VerticalOffset}----");
                    //Console.WriteLine(Matrix4.CreateTranslation(PipePairs[i].MovePosition, PipePairs[i].VerticalOffset, 0f) + "\n");
                }
                _renderer.SetTransformRenderGroup(PipePairs[i].Group, Matrix4.CreateTranslation(PipePairs[i].MovePosition, PipePairs[i].VerticalOffset, 0f));
            }
        }
    }
}
