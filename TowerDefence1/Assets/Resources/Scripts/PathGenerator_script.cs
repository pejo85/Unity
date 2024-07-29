using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator_script : MonoBehaviour
{
    int width;
    int height;
    public int numOfCells = 0;

    private List<Vector2Int> pathCells;

    public PathGenerator_script(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public List<Vector2Int> GeneratePath(int maxNumberOfCells)
    {
        pathCells = new List<Vector2Int>();

        numOfCells = 0;
        int y = (int)height/2; // Temp to start from middle
        int x = 0;

        //Debug.Log("111 === " + pathCells.Count);
        while (x < width && pathCells.Count < maxNumberOfCells) 
        {
            //Debug.Log("222");
            pathCells.Add(new Vector2Int(x, y));

            bool valiidMove = false;
            while (!valiidMove)
            {
                int randomNum = Random.Range(0, 3);
                //Debug.Log("randomNum = " + randomNum + " , " + pathCells.Count);

                if (randomNum == 0 && IsFreePath(x + 1, y) || x % 2 == 0 || x > (width - 2))
                {
                    x++;
                    valiidMove = true;
                }
                else if (randomNum == 1 && IsFreePath(x, y + 1) && y < (height - 2))
                {
                    y++;
                    valiidMove = true;
                }
                else if (randomNum == 2 && IsFreePath(x, y - 1) && y > 2)
                {
                    y--;
                    valiidMove = true;
                }
            }
            //if (numOfCells == 0)
            //{
            //    pathCells.Add(new Vector2Int(x, y)); // First Cell
            //    numOfCells++;
            //}

            //if (randomNum == 0 && IsFreePath(x+1,y)  || x%2==0 && IsFreePath(x + 1, y) || x > (width - 2))
            //{
            //    Debug.Log("zzz");
            //    x ++;
            //    pathCells.Add(new Vector2Int(x, y));
            //    numOfCells++;
            //}
            //else if(randomNum == 1 && y < (height -2) && IsFreePath(x, y +1) )
            //{
            //    y++;
            //    pathCells.Add(new Vector2Int(x, y));
            //    numOfCells++;
            //}
            //else if(randomNum == 2 && y > 2 && IsFreePath(x, y - 1) )
            //{
            //    y--;
            //    pathCells.Add(new Vector2Int(x, y));
            //    numOfCells++;
            //}

        }

        return pathCells;
    }

    private bool IsFreePath(int x, int y)
    { 
        if (pathCells.Contains(new Vector2Int(x,y)))
        {
            return false;
        }
        return true;
    }

}
