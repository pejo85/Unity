using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_script : MonoBehaviour
{
    private GameManager_script gameManager_script;

    public Vector2 targetPosition;
    public float speed = 10f;
    private Rigidbody2D rb;
    public bool isPlayerShot;

    void Start()
    {
        gameManager_script = GameObject.Find("GameManager").GetComponent<GameManager_script>();

        rb = GetComponent<Rigidbody2D>();
        ShootRocket();
    }

    void Update()
    {
        RotateTowardsDirection();
    }

    void ShootRocket()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //Debug.Log(collision.gameObject.name + " -- " + collision.transform.position);

            if (gameManager_script != null)
            {
                if (BulletHitTarget(collision.gameObject.transform.position))
                {
                    if (gameManager_script.playerMove)
                    {
                        gameManager_script.PlayerHitOrMissTarget(collision.gameObject);
                    }
                    else
                    {
                        //Debug.Log(collision.name + " -zzz- " + collision.transform.position);
                        gameManager_script.EnemyHitOrMissTarget(collision.gameObject, new Vector2Int((int)collision.transform.position.x , (int)collision.transform.position.y) );
                    }
                    //Debug.Log("xxxx");
                    gameManager_script.bulletIsInTheAir = false;
                    //Debug.Log("yyyy");
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogError("GameManager_script reference is null in bullet_script");
            }
        }
    }

    public bool BulletHitTarget(Vector2 collisionPos)
    {
        return collisionPos == targetPosition;
    }
}
