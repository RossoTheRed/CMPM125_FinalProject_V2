using System.Collections.Generic;
using UnityEngine;

namespace EnemyBT.BlackboardData
{
    public class GridData
    {
        public GridCell[,] gridArray {get; private set;}
        public float gridSpacing {get ; private set; }
        public GridData(GridCell[,] grid, float spacing)
        {
            gridArray = grid;
            
            gridSpacing = spacing;
        }
    }
}