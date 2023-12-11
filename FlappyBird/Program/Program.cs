using System;

namespace FlappyBird.Program
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            using (Window win = new Window(16*70, 9*70, "Flappy Bird"))
            {
                win.Run();
            }
        }
    }
}
