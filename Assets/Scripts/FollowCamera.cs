using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public Transform subject;
    public float stepSize;
    public float lookAheadDistance = 2;

    Vector3 prevPos;
    Vector3 currentTarget;

	// Use this for initialization
	void Start () {
	    prevPos = subject.position;
        currentTarget = subject.position;
	}
	
	// Update is called once per frame
	void Update () {
        //currentTarget = subject.position + (Vector3) subject.GetComponent<Rigidbody2D>().velocity * lookAheadDistance;
        //prevPos = subject.position;
        
        currentTarget = subject.position;

        var targetDelta = currentTarget - transform.position;
        targetDelta *= (stepSize * Time.deltaTime);
        targetDelta.z = 0;
        transform.position += targetDelta;
        
        
        //var rb = subject.GetComponent<Rigidbody2D>();
        
        //if (rb.velocity.x != 0) {
            //currentTarget = subject.position + Vector3.right * lookAheadDistance * Mathf.Sign(rb.velocity.x);           
        //} else {
            //currentTarget = subject.position;
        //}
        
        //var targetDelta = currentTarget - transform.position;
        //targetDelta *= (stepSize * Time.deltaTime);
        //targetDelta.z = 0;
        //transform.position += targetDelta;
        
	}
}
