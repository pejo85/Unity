using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public List<Vector2Int> shootingRangeList;

    public int gridWidth = 8;
    public int gridHeight = 8;
    public int distanceBetweenGrids = 10;
    private float waitTime = 1.0f;

    public bool AllShipsAreReady = false;
    public bool gameStarted = false;
    public bool gameOver = false;
    public bool playerMove = false;
    public bool enemyMove = false;
    public bool shipIsStillAlive = false;
    public bool bulletIsInTheAir = false;
    private bool potencialShipAllignmentIsHorisontal = true;
    [SerializeField] public List<Vector2Int> potencialShootingPosList = new List<Vector2Int>();
    public List<Vector2Int> previousSuccessfullHitList = new List<Vector2Int>();

    /*
       ship4 - dome scale: x:0.6 , y:0.3  , z:0
       ship3 - dome scale: x:0.5 , y:0.3  , z:0
       ship2 - dome scale: x:0.4 , y:0.25 , z:0
       ship1 - dome scale: x:0.3 , y:0.25 , z:0
     */

    public bool sateliteIsWatching ;
    public bool settingAirDefense;

    private void Awake()
    {
        grid_script = Grid_obj.GetComponent<Grid_script>();
        cube_Script = Cube_obj.GetComponent<Cube_script>();
    }

    void Start()
    {
        grid_script.CreateGrid(Cube_obj);
        grid_script.DisableEnemyGrid();
        CreateShootingRangeList();

        //sateliteIsWatching = true;
    }

    public void StartGame()
    {
        grid_script.EnableEnemyGrid();
        
        PlaceEnemyShipsRandomly();
        DisableAllShipMovement();
        DisableEveryShipRigidBody();
        random_btn.gameObject.SetActive(false);
        startGame_btn.gameObject.SetActive(false);
        gameStarted = true;
        playerMove = true;
        settingAirDefense = false;
        grid_script.EnableAllGridPosColider();
    }

    private void CreateShootingRangeList()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                shootingRangeList.Add(new Vector2Int(x, y));
            }
        }
    }

    private void ActivatePlayerShips()
    {
        if (playerShips.gameObject.activeSelf == false)
        {
            playerShips.gameObject.SetActive(true);
        }

        foreach (Transform playerShip in playerShips.transform)
        {
            if (playerShip.gameObject.activeSelf == false)
            {
                playerShip.gameObject.SetActive(true);
            }
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

    public void PlacePlayerShipsRandomly()
    {
        ResetShipDeployment();
        ActivatePlayerShips();
        foreach (Transform playerShip in playerShips.transform)
        {
            playerShip.GetComponent<Ship_script>().placeShipRandomly();
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

    public void CheckIfGameIsReady()
    {
        foreach (Transform playerShip in playerShips.transform)
        {
            if (playerShip.GetComponent<Ship_script>().shipIsReadyForBattle == false)
            {
                grid_script.DisableAllGridPosColider();
                return;
            }
            AllShipsAreReady = true;
        }

        if (AllShipsAreReady)
        {
            startGame_btn.GetComponent<Button>().interactable = true;
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

    private void DisableEveryShipRigidBody()
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

    public void ResetShipDeployment()
    {
        ResetShipPos();
        ClearAllGridPos();
    }

    public void ResetShipPos()
    {
        foreach (Transform playerShip in playerShips.transform)
        {
            if (playerShip != null)
            {
                playerShip.GetComponent<Ship_script>().resetShip();
            }
        }
    }

    private void ClearAllGridPos()
    {
        grid_script.resetGrid();
    }

    public void MakeFreeOcupiedPos(Vector2Int[] shipAllPos, bool isEnemy)
    {
        grid_script.makeFreeGridPos(shipAllPos, isEnemy);
    }

    public bool IsValidPos(Vector2Int[] shipAllPos, bool isEnemy)
    {
        return grid_script.isValidPos(shipAllPos, isEnemy);
    }

    public bool IsInsideGrid(Vector2Int pos, bool isEnemy)
    {
        return grid_script.PosIsInsideGrid(pos, isEnemy);
    }

    public void OcupyGridPos(Vector2Int[] shipAllPos, bool isEnemy)
    {
        grid_script.ocupyGridPos(shipAllPos, isEnemy);
    }

    public void sateliteRevealsEnemyTiles(GameObject clickedCube)
    {
        GameObject[] neighbourList = new GameObject[4];

        neighbourList = CalculateClickedCubesSateliteNeighbours(clickedCube);

        foreach (GameObject neighbour in neighbourList)
        {
            neighbour.GetComponent<Cube_script>().RevealTile();
        }
    }

    public void SateliteHoversOverTile(GameObject[] HoveredCubeNeighbourList)
    {
        foreach (GameObject neighbour in HoveredCubeNeighbourList)
        {
            if (neighbour.GetComponent<Cube_script>().isRevealed == false)
            {
                neighbour.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
            
        }
    }

    public void SateliteHoversOverExitTile(GameObject[] HoveredCubeNeighbourList)
    {
        foreach (GameObject neighbour in HoveredCubeNeighbourList)
        {
            if (neighbour.GetComponent<Cube_script>().isRevealed == false)
            {
                neighbour.GetComponent<SpriteRenderer>().color = Color.gray;
            }
            
        }
    }

    public void sateliteClick()
    {
        sateliteIsWatching = true;
    }

    public void airDefenseClick()
    {
        settingAirDefense = true;
    }

    public void SettingAirDefense(GameObject Ship, bool isHovering)
    {
        //Debug.Log("Ship --- " + Ship);

        List<Vector2Int> AirDefenseneighbourList = CalculateClickedShipsAirDefenseNeighbours(Ship);

        if (Ship.GetComponent<Ship_script>().airDiffenceisOn == false)
        {
            if (isHovering)
            {


                foreach (Vector2Int cubePos in AirDefenseneighbourList)
                {
                    if (grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().isUnderAirDefense == false)
                    {
                        grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().CubeColor(Color.red);
                    }
                    

                }

                //foreach (Vector2 cubePos in shipAllPos)
                //{
                //    grid_script.grid_list_player[(int)cubePos.x, (int)cubePos.y].GetComponent<Cube_script>().CubeColor(Color.red);
                //}
            }
            else
            {
                foreach (Vector2Int cubePos in AirDefenseneighbourList)
                {
                    if (grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().isUnderAirDefense == false)
                    {
                        grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().CubeColor(Color.white);
                    }
                    
                }
                //foreach (Vector2 cubePos in shipAllPos)
                //{
                //    grid_script.grid_list_player[(int)cubePos.x, (int)cubePos.y].GetComponent<Cube_script>().CubeColor(Color.white);
                //}
            }
        }
        
        
    }

    public void SetAirDefence(GameObject Ship, bool isHovering)
    {
        List<Vector2Int> AirDefenseneighbourList = CalculateClickedShipsAirDefenseNeighbours(Ship);

        foreach (Vector2Int cubePos in AirDefenseneighbourList)
        {
            //Debug.Log("cubePos === " + cubePos);
            grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().isUnderAirDefense = true;
            grid_script.grid_list_player[cubePos.x, cubePos.y].GetComponent<Cube_script>().CubeColor(Color.green);
        }
        settingAirDefense = false;
        Ship.GetComponent<Ship_script>().airDiffenceisOn = true;
    }

    public List<Vector2Int> CalculateClickedShipsAirDefenseNeighbours(GameObject heveredShip)
    {
        //int ShipsAirDefenseNeighboursNum = clickedShip.GetComponent<Ship_script>().shipAllAerDefensePos.Count;
        //Vector2Int[] neighbourList = new Vector2Int[ShipsAirDefenseNeighboursNum];


        List<Vector2Int> neighbourList = new List<Vector2Int>();

        neighbourList = heveredShip.GetComponent<Ship_script>().CalculateshipAllAerDefensePos(heveredShip);


        return neighbourList;
    }

    public bool PosIsInsideGrid(Vector2Int pos, bool isEnemy)
    {
        return grid_script.PosIsInsideGrid(pos, isEnemy);
    }

    public GameObject[] CalculateClickedCubesSateliteNeighbours(GameObject clickedCube)
    {
        GameObject[] neighbourList = new GameObject[4];
        Vector2Int[] neighbourList11 = new Vector2Int[4];

        int x = (int)clickedCube.transform.position.x;
        int y = (int)clickedCube.transform.position.y;

        Vector2Int neighbour1Pos = new Vector2Int(x + 1, y);
        Vector2Int neighbour2Pos = new Vector2Int(x + 1, y-1);
        Vector2Int neighbour3Pos = new Vector2Int(x, y-1);

        neighbourList11[0] = new Vector2Int((int)clickedCube.transform.position.x , (int)clickedCube.transform.position.y);
        neighbourList11[1] = neighbour1Pos;
        neighbourList11[2] = neighbour2Pos;
        neighbourList11[3] = neighbour3Pos;

        // Right
        if (grid_script.sateliteNeighboursIsValidPos(neighbourList11, true) )
        {
            for (int i = 0; i < neighbourList11.Length; i++)
            {
                neighbourList[i] = grid_script.grid_list_enemy[neighbourList11[i].x - distanceBetweenGrids, neighbourList11[i].y];
            }
        }

        else
        {
            // Down 
            neighbour1Pos = new Vector2Int(x , y-1);
            neighbour2Pos = new Vector2Int(x - 1, y - 1);
            neighbour3Pos = new Vector2Int(x -1, y);

            neighbourList11[0] = new Vector2Int((int)clickedCube.transform.position.x, (int)clickedCube.transform.position.y);
            neighbourList11[1] = neighbour1Pos;
            neighbourList11[2] = neighbour2Pos;
            neighbourList11[3] = neighbour3Pos;

            if (grid_script.sateliteNeighboursIsValidPos(neighbourList11, true))
            {
                for (int i = 0; i < neighbourList11.Length; i++)
                {
                    neighbourList[i] = grid_script.grid_list_enemy[neighbourList11[i].x - distanceBetweenGrids, neighbourList11[i].y];
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

                if (grid_script.sateliteNeighboursIsValidPos(neighbourList11, true))
                {
                    for (int i = 0; i < neighbourList11.Length; i++)
                    {
                        neighbourList[i] = grid_script.grid_list_enemy[neighbourList11[i].x - distanceBetweenGrids, neighbourList11[i].y];
                    }
                }
                else
                {
                    // Up
                    neighbour1Pos = new Vector2Int(x , y+1);
                    neighbour2Pos = new Vector2Int(x + 1, y + 1);
                    neighbour3Pos = new Vector2Int(x +1, y);

                    neighbourList11[0] = new Vector2Int((int)clickedCube.transform.position.x, (int)clickedCube.transform.position.y);
                    neighbourList11[1] = neighbour1Pos;
                    neighbourList11[2] = neighbour2Pos;
                    neighbourList11[3] = neighbour3Pos;

                    if (grid_script.sateliteNeighboursIsValidPos(neighbourList11, true))
                    {
                        for (int i = 0; i < neighbourList11.Length; i++)
                        {
                            neighbourList[i] = grid_script.grid_list_enemy[neighbourList11[i].x - distanceBetweenGrids, neighbourList11[i].y];
                        }
                    }
                }

            }
        }
        return neighbourList;
    }

    public void PlayerShootsToEnemy(GameObject Cube)
    {
        if (Cube.GetComponent<Cube_script>().wasShot == false)
        {
            shootRocket(Cube, true);
        }
    }

    public void shootRocket(GameObject Cube, bool isPlayer)
    {
        Vector3 bulletStartingPos = isPlayer ? new Vector3(0, 0, 0) : new Vector3(17, 0, 0);
        GameObject bullet = Instantiate(Bullet_obj, bulletStartingPos, Quaternion.identity);
        bullet.GetComponent<bullet_script>().targetPosition = Cube.transform.position;
        bullet.GetComponent<bullet_script>().isPlayerShot = isPlayer; // Pass this info to the bullet script
        bullet.transform.position = bulletStartingPos;
        bulletIsInTheAir = true;
    }

    public void PlayerHitOrMissTarget(GameObject Cube)
    {
        if (Cube.GetComponent<Cube_script>().hitTheTarget())
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

    private void InstantiateHitMissAnimation(Vector3 position, bool hit)
    {
        GameObject animObj = Instantiate(Hit_miss_anim_obj, position, Quaternion.identity);
        animObj.GetComponent<Animator>().SetBool(hit ? "Hit" : "Miss", true);
        animObj.transform.SetParent(GameObject.Find("Animation_objects").transform);
    }

    private IEnumerator EnemyShootsToPlayer()
    {
        yield return new WaitForSeconds(waitTime);

        if (shootingRangeList.Count == 0)
        {
            Debug.Log("No more positions to shoot at. Game Over.");
            yield break;
        }

        Vector2Int shootPos = calculateEnemyShootingPos();
        GameObject Cube = grid_script.grid_list_player[shootPos.x, shootPos.y];

        shootingRangeList.Remove(shootPos);
        shootRocket(Cube, false);
    }

    public void EnemyHitOrMissTarget(GameObject Cube, Vector2Int shootPos)
    {

        if (Cube.GetComponent<Cube_script>().hitTheTarget())
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

    private void AddExtraPositions(List<Vector2Int> potentialPositions, int x, int y)
    {
        if (previousSuccessfullHitList.Contains(new Vector2Int(x, y - 1)))
        {
            AddPotentialPosition(potentialPositions, x, y - 2);
        }
        if (previousSuccessfullHitList.Contains(new Vector2Int(x, y + 1)))
        {
            AddPotentialPosition(potentialPositions, x, y + 2);
        }
    }

    private void AddPotentialPosition(List<Vector2Int> positions, int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            Vector2Int  pos = new Vector2Int(x, y);
            if (shootingRangeList.Contains(pos))
            {
                positions.Add(pos);
            }
        }                                                                   
    }

    public void HitEnemyShip(Vector2 bulletPos)
    {
        foreach (Transform enemyShip in enemyShips.transform)
        {
            Ship_script ship_script = enemyShip.GetComponent<Ship_script>();
            foreach (Vector2 pos in ship_script.shipAllPos)
            {
                if (bulletPos == pos)
                {
                    ship_script.hitCount++;
                    if (ship_script.shipIsAlive())
                    {
                        // Do something
                    }
                    else
                    {
                        ship_script.destroyShip();
                        CheckIfAllShipsAreDestroyed(ship_script.isEnemy);
                    }
                }
            }
        }
    }

    public void HitPlayerShip(Vector2 bulletPos)
    {
        foreach (Transform playerShip in playerShips.transform)
        {
            Ship_script ship_script = playerShip.GetComponent<Ship_script>();
            foreach (Vector2 pos in ship_script.shipAllPos)
            {
                if (bulletPos == pos)
                {
                    ship_script.hitCount++;
                    if (ship_script.shipIsAlive())
                    {
                        shipIsStillAlive = true;
                    }
                    else
                    {
                        ship_script.destroyShip();
                        CheckIfAllShipsAreDestroyed(ship_script.isEnemy);
                        shipIsStillAlive = false;
                        previousSuccessfullHitList.Clear();
                        potencialShootingPosList.Clear();
                    }
                }
            }
        }
    }

    public void CheckIfAllShipsAreDestroyed(bool isEnemy)
    {
        if (isEnemy)
        {
            int numOfShipsLeft = enemyShips.transform.childCount;
            if (numOfShipsLeft == 0)
            {
                GameIsOver();
            }
        }
        else
        {
            int numOfShipsLeft = playerShips.transform.childCount;
            if (numOfShipsLeft == 0)
            {
                GameIsOver();
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
}
