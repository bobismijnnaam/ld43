using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour {

    AudioSource audio;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.GetComponent<Player>() != null) {
            var lootTracker = FindObjectOfType<LootTracker>();
            var sr = GetComponent<SpriteRenderer>();
            lootTracker.FillSlot(sr.sprite);

            if (audio != null) {
                //Debug.Log("Playing!");
                //audio.Play();
                AudioSource.PlayClipAtPoint(audio.clip, transform.position);
            }

            gameObject.SetActive(false);
            Destroy(gameObject, 1);
        }
    }
}
