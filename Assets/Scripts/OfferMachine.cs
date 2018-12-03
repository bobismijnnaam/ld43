using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OfferMachine : MonoBehaviour {

    enum State {
        IDLE,
        BUILD_TENSION,
        WARMING_UP,
        COOLING_DOWN
    }

    public SpriteRenderer offer1;
    public SpriteRenderer offer2;
    public SpriteRenderer offer3;
    public SpriteRenderer offer4;
    public GameObject sacrifice;
    public float tensionDuration = 1;
    public float warmingUpDuration = 2;
    public float coolingDownDuration = 1;
    public UnityEvent animationFinishedEvent;
    public AudioClip tensionAudio;
    public AudioClip warmingUpAudio;
    public AudioClip coolingDownAudio;

    SpriteRenderer sr;

    State state;
    float duration;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        state = State.IDLE;

        SetOffersEnabled(false);
	}
	
	// Update is called once per frame
	void Update () {
        switch (state) {
            case State.IDLE:
                break;
            case State.BUILD_TENSION:
                duration += Time.deltaTime;
                if (duration >= tensionDuration) {
                    state = State.WARMING_UP;
                    duration = 0;
                    sr.enabled = true;
                    AudioSource.PlayClipAtPoint(warmingUpAudio, Camera.main.transform.position);
                }
                break;
            case State.WARMING_UP:
                duration += Time.deltaTime;
                if (duration >= warmingUpDuration) {
                    state = State.COOLING_DOWN;
                    duration = 0;
                    sr.enabled = false;
                    sacrifice.SetActive(false);
                    SetOffersEnabled(false);
                    AudioSource.PlayClipAtPoint(coolingDownAudio, Camera.main.transform.position);
                }
                break;
            case State.COOLING_DOWN:
                duration += Time.deltaTime;
                if (duration >= warmingUpDuration) {
                    state = State.IDLE;
                    animationFinishedEvent.Invoke();
                }
                break;
        }
	}

    public void DoOffering(Sprite offerSprite) {
        SetOffers(offerSprite);
        SetOffersEnabled(true);

        state = State.BUILD_TENSION;
        duration = 0;

        AudioSource.PlayClipAtPoint(tensionAudio, Camera.main.transform.position);
    }

    void SetOffers(Sprite offerSprite) {
        offer1.sprite = offerSprite;
        offer2.sprite = offerSprite;
        offer3.sprite = offerSprite;
        offer4.sprite = offerSprite;
    }

    void SetOffersEnabled(bool enabled) {
        offer1.enabled = enabled;
        offer2.enabled = enabled;
        offer3.enabled = enabled;
        offer4.enabled = enabled;
    }
}
