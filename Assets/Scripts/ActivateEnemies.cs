// Created by Austin Patel on 5/21/16 at 3:25 PM

using UnityEngine;

public class ActivateEnemies : MonoBehaviour {
    public int roomNumber;
    private GameObject[] enemies;
    private bool activated;

    void Start () {
        enemies = GameObject.FindGameObjectsWithTag("Room " + roomNumber);

        foreach (GameObject enemy in enemies)
            enemy.SetActive(false);
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (activated) return;
        if (coll.gameObject.tag.Equals("Player"))
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.SetActive(true);
                enemy.GetComponentInParent<Enemy>().start = true;
            }
            activated = true;
        }      
    }
}
