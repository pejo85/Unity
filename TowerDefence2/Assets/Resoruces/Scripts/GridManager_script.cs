//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager_script : MonoBehaviour
{
    int width = 24;
    int height = 12;
    int maxNumberOfCells = 70;
    int numOfTry = 0;
    PathGenerator_script pathGenerator_script;
    

    public List<Vector2Int> pathCells;
    private List<GameObject> tileList = new List<GameObject>();
    [SerializeField] GameObject [] TileArray = new GameObject[16];
    [SerializeField] GameObject[] natureArray = new GameObject[7];
    

    [SerializeField] GameObject tile;
    [SerializeField] GameObject cube;
    


    //float WAITTIME = 0.001f;

    private void Awake()
    {
        pathGenerator_script = GetComponent<PathGenerator_script>();
        pathGenerator_script.PathGenerator(width, height);
        pathCells = pathGenerator_script.GeneratePath(maxNumberOfCells);
    }

    void Start()
    {
        
        // Temp for white grid
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
               GameObject Cube = Instantiate(cube, new Vector3(x,0,y), Quaternion.identity);
               Cube.transform.Rotate(90,0,0);
               Cube.transform.SetParent(GameObject.Find("Cubes").transform);
            }
        }

        while (pathCells.Count < maxNumberOfCells && numOfTry < 1000000) // renders path untill tile objects equal to maxNumberOfCells ot MaxNumOfTry
        {
            pathCells = pathGenerator_script.GeneratePath(maxNumberOfCells);
            numOfTry++;
        }

        CalculatePathNeighbourNumber();

        LayPathCells(pathCells);
        CreateNature();

    }



    private void LayPathCells(List<Vector2Int> pathCells)
    {
        foreach (Vector2Int pathTile in pathCells)
        {
            int neighbourNum = pathGenerator_script.CalculatePathNeighbourNumber(pathTile.x, pathTile.y);
            if (pathTile == pathCells[0])
            {
                neighbourNum = 4;
            }
            else if (pathTile == pathCells[pathCells.Count - 1])
            {
                neighbourNum = 1;
            }

            GameObject Tile = Instantiate(TileArray[neighbourNum], new Vector3(pathTile.x, 0, pathTile.y), Quaternion.identity);
            Tile.transform.SetParent(GameObject.Find("Tiles").transform);
            tileList.Add(Tile);

            RotateTile(Tile, neighbourNum);
        }
    }

    public void CalculatePathNeighbourNumber()
    {
        foreach (Vector2Int pathCell in pathCells)
        {
            pathGenerator_script.CalculatePathNeighbourNumber(pathCell.x , pathCell.y);
        }
    }

    private void RotateTile(GameObject Tile , int neighbourNum)
    {
        switch (neighbourNum)
        {
            case 1:
                Tile.transform.Rotate(0, -90, 0); break;
            case 3:
                Tile.transform.Rotate(0, 0, 0); break;
            case 4:
                Tile.transform.Rotate(0, 90, 0); break;
            case 5:
                Tile.transform.Rotate(0, 90, 0); break;
            case 6:
                Tile.transform.Rotate(0, 90, 0); break;
            case 9:
                Tile.transform.Rotate(0, -90, 0); break;
            case 11:
                Tile.transform.Rotate(0, -90, 0); break;
            case 12:
                Tile.transform.Rotate(0, 180, 0); break;
            case 14:
                Tile.transform.Rotate(0, 90, 0); break;
        }
    }

    private void CreateNature()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (pathGenerator_script.IsFreePath(x, y))
                {
                    int natureArrayNum = NatureObjectPick(Random.Range(0, 101));
                    GameObject Tile = Instantiate(natureArray[natureArrayNum], new Vector3(x, 0, y), Quaternion.identity);
                    Tile.transform.SetParent(GameObject.Find("Nature").transform);
                }
            }
        }
    }

    private int NatureObjectPick(int  num)
    {
        int natureArrayNum = 0;
        if (num < 40)
        {
            natureArrayNum = 0; // Tile
        }
        else if (num < 55)
        {
            natureArrayNum = 1; // Tile tree
        }
        else if (num < 70)
        {
            natureArrayNum = 2;// Tile tree1
        }
        else if (num < 85)
        {
            natureArrayNum = 3; // Tile tree2
        }
        else if (num < 90)
        {
            natureArrayNum = 4;// Tile rock
        }
        else if (num < 95)
        {
            natureArrayNum = 5;// Tile hill
        }
        else if (num < 100)
        {
            natureArrayNum = 6;// Tile cristal
        }


        return natureArrayNum;
    }



    private Vector3 GetMouseWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        Vector3 worldPos = Vector3.zero;

        if (plane.Raycast(ray, out distance))
        {
            worldPos = ray.GetPoint(distance);
        }

        return new Vector3(worldPos.x, 0.2f, worldPos.z);
    }
    
    private void OnMouseDown()
    {
        Debug.Log("Mouse pos = " + GetMouseWorldPos());
    }
    
    private void OnMouseUp()
    {
        Debug.Log("Mouse pos = " + GetMouseWorldPos());
    }
    
    private void OnMouseDrag()
    {
        Debug.Log("Mouse pos = " + GetMouseWorldPos());
    }
    
    private void OnMouseOver()
    {
        Debug.Log("Mouse over = " + GetMouseWorldPos());
    }

}
