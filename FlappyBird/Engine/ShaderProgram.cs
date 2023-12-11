using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace FlappyBird.Engine
{
    /// <summary>
    /// Создание шейдерной программы
    /// </summary>
    internal class ShaderProgram
    {
        private readonly int _vertexShaderID;
        private readonly int _fragmentShaderID;
        private readonly int _program;

        public ShaderProgram(string vertShaderPath, string fragShaderPath)
        {
            _vertexShaderID = CreateShader(ShaderType.VertexShader, vertShaderPath);
            _fragmentShaderID = CreateShader(ShaderType.FragmentShader, fragShaderPath);

            //создание шейдерной программы
            _program = GL.CreateProgram();

            //связываем шейдерную программу с созданными шейдерами
            GL.AttachShader(_program, _vertexShaderID);
            GL.AttachShader(_program, _fragmentShaderID);

            //компилируем шейдерную программу
            GL.LinkProgram(_program);

            //для отладки, в случае ошибки
            GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out int code);
            if (code != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(_program);
                throw new Exception($"Ошибка компиляции шейдерной программы \n {infoLog}");
            }

            //удаляем шейдеры, использованные для компиляции
            DeleteShader(_vertexShaderID);
            DeleteShader(_fragmentShaderID);
        }

        /// <summary>
        /// Запускаем шейдерную программу
        /// </summary>
        public void RunProgram()
        {
            GL.UseProgram(_program);
        }

        /// <summary>
        /// Завершение работы шейдерной программы
        /// </summary>
        public void ExitProgram()
        {
            //выключаем шейдерную программу
            GL.UseProgram(0);

            //удаляем шейдерную программу
            GL.DeleteProgram(_program);
        }

        //создание шейдера
        private int CreateShader(ShaderType shaderType, string shaderFilePath)
        {
            //считываем содержимое шейдера в строку
            string shaderString = File.ReadAllText(shaderFilePath);
            //создаем шейдер
            int shaderID = GL.CreateShader(shaderType);
            //привязываем программный код шейдера к созданному шейдеру
            GL.ShaderSource(shaderID, shaderString);
            //компилируем
            GL.CompileShader(shaderID);

            //для отладки, в случае ошибки
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out int code);
            if (code != (int)All.True)
            {
                //информация о том, в какой строке шейдера вылезла ошибка
                string infoLog = GL.GetShaderInfoLog(shaderID);
                throw new Exception($"Ошибка компиляции шейдера №{shaderID} \n {infoLog}");
            }

            return shaderID;
        }

        private void DeleteShader(int shaderID)
        {
            GL.DetachShader(_program, shaderID);
            GL.DeleteShader(shaderID);
        }

        //для матрицы преобразований
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(_program);
                   int location = GL.GetUniformLocation(_program, name);
            GL.UniformMatrix4(location, true, ref data); //uniform для GLSL
        }

        //для текстур
        public void SetIntArray(string name, int[] data)
        {
            GL.UseProgram(_program);
            int location = GL.GetUniformLocation(_program, name);
            GL.Uniform1(location, data.Length, data);
        }
    }
}
