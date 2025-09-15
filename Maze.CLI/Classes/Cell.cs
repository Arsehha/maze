using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.CLI.Classes
{
    public class Cell
    {
        public int Set { get; set; }
        public bool HasRightWall { get; set; }
        public bool HasBottomWall { get; set; }

        public Cell()
        {
            Set = -1;
            HasRightWall = true;
            HasBottomWall = true;
        }
    }
}
