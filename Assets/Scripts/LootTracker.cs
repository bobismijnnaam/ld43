using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootTracker : MonoBehaviour {

    public Sprite placeHolder;
    public Image[] images;

    int currentSlot;

	// Use this for initialization
	void Start () {
        ResetSlots();
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            FillSlot(placeHolder);
        }
    }

    public void ResetSlots() {
        foreach (var image in images) {
            image.sprite = null;
            image.color = new Color(0, 0, 0, 0);
        }

        currentSlot = 0;
    }

    public void FillSlot(Sprite sprite) {
        if (currentSlot < images.Length) {
            images[currentSlot].sprite = sprite;
            images[currentSlot].color = Color.white;
            currentSlot++;
        } else {
            throw new Exception("Out of slots!");
        }
    }

    public Sprite GetSprite() {
        return images[0].sprite;
    }

    public int NumSlotsInUse() {
        return currentSlot;
    }
}
