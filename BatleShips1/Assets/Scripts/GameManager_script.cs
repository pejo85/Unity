using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;

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
    [SerializeField] GameObject Explotion_obj;
    [SerializeField] GameObject TargetAnim_obj;

    [SerializeField] Button startGame_btn;
    [SerializeField] Button random_btn;

    private Grid_script grid_script;
    private Cube_script cube_Script;
    private Ship_script ship_script;
    //private Bullet_script bullet_script;

    public List<Vector2Int> shootingRangeList;

    public int GRIDWIDTH = 8;
    public int GRIDHEIGHT = 8;
    public int DISTANCEBETWEENGRIDS = 10;
    private float WAITTIME = 1.5f;

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
    public bool playerIsIntersepting;

    private int sateliteUseCount = 5;
    private int airDefenseUseCount = 3;

    [SerializeField] private TextMeshProUGUI sateliteText;
    [SerializeField] private TextMeshProUGUI airDefenseText;
    public GameObject targetAnimObject;

    // Start is called before the first frame update
    void Start()
    {
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
        if (settingUpAirDefense)
        {
            foreach (Vector2Int pos in ship.GetComponent<Ship_script>().shipAllAirDefensePos)
            {
                grid_script.grid_list_player[pos.x, pos.y].GetComponent<Cube_script>().CubeColorChange(Color.green);
                grid_script.grid_list_player[pos.x, pos.y].GetComponent<Cube_script>().isUnderAirDefense = true;
            }
            airDefenseUseCount -= 1;
            FinishAirDefenseDeploying();
            TextUpdate(airDefenseText, airDefenseUseCount.ToString());
        }
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

    public void ShootRocket() //Temp from button enemy shoots player
    {
        Vector2Int shootPos = calculateEnemyShootingPos(); // Shoot from random position
        GameObject victimObject = grid_script.grid_list_player[shootPos.x, shootPos.y];

        shootingRangeList.Remove(shootPos);

        ShootRocket(victimObject, null, false, false); // VictimObj, HunterObject, AtackerIsPlayer, Interseption
    }

    public void ShootRocket(GameObject VictimObj, GameObject HunterObject, bool AtackerIsPlayer, bool Interseption)
    {
        Vector2 bulletStartingPos;
        GameObject hunterObject = HunterObject;

        bulletIsInTheAir = true;

        if (AtackerIsPlayer) // Player
        {
            if (Interseption)
            {
                bulletStartingPos = HunterObject.transform.position;

                if (hunterObject != null)
                {
                    hunterObject = Instantiate(Bullet_obj, new Vector3(0, 0, 0), Quaternion.identity); // from this Pos iti s most beautiful to watch interseption
                    //hunterObject = Instantiate(Bullet_obj, hunterObject.transform.position, Quaternion.identity);
                    hunterObject.GetComponent<Bullet_script>().interseptingRocket = true;
                    hunterObject.GetComponent<Bullet_script>().ShootRocket(VictimObj);

                    if (InterseptionSuccessed()) // if player intersepts enemys rocket, then it is players move
                    {
                        playerMove = true;
                        enemyMove = false;
                    }
                }
            }
            else // Player is just shooting
            {
                bulletStartingPos = BulletStartingPosCalculate(AtackerIsPlayer);
                hunterObject = Instantiate(Bullet_obj, new Vector3(bulletStartingPos.x, bulletStartingPos.y, 0), Quaternion.identity);
                hunterObject.GetComponent<Bullet_script>().ShootRocket(VictimObj);

                TryInterseptRocket(VictimObj, hunterObject, AtackerIsPlayer);
            }

        }
        else //Enemy
        {
            if (Interseption)
            {

            }
            else // Enemy is just shooting
            {
                bulletStartingPos = BulletStartingPosCalculate(AtackerIsPlayer);

                if (hunterObject == null)
                {
                    hunterObject = Instantiate(Bullet_obj, new Vector3(bulletStartingPos.x, bulletStartingPos.y, 0), Quaternion.identity);
                }
                GameObject victimObject = VictimObj; // Cube
                 hunterObject.GetComponent<Bullet_script>().ShootRocket(victimObject);

                TryInterseptRocket(victimObject, hunterObject, AtackerIsPlayer);
            }
            
        }
    }

    public bool InterseptionSuccessed()
    {
        return true;
    }

    //public void ShootRocket(GameObject victimObj, GameObject hunterObject , bool isPlayer, bool interseption)
    //{
    //    //Vector2Int bulletStartingPos = BulletStartingPosCalculate(isPlayer);
    //    Vector2 bulletStartingPos = victimObj.transform.position;
    //
    //    
    //   GameObject bullet = Instantiate(Bullet_obj, new Vector3(bulletStartingPos.x, bulletStartingPos.y, 0), Quaternion.identity);
    //
    //    Bullet_script bullet_script = bullet.GetComponent<Bullet_script>();
    //
    //    if(victimObj.GetComponent<Cube_script>().isUnderAirDefense)
    //    {
    //        bullet_script.interseptingRocket = true;
    //        bullet_script.speed = 15f;
    //    }
    //    bullet_script.ShootRocket(hunterObject);
    //
    //
    //    //Vector2Int targetPosTemp = new Vector2Int(15,0);
    //    //bullet.GetComponent<bullet_script>().targetPosition = Cube.transform.position;
    //    //bullet.GetComponent<bullet_script>().isPlayerShot = isPlayer; // Pass this info to the bullet script
    //    //bullet.transform.position = new Vector3(bulletStartingPos.x, bulletStartingPos.y, 0);
    //
    //    //bullet.GetComponent<Bullet_script>().ShootRocket(targetPosTemp);
    //
    //    bulletIsInTheAir = true;
    //}

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

    public void TryInterseptRocket(GameObject HunterObject, GameObject VictimObject, bool InterseptorIsPlayer)
    {
        GameObject hunterObject = HunterObject;
        GameObject victimObject = VictimObject;
        if (HunterObject.GetComponent<Cube_script>().isUnderAirDefense) // former victime
        {
            ShootRocket( VictimObject, HunterObject, true, true); //  VictimObj, HunterObject, AtackerIsPlayer, Interseption
        }
    }

    private Vector2Int calculateEnemyShootingPos()
    {
        Vector2Int shootPos;
        if (previousSuccessfullHitList.Count > 0 && potencialShootingPosList.Count > 0)
        {
            int randomIndex = Random.Range(0, potencialShootingPosList.Count);
            shootPos = potencialShootingPosList[randomIndex];
            potencialShootingPosList.RemoveAt(randomIndex);
        }
        else
        {

            int randomIndex = Random.Range(0, shootingRangeList.Count);
            shootPos = shootingRangeList[randomIndex];
        }
        return shootPos;
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
            ShootRocket(TargetCube, null, true, false); // Player ataks enemy manually
        }
    }

    public void PlayerHitOrMissTarget(GameObject Cube)
    {
        bulletIsInTheAir = false;

        if (Cube.GetComponent<Cube_script>().HitTheTarget())
        {
            playerMove = true;
            enemyMove = false;
            Cube.GetComponent<Cube_script>().wasShot = true;
            Cube.GetComponent<Cube_script>().RevealTile();
            HitEnemyShip(Cube.transform.position);
            InstantiateHitMissAnimation(Cube.transform.position, true);
        }
        else
        {
            playerMove = false;
            enemyMove = true;

            Cube.GetComponent<Cube_script>().wasShot = true;
            Cube.GetComponent<Cube_script>().RevealTile();
            InstantiateHitMissAnimation(Cube.transform.position, false);
            StartCoroutine(EnemyShootsToPlayer());
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

        shootingRangeList.Remove(shootPos);
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
            else
            {
                previousSuccessfullHitList.Clear();
                potencialShootingPosList.Clear();
            }

            StartCoroutine(EnemyShootsToPlayer()); // Continue enemy's turn if it hits
        }
        else
        {
            InstantiateHitMissAnimation(Cube.transform.position, false);
            playerMove = true;
            enemyMove = false;
        }
        //Debug.Log("111 - " + TargetAnim_obj.transform.position);
        targetAnimObject.SetActive(false);

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
                GameIsOver();
            }
        }
        else // Enemy
        {
            int numOfShipsLeft = enemyShips.transform.childCount;
            if (numOfShipsLeft == 0)
            {
                GameIsOver();
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





    public void GameIsOver()
    {
        gameOver = true;
        playerMove = false;
        enemyMove = false;
        if (playerShips.transform.childCount == 0)
        {
            Debug.Log("You Lost!!!");
        }
        else if (enemyShips.transform.childCount == 0)
        {
            Debug.Log("You Win!!!");
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
