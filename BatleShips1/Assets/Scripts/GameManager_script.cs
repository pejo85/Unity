using System.Collections;
using System.Collections.Generic;
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


}
