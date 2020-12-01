using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueNPC : MonoBehaviour
{
    public GameObject dialogueBoxPrefab;
    public DialogueElement currentDialogElement;
    public CameraController cc;
    public GameObject player;

    GameObject currentDialogueBox;
    TextMeshProUGUI textComponent;

    float charTypingTime = 0.05f;
    float drawnDialogShowTime = 1f;
    float backgroundRollingSpeed = 0.02f;

    void Start()
    {
        /*currentDialogElement = new SequentialDialogueElement(
        "dialogue_test1", true, new SequentialDialogueElement(
            "dialogue_test2", false, new SelectionDialogueElement("dialogue_test3",
                new List<string>() { "dialogue_option_test1", "dialogue_options_test2" },
                new List<System.Func<DialogueElement>>() {
                    () => currentDialogElement,
                    () => new DialogueEndElement()
                })));;*/

        currentDialogElement = new SequentialDialogueElement(
        "dialogue_test1", true, new SequentialDialogueElement(
            "dialogue_test2", false, new DialogueEndElement()));
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (currentDialogElement.GetType() != typeof(DialogueEndElement))
            {
                player = collision.gameObject;
                DialogueEnter();
            }
        }
    }

    void DialogueEnter()
    {
        GameplayState.controllability = PlayerControllability.InDialogue;
        player.GetComponent<WorldSwitcher>().DefaultWorld();
        Transform cameraTarget = player.transform.Find("CameraTarget");
        cameraTarget.position = new Vector2((cameraTarget.position.x+transform.position.x)/2, (cameraTarget.position.y + transform.position.y) / 2+1f);
        cc.ZoomIn();
        DrawDialogueText();
    }

    void DrawDialogueText()
    {
        Invoke("OnDialogueDrawn", 2.0f);
        if (currentDialogElement.isOnCharacter)
        {
            currentDialogueBox = Instantiate(dialogueBoxPrefab, player.transform);
            currentDialogueBox.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 2);
        }
        else
        {
            currentDialogueBox = Instantiate(dialogueBoxPrefab, transform);
            currentDialogueBox.transform.position = new Vector2(transform.position.x, transform.position.y + 2);
        }

        string text = Localization.GetLocalizedString(currentDialogElement.textValue);
        StartCoroutine(StretchBar(text));
        textComponent = currentDialogueBox.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    IEnumerator StretchBar(string text)
    {
        float length = text.Length*0.125f + 0.5f;
        RectTransform rtMain = currentDialogueBox.transform.Find("Background").GetComponent<RectTransform>();
        RectTransform rtLeft = currentDialogueBox.transform.Find("Leftbackground").GetComponent<RectTransform>();
        RectTransform rtRight = currentDialogueBox.transform.Find("Rightbackground").GetComponent<RectTransform>();
        rtMain.sizeDelta = new Vector2(0, rtMain.sizeDelta.y);
        rtLeft.localPosition = new Vector2(0, rtLeft.localPosition.y);
        rtRight.localPosition = new Vector2(0, rtRight.localPosition.y);
        Debug.Log(rtMain.sizeDelta.x);
        while (rtMain.sizeDelta.x < length-0.6f)
        {
            float width = rtMain.sizeDelta.x + 0.6f;
            rtMain.sizeDelta = new Vector2(width, rtMain.sizeDelta.y);
            rtLeft.localPosition = new Vector2(-width/2, rtLeft.localPosition.y);
            rtRight.localPosition = new Vector2(width/2, rtRight.localPosition.y);
            yield return new WaitForSeconds(backgroundRollingSpeed);
        }
        rtMain.sizeDelta = new Vector2(length, rtMain.sizeDelta.y);
        rtLeft.localPosition = new Vector2(-length / 2, rtLeft.localPosition.y);
        rtRight.localPosition = new Vector2(length / 2, rtRight.localPosition.y);
        BackgroundReady(text);
    }

    void BackgroundReady(string text)
    {
        StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        textComponent.text = "";
        foreach(char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(charTypingTime);
        }
        CancelInvoke("OnDialogueDrawn");
        Invoke("OnDialogueDrawn", drawnDialogShowTime);
    }

    void SwitchDialogElement(DialogueElement dialogueElement)
    {
        currentDialogElement = dialogueElement;
        if (currentDialogElement.GetType() == typeof(DialogueEndElement))
        {
            currentDialogElement = (currentDialogElement as DialogueEndElement).nextOnNextVisit;
            DialogueExit();
        }
        else
        {
            DrawDialogueText();
        }
    }

    void OnDialogueDrawn()
    {
        if (currentDialogElement.GetType() == typeof(SequentialDialogueElement))
        {
            Destroy(currentDialogueBox);
            textComponent = null;
            SwitchDialogElement((currentDialogElement as SequentialDialogueElement).next);
        }
        else if (currentDialogElement.GetType() == typeof(SelectionDialogueElement))
        {

        }
    }

    void DialogueExit()
    {
        GameplayState.controllability = PlayerControllability.Full;
        Transform cameraTarget = player.transform.Find("CameraTarget");
        cameraTarget.localPosition = Vector3.zero;
    }
}
