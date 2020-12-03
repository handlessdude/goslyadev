using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSelection : MonoBehaviour
{

    [SerializeField]
    public List<GameObject> optionItems;

    [HideInInspector]
    List<TextMeshProUGUI> textItems;

    DialogueNPC governingNPC;

    Color selected = Color.white;
    Color unselected = Color.Lerp(Color.white,Color.gray, 0.7f);

    int size;
    int currentPos;

    void Start()
    {
        if (optionItems.Count != 1)
        {
            Debug.LogWarning("DialogueSelection is not initialized correctly!");
        }

        textItems = new List<TextMeshProUGUI>();
        textItems.Add(optionItems[0].transform.Find("Text").GetComponent<TextMeshProUGUI>());
    }

    void Update()
    {
        if (GameplayState.controllability == PlayerControllability.InDialogue && governingNPC != null && !GameplayState.isPaused)
        {
            if (InputManager.GetKeyDown(KeyAction.LookUp))
            {
                SwitchUp();
            }
            if (InputManager.GetKeyDown(KeyAction.LookDown))
            {
                SwitchDown();
            }
            if (InputManager.GetKeyDown(KeyAction.Confirm))
            {
                SelectOption();
            }
        }
    }

    void SelectOption()
    {
        governingNPC.Response(currentPos);
        HideOptions();
    }

    void SwitchUp()
    {
        textItems[currentPos].color = unselected;
        currentPos = (currentPos - 1 + size) % size;
        textItems[currentPos].color = selected;
    }

    void SwitchDown()
    {
        textItems[currentPos].color = unselected;
        currentPos = (currentPos + 1) % size;
        textItems[currentPos].color = selected;
    }

    public void DisplayOptions(string[] options, DialogueNPC governingNPC)
    {
        this.governingNPC = governingNPC;
        size = options.Length;
        currentPos = 0;

        optionItems[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 32*(size-1)+12);
        textItems[0].text = Localization.GetLocalizedString(options[0]);
        textItems[0].color = selected;
        optionItems[0].SetActive(true);
        for (int i = 1; i < size; i++)
        {
            //TODO: instatiate здесь лучше было бы не использовать, их всё равно вполне ограниченное количество
            optionItems.Add(Instantiate(optionItems[0],transform));
            textItems.Add(optionItems[i].transform.Find("Text").GetComponent<TextMeshProUGUI>());
            textItems[i].text = Localization.GetLocalizedString(options[i]);
            textItems[i].color = unselected;
            optionItems[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 32 * (size - 1 - i) + 12);
        }
    }

    public void HideOptions()
    {
        for (int i = size-1; i > 0; i--)
        {
            Destroy(optionItems[i]);
            optionItems.RemoveAt(i);
            textItems.RemoveAt(i);
        }

        textItems[0].text = "";
        optionItems[0].SetActive(false);

    }
}
