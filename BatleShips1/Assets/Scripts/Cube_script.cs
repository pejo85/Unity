//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Cube_script : MonoBehaviour
{
    [SerializeField] private GameObject GameManager;

    private GameManager_script gameManager_script;

    [SerializeField] GameObject Missed_img;
    [SerializeField] GameObject FireAnimation;

    [SerializeField] public bool isOcupied = false;
    private bool isColided = false;
    public bool mouseClicked = false;
    public bool isEnemyBoard = false;
    [SerializeField] private bool gameStarted;

    public bool gameOver = false;

    private Vector3 mouseOffset;
    private Vector3 initialMousePos;

    //public Animator animator;

    private void Start()
    {
        
        gameManager_script = GameManager.GetComponent<GameManager_script>();
    }

    private void Update()
    {
        //checkIfStaysOcupied();
        gameStarted = gameManager_script.gameStarted;

    }

    private void OnMouseDown()
    {
        mouseClicked = false;

        if (gameStarted)
        {
            initialMousePos = GetMouseWorldPos();

            //mouseOffset = gameObject.transform.position - GetMouseWorldPos();
        }
    }

    private void OnMouseUp()
    {
        mouseClicked = true;

        
        if (gameStarted)
        {

            if (gameManager_script.playerMove)
            {
                // When player shoots the enemy
                if (mouseClicked && isEnemyBoard)
                {
                    gameManager_script.PlayerShootsToEnemy(this.gameObject);
                }
            }

        }

    }

    public void shoot()
    {
        if (gameManager_script.playerMove)
        {
            // Hit the target
            if (isOcupied)
            {
                Instantiate(FireAnimation, transform.position, Quaternion.identity);
                gameManager_script.HitEnemyShip(transform.position);

                // Move stayes to the player
                gameManager_script.playerMove = true;
                gameManager_script.enemyMove = false;
            }
            else // If missed
            {
                Instantiate(Missed_img, transform.position, Quaternion.identity);
                gameManager_script.playerMove = false;
                gameManager_script.enemyMove = true;
            }
        }
        else if (gameManager_script.enemyMove)
        {
            //gameManager_script.enemyShootsToPlayer();
        }

    }

    public bool hitTheTarget()
    {
        // Hit the target
        if (isOcupied)
        {
            //animator.SetBool("Hit", true);
            //GameObject fire =  Instantiate(FireAnimation, transform.position, Quaternion.identity);
            //fire.transform.SetParent(this.transform);
            return true;
        }

        // If missed

        //animator.SetBool("Miss", true);
        //GameObject miss = Instantiate(Missed_img, transform.position, Quaternion.identity);
        //miss.transform.SetParent(this.transform);
        return false;

    }

    public void playerShootsToEnemy()
    {

    }

    public void enemyShootsToPlayer()
    {

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
            isColided = true;
            if (collision.gameObject.tag == "Ship")
            {
                if (collision.GetComponent<Ship_script>().shipIsReadyForBattle == false)
                {
                    IsOcupiedWhileDragging();
                }

            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("111111");
        isColided = false ;
        if (collision.gameObject.tag == "Ship")
        {
            if (!isOcupied)
            {
                //GetComponent<SpriteRenderer>().color = Color.white;
                //collision.GetComponent<SpriteRenderer>().color = Color.white;
            }            
        }
        
    }

    public void IsOcupiedWhileDragging()
    {
        if (isOcupied)
        {
            //GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void IsOcupied()
    {
        isOcupied = true;
        //GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void IsFree()
    {
        isOcupied = false;
        //GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void checkIfStaysOcupied()
    {
        if (!isColided && isOcupied)
            {
                //GetComponent<SpriteRenderer>().color = Color.white;
            }
    }
}



