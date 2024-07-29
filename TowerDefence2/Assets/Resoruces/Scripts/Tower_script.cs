using UnityEngine;

public class Tower_script : MonoBehaviour
{
    public bool dragging = false;
    public bool placeTower = false;
    public Vector3Int DenyRedRGB = new Vector3Int(250, 150, 150);
    public Vector3Int PermitGreenRGB = new Vector3Int(100, 250, 100);
    public Vector3Int DefaultGreenRGB = new Vector3Int(70, 200, 70);

    public GameObject Tile; // object is assigned from Tile scipt after colliding
    [SerializeField] public GameObject Ball;

    public float range;  // Tower's detection range
    public string enemyTag = "enemy";  // Tag used to identify enemies
    public Transform targetEnemy;  // Current target
    public Transform partToRotate;  // Part of the tower that rotates to face the target
    public float turretTurnSpeed;  // Speed at which the tower rotates
    private Vector3 ballStartingPos;
    public float bulletSpeed;
    public bool bulletIsInTheAir;

    GameManager gameManager_script;

    private void Awake()
    {
        GameObject GameManager = GameObject.Find("GameManager");
        gameManager_script = GameManager.GetComponent<GameManager>();

        if (partToRotate == null)
        {
            partToRotate = transform.Find("weapon_cannon");  // Replace with the actual name of your child object
            if (partToRotate == null)
            {
                Debug.LogError("Part to rotate not found!");
            }
        }
    }

    void Start()
    {
        range = 5f;
        turretTurnSpeed = 70f * Time.deltaTime;
        bulletSpeed = 100f * Time.deltaTime;
    }

    void Update()
    {
        MoveTower();
        UpdateTarget();
        LockOnTarget();
    }

    public void MoveTower()
    {
        if (dragging)
        {
            transform.position = GetMouseWorldPos();
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        Vector3 worldPos = Vector3.zero;

        if (plane.Raycast(ray, out distance))
        {
            worldPos = ray.GetPoint(distance);
        }
        return new Vector3(worldPos.x, 1.5f, worldPos.z);
    }

    private void OnMouseUp()
    {
        if (placeTower == false && Tile != null)
        {
            TryTowerPlacement();
        }
    }

    public void TryTowerPlacement()
    {
        if (dragging)
        {
            if (CanPlaceTower())
            {
                PlaceTower();
            }
        }
    }

    private bool CanPlaceTower()
    {
        if (Tile.gameObject != null && Tile.tag == "nature")
        {
            if (Tile.GetComponent<Tile_script>().isOcupied == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public void PlaceTower()
    {
        dragging = false;
        placeTower = true;
        transform.position = Tile.transform.position;

        Tile.GetComponent<Tile_script>().isOcupied = true;
    }

    void UpdateTarget()
    {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        if (gameManager_script.enemyList.Count > 0)
        {
            foreach (GameObject enemy in gameManager_script.enemyList)
            {
                if (enemy != null)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy;
                    }
                }
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            targetEnemy = nearestEnemy.transform;
        }
        else
        {
            targetEnemy = null;
        }
    }

    private void LockOnTarget()
    {
        if (targetEnemy != null)
        {
            Vector3 dir = targetEnemy.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, turretTurnSpeed).eulerAngles;
            partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            if (!bulletIsInTheAir)
            {
                Debug.Log("Ready to shoot");
                ShootEnemy();
                bulletIsInTheAir = true;
            }
            
        }
    }

    private void ShootEnemy()
    {
        Transform child = transform.Find("weapon_cannon");
        Transform grandChild = child.Find("bulletPos");
        ballStartingPos = grandChild.transform.position;

        if (targetEnemy != null)
        {
            Vector3 enemyDirection = targetEnemy.GetComponent<Enemy_script>().GetCurrentSpeed();
            float enemySpeed = enemyDirection.magnitude;
            Vector3 interceptPoint = CalculateInterceptPoint(targetEnemy.position, enemyDirection.normalized, ballStartingPos, bulletSpeed, enemySpeed);
            if (interceptPoint != Vector3.zero)
            {
                GameObject bullet = Instantiate(Ball, ballStartingPos, Quaternion.identity);
                Bullet_script bullet_script = bullet.GetComponent<Bullet_script>();
                bullet_script.SetTarget(interceptPoint, bulletSpeed);
            }
        }
    }

    private Vector3 CalculateInterceptPoint(Vector3 targetPosition, Vector3 targetDirection, Vector3 origin, float projectileSpeed, float targetSpeed)
    {
        Vector3 directionToTarget = targetPosition - origin;
        float distanceToTarget = directionToTarget.magnitude;
        float timeToIntercept = distanceToTarget / projectileSpeed;

        Vector3 interceptPoint = targetPosition + targetDirection * targetSpeed * timeToIntercept;

        return interceptPoint;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
