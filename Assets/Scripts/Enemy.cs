// Created by Austin Patel on 5/21/16 on 12:40 PM

using UnityEngine;

public class Enemy : MonoBehaviour
{

    private const float BULLET_SPEED = 30, REVIVE_TIME = 1000;
    public Transform[] destinations;
    public float speed = 0.1f;
    private float fudgeFactor = 0.1f;
    private int curDest;
    private float dX, dY;
    public enum Actions { None, ShootAtDestination, ShootAtInterval };
    public Actions[] actions;
    public int shootInterval;
    private int currentShootTime;
    private GameObject player;
    private Vector3 posDelta;
    private Rigidbody2D rigidBody2D;
    private GameObject texture;
    private Vector3 newBulletLocation;
    public GameObject bullet;
    public bool start = true;
    private float rotation;
    public GameObject bullets;
    private Vector3 dirVector;
    public enum Types { Path, Follow, Surviellance };
    public Types type;
    private GameObject laser;
    private Vector3 shootPosition;
    public float laserRotation;
    private float curLaserRotation;
    public float laserRotationSpeed;
    private float startLaserRotation;
    private bool laserDirection;
    private float endLaserRotation;
    private float shootDelay = 3;
    private float curShootDelay;
    public int lives = 1;
    private int maxLives;
    public bool isDead;
    private int revivalTime;
    private SpriteRenderer spriteRenderer, childRenderer;
    private BoxCollider2D boxCollider;
    private int curDeadTime;

    void Start()
    {
        maxLives = lives;
        if (type == Types.Path)
            transform.position = destinations[0].position;
        else if (type == Types.Surviellance)
        {
            shootPosition = new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2, transform.position.z);
            laser = gameObject.transform.FindChild("Laser").gameObject;

            startLaserRotation = gameObject.transform.rotation.eulerAngles.z - laserRotation / 2;
            if (startLaserRotation < 0) startLaserRotation += 360;

            endLaserRotation = gameObject.transform.rotation.eulerAngles.z + laserRotation / 2;

            curLaserRotation = gameObject.transform.rotation.eulerAngles.z;

            spriteRenderer = GetComponent<SpriteRenderer>();
            childRenderer = transform.FindChild("Laser").GetComponent<SpriteRenderer>();
            boxCollider = gameObject.GetComponent<BoxCollider2D>();
        }

        player = GameObject.Find("Player");
        posDelta = new Vector3(0, 0, 0);
        rigidBody2D = GetComponent<Rigidbody2D>();
        if (type != Types.Surviellance)
            texture = gameObject.transform.Find("Texture").gameObject;
        newBulletLocation = new Vector3();
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            curDeadTime++;
            if (curDeadTime == REVIVE_TIME)
            {
                isDead = false;
                boxCollider.enabled = true;
                childRenderer.enabled = true;
                spriteRenderer.enabled = true;
                lives = maxLives;
            }
            return;
        }

        if (!start) return;

        if (type == Types.Path)
        {
            // If the current position is equal to the position to go to
            if (PositionsCloseEnough(transform.position, destinations[curDest].position))
            {
                // Preform the action at this destination
                switch (actions[curDest])
                {
                    case Actions.None:
                        break;

                    case Actions.ShootAtDestination:
                        Shoot();
                        break;
                }

                curDest++;
                if (curDest == destinations.Length) curDest = 0;
            }

            ReadjustDirection(transform.position, destinations[curDest].position);

            MoveInDirection();

            PointTowardsPlayer();
        }
        else if (type == Types.Follow)
        {
            ReadjustDirection(transform.position, player.transform.position);
            PointTowardsPlayer();
            MoveInDirection();

            foreach (Actions a in actions)
            {
                if (a == Actions.ShootAtInterval)
                {
                    currentShootTime++;
                    if (currentShootTime == shootInterval)
                    {
                        Shoot();
                        currentShootTime = 0;
                    }
                }
            }

            PointTowardsPlayer();
        }
        else if (type == Types.Surviellance)
            CastRay();
    }

    // Moves the enemy in a certain direction at its speed
    void MoveInDirection()
    {
        posDelta.Set(0, 0, 0);
        posDelta.Set(dirVector.x * speed, dirVector.y * speed, posDelta.z);
        rigidBody2D.velocity = new Vector2(0, 0);
        rigidBody2D.transform.Translate(posDelta);
    }

    bool PositionsCloseEnough(Vector2 pos1, Vector2 pos2)
    {
        return (Mathf.Abs(pos1.x - pos2.x) <= fudgeFactor) && (Mathf.Abs(pos1.y - pos2.y) <= fudgeFactor);
    }

    // Points the enemy towards the player
    void PointTowardsPlayer()
    {
        Vector2 position = transform.position;
        float posX = position.x;
        float posY = position.y;

        Vector2 playerPosition = player.transform.position;
        float playerX = playerPosition.x;
        float playerY = playerPosition.y;

        float deltaX = posX - playerX;
        float deltaY = posY - playerY;

        rotation = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;

        texture.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation + 180));
    }

    void Shoot()
    {
        newBulletLocation = new Vector3(transform.position.x, transform.position.y, bullet.transform.position.z);
        GameObject newBullet;

        if (type == Types.Surviellance)
            rotation += 90;

        newBullet = (GameObject)Instantiate(bullet, newBulletLocation, Quaternion.Euler(0, 0, rotation));

        float velX = -BULLET_SPEED * Mathf.Cos(rotation * Mathf.Deg2Rad);
        float velY = -BULLET_SPEED * Mathf.Sin(rotation * Mathf.Deg2Rad);

        if (type == Types.Surviellance)
            rotation -= 90;

        newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(velX, velY);
        newBullet.transform.parent = bullets.transform; // Put all the bullets under a single parent
        Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    // Calculate the velocity between two points
    void ReadjustDirection(Vector3 pos1, Vector3 pos2)
    {
        dX = -(pos1.x - pos2.x);
        dY = -(pos1.y - pos2.y);

        dirVector = new Vector2(dX, dY).normalized;
    }

    void CastRay()
    {
        
        if (!start) return;

        float deg = gameObject.transform.localRotation.eulerAngles.z;
        
        // Rotate the suveillance camera
        deg += laserRotationSpeed;
        
        if (Mathf.Abs(deg - endLaserRotation) < 1 && false == laserDirection)
        {
            laserRotationSpeed = -laserRotationSpeed;
            laserDirection = true;
        }
        if (Mathf.Abs(deg - startLaserRotation) < 1 && true == laserDirection)
        {
            laserRotationSpeed = -laserRotationSpeed;
            laserDirection = false;
        }

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0,0,deg));

        curLaserRotation = deg;
        rotation = deg;

        Vector2 dir = -(Vector2)(Quaternion.Euler(0, 0, curLaserRotation) * Vector2.up).normalized;

        RaycastHit2D hit = Physics2D.Raycast(shootPosition, dir, 20);

        if (hit.collider != null)
        {
            // Set the position of the red pointer
            float pX = shootPosition.x;
            float pY = shootPosition.y;
            float cX = hit.point.x;
            float cY = hit.point.y;
            float dX1 = cX - pX;
            float dY1 = cY - pY;
            float x2ndPow = Mathf.Pow(dX1, 2);
            float y2ndPow = Mathf.Pow(dY1, 2);
            float hypotenuse = Mathf.Sqrt(x2ndPow + y2ndPow) + 1;
            float sX = hypotenuse;
            float sY = laser.transform.localScale.y;

            laser.transform.localPosition = new Vector3(0, -hypotenuse / 2, 0);
            laser.transform.localScale = new Vector3(sX, sY, laser.transform.localScale.z);

            // Collision 
            if (hit.collider.gameObject.tag.Equals("Player"))
            {
                curShootDelay++;
                if (curShootDelay > shootDelay)
                {
                    Shoot();
                    curShootDelay = 0;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name.Equals("Player"))
        {
            player.GetComponent<Player>().loseLife();
            Destroy(gameObject);
        }
    }

    public void loseLife()
    {
        if (isDead) return;

        lives -= 1;
        if (lives <= 0)
        {
            GameObject.Find("Canvas").GetComponent<CanvasManager>().updateScore(1);
            isDead = true;

            if (type == Types.Surviellance)
            {
                spriteRenderer.enabled = false;
                childRenderer.enabled = false;
                boxCollider.enabled = false;
                curDeadTime = 0;
            }
        }
    }
}