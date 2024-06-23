using System.Collections;
using System.Collections.Generic;
using TMPro;


//using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager_script : MonoBehaviour
{
    [SerializeField] GameObject Grid_obj;
    [SerializeField] GameObject Cube_obj;
    [SerializeField] GameObject playerShips;
    [SerializeField] GameObject enemyShips;
    [SerializeField] GameObject Hit_miss_anim_obj;
    [SerializeField] GameObject Bullet_obj;
    [SerializeField] GameObject Explotion_obj;
    [SerializeField] GameObject DecoreShips;
    [SerializeField] GameObject TargetAnim_obj; // orive sachiroa?
    public GameObject targetAnimObject;  // orive sachiroa?
    [SerializeField] Image WinImage;
    [SerializeField] Image LooseImage;

    [SerializeField] Button startGame_btn;
    [SerializeField] Button random_btn;
    [SerializeField] Button Menu_btn;

    private Grid_script grid_script;
    private Cube_script cube_Script;
    private Ship_script ship_script;
    //private Bullet_script bullet_script;

    public List<Vector2Int> shootingRangeList;

    public int GRIDWIDTH = 8;
    public int GRIDHEIGHT = 8;
    public int DISTANCEBETWEENGRIDS = 10;
    private float WAITTIME = 1.5f;
    private float GAMEOVERWAITTIME = 3f;

    public bool allShipsAreReadyForBattle;
    public bool gameStarted;
    public bool gameOver;
    public bool playerMove;
    public bool enemyMove;
    public bool pause;
    public bool shipIsStillAlive;
    public bool bulletIsInTheAir;
    private bool potencialShipAllignmentIsHorisontal = true;
    [SerializeField] public List<Vector2Int> potencialShootingPosList = new List<Vector2Int>();
    public List<Vector2Int> previousSuccessfullHitList = new List<Vector2Int>();

    public bool sateliteIsWatching;
    public bool settingUpAirDefense;
    public bool canDeployAirDefence;
    public bool playerIsIntersepting;
    public bool interseptionWasSuccess;

    private int sateliteUseCount = 5;
    private int airDefenseUseCount = 3;
    public float rocketNormalSpeed;
    public float rocketInterseptionlSpeed;

    [SerializeField] private TextMeshProUGUI sateliteText;
    [SerializeField] private TextMeshProUGUI airDefenseText;
    


    public Vector3 tileDefaultColor = new Vector3(247,255,255);
    public Vector3 tileAirDefenseHoveringPermitColor = new Vector3(200f, 250f, 250f);
    public Vector3 tileAirDefenseHoveringDenyColor = new Vector3(240f, 200f, 200f);
    public Vector3 tileAirDefenseSetColor = new Vector3(215f, 255f, 220f);
    
    public Vector3 tileEnemyDefaultColor = new Vector3(220, 220, 220);


    // Start is called before the first frame update
    void Start()
    {
        rocketNormalSpeed = 10f;
        rocketInterseptionlSpeed = 3f;


    grid_script = Grid_obj.GetComponent<Grid_script>();
        //bullet_script = Bullet_obj.GetComponent<Bullet_script>();
        targetAnimObject = Instantiate(TargetAnim_obj, new Vector3(0, 0, 0), Quaternion.identity);
        targetAnimObject.SetActive(false);


        CreateGrid();

        TextUpdate(airDefenseText, airDefenseUseCount.ToString());
        TextUpdate(sateliteText, sateliteUseCount.ToString());

        CreateShootingRangeList();

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateGrid()
    {
        grid_script.CreateGrid(Cube_obj);
        //grid_script.CreateGridFrame(Cube_frame);

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
        ActivateAirDefenceOnEnemyShips();
    }

    public void ActivateAirDefenceOnEnemyShips()
    {
        // airDefenceOption1 = 1:1:1
        // airDefenceOption2 = 2:1

        List<GameObject> airDefencePoint1ships = new List<GameObject>();
        List<GameObject> airDefencePoint2ships = new List<GameObject>();
        
        foreach (Transform enemyShip in enemyShips.transform)
        {
            if (enemyShip.GetComponent<Ship_script>().airDefenceNumber == 1)
            {
                airDefencePoint1ships.Add(enemyShip.gameObject);
            }
            else
            {
                airDefencePoint2ships.Add(enemyShip.gameObject);
            }
        }


        int airDefenceOptionNum = Utility_script.random_num(0, 2);
        if (airDefenceOptionNum == 0) // airDefenceOption1 = 1:1:1
        {
            int randomNum1 = Utility_script.random_num(0, airDefencePoint1ships.Count);
            ActivateAirDefense(airDefencePoint1ships[randomNum1]);
            airDefencePoint1ships.RemoveAt(randomNum1);

            int randomNum2 = Utility_script.random_num(0, airDefencePoint1ships.Count);
            ActivateAirDefense(airDefencePoint1ships[randomNum2]);
            airDefencePoint1ships.RemoveAt(randomNum2);

            int randomNum3 = Utility_script.random_num(0, airDefencePoint1ships.Count);
            ActivateAirDefense(airDefencePoint1ships[randomNum3]);
            airDefencePoint1ships.RemoveAt(randomNum3);
        }
        else if (airDefenceOptionNum == 1)  // airDefenceOption12 = 2:1
        {
            int randomNum1 = Utility_script.random_num(0, airDefencePoint1ships.Count);
            ActivateAirDefense(airDefencePoint1ships[randomNum1]);
            airDefencePoint1ships.RemoveAt(randomNum1);

            int randomNum2 = Utility_script.random_num(0, airDefencePoint2ships.Count);
            ActivateAirDefense(airDefencePoint2ships[randomNum2]);
            airDefencePoint1ships.RemoveAt(randomNum2);
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
        PlayerMove();
        settingUpAirDefense = false;
        grid_script.EnableAllGridPosColider();
        grid_script.GameHasStarted(gameStarted);
        DecoreShips.gameObject.SetActive(true);
    }

    public void PauseGame()
    {
        pause = true;
    }

    public void ResumeGame()
    {
        pause = false;
        if (enemyMove)
        { 
            StartCoroutine(EnemyShootsToPlayer()); // continue shooting if it was enemy move before pause
        }
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
                //Cube.GetComponent<SpriteRenderer>().color = Color.cyan;
                Cube.GetComponent<Cube_script>().CubeColorChange(tileAirDefenseHoveringPermitColor);
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
                //neighbour.GetComponent<SpriteRenderer>().color = Color.gray;
                neighbour.GetComponent<Cube_script>().CubeColorChange(tileEnemyDefaultColor);
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
            TextUpdate(sateliteText , sateliteUseCount.ToString());
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

    }

    public void SettingAirDefense(GameObject Ship, bool isHovering)
    {
        if (Ship.GetComponent<Ship_script>().airDiffenceIsActivated == false)
        {
            if (isHovering)
            {
                if (CanDeployAirDefence(Ship))
                {
                    foreach (Vector2Int cubePos in Ship.GetComponent<Ship_script>().shipAllAirDefensePos)
                    {
                        if (grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().isUnderAirDefense == false)
                        {
                            grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().CubeColorChange(tileAirDefenseHoveringPermitColor);
                        }
                    }
                }
                else
                {
                    foreach (Vector2Int cubePos in Ship.GetComponent<Ship_script>().shipAllAirDefensePos)
                    {
                        if (grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().isUnderAirDefense == false)
                        {
                            grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().CubeColorChange(tileAirDefenseHoveringDenyColor);
                        }
                    }
                }
                
            }
            else
            {
                foreach (Vector2Int cubePos in Ship.GetComponent<Ship_script>().shipAllAirDefensePos)
                {
                    if (grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().isUnderAirDefense == false)
                    {
                        //grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().CubeColorChange(Color.white);
                        grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().CubeColorChange(tileDefaultColor);
                    }

                }
            }
        }
    }

    public void ActivateAirDefense(GameObject ship)
    {
        if (ship.GetComponent<Ship_script>().isPlayer)
        {
            if (settingUpAirDefense)
            {
                if (CanDeployAirDefence(ship))
                {
                    foreach (Vector2Int pos in ship.GetComponent<Ship_script>().shipAllAirDefensePos)
                    {
                        grid_script.grid_list_player[pos.x, pos.y].GetComponent<Cube_script>().CubeColorChange(tileAirDefenseSetColor);
                        grid_script.grid_list_player[pos.x, pos.y].GetComponent<Cube_script>().isUnderAirDefense = true;
                    }
                    airDefenseUseCount -= ship.GetComponent<Ship_script>().airDefenceNumber;
                    FinishAirDefenseDeploying();
                    TextUpdate(airDefenseText, airDefenseUseCount.ToString());
                }
            }
        }
        else // activate AirDefence on Enemy ship
        {
            if (CanDeployAirDefence(ship))
            {
                foreach (Vector2Int pos in ship.GetComponent<Ship_script>().shipAllAirDefensePos)
                {
                    grid_script.grid_list_enemy[pos.x - DISTANCEBETWEENGRIDS, pos.y].GetComponent<Cube_script>().CubeColorChange(tileAirDefenseSetColor);
                    grid_script.grid_list_enemy[pos.x - DISTANCEBETWEENGRIDS, pos.y].GetComponent<Cube_script>().isUnderAirDefense = true;
                }
                airDefenseUseCount -= ship.GetComponent<Ship_script>().airDefenceNumber;
                FinishAirDefenseDeploying();
            }
        }
    }

    public bool CanDeployAirDefence(GameObject ship)
    {
        if ((airDefenseUseCount - ship.GetComponent<Ship_script>().airDefenceNumber) >= 0)
        {
            canDeployAirDefence = true;
        }
        else
        {
            canDeployAirDefence = false;
        }
        return canDeployAirDefence;
    }

    private void TextUpdate(TextMeshProUGUI textObj, string textStr)
    {
        textObj.text = textStr;
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

    public void ShootRocket(GameObject VictimObj, GameObject HunterObject, bool AtackerIsPlayer, bool Interseption)
    {
        Vector2 bulletStartingPos;
        bulletStartingPos = new Vector2(0, 0);
        GameObject hunterObject = HunterObject;

        bulletIsInTheAir = true;

        if (AtackerIsPlayer) // Player
        {
            if (Interseption)
            {
                if (hunterObject != null)
                {
                    rocketInterseptionlSpeed = RocketSpeedRandom(); // Temp - ramdomly accelerates interception rocket

                    Vector3Int tempInterceptionStartingPos = new Vector3Int(0, 0, 0);
                    if (Utility_script.random_num(0,2) == 1)
                    { tempInterceptionStartingPos = new Vector3Int(0, 7, 0); }

                    hunterObject = Instantiate(Bullet_obj, tempInterceptionStartingPos, Quaternion.identity); // from this Pos it is the most beautiful to watch interseption
                    hunterObject.GetComponent<Bullet_script>().interseptingRocket = true;
                    hunterObject.GetComponent<Bullet_script>().ShootRocket(VictimObj, rocketInterseptionlSpeed);
                }
            }
            else // Player is just shooting
            {
                hunterObject = Instantiate(Bullet_obj, new Vector3(bulletStartingPos.x, bulletStartingPos.y, 0), Quaternion.identity);
                hunterObject.GetComponent<Bullet_script>().ShootRocket(VictimObj, rocketNormalSpeed);


                GameObject victimObject = VictimObj; // Cube
                TryInterseptRocket(victimObject, hunterObject, !AtackerIsPlayer);

            }
        }
        else //Enemy
        {
            if (Interseption)
            {
                Debug.Log("Enemy is intersepting...");

                rocketInterseptionlSpeed = RocketSpeedRandom(); // Temp - ramdomly accelerates interception rocket

                Vector3Int tempInterceptionStartingPos = new Vector3Int(17, 0, 0);
                if (Utility_script.random_num(0, 2) == 1)
                { tempInterceptionStartingPos = new Vector3Int(17, 7, 0); }

                hunterObject = Instantiate(Bullet_obj, tempInterceptionStartingPos, Quaternion.identity); // from this Pos it is the most beautiful to watch interseption
                hunterObject.GetComponent<Bullet_script>().interseptingRocket = true;
                hunterObject.GetComponent<Bullet_script>().ShootRocket(VictimObj, rocketInterseptionlSpeed);

            }
            else // Enemy is just shooting
            {
                bulletStartingPos = BulletStartingPosCalculate(AtackerIsPlayer);

                if (hunterObject == null)
                {
                    hunterObject = Instantiate(Bullet_obj, new Vector3(bulletStartingPos.x, bulletStartingPos.y, 0), Quaternion.identity);
                }
                GameObject victimObject = VictimObj; // Cube
                 hunterObject.GetComponent<Bullet_script>().ShootRocket(victimObject, rocketNormalSpeed);

                TryInterseptRocket(victimObject, hunterObject, AtackerIsPlayer);
            }
            
        }
    }

    public void TryInterseptRocket(GameObject HunterObject, GameObject VictimObject, bool InterseptorIsPlayer)
    {
        GameObject hunterObject = HunterObject;
        GameObject victimObject = VictimObject;

        if (HunterObject.GetComponent<Cube_script>().isUnderAirDefense) // former victime
        {
            if (InterseptorIsPlayer)
            {
                ShootRocket(VictimObject, HunterObject, true, true); //  VictimObj, HunterObject, AtackerIsPlayer, Interseption
            }
            else // InterseptorIsEnemy
            {
                ShootRocket(VictimObject, HunterObject, false, true); //  VictimObj, HunterObject, AtackerIsPlayer, Interseption
            }
            
        }
    }

    // Temp
    public int RocketSpeedRandom()
    {
        int rocketInterseptionlSpeed = 0;
        int presentage = Utility_script.random_num(0, 100);
        
        //Debug.Log("presentage = " + presentage);
        // Temp for random acceleration
        switch (presentage)
        {
            case < 40:
                rocketInterseptionlSpeed = 6;
                break;
            case < 60:
                rocketInterseptionlSpeed = 7;
                break;
            case < 75:
                rocketInterseptionlSpeed = 8;
                break;
            case < 85:
                rocketInterseptionlSpeed = 9;
                break;
            case < 90:
                rocketInterseptionlSpeed = 10;
                break;
            case < 95:
                rocketInterseptionlSpeed = 11;
                break;
            case < 100:
                rocketInterseptionlSpeed = 12;
                break;
        }

        return rocketInterseptionlSpeed;
    }

    public void RocketUpdate()
    {

        rocketInterseptionlSpeed += 1;
    }

    public void InterseptionSuccessed()
    {
        if (interseptionWasSuccess)
        {
            interseptionWasSuccess = false; // reset

            if (playerMove)
            {
                EnemyMove();
                StartCoroutine(EnemyShootsToPlayer()); // If enemy intercepts Players rocket, it is enemys move and it mast shoot
            }
            else // Enemy move
            {
                PlayerMove(); 
            }
        }
    }

    public void PlayerMove()
    {
        playerMove = true;
        enemyMove = false;
    }

    public void EnemyMove()
    {
        playerMove = false;
        enemyMove = true;
    }

    public Vector2Int BulletStartingPosCalculate(bool isPlayer)
    {
        Vector2Int bulletStartingPos;
        GameObject playerRandomShip = playerShips.transform.GetChild(Random.Range(0, playerShips.transform.childCount)).gameObject;
        GameObject enemyRandomShip = enemyShips.transform.GetChild(Random.Range(0, enemyShips.transform.childCount)).gameObject;

        if (isPlayer)
        {
            Vector2Int tempBulletPos = (playerRandomShip.GetComponent<Ship_script>().shipAllPosArray[playerRandomShip.GetComponent<Ship_script>().shipAllPosArray.Length - 1]);
            bulletStartingPos = tempBulletPos;
        }
        else // Enemy
        {
            Vector2Int tempBulletPos = new Vector2Int(Random.Range(GRIDWIDTH + DISTANCEBETWEENGRIDS , GRIDHEIGHT), Random.Range(0, 8));

            bulletStartingPos = tempBulletPos;
        }
        return bulletStartingPos;
    }

    

    private Vector2Int calculateEnemyShootingPos()
    {
        Vector2Int shootPos;
        if (previousSuccessfullHitList.Count > 0 && potencialShootingPosList.Count > 0)
        {
            int randomIndex = Random.Range(0, potencialShootingPosList.Count);
            shootPos = potencialShootingPosList[randomIndex];
        }
        else
        {
            int randomIndex = Random.Range(0, shootingRangeList.Count);
            shootPos = shootingRangeList[randomIndex];
        }
        return shootPos;
    }

    public void RemoveShootingPos(Vector2Int shootPos)
    {
        potencialShootingPosList.Remove(shootPos);
        shootingRangeList.Remove(shootPos);
    }

    public void Explotion(Vector2 pos)
    {
        GameObject Expl = Instantiate(Explotion_obj, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        StartCoroutine(DestroyExplotionObjects(Expl));
        
    }

    private IEnumerator DestroyExplotionObjects(GameObject Explotion_obj)
    {
        yield return new WaitForSeconds(WAITTIME);
        Destroy(Explotion_obj);
    }

    public void PlayerShootsToEnemy(GameObject TargetCube)
    {
        if (TargetCube.GetComponent<Cube_script>().wasShot == false && !bulletIsInTheAir)
        {
            ShootRocket(TargetCube, null, true, false); // Player ataks enemy manually - VictimObj, HunterObject, AtackerIsPlayer, Interseption
        }
    }

    public void PlayerHitOrMissTarget(GameObject Cube)
    {
        bulletIsInTheAir = false;

        if (Cube.GetComponent<Cube_script>().HitTheTarget())
        {
            PlayerMove();

            Cube.GetComponent<Cube_script>().wasShot = true;
            Cube.GetComponent<Cube_script>().RevealTile();
            HitEnemyShip(Cube.transform.position);
            InstantiateHitMissAnimation(Cube.transform.position, true);
        }
        else
        {
            EnemyMove();

            Cube.GetComponent<Cube_script>().wasShot = true;
            Cube.GetComponent<Cube_script>().RevealTile();
            InstantiateHitMissAnimation(Cube.transform.position, false);

            if (!pause)
            {
                StartCoroutine(EnemyShootsToPlayer());
            }
            
        }
    }

    private IEnumerator EnemyShootsToPlayer()
    {
        Vector2Int shootPos = calculateEnemyShootingPos();
        targetAnimObject.transform.position = new Vector3(shootPos.x, shootPos.y, 0); ;
        targetAnimObject.SetActive(true);

        yield return new WaitForSeconds(WAITTIME);

        if (shootingRangeList.Count == 0)
        {
            Debug.Log("No more positions to shoot at. Game Over.");
            yield break;
        }

        GameObject victimObject = grid_script.grid_list_player[shootPos.x, shootPos.y];

        ShootRocket(victimObject, null, false, false); // VictimObj, HunterObject, AtackerIsPlayer, Interseption
    }

    public void EnemyHitOrMissTarget(GameObject Cube, Vector2Int shootPos)
    {
        bulletIsInTheAir = false;
        if (Cube.GetComponent<Cube_script>().HitTheTarget())
        {
            HitPlayerShip(Cube.transform.position);
            InstantiateHitMissAnimation(Cube.transform.position, true);

            if (shipIsStillAlive)
            {
                UpdateHuntingMode(shootPos);
            }
            else // Player ship is still alive and 
            {
                previousSuccessfullHitList.Clear();
                potencialShootingPosList.Clear();
            }
            RemoveShootingPos(shootPos);

            targetAnimObject.SetActive(false);
            if (!pause)
            {
                StartCoroutine(EnemyShootsToPlayer()); // Continue enemy's turn if it hits
            }
            
        }
        else // enemy missed the target
        {
            RemoveShootingPos(shootPos);
            InstantiateHitMissAnimation(Cube.transform.position, false);
            PlayerMove();

            targetAnimObject.SetActive(false);
        }
    }

    public void HitPlayerShip(Vector2 bulletPos)
    {
        foreach (Transform playerShip in playerShips.transform)
        {
            Ship_script ship_script = playerShip.GetComponent<Ship_script>();
            foreach (Vector2 pos in ship_script.shipAllPosArray)
            {
                if (bulletPos == pos)
                {
                    ship_script.shipHitCount++;
                    if (ship_script.ShipIsAlive())
                    {
                        shipIsStillAlive = true;
                    }
                    else
                    {
                        ship_script.DestroyShip();
                        CheckIfAllShipsAreDestroyed(ship_script.isPlayer);
                        shipIsStillAlive = false;
                        previousSuccessfullHitList.Clear();
                        potencialShootingPosList.Clear();
                    }
                }
            }
        }
    }

    public void HitEnemyShip(Vector2 bulletPos)
    {
        foreach (Transform enemyShip in enemyShips.transform)
        {
            Ship_script ship_script = enemyShip.GetComponent<Ship_script>();
            foreach (Vector2 pos in ship_script.shipAllPosArray)
            {
                if (bulletPos == pos)
                {
                    ship_script.shipHitCount++;
                    if (ship_script.ShipIsAlive())
                    {
                        // Do something
                    }
                    else
                    {
                        ship_script.DestroyShip();
                        CheckIfAllShipsAreDestroyed(ship_script.isPlayer);
                    }
                }
            }
        }
    }

    public void CheckIfAllShipsAreDestroyed(bool isPlayer)
    {
        if (isPlayer)
        {
            int numOfShipsLeft = playerShips.transform.childCount;
            if (numOfShipsLeft == 0)
            {
                StartCoroutine(GameIsOver());
            }
        }
        else // Enemy
        {
            int numOfShipsLeft = enemyShips.transform.childCount;
            if (numOfShipsLeft == 0)
            {
                StartCoroutine(GameIsOver());
            }
        }
    }

    private void InstantiateHitMissAnimation(Vector3 position, bool hit)
    {
        GameObject animObj = Instantiate(Hit_miss_anim_obj, position, Quaternion.identity);
        animObj.GetComponent<Animator>().SetBool(hit ? "Hit" : "Miss", true);
        animObj.transform.SetParent(GameObject.Find("Animation_objects").transform);
    }

    private void UpdateHuntingMode(Vector2Int hitPos)
    {
        previousSuccessfullHitList.Add(hitPos);
        potencialShootingPosList = GetPotentialShootingPositions(hitPos);
    }

    private List<Vector2Int> GetPotentialShootingPositions(Vector2Int hitPos)
    {
        List<Vector2Int> potentialPositions = new List<Vector2Int>();
        int x = hitPos.x;
        int y = hitPos.y;

        // If we have only one hit, add all four adjacent positions
        if (previousSuccessfullHitList.Count == 1)
        {
            AddPotentialPosition(potentialPositions, x - 1, y);
            AddPotentialPosition(potentialPositions, x + 1, y);
            AddPotentialPosition(potentialPositions, x, y - 1);
            AddPotentialPosition(potentialPositions, x, y + 1);
        }
        else if (previousSuccessfullHitList.Count > 1)
        {
            // Determine the orientation of the ship
            Vector2Int firstHitPos = previousSuccessfullHitList[0];
            int firstHitX = firstHitPos.x;
            int firstHitY = firstHitPos.y;

            if (firstHitX == x) // Ship is vertical
            {
                potencialShipAllignmentIsHorisontal = false;

                AddPotentialPosition(potentialPositions, x, y - 1);
                AddPotentialPosition(potentialPositions, x, y + 1);

                if (previousSuccessfullHitList.Contains(new Vector2Int(x, y - 1)))
                {
                    AddPotentialPosition(potentialPositions, x, y - 2);
                }
                if (previousSuccessfullHitList.Contains(new Vector2Int(x, y + 1)))
                {
                    AddPotentialPosition(potentialPositions, x, y + 2);
                }

            }
            else if (firstHitY == y) // Ship is horizontal
            {
                potencialShipAllignmentIsHorisontal = true;

                AddPotentialPosition(potentialPositions, x - 1, y);
                AddPotentialPosition(potentialPositions, x + 1, y);

                if (previousSuccessfullHitList.Contains(new Vector2Int(x - 1, y)))
                {
                    AddPotentialPosition(potentialPositions, x - 2, y);
                }
                if (previousSuccessfullHitList.Contains(new Vector2Int(x + 1, y)))
                {
                    AddPotentialPosition(potentialPositions, x + 2, y);
                }

            }
        }
        else
        {
            Debug.Log("shouldnot be 0 !!!!!!!!!!");
        }
        return potentialPositions;
    }

    private void AddPotentialPosition(List<Vector2Int> positions, int x, int y)
    {
        if (x >= 0 && x < GRIDWIDTH && y >= 0 && y < GRIDHEIGHT)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (shootingRangeList.Contains(pos))
            {
                positions.Add(pos);
            }
        }
    }





    public IEnumerator GameIsOver()
    {
        gameOver = true;
        playerMove = false;
        enemyMove = false;

        yield return new WaitForSeconds(GAMEOVERWAITTIME);

        if (playerShips.transform.childCount == 0)
        {
            Debug.Log("You Lost!!!");
            LooseImage.gameObject.SetActive(true);
        }
        else if (enemyShips.transform.childCount == 0)
        {
            Debug.Log("You Win!!!");
            WinImage.gameObject.SetActive(true);
        }
    }

    public void MenuClick()
    {
        Menu_btn.transform.GetChild(0).gameObject.SetActive(true);
        PauseGame();
    }

    public void MenuCancelButtonClick()
    {
        Menu_btn.transform.GetChild(0).gameObject.SetActive(false);
        ResumeGame();
    }


    public void ExitBTN()
    {
        SceneManager.LoadScene(0);
    }


    public void NewGameBTN()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitBTN()
    {
        SceneManager.LoadScene(0);
    }





    public void ChangeTileColor(GameObject tile, Vector3 color)
    {
        tile.GetComponent<Cube_script>().CubeColorChange(color);
    }




}
