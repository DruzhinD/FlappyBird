using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Threading;
using OpenTK.Graphics.OpenGL4;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace FlappyBird.AllCode
{
    unsafe class Window : GameWindow
    {
        private bool running = false;
        private bool _screen = false;
        private Renderer _renderer;
        private Background _background;
        private Background _titlescreen;
        private Background _deathscreen;
        private Player _player;
        private ScoreTable _scoreTable;
        private Pipes _pipes;
        private DisplayDevice _display;
        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }
        protected override void OnLoad(EventArgs e)
        {
            CursorVisible = false;
            VSync = VSyncMode.On;
            _display = DisplayDevice.GetDisplay(DisplayIndex.Default);
            Location = new System.Drawing.Point((_display.Width - Width) / 2, (_display.Height - Height) / 2);
            StartOrRestartGame(true);
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (_player.Alive && running)
            {
                _pipes.MovePipes((float)e.Time);
                _player.MovePlayer((float)e.Time);
                _player.DetectCollision(ref _pipes, ref _scoreTable, (float)e.Time);
            }
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _renderer.Render();
            SwapBuffers();
            base.OnRenderFrame(e);
        }
        private byte jumpCounter = 0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();
            if (input.IsKeyDown(Key.Space) && running == false)
            {
                _renderer.RenderGroupVisible(_titlescreen.Group, false);
                running = true;
            }
            if (input.IsKeyDown(Key.Space) && running)
            {
                if (jumpCounter < 1)
                {
                    jumpCounter++;
                    _player.Jump();
                }
            }
            else if (input.IsKeyUp(Key.Space) && running)
            {
                jumpCounter = 0;
            }
            if (input.IsKeyDown(Key.F11) && _screen == false)
            {
                if (WindowState == WindowState.Fullscreen)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Fullscreen;

                _screen = true;
            }
            else if (input.IsKeyUp(Key.F11) && _screen == true)
            {
                _screen = false;
            }
            if (!_player.Alive)
            {
                _renderer.RenderGroupVisible(_deathscreen.Group, true);
                running = false;
            }
            if (input.IsKeyDown(Key.Enter) && !running)
            {
                StartOrRestartGame(false);
            }
            if (input.IsKeyDown(Key.Escape))
                Exit();
            base.OnUpdateFrame(e);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize(e);
        }
        protected override void OnUnload(EventArgs e)
        {
            _renderer.ClearAllRenderGroups();
            base.OnUnload(e);
        }
        private void StartOrRestartGame(bool startFlag)
        {
            if (startFlag != true)
                _renderer.ClearAllRenderGroups();

            _renderer = new Renderer();
            _background = new Background(_renderer, 10);
            _player = new Player(_renderer);
            _pipes = new Pipes(_renderer, 3, 0.76f, 0.31f);
            _scoreTable = new ScoreTable(_renderer);
            _titlescreen = new Background(_renderer, 13);
            _deathscreen = new Background(_renderer, 6f);
            _renderer.RenderGroupVisible(_deathscreen.Group, false);
        }
    }
    class Background
    {
        private Renderer _renderer;
        private Rectangle _rectangle;
        public int Group { get; }
        public Background(Renderer renderer, float textureIndex)
        {
            _renderer = renderer;
            _rectangle = new Rectangle(0f, 0f, 2f, 2f, textureIndex);
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, _rectangle);
        }
    }
    class PipePair
    {
        private readonly Renderer _renderer;
        public Rectangle RectangleBottom { get; }
        public Rectangle RectangleTop { get; }
        public float MovePosition { get; set; }
        public float OffsetY { get; set; }
        public float ConstOffsetY { get; }
        public int Group { get; }
        public PipePair(Renderer renderer, float offsetX, float offsetY)
        {
            _renderer = renderer;
            ConstOffsetY = offsetY;
            RectangleTop = new Rectangle(1f, 2 + ConstOffsetY, 0.15f, -2f, 12f, Rectangle.RectMode.Left);
            RectangleBottom = new Rectangle(1f, -2 - ConstOffsetY, 0.15f, 2f, 12f, Rectangle.RectMode.Left);
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, RectangleBottom);
            _renderer.AddRectangleToGroup(Group, RectangleTop);
            MovePosition = offsetX;
        }
    }
    class Pipes
    {
        private readonly Renderer _renderer;
        public PipePair[] PipePairs;
        public Pipes(Renderer renderer, int count, float offsetX, float offsetY)
        {
            _renderer = renderer;
            PipePairs = new PipePair[count];
            for (int i = 0; i < count; i++)
            {
                PipePairs[i] = new PipePair(_renderer, i * offsetX, offsetY);
                PipePairs[i].OffsetY = GenFloatNumber(PipePairs[i].ConstOffsetY);
                Thread.Sleep(1);
            }
        }
        public float PipeSpeedFrequency { get; private set; } = 0.5f;
        public void MovePipes(float frameTime)
        {
            for (int i = 0; i < PipePairs.Length; i++)
            {
                PipePairs[i].MovePosition -= PipeSpeedFrequency * frameTime;
                if (PipePairs[i].MovePosition < -2f - 0.25f)
                {
                    PipePairs[i].MovePosition = 0f;
                    PipePairs[i].OffsetY = GenFloatNumber(PipePairs[i].ConstOffsetY);
                    PipeSpeedFrequency += 0.005f;
                }
                _renderer.SetTransformRenderGroup(
                    PipePairs[i].Group, Matrix4.CreateTranslation(PipePairs[i].MovePosition, PipePairs[i].OffsetY, 0f));
            }
        }
        private float GenFloatNumber(float offsetY)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            double returnNum;
            do
            {
                returnNum = rnd.NextDouble();
            } while (returnNum > 1 - (Math.Abs(offsetY) + 0.05));
            sbyte[] signNums = new sbyte[] { -1, 1 };
            returnNum *= signNums[rnd.Next(0, 1 + 1)];
            return (float)returnNum;
        }
    }
    class Player
    {
        private readonly Renderer _renderer;
        public Rectangle Rect { get; }
        private float _velocity;
        private float _position;
        private float _angle;
        private float _height = 0.16f * 1.5f;
        private float _width = 0.09f * 1.5f;
        public int Group { get; }
        public int Score { get; private set; }
        public bool Alive { get; private set; }
        public Player(Renderer renderer)
        {
            _renderer = renderer;
            Rect = new Rectangle(0f, 0f, _width, _height, 11f); 
            Group = _renderer.CreateRenderGroup();
            _renderer.AddRectangleToGroup(Group, Rect);
            Alive = true;
        }
        public void MovePlayer(float frameTime)
        {
            if (_velocity < 3f)
                _velocity -= 0.07f * frameTime;
            _position += _velocity;
            if (_velocity < 0 && _angle > -10)
                _angle -= 1;
            if (_velocity >= 0.001f && _angle < 9)
                _angle += 1;
            Matrix4 transform = Matrix4.Identity;
            transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_angle));
            transform *= Matrix4.CreateTranslation(0f, _position, 0f);   
            _renderer.SetTransformRenderGroup(Group, transform);
        }
        public void DetectCollision(ref Pipes pipes, ref ScoreTable scoreTable, float frameTime)
        {
            if (this._position > 1f - _height / 3 || this._position < -1f + _height / 3)
            {
                Alive = false;
                ChangeTexture();
            }

            foreach (PipePair pair in pipes.PipePairs)
            {
                if (_position > pair.OffsetY + pair.ConstOffsetY - this._height / 2.5f &&
                    pair.MovePosition < -1f + this._width / 2f &&
                    pair.MovePosition + 0.15f > -1f - this._width / 2f)
                {
                    Alive = false;
                    ChangeTexture();
                    break;
                }
                if (_position < pair.OffsetY - pair.ConstOffsetY + this._height / 2.5f &&
                    pair.MovePosition < -1f + this._width / 2f &&
                    pair.MovePosition + 0.15f > -1f - this._width / 2f)
                {
                    Alive = false;
                    ChangeTexture();
                    break;
                }
                if (pair.MovePosition < -1f && pair.MovePosition > -1f - pipes.PipeSpeedFrequency * frameTime)
                {
                    Score++;
                    scoreTable.ChangeScoreTable(Score);
                }
            }
            void ChangeTexture()
            {
                for (int i = 5; i <= 23; i += 6)
                {
                    this.Rect.Verticies[i] = 12f;
                }
            }
        }
        public void Jump()
        {
            if (_position == -1f)
                _position = -0.999f;
            _velocity = 0.025f;
            _angle += 1;
        }
    }
    class ScoreTable
    {
        private readonly Renderer _renderer;
        public Dictionary<Rectangle, int> Rectangles { get; private set; }
        private float _height = 0.16f * 1f;
        private float _width = 0.09f * 1f;
        private float currentOffsetX = 0f;
        public ScoreTable(Renderer renderer)
        {
            _renderer = renderer;
            KeyValuePair<Rectangle, int> currentRect = GenNewRectangle();
            Rectangles = new Dictionary<Rectangle, int>()
            {
                { currentRect.Key, currentRect.Value },
            };
        }
        private KeyValuePair<Rectangle, int> GenNewRectangle()
        {
            int Group = _renderer.CreateRenderGroup();
            KeyValuePair<Rectangle, int> currentRect = new KeyValuePair<Rectangle, int>(
                new Rectangle(-1f + currentOffsetX, -1f, _width, _height, 0f, Rectangle.RectMode.Left), Group);
            currentOffsetX += _width;
            _renderer.AddRectangleToGroup(currentRect.Value, currentRect.Key);
            return currentRect;
        }
        public void ChangeScoreTable(int score)
        {
            string stringScore = score.ToString();
            if (Rectangles.Count != stringScore.Length)
            {
                KeyValuePair<Rectangle, int> currentRect = GenNewRectangle();
                Rectangles.Add(currentRect.Key, currentRect.Value);
            }
            int numPosition = 0;
            foreach (var RectPair in Rectangles)
            {
                for (int i = 5; i <= 23; i += 6)
                    RectPair.Key.Verticies[i] = float.Parse(stringScore[numPosition].ToString());
                numPosition++;
            }
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
        public enum RectMode
        {
            Center, 
            Left 
        }
        public Rectangle(float posX, float posY, float width, float height, float textureIndex, RectMode mode = RectMode.Center)
        {
            if (mode == RectMode.Center)
            {
                float[] verticies =
                {
                    
                    posX - width/2, posY - height/2, 0.0f,    0.0f, 1.0f, textureIndex, 
                    posX - width/2, posY + height/2, 0.0f,    0.0f, 0.0f, textureIndex, 
                    posX + width/2, posY - height/2, 0.0f,    1.0f, 1.0f, textureIndex, 
                    posX + width/2, posY + height/2, 0.0f,    1.0f, 0.0f, textureIndex 
                };

                this.Verticies = verticies;
            }
            else if (mode == RectMode.Left)
            {
                float[] verticies =
                {   
                    posX, posY, 0.0f,                         0.0f, 1.0f,   textureIndex, 
                    posX, posY + height, 0.0f,                0.0f, 0.0f,   textureIndex, 
                    posX + width, posY, 0.0f,                 1.0f, 1.0f,   textureIndex, 
                    posX + width, posY + height, 0.0f,        1.0f, 0.0f,   textureIndex 
                 };

                this.Verticies = verticies;
            }
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
    class Renderer
    {
        public List<RenderGroup> RenderGroups { get; private set; }
        private ShaderProgram _shader;
        private int _vao;
        private int _vbo;
        private int _ebo;
        unsafe public static int VertexSize = sizeof(Vertex);
        public Renderer()
        {
            RenderGroups = new List<RenderGroup>();
            GL.ClearColor(1.0f, 1f, 1.0f, 1.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _shader = new ShaderProgram(@"Shaders/shader.vert", @"Shaders/shader.frag");
            TextureLoader loader = new TextureLoader("../../Resources/resources.txt");
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexSize * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 6 * sizeof(uint), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexSize, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexSize, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, VertexSize, 5 * sizeof(float));
            GL.EnableVertexAttribArray(2);
            _shader.RunProgram();
            _shader.SetMatrix4("transform", Matrix4.Identity);   
            loader.UseTextures();
            _shader.SetIntArray("textures", loader.GetTextureIndicies());
        }
        public void Render()
        {
            foreach (RenderGroup group in RenderGroups)
            {
                if (!group.Visible)
                    continue;
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, group.Rectangles.Count * 4 * VertexSize, group.Verticies, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, group.Rectangles.Count * 6 * sizeof(uint), group.Indicies, BufferUsageHint.DynamicDraw);
                _shader.SetMatrix4("transform", group.TransformationMatrix);
                GL.BindVertexArray(_vao);
                GL.DrawElements(BeginMode.Triangles, 6 * sizeof(uint), DrawElementsType.UnsignedInt, 0);
            }
        }
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
        public void AddRectangleToGroup(int index, Rectangle rect)
        {
            
            RenderGroups[index].Rectangles.Add(rect);
        }
        public void RenderGroupVisible(int index, bool state)
        {
            RenderGroups[index].Visible = state;
        }
        public void ClearRenderGroup(int index)
        {
            RenderGroups[index].Rectangles.Clear();
        }
        public void ClearAllRenderGroups()
        {
            if (RenderGroups != null)
                for (int i = 0; i < RenderGroups.Count; i++)
                    RenderGroups[i].Rectangles.Clear();
            _shader.ExitProgram();
        }
    }
    unsafe struct Vertex
    {
        public fixed float Position[3];
        public fixed float TexCoords[2];
        public float TextureIndex;
    }
    internal class ShaderProgram
    {
        private readonly int _vertexShaderID;
        private readonly int _fragmentShaderID;
        private readonly int _program;
        public ShaderProgram(string vertShaderPath, string fragShaderPath)
        {
            _vertexShaderID = CreateShader(ShaderType.VertexShader, vertShaderPath);
            _fragmentShaderID = CreateShader(ShaderType.FragmentShader, fragShaderPath);
            _program = GL.CreateProgram();
            GL.AttachShader(_program, _vertexShaderID);
            GL.AttachShader(_program, _fragmentShaderID);
            GL.LinkProgram(_program);
            GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out int code);
            if (code != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(_program);
                throw new Exception($"Ошибка компиляции шейдерной программы \n {infoLog}");
            }   
            DeleteShader(_vertexShaderID);
            DeleteShader(_fragmentShaderID);
        }
        public void RunProgram()
        {
            GL.UseProgram(_program);
        }
        public void ExitProgram()
        {
            GL.UseProgram(0);   
            GL.DeleteProgram(_program);
        }
        private int CreateShader(ShaderType shaderType, string shaderFilePath)
        {
            string shaderString = File.ReadAllText(shaderFilePath);
            int shaderID = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderID, shaderString);
            GL.CompileShader(shaderID);
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out int code);
            if (code != (int)All.True)
            {   
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
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(_program);
            int location = GL.GetUniformLocation(_program, name);
            GL.UniformMatrix4(location, true, ref data); 
        }
        public void SetIntArray(string name, int[] data)
        {
            GL.UseProgram(_program);
            int location = GL.GetUniformLocation(_program, name);
            GL.Uniform1(location, data.Length, data);
        }
    }
    class Texture
    {
        public readonly int program;
        public Texture(string path)
        {
            program = GL.GenTexture();
            Use();
            using (Bitmap image = new Bitmap(path))
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
            GL.BindTexture(TextureTarget.Texture2D, program);
        }
    }
    class TextureLoader
    {
        private string _directoryPath;
        private string[] _texturePaths;
        private Texture[] _textures;
        public TextureLoader(string configFilePath)
        {
            _directoryPath = Path.GetDirectoryName(configFilePath);
            string configFile = LoadSource(configFilePath);
            _texturePaths = configFile.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < _texturePaths.Length; i++)
            {
                _texturePaths[i] = _directoryPath + @"\" + _texturePaths[i];
            }
            if (_texturePaths.Length > 32)
                throw new Exception("Слишком много текстур для загрузки");
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
        private string LoadSource(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }
    }
}