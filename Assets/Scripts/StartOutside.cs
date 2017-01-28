using UnityEngine;
using System.Collections;

public class StartOutside : MonoBehaviour {

    public GameObject outsideLight;
    public GameObject playerLight;
    public GameObject closingDoor;
    public GameObject[] grounds;
    public GameObject water;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag.Equals("Player"))
        {
            outsideLight.SetActive(true);
            playerLight.SetActive(false);
            closingDoor.SetActive(true);
            water.SetActive(true);

            foreach (GameObject ground in grounds)
                ground.SetActive(true);
        }
    }
}
