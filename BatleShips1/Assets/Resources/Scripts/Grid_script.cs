
using UnityEngine;

public class Grid_script : MonoBehaviour
{
    [SerializeField] private GameObject GameManager;
    private GameManager_script gameManager_script;

    public GameObject[,] grid_list_player;
    public GameObject[,] grid_list_enemy;

    private int gridWidth;
    private int gridHeight;
    private int distanceBetweenGrids;

    public bool gameOver = false;
    public bool gameStarted;

    private void Awake()
    {
        gameManager_script = GameManager.GetComponent<GameManager_script>();

        gridWidth = gameManager_script.GRIDWIDTH;
        gridHeight = gameManager_script.GRIDHEIGHT;
        distanceBetweenGrids = gameManager_script.DISTANCEBETWEENGRIDS;

        grid_list_player = new GameObject[gridWidth, gridHeight];
        grid_list_enemy = new GameObject[gridWidth, gridHeight];
    }


    public void CreateGrid(GameObject Cube)
    {
        CreatePlayerGrid(Cube);
        CreateEnemyGrid(Cube);
    }

    //public void CreateGridFrame(GameObject frame)
    //{
    //    for (int y = 0; y < gridHeight; y++)
    //    {
    //        for (int x = 0; x < gridWidth; x++)
    //        {
    //            GameObject cube_frame =  Instantiate(frame, new Vector2(x, y), Quaternion.identity);
    //            cube_frame.transform.SetParent(GameObject.Find("Grid").transform);
    //        }
    //    }
    //
    //    for (int y = 0; y < gridHeight; y++)
    //    {
    //        for (int x = 0; x < gridWidth; x++)
    //        {
    //            GameObject cube_frame = Instantiate(frame, new Vector2(x + distanceBetweenGrids, y), Quaternion.identity);
    //            cube_frame.transform.SetParent(GameObject.Find("Grid").transform);
    //        }
    //    }
    //}

    private void CreatePlayerGrid(GameObject cube)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                grid_list_player[x, y] = Instantiate(cube, new Vector2(x, y), Quaternion.identity);
                grid_list_player[x, y].transform.SetParent(GameObject.Find("Grid").transform);
                //grid_list_player[x, y].GetComponent<SpriteRenderer>().color = Color.white;
                grid_list_player[x, y].GetComponent<Cube_script>().isPlayerBoard = true;

                //AssignSpriteSliceToCube(grid_list_player[x, y], x, y);


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
                //grid_list_enemy[x, y].GetComponent<SpriteRenderer>().sortingOrder = 40;
                grid_list_enemy[x, y].GetComponent<SpriteRenderer>().sortingOrder = 0;

                gameManager_script.ChangeTileColor(grid_list_enemy[x, y], gameManager_script.tileEnemyDefaultColor);
                //AssignSpriteSliceToCube(grid_list_enemy[x, y] , x , y);
            }
        }
    }

    private void AssignSpriteSliceToCube(GameObject cube , int x , int y)
    {
        Sprite[] slicedSprites = Resources.LoadAll<Sprite>("Sprites/Ocean/8_1"); // Ensure the path is correct
        int targetWidth = 8; // The width of the grid you want to create
        int targetHeight = 8; // The height of the grid you want to create
        int totalColumns = 13; // The total number of columns in the sliced sprite

        cube.gameObject.transform.localScale = new Vector3(3f, 3f, 3);

        // Calculate the name of the sliced sprite
        int spriteIndex = (totalColumns * (targetHeight - 1 - y)) + (totalColumns - targetWidth) + x;
        string spriteName = "8_1_" + spriteIndex.ToString();
        Sprite loadedSprite = null;

        foreach (Sprite sprite in slicedSprites)
        {
            if (sprite.name == spriteName)
            {
                loadedSprite = sprite;
                break;
            }
        }

        if (loadedSprite != null)
        {
            //Debug.Log("Sprite loaded successfully: " + spriteName);
            cube.GetComponent<SpriteRenderer>().sprite = loadedSprite;
        }
        else
        {
            Debug.LogError("Failed to load sprite: " + spriteName);
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

    public void ResetGrid()
    {
        // Reset Player grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid_list_player[x, y].gameObject != null)
                {
                    grid_list_player[x, y].gameObject.GetComponent<Cube_script>().ResetCubeProperties();
                }

            }
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid_list_player[x, y].gameObject != null)
                {
                    grid_list_enemy[x, y].gameObject.GetComponent<Cube_script>().ResetCubeProperties();
                }
            }
        }
    }

    public bool IsValidPos(Vector2Int[] shipAllPosArray, bool isPlayer)
    {
        // Player
        if (isPlayer)
        {
            if (ShipIsInsideGrid(shipAllPosArray, isPlayer))
            {
                // if position is already ocupied
                for (int i = 0; i < shipAllPosArray.Length; i++)
                {
                    if (grid_list_player[(shipAllPosArray[i].x), shipAllPosArray[i].y].gameObject.GetComponent<Cube_script>().isOcupied)
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
        else // Enemy
        {
            if (ShipIsInsideGrid(shipAllPosArray, isPlayer))
            {
                // if position is already ocupied
                for (int i = 0; i < shipAllPosArray.Length; i++)
                {
                    if (grid_list_enemy[(shipAllPosArray[i].x - distanceBetweenGrids), shipAllPosArray[i].y].gameObject.GetComponent<Cube_script>().isOcupied)
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

    public bool ShipIsInsideGrid(Vector2Int[] shipAllPosArray, bool isPlayer)
    {
        if (isPlayer)
        {
            for (int i = 0; i < shipAllPosArray.Length; i++)
            {
                if (shipAllPosArray[i].x < grid_list_player[0, 0].transform.position.x || shipAllPosArray[i].x > grid_list_player[gridWidth - 1, gridHeight - 1].transform.position.x)
                {
                    //NOT INSIDE GRID on X axis
                    return false;
                }

                if (shipAllPosArray[i].y < grid_list_player[0, 0].transform.position.y || shipAllPosArray[i].y > grid_list_player[gridWidth - 1, gridHeight - 1].transform.position.y)
                {
                    //NOT INSIDE GRID on Y axis
                    return false;
                }
            }
            return true;
        }
        else // Enemy
        {
            for (int i = 0; i < shipAllPosArray.Length; i++)
            {
                if (shipAllPosArray[i].x < grid_list_enemy[0, 0].transform.position.x || shipAllPosArray[i].x > grid_list_enemy[gridWidth - 1, gridHeight - 1].transform.position.x)
                {
                    //NOT INSIDE GRID on X axis
                    return false;
                }

                if (shipAllPosArray[i].y < grid_list_enemy[0, 0].transform.position.y || shipAllPosArray[i].y > grid_list_enemy[gridWidth - 1, gridHeight - 1].transform.position.y)
                {
                    //NOT INSIDE GRID on Y axis
                    return false;
                }
            }
            return true;
        }

    }

    public bool PosIsInsideGrid(Vector2Int pos, bool isPlayer)
    {
        if (isPlayer)
        {
            if (pos.x < grid_list_player[0, 0].transform.position.x || pos.x > grid_list_player[gridWidth - 1, gridHeight - 1].transform.position.x)
            {
                //NOT INSIDE GRID on X axis
                return false;
            }

            if (pos.y < grid_list_player[0, 0].transform.position.y || pos.y > grid_list_player[gridWidth - 1, gridHeight - 1].transform.position.y)
            {
                //NOT INSIDE GRID on Y axis
                return false;
            }
        }
        else // Enemy
        {
            if (pos.x < grid_list_enemy[0, 0].transform.position.x || pos.x > grid_list_enemy[gridWidth - 1, gridHeight - 1].transform.position.x)
            {
                return false; //NOT INSIDE GRID on X axis
            }
            if (pos.y < grid_list_enemy[0, 0].transform.position.y || pos.y > grid_list_enemy[gridWidth - 1, gridHeight - 1].transform.position.y)
            {
                return false; //NOT INSIDE GRID on Y axis }
            }
        }
        return true;
    }

    public void OcupyGridPos(Vector2Int[] shipAllPosArray, bool isPlayer)
    {
        if (isPlayer)
        {
            for (int i = 0; i < shipAllPosArray.Length; i++)
            {
                grid_list_player[shipAllPosArray[i].x, shipAllPosArray[i].y].gameObject.GetComponent<Cube_script>().IsOcupied();
            }
        }
        else // Enemy
        {
            for (int i = 0; i < shipAllPosArray.Length; i++)
            {
                grid_list_enemy[shipAllPosArray[i].x - distanceBetweenGrids, shipAllPosArray[i].y].gameObject.GetComponent<Cube_script>().IsOcupied();
            }
        }
    }

    public void DisableAllGridPosColider()
    {
        // Disable Box Colider on Player Grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid_list_player[x, y].gameObject != null)
                {
                    grid_list_player[x, y].gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }

            }
        }
    }

    public void EnableAllGridPosColider()
    {
        // Enable Box Colider on Player Grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid_list_player[x, y].gameObject != null)
                {
                    grid_list_player[x, y].gameObject.GetComponent<BoxCollider2D>().enabled = true;
                }

            }
        }
    }

    public void GameHasStarted(bool gameHasStarted)
    {
        //  Player grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid_list_player[x, y].gameObject != null)
                {
                    grid_list_player[x, y].gameObject.GetComponent<Cube_script>().GameStarted(gameHasStarted);
                }

            }
        }
        //  Enemy grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid_list_player[x, y].gameObject != null)
                {
                    grid_list_enemy[x, y].gameObject.GetComponent<Cube_script>().GameStarted(gameHasStarted);
                }
            }
        }
    }

    public bool sateliteNeighboursIsValidPos(Vector2Int[] neighbourPosList, bool isPlayer)
    {
        // Enemy
        if (!isPlayer)
        {
            for (int y = 0; y < neighbourPosList.Length; y++)
            {
            }
            if (ShipIsInsideGrid(neighbourPosList, isPlayer))
            {
                return true;
            }
        }
        return false;
    }

    public void makeFreeGridPos(Vector2Int[] shipAllPos, bool isPlayer)
    {
        if (isPlayer)
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                grid_list_player[shipAllPos[i].x, shipAllPos[i].y].gameObject.GetComponent<Cube_script>().IsFree();
            }
        }
        else // Enemy
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                grid_list_enemy[shipAllPos[i].x - distanceBetweenGrids, shipAllPos[i].y].gameObject.GetComponent<Cube_script>().IsFree();
            }
        }

    }

    public bool isInsideGridPotencialShootingList(Vector2Int potencialPos)
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
        return true;
    }

    public void changeGridCubeColors(Vector2[] shipAllPos, Color color1)
    {
        for (int i = 0; i < shipAllPos.Length; i++)
        {
            Debug.Log(shipAllPos[1] + "," + color1);
            grid_list_player[(int)shipAllPos[i].x, (int)shipAllPos[i].y].gameObject.GetComponent<SpriteRenderer>().color = color1;
        }
    }

}
