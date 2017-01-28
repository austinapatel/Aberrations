// Created by Austin Patel on 5/28/16 at 4:57 PM

using UnityEngine;

public class EnemySpawners : MonoBehaviour {

    private const int SPAWN_DISTANCE = 4; // Distance not allowed for enemy to spawn near player 
    public GameObject spawnerParent;
    private Transform[] spawnLocations;
    public GameObject[] enemies;
    private int[] counts;
    private int[] delays;
    public int delayMax;
    public GameObject parent;
    public GameObject player;
    private int curTime;
    private const int TIME_DELAY = 300;

	void Start () {
        spawnLocations = new Transform[spawnerParent.transform.childCount];

        for (int i = 0; i < spawnLocations.Length; i++)
            spawnLocations[i] = spawnerParent.transform.GetChild(i);

        counts = new int[spawnLocations.Length];
        delays = new int[spawnLocations.Length];

        for (int i = 0; i < delays.Length; i++)
            delays[i] = (int) (Random.value * delayMax) + 100;

        player = GameObject.Find("Player");
	}
	
	void FixedUpdate () {
        curTime++;
        if (curTime == TIME_DELAY)
        {
            for (int i = 0; i < delays.Length; i++)
                if (delays[i] > 40)
                {
                    delays[i] -= 1;
                    if (counts[i] > delays[i])
                        counts[i] = delays[i] - 1;
                }

            curTime = 0;
        }
        
	    for (int i = 0; i < counts.Length; i++)
        {
            counts[i]++;
            if (counts[i] == delays[i])
            {
                counts[i] = 0;
                //delays[i] = (int) (Random.value * delayMax) + 1;
                int spot = (int) (Random.value * enemies.Length);
                if (PointsToClose(player.transform.position, spawnLocations[i].position)) continue;
                GameObject obj = (GameObject) Instantiate(enemies[spot], spawnLocations[i].position, enemies[spot].transform.rotation);
                obj.transform.parent = parent.transform;
            }
        }
	}

    bool PointsToClose(Vector2 p1, Vector2 p2)
    {
        if (Mathf.Abs(p1.x - p2.x) < SPAWN_DISTANCE) return true;
        if (Mathf.Abs(p1.y - p2.y) < SPAWN_DISTANCE) return true;
        return false;
    }
}
