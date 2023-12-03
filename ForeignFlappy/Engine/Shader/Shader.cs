using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.IO;

namespace FlappyBird.Engine
{
    class Shader
    {
        public readonly int Handle;
        public Shader(string vertShaderPath, string fragShaderPath)
        {
            //load, create and compile vertexshader
            string ShaderSource = LoadSource(vertShaderPath);
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, ShaderSource);
            CompileShader(vertexShader);

            //load, create and compile fragmentshader
            ShaderSource = LoadSource(fragShaderPath);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, ShaderSource);
            CompileShader(fragmentShader);

            //create and link final Program/
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            LinkProgram(Handle);

            //cleanup
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public int GetAttributeLocation(string name)
        {
            return GL.GetAttribLocation(Handle, name);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, true, ref data);
        }

        public void SetIntArray(string name, int[] data)
        {
            GL.UseProgram(Handle);
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, data.Length, data);
        }

        private void LinkProgram(int program)
        { 
            //link the program
            GL.LinkProgram(program);

            //error checking
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        private void CompileShader(int shader)
        {
            //compile the shader
            GL.CompileShader(shader);

            //error checking
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int code);
            if (code != (int)All.True)
            {
                string infolog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occured whilst compiling shader({shader}).\n\n{infolog}");
            }
        }

        private string LoadSource(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
