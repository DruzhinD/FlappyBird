using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlappyBird.Game;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace EmptyNS
{
    unsafe class Window : GameWindow
    {
        private bool _screen = false;
        private bool running = false; //по сути нужен для запуска игры, то есть ожидает нажатия пробела

        private Renderer _renderer;
        private Background _background; //отмечен как неиспользуемый из-за, т.к. фон является статическим и неизменяется
        private Player _player;
        /// <summary>колонны</summary>
        private Pipes _pipes;
        private int _titlescreen;
        private int _deathscreen;

        public int Score { get { return _player.Score; } }

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }

        protected override void OnLoad(EventArgs e)
        {
            //CursorVisible = false;
            _renderer = new Renderer();
            _background = new Background(_renderer);
            _player = new Player(_renderer);
            _pipes = new Pipes(_renderer, 3, 0.74f);

            _titlescreen = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(_titlescreen, new Rectangle(0f, 0f, 2f, 2f, 3f));

            _deathscreen = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(_deathscreen, new Rectangle(0f, 0f, 2f, 2f, 4f));
            _renderer.RenderGroupVisible(_deathscreen, false);

            //WindowState = WindowState.Fullscreen;

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (_player.Alive && running)
            {
                _pipes.MovePipes((float)e.Time);
                _player.MovePlayer((float)e.Time);
                _player.DetectCollision(_pipes);
            }

            GL.Clear(ClearBufferMask.ColorBufferBit);
            _renderer.Render();
            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Space) && running)
                _player.Jump();

            if (input.IsKeyDown(Key.Space) && running == false)
            {
                _renderer.RenderGroupVisible(_titlescreen, false);
                running = true;
            }

            if (input.IsKeyDown(Key.Escape))
                Exit();

            if (input.IsKeyDown(Key.F11) && _screen == false)
            {
                if (WindowState == WindowState.Fullscreen)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Fullscreen;

                _screen = true;
            }

            if (input.IsKeyUp(Key.F11) && _screen == true)
            {
                _screen = false;
            }

            if (!_player.Alive)
            {
                _renderer.RenderGroupVisible(_deathscreen, true);
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize(e);
        }
    }

    class Player
    {
        private Renderer _renderer;
        private Rectangle _rectangle;

        private float _velocity;
        private float _position;
        private float _angle;
        private float _height = 0.3f;
        private float _width = 0.2f;

        public int Group { get; }
        public int Score { get; private set; }
        public bool Alive { get; private set; }

        public Player(Renderer renderer)
        {
            _renderer = renderer;
            _rectangle = new Rectangle(0f, 0f, _width, _height, 1f);
            //сохраняем индекс созданного буфера
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, _rectangle);

            Alive = true;
        }

        //etime - время одного кадра, при 75гц - 0,0134с
        public void MovePlayer(float eTime)
        {
            if (_velocity < 3f)
                _velocity -= 0.07f * eTime; //для 75гц = 0,0094

            _position += _velocity;
            Console.WriteLine($"position: {Math.Round(_position, 2)}    velocity: {_velocity}");
            Console.WriteLine($"angle: {_angle}");
            //птица опускает клюв, если она начинает падать 
            if (_velocity < 0 && _angle > -5)
                _angle -= 1;
            //птица поднимает клюв, когда начинает взлетать
            if (_velocity > 0.02f && _angle < 9)
                _angle += 1;
            Matrix4 transform = Matrix4.Identity;
            transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_angle));
            transform *= Matrix4.CreateTranslation(0f, _position, 0f);

            //передаем через Group индекс буфера и матрицу "состояния"
            _renderer.SetTransformRenderGroup(Group, transform);
        }

        public void DetectCollision(Pipes pipes)
        {
            //top/bottom colision
            if (this._position > 1f - _height / 3 || this._position < -1f + _height / 3)
                Alive = false;

            foreach (PipePair pair in pipes.PipePairs)
            {
                //top pipe collision
                if (_position > pair.HorizontalOffset + 0.4f - _height / 3 && pair.MovePosition < -1f + _width / 3 && pair.MovePosition > -1f + _width / 3 - 0.25f)
                {
                    Alive = false;
                    break;
                }

                //bottom pipe collision
                if (_position < pair.HorizontalOffset - 0.4f + _height / 3 && pair.MovePosition < -1f + _width / 3 && pair.MovePosition > -1f + _width / 3 - 0.25f)
                {
                    Alive = false;
                    break;
                }

                if (pair.MovePosition < -1f && pair.MovePosition > -1.005f)
                {
                    Score++;
                }
            }
        }

        public void Jump()
        {
            if (_position == -1f)
                _position = -0.999f;
            _velocity = 0.03f;
            Console.WriteLine("--ПРОБЕЛ--");
            //_angle = 6;
        }
    }

    class Pipes
    {
        private Renderer _renderer;
        public PipePair[] PipePairs;

        private float _offset;

        public Pipes(Renderer renderer, int count, float offset)
        {
            _renderer = renderer;
            PipePairs = new PipePair[count];
            _offset = offset;

            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                PipePairs[i] = new PipePair(_renderer, i * offset);
                PipePairs[i].HorizontalOffset = (float)rand.Next(-4, 4) / 10;
            }
        }

        public void MovePipes(float eTime)
        {
            for (int i = 0; i < PipePairs.Length; i++)
            {
                PipePairs[i].MovePosition -= 0.5f * eTime; //0.5 по умолч

                if (PipePairs[i].MovePosition < -2f - 0.25f)
                {
                    PipePairs[i].MovePosition = 0f;
                    Random rand = new Random();
                    PipePairs[i].HorizontalOffset = (float)rand.Next(-4, 4) / 10;
                }

                _renderer.SetTransformRenderGroup(PipePairs[i].Group, Matrix4.CreateTranslation(PipePairs[i].MovePosition, PipePairs[i].HorizontalOffset, 0f));
            }
        }
    }

    class PipePair
    {
        private Renderer _renderer;
        private Rectangle _rectangle0;
        private Rectangle _rectangle1;

        public float MovePosition { get; set; }
        public float HorizontalOffset { get; set; }
        public float Offset { get; }

        public int Group { get; }

        public PipePair(Renderer renderer, float offset)
        {
            _renderer = renderer;
            _rectangle0 = new Rectangle(1f, -3.4f, 0.25f, 3f, 2f, Rectangle.RectMode.Left);
            _rectangle1 = new Rectangle(1f, 3.4f, 0.25f, -3f, 2f, Rectangle.RectMode.Left);

            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, _rectangle0);
            _renderer.AddRectangleToGroup(Group, _rectangle1);

            Offset = offset;
            MovePosition = offset;
        }
    }

    class Rectangle
    {
        public float[] Verticies { get; set; }

        public uint[] Indicies { get; set; } =
        {
            0, 1, 2,
            1, 2, 3
        };

        public float PosX { get; }
        public float PosY { get; }
        public float Width { get; }
        public float Height { get; }
        public float TextureIndex { get; }

        public enum RectMode
        {
            Center,
            Left
        }

        public Rectangle(float posX, float posY, float width, float height, float textureIndex, RectMode mode = RectMode.Center)
        {
            PosX = posX;
            PosY = posY;
            Width = width;
            Height = height;
            TextureIndex = textureIndex;

            if (mode == RectMode.Center)
            {
                float[] verticies =
                {
                    posX - width/2, posY - height/2, 0.0f,    0.0f, 1.0f, textureIndex, //bottom left
                    posX - width/2, posY + height/2, 0.0f,    0.0f, 0.0f, textureIndex, //top left
                    posX + width/2, posY - height/2, 0.0f,    1.0f, 1.0f, textureIndex, //bottom right
                    posX + width/2, posY + height/2, 0.0f,    1.0f, 0.0f, textureIndex //top right
                };

                this.Verticies = verticies;
            }
            else if (mode == RectMode.Left)
            {
                float[] verticies =
                {   //position                                texture coords
                    posX, posY, 0.0f,                         0.0f, 1.0f,   textureIndex, //bottom left
                    posX, posY + height, 0.0f,                0.0f, 0.0f,   textureIndex, //top left
                    posX + width, posY, 0.0f,                 1.0f, 1.0f,   textureIndex, //bottom right
                    posX + width, posY + height, 0.0f,        1.0f, 0.0f,   textureIndex //top right
                 };

                this.Verticies = verticies;
            }
        }
    }

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

                foreach (Rectangle rect in Rectangles)
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

    class Background
    {
        private Renderer _renderer;
        private Rectangle _rectangle;

        public int Group { get; }

        public Background(Renderer renderer)
        {
            _renderer = renderer;
            _rectangle = new Rectangle(0f, 0f, 2f, 2f, 0f);
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, _rectangle);
        }
    }

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

    class Texture
    {
        public readonly int Handle;

        public Texture(string path)
        {
            Handle = GL.GenTexture();

            Use();

            using (System.Drawing.Bitmap image = new System.Drawing.Bitmap(path))
            {
                var data = image.LockBits(
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    image.Width,
                    image.Height,
                    0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }

    class TextureLoader
    {
        private string _directoryPath;
        private string[] _texturePaths;
        private Texture[] _textures;

        public TextureLoader(string configFilePath)
        {
            _directoryPath = CropAfterLastBackSlash(configFilePath);

            string configFile = LoadSource(configFilePath);
            _texturePaths = configFile.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < _texturePaths.Length; i++)
            {
                _texturePaths[i] = _directoryPath + _texturePaths[i];
            }

            if (_texturePaths.Length > 32)
                throw new Exception("Too many Textures");

            _textures = new Texture[_texturePaths.Length];

            for (int i = 0; i < _textures.Length; i++)
                _textures[i] = new Texture(_texturePaths[i]);
        }

        public void UseTextures()
        {
            var units = Enum.GetValues(typeof(TextureUnit));
            for (int i = 0; i < _textures.Length; i++)
            {
                _textures[i].Use((TextureUnit)units.GetValue(i));
            }
        }

        public int[] GetTextureIndicies()
        {
            int[] indicies = new int[_textures.Length];
            for (int i = 0; i < indicies.Length; i++)
                indicies[i] = i;

            return indicies;
        }

        private string CropAfterLastBackSlash(string path)
        {
            int index = path.LastIndexOf('/') + 1;
            return path.Substring(0, index);
        }

        private string LoadSource(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }
    }

    unsafe struct Vertex
    {
        public fixed float Position[3];
        public fixed float TexCoords[2];
        public float TextureIndex;
    }
}
