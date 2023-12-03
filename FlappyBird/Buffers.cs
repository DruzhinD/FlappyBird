using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyBird
{
    internal class Buffers
    {
        /// <summary>
        /// сдвиг в буфере от начала массива
        /// </summary>
        int shift = 0;
        /// <summary>
        /// заглушка вместо массива 
        /// данных - все берем из буфера            
        /// </summary>
        int proxyArray = 0;
        /// <summary>
        /// размерность пространства точек
        /// </summary>
        int DimentionVertex = 2;
        /// <summary>
        /// размерность пространства цвета
        /// </summary>
        int DimentionColor = 3;
        /// <summary>
        /// ключевые индексы VBO
        /// </summary>
        int vboVertexID = -1, vboColorID = -1;

        /// <summary>
        /// массив вершин птички
        /// </summary>
        float[] vertexes;

        /// <summary>
        /// массив цветов вершин
        /// </summary>
        float[] vertexColors;

        /// <summary>
        /// Создание VBO буффера
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int CreateVBO(float[] data)
        {
            // id vbo буфера
            int vbo = GL.GenBuffer();
            // указание на начала работы по буферизации
            // данных для VBO буфера c id 
            // указание типа буфера:
            // BufferTarget.ArrayBuffer 
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                data.Length * sizeof(float), data,
                BufferUsageHint.StaticDraw);
            // завершение работы с буфером и отправка
            // данных в VBO в видеопамять
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            return vbo;
        }
        /// <summary>
        /// Создание буферов VBO
        /// Вызывается в OnLoad
        /// </summary>
        public void InitVBO()
        {
            //ВРЕМЕННЫЙ КОСТЫЛЬ Для птички
            //Draw();
            vboVertexID = CreateVBO(vertexes);
            vboColorID = CreateVBO(vertexColors);
        }

        /// <summary>
        /// Вызывается в OnUnLoad
        /// </summary>
        public void DelleteVBO()
        {
            GL.DeleteBuffer(vboVertexID);
            GL.DeleteBuffer(vboColorID);
        }

        /// <summary>
        /// Вызывается в OnRenderFrame
        /// </summary>
        public void DrawVBO()
        {
            // Включаем использование вершинного масивов
            GL.EnableClientState(ArrayCap.VertexArray);
            // Включаем использование вершинного
            // масивов цветов
            GL.EnableClientState(ArrayCap.ColorArray);

            // начинаем работать с буфером координат
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboVertexID);
            // Читаем данные из буферного массива
            GL.VertexPointer(DimentionVertex,
            VertexPointerType.Float, shift, proxyArray);

            // начинаем работать с буфером цвета
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboColorID);
            // Читаем данные из буферного массива цветов
            GL.ColorPointer(DimentionColor,
            ColorPointerType.Float, shift, proxyArray);
            // ОТРИСОВКА
            GL.DrawArrays(PrimitiveType.Polygon, 0, 4);
            // завершение работы с буферами
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Выключаем использование вершинного
            // массива координат
            GL.DisableClientState(ArrayCap.VertexArray);
            // Выключаем использование вершинного
            // массива цветов
            GL.DisableClientState(ArrayCap.ColorArray);
        }
    }
}
