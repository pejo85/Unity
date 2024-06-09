using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_script : MonoBehaviour
{
    private GameObject gameManager;
    private GameManager_script gameManager_script;

    private Vector3 initialMousePos;
    private Vector3 mouseOffset;
    public bool mouseIsClicked;

    public bool isEnemyBoard;
    public bool isPlayerBoard;

    public bool isOcupied;
    public bool isRevealed;
    public bool isUnderAirDefense;
    public bool wasShot;

    private bool gameStarted;

    //public bool gameOver = false;
    //GameObject[] CubesSateliteNeighboursList = new GameObject[4];


    

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManager_script = gameManager.GetComponent<GameManager_script>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetCubeProperties()
    {
        mouseIsClicked = false;
        isOcupied = false;
        isRevealed = false;
        isUnderAirDefense = false;
        wasShot = false;
        gameStarted = false;
        
    }

    public void IsOcupied()
    {
        isOcupied = true;
    }

    public void IsFree()
    {
        isOcupied = false;
    }

    public void CubeColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

}
