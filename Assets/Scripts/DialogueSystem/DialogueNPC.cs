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

    float charTypingTime = 0.06f;
    float drawnDialogShowTime = 1f;
    float backgroundRollingSpeed = 0.02f;

    Coroutine typeText;
    Coroutine stretchBar;

    bool zoomedOut = false;
    protected bool inDialogue = false;

    enum DialogueState
    {
        Misc,
        DrawingBox,
        TypingText,
        DrawnDialogDelay,
        Selection
    }

    DialogueState dialogueState = DialogueState.Misc;

    protected override void Action()
    {
        base.Action();
        if (currentDialogElement.GetType() != typeof(DialogueEndElement))
        {
            DialogueEnter();
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
            //player.GetComponent<WorldSwitcher>().DefaultWorld();
            Transform cameraTarget = player.transform.Find("CameraTarget");
            cameraTarget.position = new Vector2((cameraTarget.position.x + transform.position.x) / 2, (cameraTarget.position.y + transform.position.y) / 2 + 1f);
            zoomedOut = cc.ZoomedOut();
            cc.ZoomIn();
            dialogueSelection.SetGoverningNPC(this);
            DrawDialogueText();
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
        textComponent = currentDialogueBox.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        stretchBar = StartCoroutine(StretchBar(text));
    }

    float CalculateLineLength(string text)
    {
        float length = text.Length * 0.125f + 0.5f;
        if (length > 4.5f)
        {
            length = 4.5f;
        }
        return length;
    }
    float CalculateBubleHeight(string text)
    {
        float length = text.Length * 0.125f + 0.5f;
        if (length < 4.5f)
        {
            length = 4.5f;
        }
        return length / 4.5f;
    }
    IEnumerator StretchBar(string text)
    {
        dialogueState = DialogueState.DrawingBox;
        float length = CalculateLineLength(text);
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
        print(dialogueState);
        switch (dialogueState)
        {
            case DialogueState.Selection:
                {
                    dialogueState = DialogueState.Misc;
                    Destroy(currentDialogueBox);
                    SwitchDialogElement((currentDialogElement as SelectionDialogueElement).ChooseNext(i));
                    break;
                }
            case DialogueState.DrawingBox:
                {
                    StopCoroutine(stretchBar);
                    string text = Localization.GetLocalizedString(currentDialogElement.textValue);
                    float length = CalculateLineLength(text);
                    RectTransform rtMain = currentDialogueBox.transform.Find("Background").GetComponent<RectTransform>();
                    RectTransform rtLeft = currentDialogueBox.transform.Find("Leftbackground").GetComponent<RectTransform>();
                    RectTransform rtRight = currentDialogueBox.transform.Find("Rightbackground").GetComponent<RectTransform>();
                    rtMain.sizeDelta = new Vector2(length, rtMain.sizeDelta.y);
                    rtLeft.localPosition = new Vector2(-length / 2, rtLeft.localPosition.y);
                    rtRight.localPosition = new Vector2(length / 2, rtRight.localPosition.y);
                    BackgroundReady(text);
                    break;
                }
            case DialogueState.TypingText:
                {
                    CancelInvoke("OnDialogueDrawn");
                    StopCoroutine(typeText);
                    textComponent.text = Localization.GetLocalizedString(currentDialogElement.textValue);
                    dialogueState = DialogueState.DrawnDialogDelay;
                    //Invoke("OnDialogueDrawn", drawnDialogShowTime);
                    break;
                }
            case DialogueState.DrawnDialogDelay:
                {
                    CancelInvoke("OnDialogueDrawn");
                    OnDialogueDrawn();
                    break;
                }
        }
    }

    void BackgroundReady(string text)
    {
        typeText = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        dialogueState = DialogueState.TypingText;
        textComponent.text = "";
        foreach(char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(charTypingTime);
        }
        CancelInvoke("OnDialogueDrawn");
        dialogueState = DialogueState.DrawnDialogDelay;
        //Invoke("OnDialogueDrawn", drawnDialogShowTime);
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
            
            dialogueState = DialogueState.Selection;
            //Invoke("SelectionDelay", 1f);
        }
    }

    void SelectionDelay()
    {
        CancelInvoke("SelectionDelay");
        dialogueState = DialogueState.Selection;
    }

    protected virtual void DialogueExit()
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
