using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_script_1 : MonoBehaviour
{

    private GameManager_script_1 gameManager_script_1;

    public Vector2 targetPosition;
    public float speed = 10f;
    private Rigidbody2D rb;
    public bool isPlayerShot;


    // Start is called before the first frame update
    void Start()
    {
        gameManager_script_1 = GameObject.Find("GameManager_1").GetComponent<GameManager_script_1>();

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsDirection();
    }

    void ShootRocket(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
        Vector2 startPosition = transform.position;
        Vector2 direction = (targetPosition - startPosition).normalized;

        float distance = Vector2.Distance(startPosition, targetPosition);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float time = distance / speed;
        float vx = direction.x * speed;
        float vy = (direction.y * speed) + (0.5f * Mathf.Abs(Physics2D.gravity.y) * time);

        rb.velocity = new Vector2(vx, vy);
        rb.rotation = angle;
    }

    void RotateTowardsDirection()
    {
        Vector2 direction = rb.velocity;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }


    //////////// TEMP Disabled //////////////

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision != null)
    //    {
    //        //Debug.Log(collision.gameObject.name + " -- " + collision.transform.position);
    //
    //        if (gameManager_script_1 != null)
    //        {
    //            if (BulletHitTarget(collision.gameObject.transform.position))
    //            {
    //                if (gameManager_script_1.playerMove)
    //                {
    //                    gameManager_script_1.PlayerHitOrMissTarget(collision.gameObject);
    //                }
    //                else
    //                {
    //                    //Debug.Log(collision.name + " -zzz- " + collision.transform.position);
    //                    gameManager_script_1.EnemyHitOrMissTarget(collision.gameObject, new Vector2Int((int)collision.transform.position.x, (int)collision.transform.position.y));
    //                }
    //                //Debug.Log("xxxx");
    //                gameManager_script_1.bulletIsInTheAir = false;
    //                //Debug.Log("yyyy");
    //                Destroy(gameObject);
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("GameManager_script reference is null in bullet_script");
    //        }
    //    }
    //}

    public bool BulletHitTarget(Vector2 collisionPos)
    {
        return collisionPos == targetPosition;
    }
}
