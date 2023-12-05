using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace FlappyBird.Engine
{
    class Renderer
    {
        private List<RenderGroup> _renderGroups;

        private ShaderProgram _shader;

        private int _vao;
        private int _vbo;
        private int _ebo;

        unsafe public static int VertexSize = sizeof(Vertex);

        public Renderer()
        {
            _renderGroups = new List<RenderGroup>();

            GL.ClearColor(1.0f, 1f, 1.0f, 1.0f);

            //включаем возможность использования прозрачных текстур
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //инициализируем шейдеры и все текстуры
            _shader = new ShaderProgram(@"Shaders/shader.vert", @"Shaders/shader.frag");
            TextureLoader loader = new TextureLoader("../../Resources/resources.config");

            //создание VAO
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            //создаем VBO и задаем его размер, равный одной вершине
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexSize * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            //создаем EBO и задаем его размер, равный одному набору индексов
            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 6 * sizeof(uint), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            
            //указываем входные переменные для шейдеров
            //первый аргумент - layout (location=...)
            //для aPosition
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexSize, 0);

            //для aTexCoord
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexSize, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            //для aTexIndex
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, VertexSize, 5 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            _shader.RunProgram();

            //задаем единичную матрицу трансформации в качестве глобальной переменной шейдера (uniform)
            _shader.SetMatrix4("transform", Matrix4.Identity);
            
            //связываем индексы текстур с самими текстурами
            loader.UseTextures();
            _shader.SetIntArray("textures", loader.GetTextureIndicies());
        }
        public void Render()
        {
            foreach (RenderGroup group in _renderGroups)
            {
                if (!group.Visible)
                    continue;

                //сохраняем данные о прямоугольниках (объектах) в буферы
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, group.Rectangles.Count * 4 * VertexSize, group.Verticies, BufferUsageHint.DynamicDraw);


                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, group.Rectangles.Count * 6 * sizeof(uint), group.Indicies, BufferUsageHint.DynamicDraw);

                //трансформация прямоугольника
                _shader.SetMatrix4("transform", group.TransformationMatrix);

                //отрисовка
                GL.BindVertexArray(_vao);
                GL.DrawElements(BeginMode.Triangles, 6 * sizeof(uint), DrawElementsType.UnsignedInt, 0);
            }
        }

        /// <summary>
        /// Изменяем текущее состояние/положение объекта
        /// </summary>
        /// <param name="index">индекс буфера</param>
        /// <param name="transformationMatrix">матрица текущего состояния</param>
        public void SetTransformRenderGroup(int index, Matrix4 transformationMatrix)
        {
            _renderGroups[index].TransformationMatrix = transformationMatrix;
        }

        public int CreateRenderGroup()
        {
            RenderGroup renderGroup = new RenderGroup();
            _renderGroups.Add(renderGroup);
            return _renderGroups.IndexOf(renderGroup);
        }

        //вероятно добавляем прямоугольник в список буфера
        public void AddRectangleToGroup(int index, Rectangle rect)
        {
            //Console.Write(index + " ");
            _renderGroups[index].Rectangles.Add(rect);
        }

        public void RenderGroupVisible(int index, bool state)
        {
            _renderGroups[index].Visible = state;
        }

        public void ClearRenderGroup(int index)
        {
            _renderGroups[index].Rectangles.Clear();
        }
    }
}
