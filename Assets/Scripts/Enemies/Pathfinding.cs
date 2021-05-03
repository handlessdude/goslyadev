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
    public MapPathNode mapPathNode;
    public PathNode parent;
    public bool isInAir = false;
    public Vector2Int jumpDirection;

    public bool jumpNode = false;
    public float optJumpForce;
    public float optVelocityX;

    public PathNode(Vector2 pos, MapPathNode mapPathNode)
    {
        this.pos = pos;
        this.mapPathNode = mapPathNode;
        gCost = 0;
        hCost = 0;
        fCost = 0;
    }
}

//используется для подготовки карты
public class MapPathNode
{
    public Vector2Int pos;
    public MapPathNode left;
    public MapPathNode right;
    public MapPathNode up;
    public MapPathNode down;

    //TODO: начать использовать этот параметр
    public int maxCharacterHeight;
    
    public MapPathNode(Vector2Int pos)
    {
        this.pos = pos;
    }

    public bool IsOnGround()
    {
        return this.down == null;
    }

    public MapPathNode FindGroundNode()
    {
        MapPathNode result = this;
        while (!result.IsOnGround())
        {
            result = result.down;
        }
        return result;
    }

    public HashSet<MapPathNode> GetNeighbourNodes()
    {
        HashSet<MapPathNode> result = new HashSet<MapPathNode>();
        if (this.left != null)
        {
            result.Add(this.left);
        }

        if (this.right != null)
        {
            result.Add(this.right);
        }

        if (this.up != null)
        {
            result.Add(this.up);
        }

        if (this.down != null)
        {
            result.Add(this.down);
        }

        return result;
    }

    public static explicit operator PathNode(MapPathNode mp)
    {
        return new PathNode(Pathfinder.GetCellCenter(mp.pos), mp);
    }
}

public class Pathfinder
{
    //GameObject colliderInst;
    //Collider2D colliderChecker;
    public Dictionary<Vector2Int, MapPathNode> map;

    Transform[] areaMarkers;

    public static float cellSize = 1.0f;
    public static int maxCellsInArea = 2600;

    public Pathfinder(Transform[] areaMarkers)
    {
        //Object colliderObject = Resources.Load("CircleChecker");
        //colliderInst = GameObject.Instantiate(colliderObject) as GameObject;
        //colliderChecker = colliderInst.GetComponent<Collider2D>();

        this.areaMarkers = areaMarkers;

        InitializeMap();
    }

    bool IsSpaceWalkable(Vector2 pos1, Vector2 pos2)
    {
        RaycastHit2D[] raycastHits = Physics2D.LinecastAll(pos1, pos2);
        return !raycastHits.Any(x => ((x.collider.isTrigger == false) && (x.transform.tag != "Enemy") && (x.transform.tag != "Player")));
    }

    //здесь есть повторяющиеся вычисления, которые можно было бы перенести в FindPath, но он и без того слабо читаем
    bool IsNodeReachable(PathNode start, PathNode destination, Rigidbody2D agentBody, float jumpForce, float maxVelocityX)
    {
        float deltaY = destination.pos.y - start.pos.y;
        float velocityY = jumpForce * Time.fixedDeltaTime;
        float gravity = - Physics2D.gravity.y * agentBody.gravityScale;
        float maxJumpHeight = velocityY * velocityY / (2 * gravity);
        if (deltaY > maxJumpHeight)
        {
            return false;
        }

        float jumpTime;
        float deltaX = Mathf.Abs(destination.pos.x - start.pos.x); 
        if (deltaY <= 0)
        {
            jumpTime = 2 * velocityY / gravity;
            float groundLevelVelocity = velocityY - gravity * jumpTime;
            float fallTime = (-groundLevelVelocity - Mathf.Sqrt(groundLevelVelocity*groundLevelVelocity - 2*gravity*deltaY))/(-gravity);
            return deltaX < maxVelocityX * (jumpTime + fallTime);
        }

        float ascendTime = (-velocityY + Mathf.Sqrt(velocityY * velocityY - 2 * gravity * deltaY)) / (-gravity);
        float targetLevelVelocity = velocityY - gravity * ascendTime;
        jumpTime = targetLevelVelocity / gravity;
        return deltaX < maxVelocityX * (jumpTime + ascendTime);
    }

    void UpdateOptimalJumpParameters(PathNode start, PathNode destination, Rigidbody2D agentBody, float jumpForce, float maxVelocityX)
    {
        start.jumpNode = true;
    }

    //TODO: рефактор
    void InitializeMap()
    {
        map = new Dictionary<Vector2Int, MapPathNode>();
        HashSet<Vector2Int> closed = new HashSet<Vector2Int>();
        
        foreach (var marker in areaMarkers)
        {
            Vector2Int pos = ConvertToIntCoords(marker.position);
            
            MapPathNode start = new MapPathNode(pos);
            Queue<MapPathNode> q = new Queue<MapPathNode>();
            q.Enqueue(start);
            map[pos] = start;
            while (q.Count != 0 && map.Count < maxCellsInArea)
            {
                MapPathNode node = q.Dequeue();
                if (closed.Contains(node.pos))
                {
                    continue;
                }
                closed.Add(node.pos);

                Vector2 center = GetCellCenter(node.pos);
                Vector2Int p;
                if (IsSpaceWalkable(center, center + new Vector2(cellSize, 0f)))
                {
                    p = node.pos + new Vector2Int(1, 0);
                    MapPathNode right;
                    if (map.ContainsKey(p))
                    {
                        right = map[p];
                    }
                    else
                    {
                         right = new MapPathNode(p);
                        map[p] = right;
                        q.Enqueue(right);
                    }
                    
                    node.right = right;
                }

                if (IsSpaceWalkable(center, center + new Vector2(-cellSize, 0f)))
                {
                    p = node.pos + new Vector2Int(-1, 0);
                    MapPathNode left;
                    if (map.ContainsKey(p))
                    {
                        left = map[p];
                    }
                    else
                    {
                        left = new MapPathNode(p);
                        map[p] = left;
                        q.Enqueue(left);
                    }
                    node.left = left;
                }

                if (IsSpaceWalkable(center, center + new Vector2(0f, cellSize)))
                {
                    p = node.pos + new Vector2Int(0, 1);
                    MapPathNode up;
                    if (map.ContainsKey(p))
                    {
                        up = map[p];
                    }
                    else
                    {
                        up = new MapPathNode(p);
                        map[p] = up;
                        q.Enqueue(up);
                    }
                    node.up = up;
                }

                if (IsSpaceWalkable(center, center + new Vector2(0f, -cellSize)))
                {
                    p = node.pos + new Vector2Int(0, -1);
                    MapPathNode down;
                    if (map.ContainsKey(p))
                    {
                        down = map[p];
                    }
                    else
                    {
                        down = new MapPathNode(p);
                        map[p] = down;
                        q.Enqueue(down);
                    }
                    node.down = down;
                }

            }
        }
    }

    public static Vector2Int ConvertToIntCoords(Vector2 v)
    {
        return new Vector2Int(Mathf.FloorToInt(v.x/cellSize), Mathf.FloorToInt(v.y / cellSize));
    }

    public static Vector2 GetCellCenter(Vector2Int v)
    {
        return new Vector2(v.x + cellSize/2, v.y + cellSize/2);
    }

    float heuristic_cost_estimate(PathNode nodeA, PathNode nodeB)
    {
        float deltaX = Mathf.Abs(nodeA.pos.x - nodeB.pos.x);
        float deltaY = Mathf.Abs(nodeA.pos.y - nodeB.pos.y);

        float gravity_bonus = nodeA.pos.y - nodeB.pos.y > 0 ? 0.05f : -0.05f;

        return deltaX*(1-gravity_bonus) + deltaY*(1+gravity_bonus);
    }

    LinkedList<PathNode> ConstructPath(PathNode end, MapPathNode start, Rigidbody2D agentBody, float jumpForce, float maxVelocityX)
    {
        LinkedList<PathNode> path = new LinkedList<PathNode>();
        PathNode jumpDest = end;
        PathNode lastNode = end;
        while (end != null)
        {
            if (!end.isInAir)
            {
                if (path.Count != 0 && jumpDest != lastNode)
                {
                    UpdateOptimalJumpParameters(end, jumpDest, agentBody, jumpForce, maxVelocityX);
                }
                jumpDest = end;
                path.AddFirst(end);
            }
            lastNode = end;
            end = end.parent;
        }

        return path;
    }

    LinkedList<PathNode> ConstructBadPath(PathNode end, MapPathNode start, Rigidbody2D agentBody, float jumpForce, float maxVelocityX)
    {
        LinkedList<PathNode> path = new LinkedList<PathNode>();
        PathNode jumpDest = end;
        while (end != null)
        {
            path.AddFirst(end);
            end = end.parent;
        }

        return path;
    }

    public LinkedList<PathNode> FindPath(Vector2 agentPos, Vector2 targetPos, Rigidbody2D agentBody, float jumpForce, float maxVelocityX)
    {
        Vector2Int startPos = ConvertToIntCoords(agentPos);
        Vector2Int endPos = ConvertToIntCoords(targetPos);
        if (!map.ContainsKey(startPos) || !map.ContainsKey(endPos))
        {
            return null;
        }
        return FindPath(map[startPos], map[endPos], agentBody, jumpForce, maxVelocityX);
    }

    public LinkedList<PathNode> FindPath(MapPathNode startNode, MapPathNode endNode, Rigidbody2D agentBody, float jumpForce, float maxVelocityX)
    {
        Debug.Log("Search started");
        List<PathNode> openSet = new List<PathNode>();
        
        int num_iterations = 0;
        startNode = startNode.FindGroundNode();
        endNode = endNode.FindGroundNode();
        openSet.Add((PathNode)startNode);
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
            openSet.Remove(currentNode);
            if (num_iterations++ > 1000)
            {
                return null;
            }

            if (currentNode.mapPathNode == endNode)
            {
                Debug.Log("FOUND!");
                return ConstructPath(currentNode, startNode, agentBody, jumpForce, maxVelocityX);
            }

            PathNode lastGroundNode = currentNode;
            while (lastGroundNode.isInAir)
            {
                lastGroundNode = lastGroundNode.parent;
            }

            foreach (MapPathNode _neighbour in currentNode.mapPathNode.GetNeighbourNodes())
            {
                PathNode neighbour = (PathNode)_neighbour;

                neighbour.isInAir = !neighbour.mapPathNode.IsOnGround();

                if (neighbour.isInAir || currentNode.isInAir)
                {
                    if (_neighbour == currentNode.mapPathNode.left)
                    {
                        if (currentNode.isInAir && currentNode.jumpDirection == Vector2Int.right)
                        {
                            continue;
                        }
                        neighbour.jumpDirection = Vector2Int.left;
                    }
                    else if (_neighbour == currentNode.mapPathNode.right)
                    {
                        if (currentNode.isInAir && currentNode.jumpDirection == Vector2Int.left)
                        {
                            continue;
                        }
                        neighbour.jumpDirection = Vector2Int.right;
                    }
                    else if (neighbour.isInAir)
                    {
                        neighbour.jumpDirection = currentNode.jumpDirection;
                    }
                }

                float ground_modifier = neighbour.isInAir? 1.2f : 1.0f;

                neighbour.parent = currentNode;
                neighbour.gCost = currentNode.gCost + heuristic_cost_estimate(currentNode, neighbour);
                neighbour.hCost = heuristic_cost_estimate(neighbour, (PathNode)endNode)*ground_modifier;
                neighbour.fCost = neighbour.hCost + neighbour.gCost;

                bool breaker = false;
                foreach (PathNode openNode in openSet)
                {
                    if ((openNode.mapPathNode == neighbour.mapPathNode) && (neighbour.gCost > openNode.gCost))
                    {
                        breaker = true;
                        break;
                    }
                }
                if (breaker)
                {
                    continue;
                }

                if (!neighbour.isInAir && neighbour.parent.isInAir && !IsNodeReachable(lastGroundNode, neighbour, agentBody, jumpForce, maxVelocityX))
                {
                    continue;
                }



                openSet.Add(neighbour);
            }
        }
        return null;
    }
}
