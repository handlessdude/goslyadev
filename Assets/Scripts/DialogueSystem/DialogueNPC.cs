using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueNPC : Interactable
{
    public GameObject dialogueBoxPrefab;
    public DialogueElement currentDialogElement;
    public CameraController cc;
    public DialogueSelection dialogueSelection;
    //public GameObject player;

    protected GameObject currentDialogueBox;
    TextMeshProUGUI textComponent;

    protected float selfBubbleHeight = 1.5f;
    protected float characterBubbleHeight = 1.5f;

    float charTypingTime = 0.05f;
    float drawnDialogShowTime = 1f;
    float backgroundRollingSpeed = 0.02f;

    bool zoomedOut = false;
    protected bool inDialogue = false;

    /*void Start()
    {
        currentDialogElement = new SequentialDialogueElement(
        "dialogue_test1", true, new SequentialDialogueElement(
            "dialogue_test2", false, new SelectionDialogueElement("dialogue_test3",
                new string[] { "dialogue_option_test1", "dialogue_options_test2" },
                new System.Func<DialogueElement>[] {
                    () => currentDialogElement,
                    () => new DialogueEndElement(currentDialogElement)
                })));;

        /*currentDialogElement = new SequentialDialogueElement(
        "dialogue_test1", true, new SequentialDialogueElement(
            "dialogue_test2", false, new DialogueEndElement()));
    }*/

    protected override void Action()
    {
        base.Action();
        if (currentDialogElement.GetType() != typeof(DialogueEndElement))
        {
            DialogueEnter();
        }
    }

    protected virtual void RotateSelf()
    {
        if (transform.position.x < player.transform.position.x)
        {
            transform.localScale = new Vector2(transform.localScale.x * (-1f), 1);
            foreach (Transform child in transform)
            {
                child.localScale = new Vector2(child.localScale.x * (-1), child.localScale.y);
            }
        }
    }
    

    void DialogueEnter()
    {
        if (!inDialogue)
        {
            inDialogue = true;
            RotateSelf();
            player.GetComponent<PlayerController>().RotateToVector(transform.position);
            GameplayState.controllability = PlayerControllability.InDialogue;
            player.GetComponent<WorldSwitcher>().DefaultWorld();
            Transform cameraTarget = player.transform.Find("CameraTarget");
            cameraTarget.position = new Vector2((cameraTarget.position.x + transform.position.x) / 2, (cameraTarget.position.y + transform.position.y) / 2 + 1f);
            zoomedOut = cc.ZoomedOut();
            cc.ZoomIn();
            DrawDialogueText();
        }
    }

    void DrawDialogueText()
    {
        if (currentDialogElement.isOnCharacter)
        {
            currentDialogueBox = Instantiate(dialogueBoxPrefab);
            currentDialogueBox.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + characterBubbleHeight);
        }
        else
        {
            currentDialogueBox = Instantiate(dialogueBoxPrefab);
            currentDialogueBox.transform.position = new Vector2(transform.position.x, transform.position.y + selfBubbleHeight);
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

    public virtual void Response(int i)
    {
        if (currentDialogElement.GetType() == typeof(SelectionDialogueElement))
        {
            Destroy(currentDialogueBox);
            SwitchDialogElement((currentDialogElement as SelectionDialogueElement).ChooseNext(i));
        }
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

    protected virtual void OnDialogueDrawn()
    {
        if (currentDialogElement.GetType() == typeof(SequentialDialogueElement))
        {
            Destroy(currentDialogueBox);
            textComponent = null;
            SwitchDialogElement((currentDialogElement as SequentialDialogueElement).next);
        }
        else if (currentDialogElement.GetType() == typeof(SelectionDialogueElement))
        {
            dialogueSelection.DisplayOptions((currentDialogElement as SelectionDialogueElement).playerChoices, this);
        }
    }

    void DialogueExit()
    {
        inDialogue = false;
        RotateSelf();
        if (zoomedOut)
        {
            cc.ZoomOut();
        }
        GameplayState.controllability = PlayerControllability.Full;
        Transform cameraTarget = player.transform.Find("CameraTarget");
        cameraTarget.localPosition = Vector3.zero;
    }
}
