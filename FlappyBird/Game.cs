using FlappyBird.GameObjects;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyBird
{
    internal class Game : GameWindow
    {
        Bird bird;
        bool downFlag = false;

        public Game(int winSizeX = 800, int winSizeY = 800)
            :base(winSizeX, winSizeY)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.Tan);

            bird = new Bird(new Vector2(0.0f, 0.0f), 0.2f, new Vector3(0.4f, 0.1f, 0.8f));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Key.Escape))
                Exit();
            
            if (keyState.IsKeyDown(Key.W) & downFlag == false)
            {
                bird.MoveUp();
                //downFlag = true;
            }
            else if (keyState.IsKeyUp(Key.W))
            {
                bird.MoveDown();
                downFlag = false;
            }
            else
            {
                bird.MoveDown();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            bird.Draw();

            SwapBuffers();
            
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }
    }
}
