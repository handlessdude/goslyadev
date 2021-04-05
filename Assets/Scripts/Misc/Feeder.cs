using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feeder : Interactable
{
    public GameObject successNotification;

    public Animator lightAnimator;
    public GameObject lights;
    public GameObject oldRobot;
    public GameObject newRobot;

    GameObject instantiated;
    // Start is called before the first frame update
    void Start()
    {
        if (GameplayState.isPreparationEnded)
            EndGamePreparations();
    }

    protected override void Action()
    {
        print(GameplayState.barrels);
        print(GameplayState.feededBarrels);
        base.Action();
        if (GameplayState.barrels > 0)
        {
            GameplayState.feededBarrels += 1;
            GameplayState.barrels -= 1;
            DisplaySuccessNotification();
            if (GameplayState.feededBarrels > 1)
            {
                EndGamePreparations();
                GameplayState.isPreparationEnded = true;
            }
        }
    }

    void EndGamePreparations()
    {
        lights.SetActive(true);
        lightAnimator.Play("lit");
        oldRobot.SetActive(false);
        newRobot.SetActive(true);
    }

    void DisplaySuccessNotification()
    {
        if (instantiated)
        {
            Destroy(instantiated);
        }

        instantiated = Instantiate(successNotification, player.transform);
        instantiated.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 1.0f);
        Invoke("DestroyNotification", 2.0f);
    }

    void DestroyNotification()
    {
        CancelInvoke("DestroyNotification");
        if (instantiated)
        {
            Destroy(instantiated);
        }
    }
}
