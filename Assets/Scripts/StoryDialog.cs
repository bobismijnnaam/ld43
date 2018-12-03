using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoryDialog : MonoBehaviour {

    [TextArea(3, 10)]
    public string[] dialogues;
    public Text dialogueText;
    public Image dialogueBox;
    public UnityEvent dialogueFinishedEvent;
    public AudioClip talkAudio;

    enum State {
        DOING_DIALOGUE,
        IDLE
    }

    State state = State.IDLE;
    int currentDialogue;
    int currentLine;

    public void StartDialogue(int i) {
        if (i >= dialogues.Length) {
            throw new Exception("Dialogue index " + i + " is out of bounds! Max: " + dialogues.Length);
        }

        Debug.Log("Starting dialogue " + i);

        currentDialogue = i;
        currentLine = 0;

        SetDialogueBoxVisible(true);
        SetTextVisible(true);
        ShowLine(currentDialogue, currentLine);
        state = State.DOING_DIALOGUE;
    }

	// Use this for initialization
	void Start () {
		SetDialogueBoxVisible(false);
        SetTextVisible(false);
	}
	
	// Update is called once per frame
	void Update () {
        switch (state) {
            case State.IDLE:
                break;
            case State.DOING_DIALOGUE:
                if (Input.GetButtonDown("Jump")) {
                    currentLine++;
                    if (currentLine < GetNumLinesOfDialogue(currentDialogue)) {
                        ShowLine(currentDialogue, currentLine);
                    } else {
                        SetDialogueBoxVisible(false);
                        SetTextVisible(false);
                        state = State.IDLE;
                        dialogueFinishedEvent.Invoke();
                    }
                }
                break;
            default:
                throw new Exception("Unexpected state: " + state);
        }
	}

    int GetNumLinesOfDialogue(int i) {
        string dialogue = dialogues[i];
        var lines = dialogue.Split('\n');
        return lines.Length;
    }

    void ShowLine(int dialogue, int i) {
        var lines = dialogues[dialogue].Split('\n');
        dialogueText.text = lines[i];

        StartCoroutine(playTalkAudio(0));
        StartCoroutine(playTalkAudio(0.2f));
        StartCoroutine(playTalkAudio(0.4f));
        StartCoroutine(playTalkAudio(0.6f));
    }

    IEnumerator playTalkAudio(float delay) {
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(talkAudio, Camera.main.transform.position, 1);
    }

    void SetDialogueBoxVisible(bool visible) {
        dialogueBox.enabled = visible;
    }

    void SetTextVisible(bool visible) {
        dialogueText.enabled = visible;
    }
}
