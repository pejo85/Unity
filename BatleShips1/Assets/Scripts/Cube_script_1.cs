using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_script_1 : MonoBehaviour
{
    [SerializeField] private GameObject GameManager;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
