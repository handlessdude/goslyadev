﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class TestAgent : MonoBehaviour
{

    public Transform[] areaMarkers;
    public GameObject target;
    public Rigidbody2D rb;
    public Vector2Int targetPos = Vector2Int.zero;
    LinkedList<PathNode> path;

    // Start is called before the first frame update
    void Start()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (Pathfinder.map != null)
        {
            foreach (var t in Pathfinder.map)
            {
                Vector2 v = Pathfinder.GetCellCenter(t.Key);
                //Gizmos.DrawIcon(v, "sv_icon_dot" + t.Value.children.Count() + "_pix16_gizmo", true);
                Gizmos.DrawIcon(v, "sv_icon_dot7_pix16_gizmo", true);
                foreach (var child in t.Value.children)
                {
                    Gizmos.DrawLine(v, Pathfinder.GetCellCenter(child.pos));
                }
            }
        }
        if (path != null)
        {
            foreach (var node in path)
            {
                if (node.jumpNode)
                {
                    Gizmos.DrawIcon(node.pos, "sv_icon_dot11_pix16_gizmo", true);
                }
                else
                {
                    Gizmos.DrawIcon(node.pos, "sv_icon_dot13_pix16_gizmo", true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPos != new Vector2Int(Mathf.FloorToInt(target.transform.position.x), Mathf.FloorToInt(target.transform.position.y)))
        {
            path = Pathfinder.FindPath(transform.position, target.transform.position, rb, 800f, 6);
            //Debug.Log("Here " + (path == null));
            targetPos = new Vector2Int(Mathf.FloorToInt(target.transform.position.x), Mathf.FloorToInt(target.transform.position.y));
        }
        
    }
}
