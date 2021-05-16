using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DeadKoker : DialogueNPC
{
    public ScreenFader screenFader;

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = collision.gameObject;
            Action();
        }
    }

    protected override void DialogueExit()
    {
        base.DialogueExit();
        screenFader.fadeState = ScreenFader.FadeState.In;
        Invoke("LoadScene", 5.0f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
}
