using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyBird.GameObjects
{
    internal class Bird : Entity
    {
        /// <summary>
        /// размер птицы
        /// </summary>
        float size = 0.5f;
        /// <summary>
        /// массив вершин птички
        /// </summary>
        protected Vector2[] vertexes = new Vector2[4];

        /// <summary>
        /// гравитация для птички
        /// </summary>
        float gravity = 0.01f;

        public Bird(Vector2 position, float size, Vector3 color)
            :base(position, color)

        {
            this.size = size;
        }

        public override void Draw()
        {
            //половина длины диагонали квадрата (птицы),
            //т.к. создание вершин происходит относительно центра квадрата
            float halfDiagonal = size * (float)Math.Sqrt(2) / 2;
            //создание вершин птицы
            for (int i = 0; i < vertexes.Length; i++)
            {
                //от правой нижней вершины до левой нижней
                //обход против часовой стрелки
                vertexes[i].X = (float)(position.X + halfDiagonal * Math.Cos(i * 2 * Math.PI / 4 - Math.PI / 4));
                vertexes[i].Y = (float)(position.Y + halfDiagonal * Math.Sin(i * 2 * Math.PI / 4 - Math.PI / 4));
            }

            GL.Begin(PrimitiveType.Quads);
                GL.Color3(Color);
                foreach (var vertex in vertexes)
                    GL.Vertex2(vertex);
            GL.End();
        }

        public void MoveUp()
        {
            position.Y += gravity * 20;
            if (position.Y >= 1.0f - size) 
            {
                position.Y = 1.0f - size;
            }

        }

        public void MoveDown()
        {
            position.Y -= gravity;

            if (position.Y <= -1.0f + size)
            {
                position.Y = -1.0f + size;
            }
        }
    }
}
