using System;

namespace RubikCube
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RubiksCube.RubiksCubeGame game = new RubiksCube.RubiksCubeGame())
            {
                game.Run();
            }
        }
    }
}

