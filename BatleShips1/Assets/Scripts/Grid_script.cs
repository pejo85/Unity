//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Grid_script : MonoBehaviour
{
    [SerializeField] GameObject GameManager;

    private GameManager_script gameManager_script;

    public GameObject[,] grid_list_player;
    public GameObject[,] grid_list_enemy;

    private int gridWidth;
    private int gridHeight;
    private int distanceBetweenGrids;

    public bool gameOver = false;

    private void Awake()
    {

        gameManager_script = GameManager.GetComponent<GameManager_script>();

        gridWidth = gameManager_script.gridWidth;
        gridHeight = gameManager_script.gridHeight;
        distanceBetweenGrids = gameManager_script.distanceBetweenGrids;

        grid_list_player = new GameObject[gridWidth, gridHeight];
        grid_list_enemy = new GameObject[gridWidth, gridHeight];
    }

    void Start()
    {

    }

    public void CreateGrid(GameObject Cube)
    {
        CreatePlayerGrid(Cube);
        CreateEnemyGrid(Cube);  
    }

    private void CreatePlayerGrid(GameObject cube)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                grid_list_player[x, y] = Instantiate(cube, new Vector2(x, y), Quaternion.identity);
                grid_list_player[x, y].transform.SetParent(GameObject.Find("Grid").transform);
                grid_list_player[x, y].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    private void CreateEnemyGrid(GameObject Cube)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                grid_list_enemy[x, y] = Instantiate(Cube, new Vector2(x + distanceBetweenGrids, y), Quaternion.identity);
                grid_list_enemy[x, y].transform.SetParent(GameObject.Find("Grid").transform);
                grid_list_enemy[x, y].GetComponent<Cube_script>().isEnemyBoard = true;
                grid_list_enemy[x, y].GetComponent<SpriteRenderer>().sortingOrder = 5;
                grid_list_enemy[x, y].GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }
    }

    public void DisableEnemyGrid()
    {
        foreach (GameObject Cube in grid_list_enemy)
        {
            Cube.SetActive(false);
        }
    }

    private void DisablePlayerGrid()
    {
        foreach (GameObject cube in grid_list_player)
        {
            cube.SetActive(false);
        }
    }

    public void EnablePlayerGrid()
    {
        foreach (GameObject cube in grid_list_player)
        {
            if (cube != null)
            {
                cube.SetActive(true);
            }
        }
    }

    public void EnableEnemyGrid()
    {
        foreach (GameObject cube in grid_list_enemy)
        {
            cube.SetActive(true);
        }
    }

    public bool sateliteNeighboursIsValidPos(Vector2[] neighbourPosList, bool isEnemy)
    {
        // Enemy
        if (isEnemy)
        {
            if (isInsideGrid(neighbourPosList, isEnemy))
            {
                return true;
            }

        }
        return false;
    }

    public bool isValidPos(Vector2[] shipAllPos, bool isEnemy)
    {
        // Enemy
        if (isEnemy)
        {
            if (isInsideGrid(shipAllPos, isEnemy) )
            {
                // if position is already ocupied
                for (int i = 0; i < shipAllPos.Length; i++)
                {
                    if (grid_list_enemy[(int)(shipAllPos[i].x - distanceBetweenGrids), (int)shipAllPos[i].y].gameObject.GetComponent<Cube_script>().isOcupied)
                    {
                        return false; 
                    }
                }
                // if all coords are inside grid and already not ocupied, then it is walid pos
                return true;
            }
            // if it is not inside grid
            else
            {
                //Debug.Log("NOT INSIDE GRID...");
                return false;
            }
        }

        // Player
        else
        {
            if (isInsideGrid(shipAllPos, isEnemy))
            {
                // if position is already ocupied
                for (int i = 0; i < shipAllPos.Length; i++)
                {
                    if (grid_list_player[(int)(shipAllPos[i].x), (int)shipAllPos[i].y].gameObject.GetComponent<Cube_script>().isOcupied)
                    {
                        return false;
                    }
                }
                // if all coords are inside grid and already not ocupied, then it is walid pos
                return true;
            }
            // if it is not inside grid
            else
            {
                //Debug.Log("NOT INSIDE GRID...");
                return false;
            }
        }


    }

    public bool isInsideGrid(Vector2[] shipAllPos, bool isEnemy)
    {
        // Enemy
        if (isEnemy)
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                if (shipAllPos[i].x < grid_list_enemy[0, 0].transform.position.x || shipAllPos[i].x > grid_list_enemy[gridWidth - 1, gridHeight - 1].transform.position.x)
                {
                    //NOT INSIDE GRID on X axis
                    return false;
                }

                if (shipAllPos[i].y < grid_list_enemy[0, 0].transform.position.y || shipAllPos[i].y > grid_list_enemy[gridWidth - 1, gridHeight - 1].transform.position.y)
                {
                    //NOT INSIDE GRID on Y axis
                    return false;
                }
            }
            return true;
        }

        // Player
        else
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                if (shipAllPos[i].x < grid_list_player[0, 0].transform.position.x || shipAllPos[i].x > grid_list_player[gridWidth - 1, gridHeight - 1].transform.position.x)
                {
                    //NOT INSIDE GRID on X axis
                    return false;
                }

                if (shipAllPos[i].y < grid_list_player[0, 0].transform.position.y || shipAllPos[i].y > grid_list_player[gridWidth - 1, gridHeight - 1].transform.position.y)
                {
                    //NOT INSIDE GRID on Y axis
                    return false;
                }
            }
            return true;
        }

    }

    public bool isInsideGridPotencialShootingList(Vector2 potencialPos)
    {
        if (potencialPos.x < grid_list_player[0, 0].transform.position.x || potencialPos.x > grid_list_player[gridWidth - 1, gridHeight - 1].transform.position.x)
        {
            //NOT INSIDE GRID on X axis
            return false;
        }

        if (potencialPos.y < grid_list_player[0, 0].transform.position.y || potencialPos.y > grid_list_player[gridWidth - 1, gridHeight - 1].transform.position.y)
        {
            //NOT INSIDE GRID on Y axis
            return false;
        }
        return true ;
    }

    public void ocupyGridPos(Vector2[] shipAllPos, bool isEnemy)
    {
        // Enemy
        if (isEnemy)
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                grid_list_enemy[(int)shipAllPos[i].x - distanceBetweenGrids, (int)shipAllPos[i].y].gameObject.GetComponent<Cube_script>().IsOcupied();
            }
        }

        // Player
        else
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                grid_list_player[(int)shipAllPos[i].x, (int)shipAllPos[i].y].gameObject.GetComponent<Cube_script>().IsOcupied();
            }
        }
    }

    public void makeFreeGridPos(Vector2[] shipAllPos, bool isEnemy)
    {
        if (isEnemy)
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                grid_list_enemy[(int)shipAllPos[i].x - distanceBetweenGrids, (int)shipAllPos[i].y].gameObject.GetComponent<Cube_script>().IsFree();
            }
        }
        // PLAYER
        else
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                grid_list_player[(int)shipAllPos[i].x, (int)shipAllPos[i].y].gameObject.GetComponent<Cube_script>().IsFree();
            }
        }
        
    }

    public void resetGrid()
    {
        // Reset Player grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid_list_player[x, y].gameObject != null)
                {
                    grid_list_player[x, y].gameObject.GetComponent<Cube_script>().IsFree();
                }
                
            }
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid_list_player[x, y].gameObject != null)
                {
                    grid_list_enemy[x, y].gameObject.GetComponent<Cube_script>().IsFree();
                }   
            }
        }
    }

    //public void changeGridCubeColors(Vector2[] shipAllPos, Color color1)
    //{
    //    for (int i = 0; i < shipAllPos.Length; i++)
    //    {
    //        Debug.Log(shipAllPos[1] + "," + color1);
    //        grid_list_player[(int)shipAllPos[i].x, (int)shipAllPos[i].y].gameObject.GetComponent<SpriteRenderer>().color = color1;
    //    }
    //}






}
