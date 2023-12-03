using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace FlappyBird.Engine
{
    class Renderer
    {
        private List<RenderGroup> _renderGroups;

        private Shader _shader;

        private int _vao;
        private int _vbo;
        private int _ebo;

        unsafe public static int VertexSize = sizeof(Vertex);

        public Renderer()
        {
            //Console.WriteLine(VertexSize);
            _renderGroups = new List<RenderGroup>();

            //set clear color to some bluish color
            GL.ClearColor(1.0f, 1f, 1.0f, 1.0f);

            //enables opengl to use transparent textures
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //initialises the shader and all textures
            _shader = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");
            TextureLoader loader = new TextureLoader("../../Resources/resources.config");

            //creates our vertex array object
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            //creates the vbo and sets its size to the size of one vertex
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexSize * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            //creates the ebo and sets its size to one set od indicies
            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 6 * sizeof(uint), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            //sets the shaders inputs
            //for aPosition
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexSize, 0);
            GL.EnableVertexAttribArray(0);

            //for aTexCoord
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexSize, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            //for aTexIndex
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, VertexSize, 5 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            _shader.Use();

            //setting the deafult transformation uniform
            _shader.SetMatrix4("transform", Matrix4.Identity);

            //assigns the texture index to the textures
            loader.UseTextures();
            _shader.SetIntArray("textures", loader.GetTextureIndicies());
        }
        public void Render()
        {
            foreach (RenderGroup group in _renderGroups)
            {
                if (!group.Visible)
                    continue;

                //store the data of the rectangle in the buffers
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, group.Rectangles.Count * 4 * VertexSize, group.Verticies, BufferUsageHint.DynamicDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, group.Rectangles.Count * 6 * sizeof(uint), group.Indicies, BufferUsageHint.DynamicDraw);

                //transforming the rectangle
                _shader.SetMatrix4("transform", group.TransformationMatrix);

                //drawing
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
