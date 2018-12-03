using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryLine : MonoBehaviour {

    enum ArcState {
        INITIAL,
        ON_QUEST,
        OFFERING,
        OFFERING_FINISHED
    }

    enum GameState {
        FADING_IN,
        IN_GAME,
        FADING_OUT
    }

    public GameObject sacrifice;
    public Image fadeSprite;
    public Transform spawnPositionObj;
    public Transform wiseManTalking;

    public float fadeMaxDuration = 2;

    StoryDialog dialogue;
    LootTracker loot;
    Player player;
    FollowCamera followCam;
    OfferMachine offerMachine;
    AudioSource levelUpAudio;

    ArcState arcState;
    GameState gameState;
    int storyProgress = 0; // Indicates which dialogue is next
    float fadeDuration = 0;

	// Use this for initialization
	void Start () {
		dialogue = FindObjectOfType<StoryDialog>();
        loot = FindObjectOfType<LootTracker>();
        player = FindObjectOfType<Player>();
        followCam = FindObjectOfType<FollowCamera>();
        offerMachine = FindObjectOfType<OfferMachine>();
        levelUpAudio = GetComponent<AudioSource>();

        arcState = ArcState.INITIAL;

        player.enabled = false;
        FadeIn();
	}
	
	// Update is called once per frame
	void Update () {
        switch (gameState) {
            case GameState.FADING_IN:
                fadeDuration += Time.deltaTime;
                if (fadeDuration >= fadeMaxDuration) {
                    SetFadeAlpha(0);
                    gameState = GameState.IN_GAME;

                    SetPlayerFocus(false);
                } else {
                    SetFadeAlpha(1 - fadeDuration / fadeMaxDuration);
                }
                break;
            case GameState.FADING_OUT:
                fadeDuration += Time.deltaTime;
                if (fadeDuration >= fadeMaxDuration) {
                    SetFadeAlpha(1);
                    FadeIn();

                    // Respawn playerk
                    player.Respawn();

                    // Move camera as well
                    var pos = player.transform.position;
                    pos.z = followCam.transform.position.z;
                    followCam.transform.position = pos;

                    // Respawn sacrifice
                    sacrifice.SetActive(true);

                    if (storyProgress == 4) {
                        // Finished!
                        SceneManager.LoadScene("GameFinished");
                    }
                } else {
                    SetFadeAlpha(fadeDuration / fadeMaxDuration);
                }
                break;
            case GameState.IN_GAME:
                break;
        }
	}

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.GetComponent<Player>() != null) {
            PlayerOnPedestal();
        }
    }

    void FadeIn() {
        gameState = GameState.FADING_IN;
        fadeDuration = 0;
        SetFadeAlpha(1);
    }

    void FadeOut() {
        gameState = GameState.FADING_OUT;
        fadeDuration = 0;
        SetFadeAlpha(0);
    }

    void SetFadeAlpha(float alpha) {
        var c = fadeSprite.color;
        c.a = alpha;
        fadeSprite.color = c;
    }

    void PlayerOnPedestal() {
        if (arcState == ArcState.INITIAL) {
            StartDialogueWithPlayer(storyProgress);
        } else if (arcState == ArcState.ON_QUEST) {
            if (loot.NumSlotsInUse() != 4) {
                StartDialogueWithPlayer(5);
            } else {
                DoOffering();
            }
        }
    }

    void DoOffering() {
        SetPlayerFocus(true);
        arcState = ArcState.OFFERING;
        followCam.subject = sacrifice.transform;
        offerMachine.DoOffering(loot.GetSprite());
        loot.ResetSlots();
    }

    void StartDialogueWithPlayer(int i) {
        dialogue.StartDialogue(i);
        player.SetLookLeft(false);
        SetPlayerFocus(true);
        followCam.subject = wiseManTalking;
    }

    void SetPlayerFocus(bool focusEnabled) {
        if (focusEnabled) {
            player.StandStill();
            var pos = player.transform.position;
            pos.x = transform.position.x;
            pos.y = transform.position.y + 0.5f;
            player.transform.position = pos;
        }

        player.enabled = !focusEnabled;
    }

    void GrantPower(int powerIndex) {
        switch (powerIndex) {
            case 0:
                player.hasJump = true;
                break;
            case 1:
                player.hasLongJump = true;
                break;
            case 2:
                player.hasRun = true;
                break;
            case 3:
                player.hasWallJump = true;
                break;
        }
    }

    public void DialogueFinished() {
        if (arcState == ArcState.INITIAL) {
            SetPlayerFocus(false);
            arcState = ArcState.ON_QUEST;
            GrantPower(storyProgress);
            AudioSource.PlayClipAtPoint(levelUpAudio.clip, followCam.transform.position);
            followCam.subject = player.transform;
        } else if (arcState == ArcState.ON_QUEST) {
            followCam.subject = player.transform;
            SetPlayerFocus(false);
        } else if (arcState == ArcState.OFFERING_FINISHED) {
            storyProgress++;
            arcState = ArcState.INITIAL;
            followCam.subject = player.transform;
            FadeOut();
        }
    }

    public void OfferingFinished() {
        arcState = ArcState.OFFERING_FINISHED;
        followCam.subject = wiseManTalking;
        StartDialogueWithPlayer(4);
    }
}
