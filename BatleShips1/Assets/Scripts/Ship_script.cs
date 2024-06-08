//using System.Collections;
//using System.Collections.Generic;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
//using UnityEngine.UIElements;
//using static UnityEditor.PlayerSettings;


public class Ship_script : MonoBehaviour
{
    GameManager_script gameManager_script;
    //[SerializeField] GameObject grid; // Temp
    //Grid_script grid_script; // Temp

    [SerializeField] float ship_grid_offset; // ship 4 int offset =  grid[0,0] = [1.5 , 0]

    [SerializeField] public Vector2Int[] shipAllPos = new Vector2Int[1];

    [SerializeField] public bool isEnemy = false;
    [SerializeField] public bool shipIsVertical = false;

    public bool shipIsReadyForBattle = false;
    public bool shipCanMove = true;
    public bool isDraggingShip = false;
    public bool mouseClicked = false;
    public bool shipCanRotate = true;
    public bool shipPlaced = false;
    private bool rollbackShipPos = false;
    public bool shipReset = false;


    public Vector2 shipStartingPos;
    private Vector2 shipLastLegitPos;

    private Vector3 mouseOffset;
    private Vector3 initialMousePos;

    private int gridWidth;
    private int gridHeight;
    private int distanceBetweenGrids;
    private int deployShipTryNumber = 0;
    private int deployShipMaxTryNumber = 100;
    public bool gameOver = false;
    public bool shield = false;
    
    public bool airDiffenceisOn = false;
    [SerializeField] private bool gameStarted;
    [SerializeField] public List<Vector2Int> shipAllAerDefensePos = new List<Vector2Int>();


    public int hitCount = 0;

    private float dragThreshold = 0.1f; // Threshold distance to consider a movement as a drag

    private void Awake()
    {
        gameManager_script = FindObjectOfType<GameManager_script>();
        //grid_script = grid.GetComponent<Grid_script>();

        gridWidth = gameManager_script.gridWidth;
        gridHeight = gameManager_script.gridHeight;
        distanceBetweenGrids = gameManager_script.distanceBetweenGrids;

        shipStartingPos = transform.position;
    }

    void Start()
    {
        //shipStartingPos = transform.position;
        shipLastLegitPos = shipStartingPos;
    }

    private void Update()
    {
        gameStarted = gameManager_script.gameStarted;
    }

    private void OnMouseOver()
    {
        //Debug.Log("isEnemy = " + isEnemy);
        //Debug.Log("gameManager_script.AllShipsAreReady = " + gameManager_script.AllShipsAreReady);
        //Debug.Log("gameStarted = " + gameStarted);
        //Debug.Log("isHovering on = " + gameObject.name);
        if (!isEnemy && gameManager_script.AllShipsAreReady && !gameStarted && gameManager_script.settingAirDefense)

        {
            //Debug.Log(shipAllPos);
            gameManager_script.SettingAirDefense(this.gameObject , true);
        }

    }

    private void OnMouseExit()
    {
        if (!isEnemy && gameManager_script.AllShipsAreReady && !gameStarted && gameManager_script.settingAirDefense)
        {
            gameManager_script.SettingAirDefense(this.gameObject, false);
        }


    }


    private void OnMouseDown()
    {
        if (shipCanMove)
        {
            initialMousePos = GetMouseWorldPos();

            //if (shipIsReadyForBattle)
            if (shipIsReadyForBattle && !gameManager_script.settingAirDefense) // If ship is deployed on the grid and I want to redeploy
            {
                shipIsReadyForBattle = false;
                makeFreeOcupiedPos();
            }
            mouseOffset = gameObject.transform.position - GetMouseWorldPos();
        }
    }

    private void OnMouseDrag()
    {
        if (shipCanMove)
        {
            if (!isDraggingShip && Vector3.Distance(initialMousePos, GetMouseWorldPos()) > dragThreshold)
            {
                isDraggingShip = true;
            }

            if (isDraggingShip)
            {
                transform.position = GetMouseWorldPos() + mouseOffset;
                Vector2Int temp_pos = Utility_script.round_pos(transform.position);
            }
        }
    }

    private void OnMouseUp()
    {
        if (!isDraggingShip)
        {
            mouseClicked = true;
        }

        if (mouseClicked && shipCanRotate && !gameManager_script.settingAirDefense)
        {
            clickOnShip();
        }

        if (shipCanMove && isDraggingShip)
        {
            endDraggingShip();
        }
        if (gameManager_script.settingAirDefense)
        {
            gameManager_script.SetAirDefence(this.gameObject, false);
        }

    }

    public List<Vector2Int> CalculateshipAllAerDefensePos(GameObject Ship)
    {
        List<Vector2Int> neighbourList = new List<Vector2Int>();

        int x = shipAllPos[0].x - 1;
        int y = shipAllPos[0].y - 1;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < shipAllPos.Length + 2; j++)
            {
                if (shipIsVertical)
                {
                    if (gameManager_script.PosIsInsideGrid(new Vector2Int(x + i, y + j), isEnemy))
                    {
                        //shipAllAerDefensePos.Add(new Vector2Int(x + i, y + j));
                        neighbourList.Add(new Vector2Int(x + i, y + j));
                    }
                }
                // Horisontal
                else
                {
                    if (gameManager_script.PosIsInsideGrid(new Vector2Int(x + j, y + i), isEnemy))
                    {
                        //shipAllAerDefensePos.Add(new Vector2Int(x + j, y + i));
                        neighbourList.Add(new Vector2Int(x + j, y + i));
                    }
                }
                
            }
        }

        return neighbourList;
        //return shipAllAerDefensePos;
    }

    private void clickOnShip()
    {
        Vector2Int currentPos = new Vector2Int(shipAllPos[0].x, shipAllPos[0].y);

        changeShipOrientation();
        makeFreeOcupiedPos(); // Clear current ocupied pos
        placeShip(currentPos);

        mouseClicked = false;
    }

    private void endDraggingShip()
    {
        Vector2Int currentPosRounded = Vector2Int.zero;

        if (shipIsVertical)
        {
            currentPosRounded = Utility_script.round_pos(transform.position - new Vector3(0, ship_grid_offset, 0));
        }
        // Ship is in Horisontal mode
        else
        {
            currentPosRounded = Utility_script.round_pos(transform.position - new Vector3(ship_grid_offset, 0, 0));
        }

        shipAllPosCalculate(currentPosRounded);

        //if (grid_script.isValidPos(shipAllPos, isEnemy))
        if (gameManager_script.IsValidPos(shipAllPos, isEnemy))
        {
            placeShip(currentPosRounded);
        }
        else
        {
            transform.position = shipLastLegitPos;
            shipAllPosCalculate(currentPosRounded);
            updateShipPos();

            


        }
        isDraggingShip = false;
    }

    private void makeFreeOcupiedPos()
    {
        //grid_script.makeFreeGridPos(shipAllPos, isEnemy);
        gameManager_script.MakeFreeOcupiedPos(shipAllPos, isEnemy);
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void placeShip(Vector2Int currentPosRounded)
    {
        shipAllPosCalculate(currentPosRounded);

        //if (grid_script.isValidPos(shipAllPos, isEnemy))
        if (gameManager_script.IsValidPos(shipAllPos, isEnemy))
        {
            transform.position = new Vector2(shipAllPos[0].x, shipAllPos[0].y);
            shipLastLegitPos = shipAllPos[0];
            ocupyGridPos();
            updateShipPos();
        }
        else
        {
            rollbackShipMovement();
            
        }
    }

    private void rollbackShipMovement()
    {
        if (isDraggingShip)
        {
            transform.position = shipLastLegitPos;
            shipAllPosCalculate(shipAllPos[0]);
            
        }

        else if (mouseClicked && shipCanRotate)
        {
            Debug.Log("Rolback rotation");
            rollbackShipPos = true;

            changeShipOrientation();
            shipAllPosCalculate(new Vector2Int(shipAllPos[0].x, shipAllPos[0].y));
            ocupyGridPos();
            updateShipPos();
            

        }
    }

    public void placeShipRandomly()
    {
        if (isEnemy) 
        { 
            GetComponent<SpriteRenderer>().flipX = true;
        }

        chooseShipOrientation();
        
        while (deployShipTryNumber < deployShipMaxTryNumber && !shipPlaced)
        {
            placeShip( calculateRandomPos() );
            deployShipTryNumber++;
        }
        if (!shipPlaced)
        {
            Debug.Log("Failed to place Enemy ship after " + deployShipMaxTryNumber + " attempts.");
        }
        deployShipTryNumber = 0;
    }

    private Vector2Int calculateRandomPos()
    {
        int shipRandomX;
        int shipRandomY;
        if (isEnemy)
        {
            shipRandomX = Utility_script.random_num(0, gridWidth + distanceBetweenGrids); // (0 + 10 , 8)
            shipRandomY = Utility_script.random_num(0, gridHeight); // (0,8)
        }
        // Player
        else
        {
            shipRandomX = Utility_script.random_num(0, gridWidth ); // (0,8)
            shipRandomY = Utility_script.random_num(0, gridHeight); // (0,8)
        }
        return  new Vector2Int(shipRandomX, shipRandomY);

    }

    private void chooseShipOrientation()
    {
        int isVerticalNum = Utility_script.random_num(0, 2);
        if (isVerticalNum == 1)
        {
            makeShipVertical();
        }
        else
        {
            makeShipHorizontal();
        }
    }

    private void changeShipOrientation()
    {
        if (shipIsVertical)
        {
            makeShipHorizontal();
        }
        else
        {
            makeShipVertical();
        }

    }

    private void makeShipVertical()
    {
        if (shipAllPos.Length > 1) // 1 size ship doesnot need rotation
        {
            if (isEnemy)
            {
                transform.Rotate(0, 0, -90);
            }
            else // Player
            {
                transform.Rotate(0, 0, 90);
            }
            shipIsVertical = true;
        }
    }

    private void makeShipHorizontal()
    {
        if (shipAllPos.Length > 1) // 1 size ship doesnot need rotation
        {
            if (isEnemy) // when placed automated, default position is horisontal, so there is no need to rotate second time
            {
                // None..
            }
            else // Player
            {
                if (mouseClicked) // if rotate manually , then needed to change rotation horisontally - when placed automated, default position is horisontal, so there is no need to rotate second time
                {
                    transform.Rotate(0, 0, -90);
                }
                if (shipReset)
                {
                    transform.Rotate(0, 0, -90);
                }

            }
            shipIsVertical = false;
        }
    }

    public void updateShipPos()
    {
        if (!rollbackShipPos)
        {
            if (shipIsVertical)
            {
                transform.position += new Vector3(0, ship_grid_offset, 0);
            }
            else
            {
                transform.position += new Vector3(ship_grid_offset, 0, 0);
            }
        }
        rollbackShipPos = false;

    }

    public void shipAllPosCalculate(Vector2Int pos)
    {
        if (shipIsVertical)
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                shipAllPos[i] = new Vector2Int(pos.x, pos.y + i);
            }
        }
        // Horisontal
        else
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                shipAllPos[i] = new Vector2Int(pos.x + i, pos.y);
            }
        }
    }

    private void shipAllPosReset()
    {
        if (shipIsVertical)
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                shipAllPos[i] = new Vector2Int(0, 0);
            }
        }
        // Horisontal
        else
        {
            for (int i = 0; i < shipAllPos.Length; i++)
            {
                shipAllPos[i] = new Vector2Int(0, 0);
            }
        }
    }

    private void ocupyGridPos()
    {
        shipIsReadyForBattle = true;
        gameManager_script.OcupyGridPos(shipAllPos, isEnemy);
        gameManager_script.CheckIfGameIsReady();
        shipPlaced = true;
        CalculateshipAllAerDefensePos(this.gameObject);

    }

    public void resetShip()
    {
        shipReset = true;
        transform.position = shipStartingPos;
        shipIsReadyForBattle = false;
        shipPlaced = false;
        shipAllPosReset();
        if (shipIsVertical)
        {
            makeShipHorizontal();
        }


        shipReset = false;

    }

    public bool shipIsAlive()
    {
        //Debug.Log("Ship hit count = " + hitCount);
        if (hitCount < shipAllPos.Length)
        { 
            return true;
        }
        
        return false;
    }

    public void destroyShip()
    {
        // 
        this.transform.SetParent(GameObject.Find("DestroyedShips").transform);
        //Destroy(gameObject);
        //Debug.Log("Ship was destroyed...");
    }

}
