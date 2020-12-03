using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToUnderstand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetKeyDown(KeyAction.WorldGreen) ||
            InputManager.GetKeyDown(KeyAction.WorldCyan) ||
            InputManager.GetKeyDown(KeyAction.WorldMagenta))
        {
            Destroy(gameObject);
        }
    }
}
