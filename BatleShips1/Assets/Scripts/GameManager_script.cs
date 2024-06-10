using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_script : MonoBehaviour
{
    [SerializeField] GameObject Grid_obj;
    [SerializeField] GameObject Cube_obj;
    [SerializeField] GameObject playerShips;
    [SerializeField] GameObject enemyShips;
    [SerializeField] GameObject Hit_miss_anim_obj;
    [SerializeField] GameObject Bullet_obj;

    [SerializeField] Button startGame_btn;
    [SerializeField] Button random_btn;

    private Grid_script grid_script;
    private Cube_script cube_Script;
    private Ship_script ship_script;
    private Bullet_script bullet_script;

    public List<Vector2Int> shootingRangeList;

    public int GRIDWIDTH = 8;
    public int GRIDHEIGHT = 8;
    public int DISTANCEBETWEENGRIDS = 10;
    private float WAITTIME = 1.0f;

    public bool allShipsAreReadyForBattle;
    public bool gameStarted;
    public bool gameOver;
    public bool playerMove;
    public bool enemyMove;
    public bool shipIsStillAlive;
    public bool bulletIsInTheAir;
    private bool potencialShipAllignmentIsHorisontal = true;
    [SerializeField] public List<Vector2Int> potencialShootingPosList = new List<Vector2Int>();
    public List<Vector2Int> previousSuccessfullHitList = new List<Vector2Int>();

    public bool sateliteIsWatching;
    public bool settingUpAirDefense;

    private int sateliteUseCount = 2;
    private int airDefenseUseCount = 2;


    // Start is called before the first frame update
    void Start()
    {
        grid_script = Grid_obj.GetComponent<Grid_script>();

        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateGrid()
    {
        grid_script.CreateGrid(Cube_obj);
        grid_script.DisableEnemyGrid();
    }

    private void PrepareGame()
    {

    }

    public void PlacePlayerShipsRandomly()  // Is called from Button "Random"
    {
        ActivatePlayerShips(); // in case, that some player ships may be deactivated
        ResetAllShipsProperties(); // In case, that some ship may already deployed somewhere
        

        foreach (Transform playerShip in playerShips.transform)
        {
            playerShip.GetComponent<Ship_script>().placeShipRandomly();


        }
    }

    public void PlaceEnemyShipsRandomly()
    {
        ActivateEnemyShips();
        foreach (Transform enemyShip in enemyShips.transform)
        {
            enemyShip.GetComponent<Ship_script>().placeShipRandomly();
        }
    }

    public void ResetAllShipsProperties()
    {
        ResetAllShipPropertiesOnFactoryDefault();
        ResetGridProperties();
    }

    public void ResetAllShipPropertiesOnFactoryDefault()
    {
        foreach (Transform playerShip in playerShips.transform)
        {
            if (playerShip != null)
            {
                playerShip.GetComponent<Ship_script>().ResetShipProperties();
            }
        }
    }

    private void ResetGridProperties()
    {
        grid_script.ResetGrid();
    }

    private void ActivatePlayerShips()
    {
        // If GameObject "PlayerShips" is disabled, activateit
        if (playerShips.gameObject.activeSelf == false)
        {
            playerShips.gameObject.SetActive(true);
        }
        // If any Ship in "PlayerShips" is disabled, activate it
        foreach (Transform playerShip in playerShips.transform)
        {
            if (playerShip.gameObject.activeSelf == false)
            {
                playerShip.gameObject.SetActive(true);
            }
        }
    }

    private void ActivateEnemyShips()
    {
        if (enemyShips.gameObject.activeSelf == false)
        {
            enemyShips.gameObject.SetActive(true);
        }

        foreach (Transform enemyShip in enemyShips.transform)
        {
            if (enemyShip.gameObject.activeSelf == false)
            {
                enemyShip.gameObject.SetActive(true);
            }
        }
    }

    public bool IsValidPos(Vector2Int[] shipAllPosArray, bool isPlayer)
    {
        return grid_script.IsValidPos(shipAllPosArray, isPlayer);
    }

    public bool PosIsInsideGrid(Vector2Int pos, bool isPlayer)
    {
        return grid_script.PosIsInsideGrid(pos, isPlayer);
    }

    public void OcupyGridPos(Vector2Int[] shipAllPosArray, bool isPlayer)
    {
        grid_script.OcupyGridPos(shipAllPosArray, isPlayer);
    }

    public void MakeFreeOcupiedPos(Vector2Int[] shipAllPos, bool isPlayer)
    {
        grid_script.makeFreeGridPos(shipAllPos, isPlayer);
    }

    public void CheckIfGameIsReady()
    {
        foreach (Transform playerShip in playerShips.transform)
        {
            if (playerShip.GetComponent<Ship_script>().shipIsReadyForBattle == false)
            {
                return;
            }
            allShipsAreReadyForBattle = true;
        }

        if (allShipsAreReadyForBattle)
        {
            grid_script.DisableAllGridPosColider(); // if not disabled, sometimes it conflicts with ship colliders when click on the ship to change horisontal or vertical

            startGame_btn.GetComponent<Button>().interactable = true;
        }
    }

    public void StartGame()
    {
        grid_script.EnableEnemyGrid();

        PlaceEnemyShipsRandomly();
        DisableAllShipMovement();
        DisableAllShipRigidBody();
        random_btn.gameObject.SetActive(false);
        startGame_btn.gameObject.SetActive(false);
        gameStarted = true;
        playerMove = true;
        settingUpAirDefense = false;
        grid_script.EnableAllGridPosColider();
        grid_script.GameHasStarted(gameStarted);
    }

    private void DisableAllShipMovement()
    {
        foreach (Transform playerShip in playerShips.transform)
        {
            playerShip.GetComponent<Ship_script>().shipCanMove = false;
            playerShip.GetComponent<Ship_script>().shipCanRotate = false;
        }
        foreach (Transform enemyShip in enemyShips.transform)
        {
            enemyShip.GetComponent<Ship_script>().shipCanMove = false;
            enemyShip.GetComponent<Ship_script>().shipCanRotate = false;
        }
    }

    private void DisableAllShipRigidBody()
    {
        foreach (Transform enemyShip in enemyShips.transform)
        {
            enemyShip.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        foreach (Transform playerShip in playerShips.transform)
        {
            playerShip.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void SateliteHoversOverCube(GameObject Tile)
    {
        GameObject[] CubesSateliteNeighboursList = new GameObject[4];

        CubesSateliteNeighboursList = CalculateClickedCubesSateliteNeighbours(Tile);

        foreach (GameObject Cube in CubesSateliteNeighboursList)
        {
            if (Cube.GetComponent<Cube_script>().isRevealed == false)
            {
                Cube.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
        }
    }

    public void SateliteHoversExitCube(GameObject Tile)
    {
        GameObject[] CubesSateliteNeighboursList = new GameObject[4];

        CubesSateliteNeighboursList = CalculateClickedCubesSateliteNeighbours(Tile);

        foreach (GameObject neighbour in CubesSateliteNeighboursList)
        {
            if (neighbour.GetComponent<Cube_script>().isRevealed == false)
            {
                neighbour.GetComponent<SpriteRenderer>().color = Color.gray;
            }
            
        }
    }

    public GameObject[] CalculateClickedCubesSateliteNeighbours(GameObject clickedCube)
    {
        GameObject[] neighbourList = new GameObject[4];
        Vector2Int[] neighbourList11 = new Vector2Int[4];

        int x = (int)clickedCube.transform.position.x;
        int y = (int)clickedCube.transform.position.y;


        Vector2Int neighbour1Pos = new Vector2Int(x + 1, y);
        Vector2Int neighbour2Pos = new Vector2Int(x + 1, y - 1);
        Vector2Int neighbour3Pos = new Vector2Int(x, y - 1);


        neighbourList11[0] = new Vector2Int((int)clickedCube.transform.position.x, (int)clickedCube.transform.position.y);
        neighbourList11[1] = neighbour1Pos;
        neighbourList11[2] = neighbour2Pos;
        neighbourList11[3] = neighbour3Pos;

        // Right
        if (grid_script.sateliteNeighboursIsValidPos(neighbourList11, false))
        {
            for (int i = 0; i < neighbourList11.Length; i++)
            {
                neighbourList[i] = grid_script.grid_list_enemy[neighbourList11[i].x - DISTANCEBETWEENGRIDS, neighbourList11[i].y];
            }
        }

        else
        {
            // Down 
            neighbour1Pos = new Vector2Int(x, y - 1);
            neighbour2Pos = new Vector2Int(x - 1, y - 1);
            neighbour3Pos = new Vector2Int(x - 1, y);

            neighbourList11[0] = new Vector2Int((int)clickedCube.transform.position.x, (int)clickedCube.transform.position.y);
            neighbourList11[1] = neighbour1Pos;
            neighbourList11[2] = neighbour2Pos;
            neighbourList11[3] = neighbour3Pos;

            if (grid_script.sateliteNeighboursIsValidPos(neighbourList11, false))
            {
                for (int i = 0; i < neighbourList11.Length; i++)
                {
                    neighbourList[i] = grid_script.grid_list_enemy[neighbourList11[i].x - DISTANCEBETWEENGRIDS, neighbourList11[i].y];
                }
            }

            else
            {
                // Left
                neighbour1Pos = new Vector2Int(x - 1, y);
                neighbour2Pos = new Vector2Int(x - 1, y + 1);
                neighbour3Pos = new Vector2Int(x, y + 1);

                neighbourList11[0] = new Vector2Int((int)clickedCube.transform.position.x, (int)clickedCube.transform.position.y);
                neighbourList11[1] = neighbour1Pos;
                neighbourList11[2] = neighbour2Pos;
                neighbourList11[3] = neighbour3Pos;

                if (grid_script.sateliteNeighboursIsValidPos(neighbourList11, false))
                {
                    for (int i = 0; i < neighbourList11.Length; i++)
                    {
                        neighbourList[i] = grid_script.grid_list_enemy[neighbourList11[i].x - DISTANCEBETWEENGRIDS, neighbourList11[i].y];
                    }
                }
                else
                {
                    // Up
                    neighbour1Pos = new Vector2Int(x, y + 1);
                    neighbour2Pos = new Vector2Int(x + 1, y + 1);
                    neighbour3Pos = new Vector2Int(x + 1, y);

                    neighbourList11[0] = new Vector2Int((int)clickedCube.transform.position.x, (int)clickedCube.transform.position.y);
                    neighbourList11[1] = neighbour1Pos;
                    neighbourList11[2] = neighbour2Pos;
                    neighbourList11[3] = neighbour3Pos;

                    if (grid_script.sateliteNeighboursIsValidPos(neighbourList11, false))
                    {
                        for (int i = 0; i < neighbourList11.Length; i++)
                        {
                            neighbourList[i] = grid_script.grid_list_enemy[neighbourList11[i].x - DISTANCEBETWEENGRIDS, neighbourList11[i].y];
                        }
                    }
                }

            }
        }
        return neighbourList;
    }

    public void SateliteClick()
    {
        if (sateliteUseCount > 0)
        {
            sateliteIsWatching = true;
        }
        else
        {
            Debug.Log("You dont have Satelite support");
        }
    }

    public void SateliteStopsWatching()
    {
        sateliteIsWatching = false;
    }

    public void SateliteRevealsEnemyTiles(GameObject clickedCube)
    {
        if (sateliteIsWatching)
        {
            GameObject[] sateliteTileList = new GameObject[4];

            sateliteTileList = CalculateClickedCubesSateliteNeighbours(clickedCube);

            foreach (GameObject tile in sateliteTileList)
            {
                tile.GetComponent<Cube_script>().RevealTile();
            }

            sateliteUseCount -= 1;
        }
    }

    public void AirDefenseClick()
    {
        if (airDefenseUseCount > 0)
        {
            settingUpAirDefense = true;
        }
        else
        {
            Debug.Log("You dont have Air Defense support");
        }
    }

    public void FinishAirDefenseDeploying()
    {
        settingUpAirDefense = false;
    }

    public void AirDefenseIsDestroyed()
    {
        settingUpAirDefense = false;
    }

    public void SettingAirDefense(GameObject Ship, bool isHovering)
    {
        if (Ship.GetComponent<Ship_script>().airDiffenceIsActivated == false)
        {
            if (isHovering)
            {
                foreach (Vector2Int cubePos in Ship.GetComponent<Ship_script>().shipAllAirDefensePos)
                {
                    if (grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().isUnderAirDefense == false)
                    {
                        grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().CubeColorChange(Color.red);
                    }
                }
            }
            else
            {
                foreach (Vector2Int cubePos in Ship.GetComponent<Ship_script>().shipAllAirDefensePos)
                {
                    if (grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().isUnderAirDefense == false)
                    {
                        grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().CubeColorChange(Color.white);
                    }

                }
            }
        }
    }

    public void ActivateAirDefense(GameObject ship)
    {
        if(settingUpAirDefense)
        {
            foreach (Vector2Int pos in ship.GetComponent<Ship_script>().shipAllAirDefensePos)
            {
                grid_script.grid_list_player[pos.x, pos.y].GetComponent<Cube_script>().CubeColorChange(Color.green);
                grid_script.grid_list_player[pos.x, pos.y].GetComponent<Cube_script>().isUnderAirDefense = true;
            }
            airDefenseUseCount -= 1;
            FinishAirDefenseDeploying();
        }
    }

    private void CreateShootingRangeList()
    {
        for (int y = 0; y < GRIDHEIGHT; y++)
        {
            for (int x = 0; x < GRIDWIDTH; x++)
            {
                shootingRangeList.Add(new Vector2Int(x, y));
            }
        }
    }





    public void ColorShipAirDefenceTiles(GameObject ship, Color color)
    {
        foreach (Vector2Int pos in ship.GetComponent<Ship_script>().shipAllAirDefensePos)
        {
            grid_script.grid_list_player[pos.x, pos.y].GetComponent<Cube_script>().CubeColorChange(color);
        }
    }




}
