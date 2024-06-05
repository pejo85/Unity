using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;



// [] Cover enemy grid with fog...
// [X] Player ship rotation on click

/*   
 *    X
 *   (X)()()()  - ship4
 *   (X)()()    - ship3
 *   in this situation, AI thinks, that it was one vertical ship
 */


public class GameManager_script : MonoBehaviour
{


    //[SerializeField] Button reset_btn;

    [SerializeField] GameObject Grid_obj;
    [SerializeField] GameObject Cube_obj;
    [SerializeField] GameObject playerShips;
    [SerializeField] GameObject enemyShips;
    [SerializeField] GameObject Hit_miss_anim_obj;

    [SerializeField] Button startGame_btn;
    [SerializeField] Button random_btn;

    private Grid_script grid_script;
    private Cube_script cube_Script;
    private Ship_script ship_script;

    //public Vector2 [,] shootingRange;
    public List <Vector2>  shootingRangeList;

    public int gridWidth = 8;
    public int gridHeight = 8;
    public int distanceBetweenGrids = 10;
    private float waitTime = 1.0f;

    private bool AllShipsAreReady = false;
    public bool gameStarted = false;
    public bool gameOver = false;
    public bool playerMove = false;
    public bool enemyMove = false;
    public bool shipIsStillAlive = false;
    private bool potencialShipAllignmentIsHorisontal = true;
    [SerializeField] public List<Vector2> potencialShootingPosList = new List<Vector2>();
    public List<Vector2> previousSuccessfullHitList = new List<Vector2>();

    //public Animator animator;

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

    }
    // activates from Button
    public void StartGame()
    {
        grid_script.EnableEnemyGrid();

        PlaceEnemyShipsRandomly();
        DisableAllShipMovement();
        DisableEnemyShipRigidBody();
        random_btn.gameObject.SetActive(false);
        startGame_btn.gameObject.SetActive(false);
        gameStarted = true;
        playerMove = true;
    }

    private void CreateShootingRangeList()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //shootingRange[x, y] = new Vector2(x,y);
                shootingRangeList.Add(new Vector2(x, y));
            }
        }

    }

    private void ActivatePlayerShips()
    {
        // If playerShips gameobject was disabled to enable it
        if (playerShips.gameObject.activeSelf == false)
        {
            playerShips.gameObject.SetActive(true);
        }

        // If some childObject ships inside playerShips gameobject was disabled to enable it
        foreach (Transform playerShip in playerShips.transform)
        {
            if (playerShip.gameObject.activeSelf == false)
            {
                playerShip.gameObject.SetActive(true);
            }

        }
    }

    //private void ShipLengthBlocksPosCalculate()
    //{
    //    Vector2 pos = Utility_script.round_pos(transform.position);
    //
    //    foreach (Transform playerShip in playerShips.transform)
    //    {
    //        playerShip.GetComponent<Ship_script>().shipAllPosCalculate(pos);
    //    }
    //}

    public void PlaceEnemyShipsRandomly()
    {
        ActivateEnemyShips();

        // Place Enemy ship on the grid
        foreach (Transform enemyShip in enemyShips.transform)
        {
            enemyShip.GetComponent<Ship_script>().placeShipRandomly();
        }
    }

    // activates from Button
    public void PlacePlayerShipsRandomly()
    {
        ResetShipDeployment();
        ActivatePlayerShips();

        // Place Enemy ship on the grid
        foreach (Transform playerShip in playerShips.transform)
        {
            playerShip.GetComponent<Ship_script>().placeShipRandomly();
        }  
    }

    private void ActivateEnemyShips()
    {
        // If EnemySips gameobject was disabled to enable it
        if (enemyShips.gameObject.activeSelf == false)
        {
            enemyShips.gameObject.SetActive(true);
        }

        // If some childObject ships inside EnemySips gameobject was disabled to enable it
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
                return;
            }
            AllShipsAreReady = true;
        }

        if (AllShipsAreReady == true)
        {
            // Activate START Button
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

    private void DisableEnemyShipRigidBody()
    {
        foreach (Transform enemyShip in enemyShips.transform)
        {
            enemyShip.gameObject.GetComponent<BoxCollider2D>().enabled = false;
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

    public void MakeFreeOcupiedPos(Vector2[] shipAllPos, bool isEnemy)
    {
        grid_script.makeFreeGridPos(shipAllPos, isEnemy);
    }

    public bool IsValidPos(Vector2[] shipAllPos, bool isEnemy)
    {
       return grid_script.isValidPos(shipAllPos, isEnemy);
    }

    public void OcupyGridPos(Vector2[] shipAllPos, bool isEnemy)
    {
        grid_script.ocupyGridPos(shipAllPos, isEnemy);
    }

    public void PlayerShootsToEnemy(GameObject Cube)
    {
        if (Cube.GetComponent<Cube_script>().wasShot == false)
        {
            //Debug.Log("Player is shooting on " + Cube.transform.position);
            if (Cube.GetComponent<Cube_script>().hitTheTarget())
            {
                playerMove = true;
                enemyMove = false;
                Cube.GetComponent<Cube_script>().wasShot = true;
                Cube.GetComponent<SpriteRenderer>().sortingOrder = 0;

                HitEnemyShip(Cube.transform.position);

                GameObject Hit_miss_anim_obj_current = Instantiate(Hit_miss_anim_obj, Cube.transform.position, Quaternion.identity);
                Hit_miss_anim_obj_current.GetComponent<Animator>().SetBool("Hit", true);
                Hit_miss_anim_obj_current.transform.SetParent(GameObject.Find("Animation_objects").transform);

            }
            else
            {

                playerMove = false;
                enemyMove = true;

                Cube.GetComponent<Cube_script>().wasShot = true;
                GameObject Hit_miss_anim_obj_current = Instantiate(Hit_miss_anim_obj, Cube.transform.position, Quaternion.identity);
                Hit_miss_anim_obj_current.GetComponent<Animator>().SetBool("Miss", true);
                Hit_miss_anim_obj_current.transform.SetParent(GameObject.Find("Animation_objects").transform);

                EnemyShootsToPlayer();

            }
        }
        

    }


    /*
     * For the first time - enemy shoots player from shootingRangeList randomly chosen positions;
     * remove this shooting position from shootingRangeList;
     * if it was hit and ship is still alive:
     *  save vector2 previousSuccessfullHitPos
     *  save vector 2 potentialNextPositions (max 4) in potencialShootingPosList (if some of this pos will be in shootingRangeList because earlyer was made shot, then remove it from potencialShootingPosList)
     *  NextShootPos vill be randomly chosen pos from potentialNextPositions
     *  shoot NextShootPos and remove it from potentialNextPositions and from shootingRangeList
     
     
     
     */

    public void EnemyShootsToPlayer()
    {
        StartCoroutine(EnemyShootsToPlayerCoroutine());
    }

    private IEnumerator EnemyShootsToPlayerCoroutine()
    {
       //yield return new WaitForSeconds(1.0f);

        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            if (shootingRangeList.Count == 0)
            {
                Debug.Log("No more positions to shoot at. Game Over.");
                yield break; // Exit the coroutine
            }

            Vector2 shootPos;

            // Check if we are in hunting mode
            if (previousSuccessfullHitList.Count > 0 && potencialShootingPosList.Count > 0)
            {
                int randomIndex = Random.Range(0, potencialShootingPosList.Count);
                shootPos = potencialShootingPosList[randomIndex];
                potencialShootingPosList.RemoveAt(randomIndex);

                shootingRangeList.RemoveAt(shootingRangeList.IndexOf(shootPos));
               // Debug.Log("shooting at - " + shootPos);
            }
            else
            {
                int randomIndex = Random.Range(0, shootingRangeList.Count);
                shootPos = shootingRangeList[randomIndex];
                shootingRangeList.RemoveAt(randomIndex);
                //Debug.Log("shooting attt - " + shootPos);
            }

            GameObject Cube = grid_script.grid_list_player[(int)shootPos.x, (int)shootPos.y];
            if (Cube.GetComponent<Cube_script>().hitTheTarget())
            {
                //yield return new WaitForSeconds(0.1f);
                Debug.Log("Hit the target");
                HitPlayerShip(Cube.transform.position);

                GameObject Hit_miss_anim_obj_current = Instantiate(Hit_miss_anim_obj, Cube.transform.position, Quaternion.identity);
                Hit_miss_anim_obj_current.GetComponent<Animator>().SetBool("Hit", true);
                Hit_miss_anim_obj_current.transform.SetParent(GameObject.Find("Animation_objects").transform);

                // Only update hunting mode if ship is still alive
                if (shipIsStillAlive)
                {
                    UpdateHuntingMode(shootPos);
                }
                else
                {
                    // Clear the lists and continue shooting randomly
                    previousSuccessfullHitList.Clear();
                    potencialShootingPosList.Clear();
                    Debug.Log("Ship sunk! Continuing enemy's turn with a random shot.");
                }

                // Continue enemy's turn after a successful hit
                if (!gameOver)
                {
                    //yield return new WaitForSeconds(0.1f); // Delay before the next shot
                    continue; // Repeat the loop for the next shot
                }
            }
            else
            {
                Debug.Log("missed the target");
                GameObject Hit_miss_anim_obj_current = Instantiate(Hit_miss_anim_obj, Cube.transform.position, Quaternion.identity);
                Hit_miss_anim_obj_current.GetComponent<Animator>().SetBool("Miss", true);
                Hit_miss_anim_obj_current.transform.SetParent(GameObject.Find("Animation_objects").transform);
                playerMove = true;
                enemyMove = false;
                //yield return new WaitForSeconds(0.1f);
                yield break; // Exit the coroutine, as it's now the player's turn
            }
        }
    }

    private void UpdateHuntingMode(Vector2 hitPos)
    {
        previousSuccessfullHitList.Add(hitPos);
        potencialShootingPosList = GetPotentialShootingPositions(hitPos);

        if (gameOver)
        {
            Debug.Log("Game over - Enemy WINS !!!");
        }
        //else if (shootingRangeList.Count > 0 && enemyMove)
        //{
        //    EnemyShootsToPlayer();
        //}
        else
        {
            // Do something
        }
    }

    
    private List<Vector2> GetPotentialShootingPositions(Vector2 hitPos)
    {
        List<Vector2> potentialPositions = new List<Vector2>();
        int x = (int)hitPos.x;
        int y = (int)hitPos.y;

        Debug.Log("previousSuccessfullHitList.Count = " + previousSuccessfullHitList.Count);

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
            Vector2 firstHitPos = previousSuccessfullHitList[0];
            int firstHitX = (int)firstHitPos.x;
            int firstHitY = (int)firstHitPos.y;

            if (firstHitX == x) // Ship is vertical
            {
                potencialShipAllignmentIsHorisontal = false;

                AddPotentialPosition(potentialPositions, x, y - 1);
                AddPotentialPosition(potentialPositions, x, y + 1);

                if (previousSuccessfullHitList.Contains(new Vector2(x , y - 1)))
                {
                    AddPotentialPosition(potentialPositions, x , y - 2);
                }
                if (previousSuccessfullHitList.Contains(new Vector2(x , y + 1)))
                {
                    AddPotentialPosition(potentialPositions, x , y + 2);
                }

            }
            else if (firstHitY == y) // Ship is horizontal
            {
                potencialShipAllignmentIsHorisontal = true;

                AddPotentialPosition(potentialPositions, x - 1, y);
                AddPotentialPosition(potentialPositions, x + 1, y);

                if (previousSuccessfullHitList.Contains(new Vector2(x - 1, y) ))
                {
                    AddPotentialPosition(potentialPositions, x - 2, y);
                }
                if (previousSuccessfullHitList.Contains(new Vector2(x + 1, y)))
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

    //private void addPotencialSeparateShipsPos()
    //{
    //    int potencialShipsNum = previousSuccessfullHitList.Count;
    //
    //    if (potencialShipAllignmentIsHorisontal) // it was mistakenly tought by AI before and now it is time to correct
    //    {
    //        potencialShipAllignmentIsHorisontal = false;
    //
    //    }
    //    else
    //    {
    //        potencialShipAllignmentIsHorisontal = true;
    //    }
    //
    //}

    private void AddPotentialPosition(List<Vector2> positions, int x, int y)
    {
        Debug.Log(x + " , " + y);
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            Vector2 pos = new Vector2(x, y);
            if (shootingRangeList.Contains(pos))
            {
                Debug.Log("adding "+ pos+ " to the potentialPositions");
                positions.Add(pos);
            }
        }
    }

    public void HitEnemyShip(Vector2 bulletPos)
    {
        foreach (Transform enemyShip in enemyShips.transform)
        {
            for (int i = 0; i < enemyShip.GetComponent<Ship_script>().shipAllPos.Length; i++)
            {
                Ship_script ship_script = enemyShip.GetComponent<Ship_script>();

                if (bulletPos == ship_script.shipAllPos[i])
                {
                   // Debug.Log("It was hit on pos " + bulletPos);
                    ship_script.hitCount++;

                    if (ship_script.shipIsAlive() )
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
            for (int i = 0; i < playerShip.GetComponent<Ship_script>().shipAllPos.Length; i++)
            {
                Ship_script ship_script = playerShip.GetComponent<Ship_script>();

                if (bulletPos == ship_script.shipAllPos[i])
                {
                    ship_script.hitCount++;

                    if (ship_script.shipIsAlive())
                    {
                        // Do something..
                        shipIsStillAlive = true;
                    }
                    else
                    {
                        ship_script.destroyShip();
                        CheckIfAllShipsAreDestroyed(ship_script.isEnemy);
                        shipIsStillAlive = false;

                        // Clear hit tracking lists after sinking a ship
                        previousSuccessfullHitList.Clear();
                        potencialShootingPosList.Clear();
                    }
                }
            }
        }
    }

    public void CheckIfAllShipsAreDestroyed(bool isEnemy)
    {
        // Player Wins
        if (isEnemy) 
        {
            int numOfShipsLeft = enemyShips.transform.childCount;
            if (numOfShipsLeft == 0)
            {
                GameIsOver();
            }
        }
        // Enemy Wins
        else
        {
            int numOfShipsLeft = playerShips.transform.childCount;

            Debug.Log("Left " + numOfShipsLeft + " number of player ships");
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
