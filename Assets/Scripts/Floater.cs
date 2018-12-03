using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour {
    
    public float speed;
    public float radius;

    Vector3 startPos;
    float currentAngle;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
        currentAngle = Random.Range(0, Mathf.PI * 2);
	}
	
	// Update is called once per frame
	void Update () {
		currentAngle += Time.deltaTime * speed;
        transform.position = startPos + Vector3.up * radius * Mathf.Sin(currentAngle);
	}
}
