using OpenTK;
using System;
using System.Collections.Generic;

namespace FlappyBird.Engine
{
    class RenderGroup
    {
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
                    //multiply array
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
