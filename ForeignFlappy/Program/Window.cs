using System;
using System.ComponentModel;
using System.Media;
using FlappyBird.Engine;
using FlappyBird.Game;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace FlappyBird.Program
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
            _renderer = new Engine.Renderer();
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
}
