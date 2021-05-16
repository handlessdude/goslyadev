using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueNPC : Interactable
{
    public GameObject dialogueBoxPrefab;
    public DialogueElement currentDialogElement;
    public DialogueObject dialogueFile;
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
        try
        {
            if (currentDialogElement.GetType() != typeof(DialogueEndElement))
            {
                DialogueEnter();
            }
        }
        catch (UnityException e){
            Debug.LogWarning(e.Message);
        };
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

    protected virtual void Start()
    {
        if (currentDialogElement == null)
        {
            DeserializeDialogueFile();
        }
    }

    protected void DeserializeDialogueFile()
    {
        if (!dialogueFile)
        {
            Debug.LogError("No dialogue file!");
            return;
        }

        Dictionary<int, DialogueElement> processed = new Dictionary<int, DialogueElement>();
        Queue<(int, DialogueElement)> q = new Queue<(int, DialogueElement)>();

        for (int i = dialogueFile.nodes.Count - 1; i >= 0; i--)
        {
            if (dialogueFile.nodes[i].type == DialogueNodeObject.Type.End)
            {
                processed[i] = new DialogueEndElement();
                processed[i].textValue = dialogueFile.nodes[i].textValue;
                processed[i].isOnCharacter = dialogueFile.nodes[i].isOnCharacter;
                if (dialogueFile.nodes[i].nextOnNextVisit < 0)
                {
                    continue;
                }
                if (processed.ContainsKey(dialogueFile.nodes[i].nextOnNextVisit))
                {
                    (processed[i] as DialogueEndElement).nextOnNextVisit = processed[dialogueFile.nodes[i].nextOnNextVisit];
                }
                else
                {
                    q.Enqueue((dialogueFile.nodes[i].nextOnNextVisit, processed[i]));
                }
            }
            else if (dialogueFile.nodes[i].type == DialogueNodeObject.Type.Sequential)
            {
                processed[i] = new SequentialDialogueElement();
                processed[i].textValue = dialogueFile.nodes[i].textValue;
                processed[i].isOnCharacter = dialogueFile.nodes[i].isOnCharacter;
                if (processed.ContainsKey(dialogueFile.nodes[i].next))
                {
                    (processed[i] as SequentialDialogueElement).next = processed[dialogueFile.nodes[i].next];
                }
                else
                {
                    q.Enqueue((dialogueFile.nodes[i].next, processed[i]));
                }
            }
            else if (dialogueFile.nodes[i].type == DialogueNodeObject.Type.Selection)
            {
                processed[i] = new SelectionDialogueElement();
                (processed[i] as SelectionDialogueElement).playerChoices = dialogueFile.nodes[i].playerChoice_keys.ToArray();
                processed[i].textValue = dialogueFile.nodes[i].textValue;
                (processed[i] as SelectionDialogueElement).next = new List<System.Func<DialogueElement>>();
                foreach (var x in dialogueFile.nodes[i].playerChoice_values)
                {
                    q.Enqueue((x, processed[i]));
                }
            }
        }

        while (q.Count != 0)
        {
            (int, DialogueElement) pair = q.Dequeue();
            DialogueElement d_node = pair.Item2;
            int i = pair.Item1;
            if (d_node is SelectionDialogueElement)
            {
                (d_node as SelectionDialogueElement).next.Add(() => processed[i]);
            }
            else if (d_node is DialogueEndElement)
            {
                (d_node as DialogueEndElement).nextOnNextVisit = processed[i];
            }
            else if (d_node is SequentialDialogueElement)
            {
                (d_node as SequentialDialogueElement).next = processed[i];
            }
        }

        currentDialogElement = processed[0];
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

        string text = Localization.GetLocalizedString(currentDialogElement.textValue.key);
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

    int CalculateNumberOfLines(string text)
    {
        return text.Length / 32 + 1;
    }

    IEnumerator StretchBar(string text)
    {
        dialogueState = DialogueState.DrawingBox;
        float length = CalculateLineLength(text);
        int lines = CalculateNumberOfLines(text);
        
        RectTransform rtMain = currentDialogueBox.transform.Find("Background").GetComponent<RectTransform>();
        RectTransform rtLeft = currentDialogueBox.transform.Find("Leftbackground").GetComponent<RectTransform>();
        RectTransform rtRight = currentDialogueBox.transform.Find("Rightbackground").GetComponent<RectTransform>();
        rtMain.sizeDelta = new Vector2(0, rtMain.sizeDelta.y);
        
        rtLeft.localPosition = new Vector2(0, rtLeft.localPosition.y);
        rtRight.localPosition = new Vector2(0, rtRight.localPosition.y);

        //если больше 2-х строк, то пора растягивать баббл
        //проблема в том, что пиксели в баббле растягиваются
        //мелочь, но надо бы переделать эту систему
        if (lines > 2)
        {
            float height_multiplier = 1.1f + (lines - 2) / 2f;
            rtMain.localScale = new Vector3(rtMain.localScale.x, rtMain.localScale.y * height_multiplier, rtMain.localScale.z);
            rtLeft.localScale = new Vector3(rtLeft.localScale.x, rtLeft.localScale.y * height_multiplier, rtLeft.localScale.z);
            rtRight.localScale = new Vector3(rtRight.localScale.x, rtRight.localScale.y * height_multiplier, rtRight.localScale.z);
            textComponent.rectTransform.anchorMax = new Vector2(0.5f, 0.5f*height_multiplier);
        }
        else
        {
            rtMain.localScale = Vector3.one;
            rtLeft.localScale = Vector3.one;
            rtRight.localScale = Vector3.one;
            textComponent.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        }

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
                    string text = Localization.GetLocalizedString(currentDialogElement.textValue.key);
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
                    textComponent.text = Localization.GetLocalizedString(currentDialogElement.textValue.key);
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
