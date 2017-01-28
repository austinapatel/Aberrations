using UnityEngine;
using System.Collections;

public class StartInside : MonoBehaviour {

    public GameObject outsideLight;
    public GameObject playerLight;
    public GameObject closingDoor;
    public GameObject[] grounds;
    public GameObject water;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag.Equals("Player"))
        {
            outsideLight.SetActive(false);
            playerLight.SetActive(true);
            closingDoor.SetActive(false);
            water.SetActive(false);

            foreach (GameObject ground in grounds)
                ground.SetActive(false);
        }
    }
}
