using System;
using System.Threading;
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
        /// <summary>флаг запущенной игры, становится true при нажатии пробела</summary>
        private bool running = false;
        /// <summary>флаг, необходимый для смены режима окна с оконного на полноэкранный и наоборот <br/>
        /// без него создается множество окон</summary>
        private bool _screen = false;

        private Renderer _renderer;
        private Background _background; //отмечен как неиспользуемый, т.к. фон является статическим и неизменяется
        private Background _titlescreen;
        private Background _deathscreen;
        private Player _player;
        private ScoreTable _scoreTable;
        /// <summary>колонны</summary>
        private Pipes _pipes;

        private DisplayDevice _display;

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }

        protected override void OnLoad(EventArgs e)
        {
            VSync = VSyncMode.On;
            //создаем экземпляр, содержащий информацию о дисплее
            _display = DisplayDevice.GetDisplay(DisplayIndex.Default);
            //создание окна по центру экрана
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

            //ожидание запуска игры
            if (input.IsKeyDown(Key.Space) && running == false)
            {
                _renderer.RenderGroupVisible(_titlescreen.Group, false);
                running = true;
            }

            //при нажатии проблема - птичка делает взмах
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

            //изменение режима отображения окна
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

            //если игрок врезался, то игра завершается
            if (!_player.Alive)
            {
                _renderer.RenderGroupVisible(_deathscreen.Group, true);
                running = false;
            }

            //если игра окончена, то по нажатию enter можно начать её заново
            if (input.IsKeyDown(Key.Enter) && !running)
            {
                StartOrRestartGame(false);
            }

            //можно закрыть программу нажает клавиши
            if (input.IsKeyDown(Key.Escape))
                Exit();

            //вывод фпс
            if (DateTime.Now.Millisecond >= 990)
            {
                Console.WriteLine($"Кол-во кадров: {UpdateFrequency}"); //количество фпс
                Console.WriteLine($"Один кадр (с): {UpdatePeriod}"); //длительность одного кадра
            }

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

        /// <summary>
        /// запуск или перезапуск игры
        /// </summary>
        /// <param name="startFlag">true - первый запуск игры <br/>
        /// false - перезапуск игры</param>
        private void StartOrRestartGame(bool startFlag)
        {
            //очистка необходима только при перезапуске, т.к. нужно очистить уже существующие объекты
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
}
