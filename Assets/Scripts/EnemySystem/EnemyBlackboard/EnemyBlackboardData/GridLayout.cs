using System.Collections.Generic;
using UnityEngine;

namespace EnemyBT.BlackboardData
{
    public class GridData
    {
        public int[,] gridArray {get; private set;}
        public float gridSpacing {get ; private set; }
        public GridData(Vector2Int grid, float spacing)
        {
            gridArray = new int[grid.x, grid.y];
            
            gridSpacing = spacing;
        }
    }
}