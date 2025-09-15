using System;
using System.Collections.Generic;

namespace Maze.CLI.Classes
{
    public class EllerMaze
    {
        private Cell[,] grid;
        private int width;
        private int height;
        private Random random;

        // Позиция игрока
        public int PlayerX { get; private set; }
        public int PlayerY { get; private set; }

        // Позиция выхода
        public int ExitX { get; private set; }
        public int ExitY { get; private set; }

        public bool IsGameCompleted { get; private set; }

        public EllerMaze(int width, int height)
        {
            this.width = width;
            this.height = height;
            grid = new Cell[height, width];
            random = new Random();
            InitializeGrid();

            // Начальная позиция игрока
            PlayerX = 0;
            PlayerY = 0;

            // Позиция выхода
            ExitX = width - 1;
            ExitY = height - 1;

            IsGameCompleted = false;
        }

        private void InitializeGrid()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[y, x] = new Cell();
                }
            }
        }

        public void Generate()
        {
            int currentSet = 0;
            Dictionary<int, int> setCounts = new Dictionary<int, int>();

            for (int y = 0; y < height - 1; y++)
            {
                AssignSetsToEmptyCells(y, ref currentSet);
                CreateRightWalls(y);
                CreateBottomWalls(y, setCounts);
                PrepareNextRow(y);
            }

            ProcessLastRow();

            EnsureExitReachable();
        }

        private void AssignSetsToEmptyCells(int y, ref int currentSet)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[y, x].Set == -1)
                {
                    grid[y, x].Set = currentSet++;
                }
            }
        }

        private void CreateRightWalls(int y)
        {
            for (int x = 0; x < width - 1; x++)
            {
                bool shouldCreateWall = random.Next(2) == 0 ||
                                      grid[y, x].Set == grid[y, x + 1].Set;

                if (shouldCreateWall)
                {
                    grid[y, x].HasRightWall = true;
                }
                else
                {
                    grid[y, x].HasRightWall = false;
                    int setToMerge = grid[y, x + 1].Set;
                    int targetSet = grid[y, x].Set;

                    for (int i = 0; i < width; i++)
                    {
                        if (grid[y, i].Set == setToMerge)
                        {
                            grid[y, i].Set = targetSet;
                        }
                    }
                }
            }

            grid[y, width - 1].HasRightWall = true;
        }

        private void CreateBottomWalls(int y, Dictionary<int, int> setCounts)
        {
            setCounts.Clear();
            for (int x = 0; x < width; x++)
            {
                int set = grid[y, x].Set;
                if (!setCounts.ContainsKey(set))
                {
                    setCounts[set] = 0;
                }
                setCounts[set]++;
            }

            for (int x = 0; x < width; x++)
            {
                int currentSet = grid[y, x].Set;
                bool shouldCreateWall = random.Next(2) == 0;

                if (setCounts[currentSet] == 1)
                {
                    shouldCreateWall = false;
                }

                if (shouldCreateWall)
                {
                    grid[y, x].HasBottomWall = true;
                    setCounts[currentSet]--;
                }
                else
                {
                    grid[y, x].HasBottomWall = false;
                }
            }
        }

        private void PrepareNextRow(int y)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[y, x].HasBottomWall)
                {
                    grid[y + 1, x].Set = -1;
                }
                else
                {
                    grid[y + 1, x].Set = grid[y, x].Set;
                }
            }
        }

        private void ProcessLastRow()
        {
            int lastRow = height - 1;

            int currentSet = grid[lastRow, 0].Set;
            for (int x = 1; x < width; x++)
            {
                if (grid[lastRow, x].Set == -1)
                {
                    grid[lastRow, x].Set = ++currentSet;
                }
            }

            for (int x = 0; x < width - 1; x++)
            {
                if (grid[lastRow, x].Set != grid[lastRow, x + 1].Set)
                {
                    grid[lastRow, x].HasRightWall = false;

                    int setToMerge = grid[lastRow, x + 1].Set;
                    int targetSet = grid[lastRow, x].Set;

                    for (int i = 0; i < width; i++)
                    {
                        if (grid[lastRow, i].Set == setToMerge)
                        {
                            grid[lastRow, i].Set = targetSet;
                        }
                    }
                }
                else
                {
                    grid[lastRow, x].HasRightWall = true;
                }
            }

            for (int x = 0; x < width; x++)
            {
                grid[lastRow, x].HasBottomWall = true;
            }
        }

        private void EnsureExitReachable()
        {
            grid[height - 1, width - 1].HasRightWall = false;
            grid[height - 1, width - 1].HasBottomWall = false;

            if (width > 1)
            {
                grid[height - 1, width - 2].HasRightWall = false;
            }
            if (height > 1)
            {
                grid[height - 2, width - 1].HasBottomWall = false;
            }
        }

        public bool MovePlayer(int dx, int dy)
        {
            int newX = PlayerX + dx;
            int newY = PlayerY + dy;

            if (CanMove(PlayerX, PlayerY, newX, newY))
            {
                PlayerX = newX;
                PlayerY = newY;

                if (PlayerX == ExitX && PlayerY == ExitY)
                {
                    IsGameCompleted = true;
                }

                return true;
            }

            return false;
        }

        public void MoveUp() => MovePlayer(0, -1);
        public void MoveDown() => MovePlayer(0, 1);
        public void MoveLeft() => MovePlayer(-1, 0);
        public void MoveRight() => MovePlayer(1, 0);

        public void PrintMaze()
        {
            Console.Clear();
            Console.WriteLine("Лабиринт - используйте стрелки для движения");
            Console.WriteLine("P - игрок, E - выход");
            Console.WriteLine();

            Console.Write("╔");
            for (int x = 0; x < width; x++)
            {
                Console.Write("══");
                if (x < width - 1)
                {
                    Console.Write("╦");
                }
            }
            Console.WriteLine("╗");

            for (int y = 0; y < height; y++)
            {
                Console.Write("║");
                for (int x = 0; x < width; x++)
                {
                    if (x == PlayerX && y == PlayerY)
                    {
                        Console.Write(" P");
                    }
                    else if (x == ExitX && y == ExitY)
                    {
                        Console.Write(" E");
                    }
                    else
                    {
                        Console.Write("  ");
                    }

                    Console.Write(grid[y, x].HasRightWall ? "║" : " ");
                }
                Console.WriteLine();

                if (y < height - 1)
                {
                    Console.Write("╠");
                    for (int x = 0; x < width; x++)
                    {
                        Console.Write(grid[y, x].HasBottomWall ? "══" : "  ");
                        if (x < width - 1)
                        {
                            Console.Write(grid[y, x].HasBottomWall ? "╬" : "╫");
                        }
                    }
                    Console.WriteLine("╣");
                }
            }

            Console.Write("╚");
            for (int x = 0; x < width; x++)
            {
                Console.Write("══");
                if (x < width - 1)
                {
                    Console.Write("╩");
                }
            }
            Console.WriteLine("╝");

            if (IsGameCompleted)
            {
                Console.WriteLine("\n🎉 ПОЗДРАВЛЯЮ! Вы нашли выход из лабиринта! 🎉");
            }
        }

        public bool CanMove(int fromX, int fromY, int toX, int toY)
        {
            if (toX < 0 || toX >= width || toY < 0 || toY >= height)
            {
                return false;
            }

            int dx = toX - fromX;
            int dy = toY - fromY;

            if (dx == 1 && dy == 0) // Вправо
            {
                return !grid[fromY, fromX].HasRightWall;
            }
            if (dx == -1 && dy == 0) // Влево
            {
                return !grid[toY, toX].HasRightWall;
            }
            if (dx == 0 && dy == 1) // Вниз
            {
                return !grid[fromY, fromX].HasBottomWall;
            }
            if (dx == 0 && dy == -1) // Вверх
            {
                return !grid[toY, toX].HasBottomWall;
            }

            return false;
        }
    }
}
