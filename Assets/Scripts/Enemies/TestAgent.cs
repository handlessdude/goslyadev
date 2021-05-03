using System.Collections;
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
    Pathfinder p;

    // Start is called before the first frame update
    void Start()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        p = new Pathfinder(areaMarkers);
    }

    private void OnDrawGizmos()
    {
        if (p != null)
        {
            foreach (var t in p.map)
            {
                Vector2 v = Pathfinder.GetCellCenter(t.Key);
                //Gizmos.DrawIcon(v, "sv_icon_dot" + t.Value.GetNeighbourNodes().Count() + "_pix16_gizmo", true);
                
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
            path = p.FindPath(transform.position, target.transform.position, rb, 700f, 6);
            //Debug.Log("Here " + (path == null));
            targetPos = new Vector2Int(Mathf.FloorToInt(target.transform.position.x), Mathf.FloorToInt(target.transform.position.y));
        }
        
    }
}
