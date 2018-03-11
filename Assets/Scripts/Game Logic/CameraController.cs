using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float DampTime = 0.15f;
    private Vector3 Velocity = Vector3.zero;

    public Transform Target;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (Target)
        {
            Vector3 Point = Camera.main.WorldToViewportPoint(Target.position);
            Vector3 Delta = Target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Point.z));
            Vector3 Destination = transform.position + Delta;
            transform.position = Vector3.SmoothDamp(transform.position, Destination, ref Velocity, DampTime);
        }
    }
}
