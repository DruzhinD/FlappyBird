﻿using OpenTK;
using System;
using System.Collections.Generic;

namespace FlappyBird.Engine
{
    /// <summary>группа объектов (прямоугольников) для обработки</summary>
    class RenderGroup
    {
        //в случае false объект игнорируется, и как следствие не отрисовывается на экран
        public bool Visible;

        public List<Rectangle> Rectangles { get; set; }

        public Matrix4 TransformationMatrix { get; set; } = Matrix4.Identity;

        public float[] Verticies
        {
            get
            {
                float[] temparray = new float[0];
                int length = 0;
                foreach (Rectangle rect in Rectangles)
                {
                    length += rect.Verticies.Length;
                    Array.Resize<float>(ref temparray, length);

                    rect.Verticies.CopyTo(temparray, length - Renderer.VertexSize);
                }
                return temparray;
            }
        }

        public uint[] Indicies
        {
            get
            {
                uint[] temparry = new uint[0];
                int length = 0;
                uint counter = 0;

                foreach(Rectangle rect in Rectangles)
                {
                    //умножение массива
                    uint[] multarray = new uint[rect.Indicies.Length];
                    for (int i = 0; i < rect.Indicies.Length; i++)
                        multarray[i] = rect.Indicies[i] + counter * 4;

                    length += rect.Indicies.Length;
                    Array.Resize(ref temparry, length);
                    multarray.CopyTo(temparry, length - 6);

                    counter++;
                }
                return temparry;
            }
        }

        public RenderGroup(bool visible = true)
        {
            Rectangles = new List<Rectangle>();
            Visible = visible;
        }
    }
}
