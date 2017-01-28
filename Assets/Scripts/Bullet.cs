    // Created by Austin Patel on 5/21/16 at 12:10 PM

using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const int POWERUP_DROP_CHANCE = 5;
    public GameObject[] powerups;
    public GameObject killSound;

    // Delete the bullet once it collides
    void OnCollisionEnter2D(Collision2D coll)
    {
        Destroy(gameObject);
        if (gameObject.tag.Equals("Enemy Bullet"))
        {
            if (coll.gameObject.tag.Equals("Player")) coll.gameObject.GetComponent<Player>().loseLife();
        }
        else
        {
            if (coll.gameObject.tag.Contains("Room"))
            {
                if (PlayerPrefs.GetInt("muted") == 0) Instantiate(killSound, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                Enemy enemy = coll.gameObject.GetComponent<Enemy>();
                enemy.loseLife();

                if (!enemy.isDead || enemy.type == Enemy.Types.Surviellance) return;
                Vector3 destination = new Vector3();
                destination.Set(coll.gameObject.transform.position.x, coll.gameObject.transform.position.y, -10);

                if ((int) (Random.value * POWERUP_DROP_CHANCE) == 0)
                    Instantiate(powerups[(int)(Random.value * powerups.Length)], destination, Quaternion.Euler(0,0,0));

                Destroy(coll.gameObject);
            }
        }
    }
}
