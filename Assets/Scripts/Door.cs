// Created by Austin Patel on 5/21/16 at 5:53 PM

using UnityEngine;

public class Door : MonoBehaviour {

    private bool startOpen;
    public int doorOpenDuration;
    private GameObject leftDoor, rightDoor;
    private float moveDistance;
    private float movePerFrame;
    private int curTime;
    private bool opened;

	void Start () {
        leftDoor = transform.Find("Left").gameObject;
        rightDoor = transform.Find("Right").gameObject;

        moveDistance = gameObject.transform.localScale.y;
        movePerFrame = moveDistance / doorOpenDuration;
	}
	
	void Update () {
        if (startOpen)
        {
            curTime++;
            if (curTime < doorOpenDuration)
            {
                leftDoor.transform.Translate(new Vector3(movePerFrame, 0, 0));
                rightDoor.transform.Translate(new Vector3(-movePerFrame, 0, 0));
            }
            else
            {
                startOpen = false;
                curTime = 0;
                opened = true;
            }         
        }
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag.Equals("Player"))
        {
            startOpen = true;
            if (opened) startOpen = false;
        }
    }
}
