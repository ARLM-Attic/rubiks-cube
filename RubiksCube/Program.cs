using System;

namespace RubiksCubeWindows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RubiksCubeGame game = new RubiksCubeGame())
            {
                game.Run();
            }
        }
    }
}

