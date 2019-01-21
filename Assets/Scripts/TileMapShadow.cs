using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NewBehaviourScript : MonoBehaviour {

    private void Awake()
    {
        GetComponent<TilemapRenderer>().receiveShadows = true;
        GetComponent<TilemapRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
