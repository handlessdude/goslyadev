using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyTrigger : MonoBehaviour
{
    [HideInInspector]
    public Enemy parent;

    public bool isAttackTrigger;
    public Enemy.Sense sense;


    private void Start()
    {
        if (!parent)
        {
            parent = GetComponentInParent<Enemy>();
            if (!parent)
            {
                Debug.LogError("Enemy Trigger couldn't find its parent's Enemy Script!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (isAttackTrigger)
            {
                parent.OnEnterAttackRange(collision.gameObject);
            }
            else
            {
                parent.OnEnterSenseRange(collision.gameObject, sense);
            }
        }
    }
}

#if UNITY_EDITOR


[CustomEditor(typeof(EnemyTrigger))]
public class EnemyTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var trigger = target as EnemyTrigger;

        trigger.isAttackTrigger = GUILayout.Toggle(trigger.isAttackTrigger, "isAttackTrigger");

        if (!trigger.isAttackTrigger)
            trigger.sense = (Enemy.Sense)EditorGUILayout.EnumPopup(trigger.sense, new GUILayoutOption[] { });

    }
}
#endif
