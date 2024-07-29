using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager_script : MonoBehaviour
{
    int width = 16;
    int height = 8;
    int maxNumberOfCells = 35;
    int numOfTrY = 0;
    PathGenerator_script pathGenerator_script;

    private List<Vector2Int> pathCells;

    [SerializeField] GameObject tile;
    [SerializeField] GameObject cube;

    float WAITTIME = 0.001f;

    void Start()
    {
        pathGenerator_script = new PathGenerator_script(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
               GameObject Cube = Instantiate(cube, new Vector3(x,0,y), Quaternion.identity);
                Cube.transform.Rotate(90,0,0);
            }
        }



        //for (int i = 0; i < 1000 || pathGenerator_script.numOfCells == maxNumberOfCells; i++)
        //{
        //    pathCells = pathGenerator_script.GeneratePath(maxNumberOfCells);
        //    Debug.Log(i);
        //}
        //pathGenerator_script.numOfCells < maxNumberOfCells ||

        pathCells = pathGenerator_script.GeneratePath(maxNumberOfCells);
        while (pathCells.Count < maxNumberOfCells && numOfTrY < 1000000) // renders path untill tile objects equal to maxNumberOfCells
        {
            pathCells = pathGenerator_script.GeneratePath(maxNumberOfCells);
            numOfTrY++;
        }
        Debug.Log("pathCells = " + pathCells.Count + " , numOfTrY = " + numOfTrY);

        StartCoroutine(LayPathCells(pathCells));
        

    }

    private IEnumerator LayPathCells(List<Vector2Int> pathCells)
    {
        foreach (Vector2Int pathTile in pathCells)
        {
            yield return new WaitForSeconds(WAITTIME);
            Instantiate(tile, new Vector3(pathTile.x, 0, pathTile.y), Quaternion.identity);
        }
    }


}
