using System;

namespace Maze.CLI.Classes
{
    public class MazeGame
    {
        private EllerMaze maze;

        public MazeGame(int width, int height)
        {
            maze = new EllerMaze(width, height);
            maze.Generate();
        }

        public void StartGame()
        {
            Console.WriteLine("Добро пожаловать в лабиринт!");
            Console.WriteLine("Управление: стрелки для движения");
            Console.WriteLine("Цель: дойти до E (выход) c помощью P (игрок)");
            Console.WriteLine("Нажмите любую клавишу для начала...");
            Console.ReadKey();

            while (!maze.IsGameCompleted)
            {
                maze.PrintMaze();

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        maze.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        maze.MoveDown();
                        break;
                    case ConsoleKey.LeftArrow:
                        maze.MoveLeft();
                        break;
                    case ConsoleKey.RightArrow:
                        maze.MoveRight();
                        break;
                    case ConsoleKey.Escape:
                        Console.WriteLine("Игра завершена.");
                        return;
                }
            }

            maze.PrintMaze();
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
