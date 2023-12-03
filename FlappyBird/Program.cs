using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyBird
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            double fps = 75;
            game.VSync = OpenTK.VSyncMode.On;
            game.Run(fps);
        }
    }
}
