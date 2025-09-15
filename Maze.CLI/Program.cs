using System;
using Maze.CLI.Classes;

class Program
{
    static void Main(string[] args)
    {

        int width = 10;
        int height = 10;

        MazeGame game = new MazeGame(width, height);
        game.StartGame();
    }
}
