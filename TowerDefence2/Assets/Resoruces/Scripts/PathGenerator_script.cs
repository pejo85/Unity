using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class PathGenerator_script : MonoBehaviour
{
    int width;
    int height;                    //    2
    public int numOfCells = 0;     //  1 C 4
    int numOfTry = 0;              //    8
    int maxNumOfTry = 1000;
    private int maxNumberOfCells;
    private bool cornerToUP = false;
    private bool cornerToRIGHT = false;
    private bool cornerToDOWN = false;
    private bool cornerToLEFT = false;

    private List<Vector2Int> pathCells;

    public void PathGenerator(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public List<Vector2Int> GeneratePath(int maxNumberOfCells)
    {
        this.maxNumberOfCells = maxNumberOfCells;
        pathCells = new List<Vector2Int>();
        numOfCells = 0;
        numOfTry = 0;  // Reset, because from grid manager it increases after each iteration in vile loop
        int y = (int)height/2; // Temp to start from middle
        int x = 1;
        
        while (x < width && pathCells.Count < maxNumberOfCells && numOfTry<maxNumOfTry) 
        {
            numOfTry++;
            pathCells.Add(new Vector2Int(x, y));

            bool valiidMove = false;

            while (!valiidMove && numOfTry < maxNumOfTry)
            {
                numOfTry++;
                int randomNum = Random.Range(0, 3);

                

                // Right
                if ((randomNum == 0 && IsFreePath(x + 1, y) && IsFreePath(x + 1, y - 1) && IsFreePath(x + 1, y + 1)) || (x % 2 == 1 && IsFreePath(x + 1, y - 1) && IsFreePath(x + 1, y + 1)) || x > (width - 2) || pathCells.Count == (maxNumberOfCells - 1))
                {
                    x++;
                    valiidMove = true;
                }
                // Up
                else if (randomNum == 1 && IsFreePath(x, y + 1) && IsFreePath(x, y + 2) && y < (height - 2))
                {
                    if (pathCells.Count > 2)
                    {
                        if ( IsCorner(pathCells[pathCells.Count - 2], pathCells[pathCells.Count-1], new Vector2Int(x, y + 1)))
                        {
                            if (CanMakeRoundRoad(x, y) )
                            {
                                if (Random.Range(0,1) == 0)
                                {
                                    MakeRoundRoad(x, y);
                                }
                            }
                            cornerToUP = false;
                        }
                    }
                    if (IsFreePath(x, y + 1)) // Check again, because maybe after making RoundRoad, it is now occupied
                    {
                        y++;
                        valiidMove = true;
                    }
                }
                // Down
                else if (randomNum == 2 && IsFreePath(x, y - 1) && IsFreePath(x, y - 2) && y > 2)
                {
                    if (pathCells.Count > 2)
                    {
                        if (IsCorner(pathCells[pathCells.Count - 2], pathCells[pathCells.Count - 1], new Vector2Int(x, y - 1)))
                        {
                            if (CanMakeRoundRoad(x, y))
                            {
                                if (Random.Range(0, 1) == 0)
                                {
                                    MakeRoundRoad(x, y);
                                }
                            }
                            cornerToDOWN = false;
                        }
                    }
                    if (IsFreePath(x, y - 1)) // Check again, because maybe after making RoundRoad, it is now occupied
                    {
                        y--;
                        valiidMove = true;
                    }
                }
            }
            //pathCells.Add(new Vector2Int(x, y));
        }
        return pathCells;
    }

    public bool IsFreePath(int x, int y)
    { 
        if (pathCells.Contains(new Vector2Int(x,y)) || !IsInsideGrid(x,y))
        {
            return false;
        }
        return true;
    }

    private bool IsInsideGrid(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    private bool IsCorner(Vector2Int beforeCell, Vector2Int activeCell, Vector2Int nextCell)
    {
        int beforeX = beforeCell.x; int beforeY = beforeCell.y;
        int activeX = activeCell.x; int activeY = activeCell.y;
        int nextX   = nextCell.x;   int nextY   = nextCell.y;

        if (beforeY == activeY && activeX == nextX && nextY > activeY) //  _| Corner
        {
            cornerToUP = true;
            return true;
        }
                                                                            // __
        else if (beforeY == activeY && activeX == nextX && nextY < activeY) //  | Corner
        {
            cornerToDOWN = true;
            return true;
        }
        return false;
    }

    public bool CanMakeRoundRoad(int x, int y)
    {
        if (maxNumberOfCells - pathCells.Count < 10)
        {
            return false; 
        }

        if (cornerToUP)
        {
            if (                                                    IsFreePath(x + 1, y)     && IsFreePath(x + 2, y)     && IsFreePath(x + 3, y)     && 
                IsFreePath(x - 1, y - 1) && IsFreePath(x, y - 1) && IsFreePath(x + 1, y - 1) && IsFreePath(x + 2, y - 1) && IsFreePath(x + 3, y - 1) && 
                IsFreePath(x - 1, y - 2) && IsFreePath(x, y - 2) && IsFreePath(x + 1, y - 2) && IsFreePath(x + 2, y - 2) && IsFreePath(x + 3, y - 2) && 
                IsFreePath(x - 1, y - 3) && IsFreePath(x, y - 3) && IsFreePath(x + 1, y - 3) && IsFreePath(x + 2, y - 3) && IsFreePath(x + 3, y - 3))
            {
                return true;
            }
        }
        
        else if (cornerToDOWN)
        {
            if (IsFreePath(x - 1, y + 3) && IsFreePath(x, y + 3) && IsFreePath(x + 1, y + 3) && IsFreePath(x + 2, y + 3) && IsFreePath(x + 3, y + 3) &&
                IsFreePath(x - 1, y + 2) && IsFreePath(x, y + 2) && IsFreePath(x + 1, y + 2) && IsFreePath(x + 2, y + 2) && IsFreePath(x + 3, y + 2) &&
                IsFreePath(x - 1, y + 1) && IsFreePath(x, y + 1) && IsFreePath(x + 1, y + 1) && IsFreePath(x + 2, y + 1) && IsFreePath(x + 3, y + 1) &&
                                                                    IsFreePath(x + 1, y)     && IsFreePath(x + 2, y)     && IsFreePath(x + 3, y))
            {
                return true;
            }
        }
        else if (cornerToRIGHT)
        {

        }
        else if (cornerToLEFT)
        {
            
        }

        return false;
    }

    public void MakeRoundRoad(int x, int y)
    {
        if (cornerToUP)
        {
            //pathCells.Add(new Vector2Int(x, y));
            pathCells.Add(new Vector2Int(x + 1, y));
            pathCells.Add(new Vector2Int(x + 2, y));
            pathCells.Add(new Vector2Int(x + 2, y - 1));
            pathCells.Add(new Vector2Int(x + 2, y - 2));
            pathCells.Add(new Vector2Int(x + 1, y - 2));
            pathCells.Add(new Vector2Int(x, y - 2));
            pathCells.Add(new Vector2Int(x, y - 1));
        }
        else if (cornerToDOWN)
        {
            pathCells.Add(new Vector2Int(x + 1, y));
            pathCells.Add(new Vector2Int(x + 2, y));
            pathCells.Add(new Vector2Int(x + 2, y + 1));
            pathCells.Add(new Vector2Int(x + 2, y + 2));
            pathCells.Add(new Vector2Int(x + 1, y + 2));
            pathCells.Add(new Vector2Int(x, y + 2));
            pathCells.Add(new Vector2Int(x, y + 1));
            //pathCells.Add(new Vector2Int(x, y));
        }  
    }

    public int CalculatePathNeighbourNumber(int x, int y)
    {
        int num = 0;
        if (!IsFreePath(x - 1, y)) { num += 1; }
        if (!IsFreePath(x, y + 1)) { num += 2; }
        if (!IsFreePath(x + 1, y)) { num += 4; }
        if (!IsFreePath(x, y - 1)) { num += 8; }

        return num;
    }


}
