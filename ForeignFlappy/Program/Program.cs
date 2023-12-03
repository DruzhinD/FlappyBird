using System;

namespace FlappyBird.Program
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            using (Window win = new Window(1000, 500, "Flappy Bird"))
            {
                win.Run();
                Console.WriteLine(win.Score);
            }
            Console.ReadKey();
        }
    }
}
