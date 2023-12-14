using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace FlappyBird.Engine
{
    /// <summary>Класс, необходимый для обработки всех объектов и вывода их на экран</summary>
    class Renderer
    {
        public List<RenderGroup> RenderGroups { get; private set; }

        private ShaderProgram _shader;

        private int _vao;
        private int _vbo;
        private int _ebo;

        /// <summary>Размер одной вершины в байтах</summary>
        unsafe public static int VertexSize = sizeof(Vertex); //24

        public Renderer()
        {
            RenderGroups = new List<RenderGroup>();

            GL.ClearColor(1.0f, 1f, 1.0f, 1.0f);

            //включаем возможность использования прозрачных текстур
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //инициализируем шейдеры и все текстуры
            _shader = new ShaderProgram(@"../../Shaders/shader.vert", @"../../Shaders/shader.frag");
            TextureLoader loader = new TextureLoader(@"../../Resources/resources.txt");

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

            Console.WriteLine($"vao: {_vao}     vbo: {_vbo}     ebo: {_ebo}");
            //указываем входные переменные для шейдеров
            //первый аргумент - layout (location=...)
            //для aPosition
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexSize, 0);
            GL.EnableVertexAttribArray(0);

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

        /// <summary>обработка объектов, совершается каждый кадр</summary>
        public void Render()
        {
            foreach (RenderGroup group in RenderGroups)
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
            RenderGroups[index].TransformationMatrix = transformationMatrix;
        }

        public int CreateRenderGroup()
        {
            RenderGroup renderGroup = new RenderGroup();
            RenderGroups.Add(renderGroup);
            return RenderGroups.IndexOf(renderGroup);
        }

        //добавляем прямоугольник в список объектов для отрисовки/манипуляций
        public void AddRectangleToGroup(int index, Rectangle rect)
        {
            RenderGroups[index].Rectangles.Add(rect);
        }

        public void RenderGroupVisible(int index, bool state)
        {
            RenderGroups[index].Visible = state;
        }

        /// <summary>
        /// удаление объекта из списка объектов для отрисовки
        /// </summary>
        /// <param name="index">индекс буфера</param>
        public void ClearRenderGroup(int index)
        {
            RenderGroups[index].Rectangles.Clear();
        }

        /// <summary>
        /// очищение всего списка объектов для отрисовки
        /// </summary>
        public void ClearAllRenderGroups()
        {
            if (RenderGroups != null)
                for (int i = 0; i < RenderGroups.Count; i++)
                    RenderGroups[i].Rectangles.Clear();
            _shader.ExitProgram();

            //удаление буферов
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
        }
    }
}
