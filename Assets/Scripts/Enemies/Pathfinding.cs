using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

//используется конкретно для A*. эти ноды одноразовые для каждого пути
public class PathNode
{
    public Vector2 pos;
    public float fCost;
    public float gCost;
    public float hCost;

    public PathNode(Vector2 pos)
    {
        this.pos = pos;
        gCost = 0;
        hCost = 0;
        fCost = 0;
    }
}

//используется для подготовки карты
public class MapPathNode
{
    public Vector2Int pos;
    MapPathNode left;
    MapPathNode right;
    MapPathNode up;
    MapPathNode down;

    //TODO: начать использовать этот параметр
    public int maxCharacterHeight;
    
    public MapPathNode(Vector2Int pos)
    {
        this.pos = pos;
    }
}

public class Pathfinder
{
    GameObject colliderInst;
    Collider2D colliderChecker;
    Dictionary<Vector2Int, MapPathNode> map;

    Transform[] areaMarkers;

    public static float cellSize = 1.0f;
    public static int maxCellsInArea = 2600;

    public Pathfinder(Transform[] areaMarkers)
    {
        Object colliderObject = Resources.Load("CircleChecker");
        colliderInst = GameObject.Instantiate(colliderObject) as GameObject;
        colliderChecker = colliderInst.GetComponent<Collider2D>();

        this.areaMarkers = areaMarkers;

        InitializeMap();
    }

    bool IsSpaceWalkable(Vector2 pos1, Vector2 pos2)
    {
        RaycastHit2D[] raycastHits = Physics2D.LinecastAll(pos1, pos2);
        return !raycastHits.Any(x => ((x.collider.isTrigger == false) && (x.transform.tag != "Enemy") && (x.transform.tag != "player")));
    }

    void InitializeMap()
    {
        map = new Dictionary<Vector2Int, MapPathNode>();
        
        foreach (var marker in areaMarkers)
        {
            Vector2Int pos = ConvertToIntCoords(marker.position);
            
            MapPathNode start = new MapPathNode(pos);
            Queue<MapPathNode> q = new Queue<MapPathNode>();
            q.Enqueue(start);
            while (q.Count != 0 && map.Count < maxCellsInArea)
            {
                MapPathNode node = q.Dequeue();
                if (map.ContainsKey(node.pos))
                {
                    continue;
                }

                map[node.pos] = node;
                Vector2 center = GetCellCenter(node.pos);

                if (IsSpaceWalkable(center, center + new Vector2(cellSize, 0f)))
                {
                    MapPathNode right = new MapPathNode(node.pos + new Vector2Int(1, 0));
                    q.Enqueue(right);
                }

                if (IsSpaceWalkable(center, center + new Vector2(-cellSize, 0f)))
                {
                    MapPathNode left = new MapPathNode(node.pos + new Vector2Int(-1, 0));
                    q.Enqueue(left);
                }

                if (IsSpaceWalkable(center, center + new Vector2(0f, cellSize)))
                {
                    MapPathNode up = new MapPathNode(node.pos + new Vector2Int(0, 1));
                    q.Enqueue(up);
                }

                if (IsSpaceWalkable(center, center + new Vector2(0f, -cellSize)))
                {
                    MapPathNode down = new MapPathNode(node.pos + new Vector2Int(0, -1));
                    q.Enqueue(down);
                }

            }
        }
    }

    Vector2Int ConvertToIntCoords(Vector2 v)
    {
        return new Vector2Int(Mathf.FloorToInt(v.x/cellSize), Mathf.FloorToInt(v.y / cellSize));
    }

    Vector2 GetCellCenter(Vector2Int v)
    {
        return new Vector2(v.x + cellSize/2, v.y + cellSize/2);
    }

    HashSet<MapPathNode> GetNeighbourNodes(MapPathNode node)
    {
        HashSet<MapPathNode> result = new HashSet<MapPathNode>();
        Vector2Int left = node.pos + new Vector2Int(-1, 0);
        Vector2Int right = node.pos + new Vector2Int(1, 0);
        Vector2Int up = node.pos + new Vector2Int(0, 1);
        Vector2Int down = node.pos + new Vector2Int(0, -1);
        if (map.ContainsKey(left))
        {
            result.Add(map[left]);
        }

        if (map.ContainsKey(right))
        {
            result.Add(map[right]);
        }

        if (map.ContainsKey(up))
        {
            result.Add(map[up]);
        }

        if (map.ContainsKey(down))
        {
            result.Add(map[down]);
        }

        return result;
    }

    float heuristic_cost_estimate(PathNode nodeA, PathNode nodeB)
    {
        float deltaX = Mathf.Abs(nodeA.pos.x - nodeB.pos.x);
        float deltaY = Mathf.Abs(nodeA.pos.y - nodeB.pos.y);

        return deltaX + deltaY;
    }

    public LinkedList<PathNode> FindPath(Vector2 agentPos, Vector2 targetPos)
    {
        Vector2Int startPos = ConvertToIntCoords(agentPos);
        Vector2Int endPos = ConvertToIntCoords(targetPos);
        if (!map.ContainsKey(startPos) && !map.ContainsKey(endPos))
        {
            return null;
        }


    }

    public LinkedList<PathNode> FindPath(PathNode startNode, PathNode endNode)
    {
        Debug.Log("Search started");
        LinkedList<PathNode> path = new LinkedList<PathNode>();
        List<PathNode> openSet = new List<PathNode>();
        List<Vector2> closedCoords = new List<Vector2>();
        List<PathNode> closedSet = new List<PathNode>();
        openSet.Add(startNode);
        while (openSet.Count != 0)
        {
            float minf = float.MaxValue;
            PathNode currentNode = null;
            foreach (PathNode p in openSet)
            {
                if (p.fCost < minf)
                {
                    minf = p.fCost;
                    currentNode = p;
                }
            }
            //Debug.Log(currentNode);
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            closedCoords.Add(new Vector2(currentNode.pos.x, currentNode.pos.y));
            if (closedCoords.Count > 50)
            {
                return null;
            }
            if ((currentNode.pos.x == endNode.pos.x) && (currentNode.pos.y == endNode.pos.y))
            {
                while (currentNode != startNode)
                {
                    path.AddFirst(currentNode);
                    currentNode = currentNode.parent;
                }
                Debug.Log("FOUND!");
                return path;
            }

            foreach (PathNode neighbour in GetNeighbourNodes(currentNode))
            {
                neighbour.gCost = currentNode.gCost + heuristic_cost_estimate(currentNode, neighbour);
                neighbour.hCost = heuristic_cost_estimate(neighbour, endNode);
                neighbour.fCost = neighbour.hCost + neighbour.gCost;

                bool breaker = false;
                foreach (PathNode openNode in openSet)
                {
                    if ((openNode.pos.x == neighbour.pos.x) && (openNode.pos.y == neighbour.pos.y) && (neighbour.gCost > openNode.gCost))
                    {
                        breaker = true;
                        break;
                    }
                }
                if (breaker)
                {
                    continue;
                }


                openSet.Add(neighbour);
            }
        }
        return path;
    }
}
