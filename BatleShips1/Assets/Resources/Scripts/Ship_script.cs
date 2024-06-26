﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Ship_script : MonoBehaviour
{
    private GameObject gameManager;
    GameManager_script gameManager_script;

    [SerializeField] float ship_grid_offset; // ship 4 int offset =  grid[0,0] = [1.5 , 0]

    [SerializeField] public Vector2Int[] shipAllPosArray = new Vector2Int[1];

    [SerializeField] public bool isPlayer = true;
    [SerializeField] public bool shipIsHorisontal = true;
    [SerializeField] GameObject decorShip;

    private int gridWidth;
    private int gridHeight;
    private int distanceBetweenGrids;
    private int deployShipTryNumber = 0;
    private int deployShipMaxTryNumber = 100;
    public int airDefenceNumber;

    private Vector3 mouseOffset;
    private Vector3 initialMousePos;
    public Vector2 shipStartingPos;
    private Vector2 shipLastLegitPos;

    public bool shipCanMove = true;
    public bool shipCanRotate = true;
    public bool shipIsDragging;
    public bool mouseIsClickedOnShip;
    public bool shipIsDeployed;
    private bool rollbackShipPos;

    public bool airDiffenceIsActivated;
    public bool shipIsReadyForBattle;

    [SerializeField] private bool gameStarted;
    public bool gameOver;

    [SerializeField] public List<Vector2Int> shipAllAirDefensePos = new List<Vector2Int>();

    public int shipHitCount = 0;
    private float dragThreshold = 0.1f; // Threshold distance to consider a movement as a drag
                                        //public bool resetShipProperties;

    private void Awake()
    {
        gameManager_script = FindObjectOfType<GameManager_script>();

        gridWidth = gameManager_script.GRIDWIDTH;
        gridHeight = gameManager_script.GRIDHEIGHT;
        distanceBetweenGrids = gameManager_script.DISTANCEBETWEENGRIDS;
    }

    void Start()
    {
        shipStartingPos = transform.position;
        shipLastLegitPos = shipStartingPos;
    }

    private void OnMouseOver()
    {
        if (! gameManager_script.pause)
        {
            if (SettingUpAirDefense())
            {
                gameManager_script.SettingAirDefense(this.gameObject, true);
            }
        }
        

    }

    private void OnMouseExit()
    {
        if (!gameManager_script.pause)
        {
            if (SettingUpAirDefense())
            {
                gameManager_script.SettingAirDefense(this.gameObject, false);
            }
        }
        
    }

    private void OnMouseDown()
    {
        if (!gameManager_script.pause)
        {
            if (shipCanMove)
            {
                initialMousePos = GetMouseWorldPos();

                if (ShipRedeploy()) // If ship is deployed on the grid and I want to redeploy
                {
                    shipIsReadyForBattle = false;
                    makeFreeOcupiedPos();
                }
                mouseOffset = gameObject.transform.position - GetMouseWorldPos();

            }
        }

        
    }

    private void OnMouseDrag()
    {
        if (!gameManager_script.pause)
        {
            if (shipCanMove)
            {
                if (!shipIsDragging && Vector3.Distance(initialMousePos, GetMouseWorldPos()) > dragThreshold)
                {
                    shipIsDragging = true;
                }

                if (shipIsDragging)
                {
                    transform.position = GetMouseWorldPos() + mouseOffset;
                    Vector2Int temp_pos = Utility_script.round_pos(transform.position);
                }
            }
        }
        
    }

    private void OnMouseUp()
    {
        if (!gameManager_script.pause)
        {
            if (!shipIsDragging)
            {
                mouseIsClickedOnShip = true;
            }

            if (ShipCanRotate())
            {
                clickOnShip();
            }

            if (shipCanMove && shipIsDragging)
            {
                endDraggingShip();
            }
            if (SettingUpAirDefense())
            {
                if (gameManager_script.CanDeployAirDefence(this.gameObject))
                {
                    ActivateAirDefense();
                }

            }
        }
        

    }

    private void clickOnShip()
    {
        Vector2Int currentPos = new Vector2Int(shipAllPosArray[0].x, shipAllPosArray[0].y);

        ChangeShipOrientation();
        makeFreeOcupiedPos(); // Clear current ocupied pos
        DeployShip(currentPos);

        mouseIsClickedOnShip = false;
    }

    private bool ShipCanRotate()
    {
        return mouseIsClickedOnShip && shipCanRotate && !gameManager_script.settingUpAirDefense;
    }

    private void endDraggingShip()
    {
        Vector2Int currentPosRounded = Vector2Int.zero;

        // Ship is in Horisontal mode
        if (shipIsHorisontal) 
        {
            currentPosRounded = Utility_script.round_pos(transform.position - new Vector3(ship_grid_offset, 0, 0));
        }
        else // Vertical
        {
            currentPosRounded = Utility_script.round_pos(transform.position - new Vector3(0, ship_grid_offset, 0));
        }

        ShipAllPosCalculate(currentPosRounded);

        if (gameManager_script.IsValidPos(shipAllPosArray, isPlayer))
        {
            DeployShip(currentPosRounded);
        }
        else
        {
            transform.position = shipLastLegitPos;
            ShipAllPosCalculate(currentPosRounded);
            UpdateShipPos();




        }
        shipIsDragging = false;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void placeShipRandomly()
    {
        ChooseShipOrientation();

        // Max try to deploy ship --> deployShipMaxTryNumber
        while (deployShipTryNumber < deployShipMaxTryNumber && !shipIsDeployed)
        {
            Vector2Int shipRandomPos = CalculateRandomPos();
            DeployShip(shipRandomPos);
            deployShipTryNumber++;
        }
        if (!shipIsDeployed)
        {
            Debug.Log("Failed to place Enemy ship after " + deployShipMaxTryNumber + " attempts.");
        }
        deployShipTryNumber = 0;

        if (!isPlayer)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void DeployShip(Vector2Int currentPos)
    {
        ShipAllPosCalculate(currentPos);

        if (gameManager_script.IsValidPos(shipAllPosArray, isPlayer))
        {
            transform.position = new Vector2(shipAllPosArray[0].x, shipAllPosArray[0].y);
            shipLastLegitPos = shipAllPosArray[0];
            OcupyGridPos();
            UpdateShipPos();
        }
        else
        {
            RollbackShipMovement();

        }
    }

    public void ShipAllPosCalculate(Vector2Int pos)
    {
        if (shipIsHorisontal)
        {
            for (int i = 0; i < shipAllPosArray.Length; i++)
            {
                shipAllPosArray[i] = new Vector2Int(pos.x + i, pos.y);
            }
        }
        else // Vertical
        {
            for (int i = 0; i < shipAllPosArray.Length; i++)
            {
                shipAllPosArray[i] = new Vector2Int(pos.x, pos.y + i);
            }
        }
    }

    private Vector2Int CalculateRandomPos()
    {
        int shipRandomX;
        int shipRandomY;

        // Player
        if (isPlayer)
        {
            shipRandomX = Utility_script.random_num(0, gridWidth); // (0,8)
            shipRandomY = Utility_script.random_num(0, gridHeight); // (0,8)
        }
        else
        {
            shipRandomX = Utility_script.random_num(0, gridWidth + distanceBetweenGrids); // (0 + 10 , 8)
            shipRandomY = Utility_script.random_num(0, gridHeight); // (0,8)
        }
        return new Vector2Int(shipRandomX, shipRandomY);

    }

    private void OcupyGridPos()
    {
        shipAllAirDefensePos = CalculateshipAllAerDefensePos(this.gameObject);
        shipIsDeployed = true;
        shipIsReadyForBattle = true;
        gameManager_script.OcupyGridPos(shipAllPosArray, isPlayer);
        gameManager_script.CheckIfGameIsReady(); // if it is last ship that was deployed
        
    }

    public void ResetShipProperties()
    {
        //shipIsHorisontal = true;  - makeShipHorizontal()-ში აკეთებს
        shipCanMove = true;
        shipCanRotate = true;
        shipIsDragging = false;
        mouseIsClickedOnShip = false;
        shipIsDeployed = false;
        airDiffenceIsActivated = false;
        shipIsReadyForBattle = false;
        gameStarted = false;
        gameOver = false;
        shipAllAirDefensePos = new List<Vector2Int>();
        shipHitCount = 0;
        transform.position = shipStartingPos;
        ShipAllPosReset();

        if (!shipIsHorisontal)
        {
            MakeShipHorizontal();
        }
    }

    private void ShipAllPosReset()
    {
        for (int i = 0; i < shipAllPosArray.Length; i++)
        {
            shipAllPosArray[i] = new Vector2Int(0, 0);
        }
    }

    private void ChooseShipOrientation()
    {
        int isHorisontalNum = Utility_script.random_num(0, 2);
        if (isHorisontalNum == 0)
        {
            MakeShipHorizontal();
            
        }
        else
        {
            MakeShipVertical();
        }
    }

    private void ChangeShipOrientation()
    {
        if (!shipIsHorisontal)
        {
            MakeShipHorizontal();
        }
        else
        {
            MakeShipVertical();
        }

    }

    private void MakeShipHorizontal()
    {
        if (shipAllPosArray.Length > 1) // 1 size ship doesnot need rotation
        {
             // when placed automated, default position is horisontal, so for Enemy there is no need to rotate second time
            if (isPlayer)
            {
                if (!shipIsHorisontal)
                {
                    transform.Rotate(0, 0, -90);
                }

            }
            shipIsHorisontal = true;
        }
    }

    private void MakeShipVertical()
    {
        if (shipAllPosArray.Length > 1) // 1 size ship doesnot need rotation
        {
            if (isPlayer) // Player
            {
                transform.Rotate(0, 0, 90);
            }
            else // Enemy
            {
                transform.Rotate(0, 0, -90);
            }
            shipIsHorisontal = false;
        }
    }

    public void UpdateShipPos()
    {
        if (!rollbackShipPos)
        {
            if (shipIsHorisontal)
            {
                transform.position += new Vector3(ship_grid_offset, 0, 0);
            }
            else // isVertical
            {
                transform.position += new Vector3(0, ship_grid_offset, 0);
            }
        }
        rollbackShipPos = false;

    }

    private void RollbackShipMovement()
    {
        if (shipIsDragging)
        {
            transform.position = new Vector3( shipLastLegitPos.x , shipLastLegitPos.y , 0);
                                 
            ShipAllPosCalculate(shipAllPosArray[0]);

        }

        else if (mouseIsClickedOnShip && shipCanRotate)
        {
            Debug.Log("Rolback rotation");
            rollbackShipPos = true;

            ChangeShipOrientation();
            ShipAllPosCalculate(new Vector2Int(shipAllPosArray[0].x, shipAllPosArray[0].y));
            OcupyGridPos();
            UpdateShipPos();


        }
    }

    public List<Vector2Int> CalculateshipAllAerDefensePos(GameObject Ship)
    {
        List<Vector2Int> neighbourList = new List<Vector2Int>();

        int x = shipAllPosArray[0].x - 1;
        int y = shipAllPosArray[0].y - 1;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < shipAllPosArray.Length + 2; j++)
            {
                if (shipIsHorisontal)
                {
                    if (gameManager_script.PosIsInsideGrid(new Vector2Int(x + j, y + i), isPlayer))
                    {
                        //shipAllAerDefensePos.Add(new Vector2Int(x + j, y + i));
                        neighbourList.Add(new Vector2Int(x + j, y + i));
                    }
                }
                else // Vertical
                {
                    if (gameManager_script.PosIsInsideGrid(new Vector2Int(x + i, y + j), isPlayer))
                    {
                        //shipAllAerDefensePos.Add(new Vector2Int(x + i, y + j));
                        neighbourList.Add(new Vector2Int(x + i, y + j));
                    }
                }

            }
        }

        return neighbourList;
        //return shipAllAerDefensePos;
    }

    public bool SettingUpAirDefense()
    {
        return isPlayer && gameManager_script.allShipsAreReadyForBattle && !gameStarted && gameManager_script.settingUpAirDefense && !airDiffenceIsActivated;
    }

    public void ActivateAirDefense()
    {
        gameManager_script.ActivateAirDefense(this.gameObject);
        airDiffenceIsActivated = true;
    }

    public bool ShipRedeploy()
    {
        return shipIsReadyForBattle && !gameManager_script.settingUpAirDefense;
    }

    private void makeFreeOcupiedPos()
    {
        gameManager_script.MakeFreeOcupiedPos(shipAllPosArray, isPlayer);
    }

    public bool ShipIsAlive()
    {
        if (shipHitCount < shipAllPosArray.Length)
        {
            return true;
        }
        return false;
    }

    public void DestroyShip()
    {
        if (decorShip != null)
        {
            decorShip.GetComponent<SpriteRenderer>().color = Color.black;
            decorShip.transform.GetChild(0).gameObject.SetActive(true);
        }
        this.transform.SetParent(GameObject.Find("DestroyedShips").transform);
    }





}




