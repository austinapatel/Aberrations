// Created by Austin Patel on 6/2/16 at 10:36 AM

using UnityEngine;

public class Lighting : MonoBehaviour {

    private const float MAX_LIGHT = 1.0f;
    private float lightChange = -0.001f;

    private Light thisLight;

	void Start () {
        thisLight = GetComponent<Light>();
        thisLight.intensity = MAX_LIGHT;
	}
	
	void FixedUpdate () {
        if (Application.platform == RuntimePlatform.Android) return;
        thisLight.intensity += lightChange;

        if (thisLight.intensity >= MAX_LIGHT)
        {
            thisLight.intensity = 0.99f;
            lightChange *= -1;
        }

        if (thisLight.intensity <= 0.0f)
        {
            lightChange *= -1; ;
            thisLight.intensity = 0.01f;
        }
    }
    
}
