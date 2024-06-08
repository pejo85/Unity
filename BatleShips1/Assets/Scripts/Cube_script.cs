//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Cube_script : MonoBehaviour
{
    [SerializeField] private GameObject GameManager;

    private GameManager_script gameManager_script;


    [SerializeField] public bool isOcupied = false;
    //private bool isColided = false;
    public bool mouseClicked = false;
    public bool isEnemyBoard = false;
    public bool wasShot = false;
    [SerializeField] private bool gameStarted;

    //public bool gameOver = false;

    private Vector3 mouseOffset;
    private Vector3 initialMousePos;

    GameObject[] CubesSateliteNeighboursList = new GameObject[4];


    public bool isRevealed = false;
    public bool isUnderAirDefense;


    private void Start()
    {
        
        gameManager_script = GameManager.GetComponent<GameManager_script>();
    }

    private void Update()
    {
        gameStarted = gameManager_script.gameStarted;

    }

    private void OnMouseOver()
    {
        //Debug.Log("isHovering on = " + gameObject.name);
        if (isEnemyBoard && gameManager_script.sateliteIsWatching)
        {
            CubesSateliteNeighboursList = gameManager_script.CalculateClickedCubesSateliteNeighbours(this.gameObject);
            gameManager_script.SateliteHoversOverTile(CubesSateliteNeighboursList);
        }
        //if (!isEnemyBoard && gameManager_script.AllShipsAreReady && !gameStarted)
        //{
        //    CubeColor(Color.yellow);
        //}
        
    }

    private void OnMouseExit()
    {
        if (isEnemyBoard && gameManager_script.sateliteIsWatching)
        {
            gameManager_script.SateliteHoversOverExitTile(CubesSateliteNeighboursList);
        }
        //if (!isEnemyBoard && gameManager_script.AllShipsAreReady && !gameStarted)
        //{
        //    CubeColor(Color.white);
        //}


    }

    private void OnMouseDown()
    {
        mouseClicked = false;

        if (gameStarted)
        {
            initialMousePos = GetMouseWorldPos();
        }
    }

    private void OnMouseUp()
    {
        mouseClicked = true;
        //Debug.Log(gameManager_script.sateliteIsWatching);
        
        if (gameStarted)
        {
            if (gameManager_script.sateliteIsWatching)
            {
                gameManager_script.sateliteRevealsEnemyTiles(this.gameObject);
                gameManager_script.sateliteIsWatching = false;
            }

            else if (gameManager_script.playerMove)
            {
                // When player shoots the enemy
                if (mouseClicked && isEnemyBoard && !gameManager_script.sateliteIsWatching)
                {
                    gameManager_script.PlayerShootsToEnemy(this.gameObject);
                }
            }

        }

    }

    public void RevealTile()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 0;
        isRevealed = true;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public bool hitTheTarget()
    {
        // Hit the target
        if (isOcupied)
        {
            return true;
        }

        // If missed
        return false;

    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //isColided = true;
            //if (collision.gameObject.tag == "Ship")
            //{
            //    if (collision.GetComponent<Ship_script>().shipIsReadyForBattle == false)
            //    {
            //        IsOcupiedWhileDragging();
            //    }
            //
            //}

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //isColided = false ;
        //if (collision.gameObject.tag == "Ship")
        //{
        //    if (!isOcupied)
        //    {
        //        //GetComponent<SpriteRenderer>().color = Color.white;
        //        //collision.GetComponent<SpriteRenderer>().color = Color.white;
        //    }            
        //}
        
    }

    //public void IsOcupiedWhileDragging()
    //{
    //    if (isOcupied)
    //    {
    //        //GetComponent<SpriteRenderer>().color = Color.red;
    //    }
    //}

    public void IsOcupied()
    {
        isOcupied = true;
    }

    public void IsFree()
    {
        isOcupied = false;
    }

    //private void checkIfStaysOcupied()
    //{
    //    if (!isColided && isOcupied)
    //        {
    //            //GetComponent<SpriteRenderer>().color = Color.white;
    //        }
    //}

    public void CubeColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

}



