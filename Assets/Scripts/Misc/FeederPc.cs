using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeederPc : Interactable
{
    public GameObject successNotification;

    GameObject instantiated;
    public GameObject Hint;
    public GameObject phone;
    // Start is called before the first frame update
    void Start()
    {
    }

    protected override void Action()
    {
        base.Action();
        if (GameplayState.boards > 0)
        {
            GameplayState.feededboards += 1;
            GameplayState.boards -= 1;
            DisplaySuccessNotification();
            if (GameplayState.feededboards > 2)
            {
                GameplayState.controllability = PlayerControllability.FirstDialog;
                Hint.SetActive(true);
                phone.SetActive(true);
            }
        }
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
