using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Start()
    {
        if (!text)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText(GameplayState.boards.ToString()+"/"+ (3 - GameplayState.feededboards).ToString());
        if (3 - GameplayState.feededboards == 0)
        {
            text.SetText("DONE!");
        }
    }
}
