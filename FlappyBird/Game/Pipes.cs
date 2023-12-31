﻿using System;
using OpenTK;
using FlappyBird.Engine;
using System.Threading;

namespace FlappyBird.Game
{
    /// <summary>все колонны</summary>
    class Pipes
    {
        private readonly Renderer _renderer;
        public PipePair[] PipePairs;

        /// <param name="renderer">экземпляр рендера</param>
        /// <param name="count">количество генерируемых колонн</param>
        /// <param name="offsetX">расстояние между разными парами колонн (ось X)</param>
        /// <param name="offsetY">расстояние между верхней и нижней частью колонны <br/>
        /// должно быть меньше 0.9f </param>
        public Pipes(Renderer renderer, int count, float offsetX, float offsetY)
        {
            _renderer = renderer;
            PipePairs = new PipePair[count];

            for (int i = 0; i < count; i++)
            {
                PipePairs[i] = new PipePair(_renderer, i * offsetX, offsetY);
                
                PipePairs[i].OffsetY = GenFloatNumber(PipePairs[i].ConstOffsetY);
                
                Thread.Sleep(1);
            }
        }

        /// <summary>скорость движения колонн в секунду</summary>
        public float PipeSpeedFrequency { get; private set; } = 0.5f;

        /// <summary>
        /// Движение колонн
        /// </summary>
        /// <param name="frameTime">длительность одного кадра</param>
        public void MovePipes(float frameTime)
        {
            for (int i = 0; i < PipePairs.Length; i++)
            {
                PipePairs[i].MovePosition -= PipeSpeedFrequency * frameTime; //0.5 по умолч

                //блок кода, необходимый для респауна колонн, при этом -2f - это расстояние между боковыми краями экрана
                //-2 из-за особенностей трансформации матрицами, на деле координата будет -1 -1*[ширина блока]
                if (PipePairs[i].MovePosition < -2f - 0.15f)
                {
                    PipePairs[i].MovePosition = 0f; //ось x
                     PipePairs[i].OffsetY = GenFloatNumber(PipePairs[i].ConstOffsetY); //ось y
                    //каждый раз, когда колонна перемещается на стартовую позицию, скорость её движения увеличивается
                    PipeSpeedFrequency += 0.005f;
                }
                _renderer.SetTransformRenderGroup(
                    PipePairs[i].Group, Matrix4.CreateTranslation(PipePairs[i].MovePosition, PipePairs[i].OffsetY, 0f));
            }
        }

        //генерирует число типа float, необходимое для задачи 
        private float GenFloatNumber(float offsetY)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            double returnNum;
            //делаем так, чтобы сгенерированное число было меньше константы по высоте
            do
            {
                returnNum = rnd.NextDouble();
            } while (returnNum > 1 - (Math.Abs(offsetY) + 0.05));

            //рандомно выбираем знак числа
            sbyte[] signNums = new sbyte[] { -1, 1 };
            returnNum *= signNums[rnd.Next(0, 1+1)];

            return (float)returnNum;
        }
    }
}
