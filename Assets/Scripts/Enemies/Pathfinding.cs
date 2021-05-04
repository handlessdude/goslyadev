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

    public bool jumpNode = false; //нужно ли прыгать агенту с этой ноды
    public bool landingNode = false; //всегда стоит после jumpNode, нужна только для бэктрекинга
    //для jumpNode хранятся параметра прыжка в дочерней ноде, то есть в landingNode,
    //это конечно плохо, но зато бэктрекинг проще
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
    public List<MapPathNode> children;

    public bool inAir = false;

    //TODO: начать использовать этот параметр
    public int maxCharacterHeight;
    
    public MapPathNode(Vector2Int pos)
    {
        this.pos = pos;
        children = new List<MapPathNode>();
    }

    public bool IsOnGround()
    {
        return !inAir;
    }

    public bool IsAdjacent(MapPathNode other)
    {
        return Mathf.Abs(this.pos.x - other.pos.x) + Mathf.Abs(this.pos.y - other.pos.y) <= 1;
    }

    public static explicit operator PathNode(MapPathNode mp)
    {
        return new PathNode(Pathfinder.GetCellCenter(mp.pos), mp);
    }
}

public class Pathfinder
{
    public Dictionary<Vector2Int, MapPathNode> map;

    Transform[] areaMarkers;

    public static float cellSize = 1.0f;
    public static int maxCellsInArea = 2600;

    public Pathfinder(Transform[] areaMarkers)
    {
        this.areaMarkers = areaMarkers;

        map = new Dictionary<Vector2Int, MapPathNode>();

        InitializeMap();
    }

    bool IsSpaceWalkable(Vector2 pos1, Vector2 pos2)
    {
        RaycastHit2D[] raycastHits = Physics2D.LinecastAll(pos1, pos2);
        return !raycastHits.Any(x => ((x.collider.isTrigger == false) && (x.transform.tag != "Enemy") && (x.transform.tag != "Player")));
    }

    //здесь есть повторяющиеся вычисления, которые можно было бы перенести в FindPath, но он и без того слабо читаем
    bool UpdateOptimalJumpParameters(PathNode start, PathNode destination, Rigidbody2D agentBody, float jumpForce, float maxVelocityX)
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
            float groundLevelVelocity = -velocityY;
            float fallTime = (-groundLevelVelocity - Mathf.Sqrt(groundLevelVelocity*groundLevelVelocity - 2*gravity*deltaY))/(-gravity);

            if (deltaX < maxVelocityX * (jumpTime + fallTime))
            {
                float optimal_time = deltaX / maxVelocityX;
                float optimal_velocityY = (gravity * optimal_time * optimal_time + 2 * deltaY) / (2 * optimal_time);
                if (optimal_velocityY < 0)
                {
                    optimal_velocityY = 0f;
                }

                destination.optJumpForce = optimal_velocityY / Time.fixedDeltaTime;
                destination.optVelocityX = maxVelocityX;

                if (optimal_velocityY == 0f)
                {
                    return true;
                }

                float parabola_height = optimal_velocityY * optimal_velocityY / (2 * gravity);
                jumpTime = 2 * optimal_velocityY / gravity;
                float parabola_half_len = jumpTime * destination.optVelocityX / 2;
                Vector2 parabola_top_point = new Vector2(start.pos.x + parabola_half_len, start.pos.y + parabola_height);
                //эвристика, но пойдет. Лайнкастить что-то близкое к настоящей параболе дорого
                return IsSpaceWalkable(start.pos, parabola_top_point) && IsSpaceWalkable(destination.pos, parabola_top_point);
            }
            else
                return false;
        }

        float ascendTime = (-velocityY + Mathf.Sqrt(velocityY * velocityY - 2 * gravity * deltaY)) / (-gravity);
        float targetLevelVelocity = velocityY - gravity * ascendTime;
        jumpTime = targetLevelVelocity / gravity;

        if (deltaX < maxVelocityX * (jumpTime + ascendTime))
        {
            float optimal_time = deltaX / maxVelocityX;
            float optimal_velocityY = gravity * optimal_time;
            ascendTime = (-optimal_velocityY + Mathf.Sqrt(optimal_velocityY * optimal_velocityY - 2 * gravity * deltaY)) / (-gravity);
            targetLevelVelocity = optimal_velocityY - gravity * ascendTime;
            destination.optJumpForce = optimal_velocityY / Time.fixedDeltaTime;
            destination.optVelocityX = maxVelocityX;
            float parabola_height = targetLevelVelocity * targetLevelVelocity / (2 * gravity);
            float parabolaTime = 2 * targetLevelVelocity / gravity;
            float parabola_half_len = parabolaTime * destination.optVelocityX / 2;
            Vector2 parabola_top_point = new Vector2(destination.pos.x - parabola_half_len, start.pos.y + parabola_height);
            return IsSpaceWalkable(start.pos, parabola_top_point) && IsSpaceWalkable(destination.pos, parabola_top_point);
        }
        else
            return false;
    }

    //TODO: рефактор
    void InitializeMap()
    {
        HashSet<Vector2Int> closed = new HashSet<Vector2Int>();

        //генерируем отдельные зоны (chunks) вокруг каждого areaMarker
        foreach (var marker in areaMarkers)
        {
            Dictionary<Vector2Int, MapPathNode>  chunk = new Dictionary<Vector2Int, MapPathNode>();
            Vector2Int pos = ConvertToIntCoords(marker.position);
            
            MapPathNode start = new MapPathNode(pos);
            Queue<MapPathNode> q = new Queue<MapPathNode>();
            q.Enqueue(start);
            chunk[pos] = start;
            //это breadth first, добавляем все ноды, которые соседние к рассматриваемой клетке
            //(даже те, что в воздухе)
            while (q.Count != 0 && chunk.Count < maxCellsInArea)
            {
                MapPathNode node = q.Dequeue();
                if (closed.Contains(node.pos))
                {
                    continue;
                }
                closed.Add(node.pos);

                Vector2 center = GetCellCenter(node.pos);
                Vector2Int p;

                List<Vector2> v2 = new List<Vector2> 
                { new Vector2(cellSize, 0f), new Vector2(-cellSize, 0f),
                 new Vector2(0f, cellSize), new Vector2(0f, -cellSize)};
                List<Vector2Int> v2int = new List<Vector2Int>
                { new Vector2Int(1, 0), new Vector2Int(-1, 0),
                 new Vector2Int(0, 1), new Vector2Int(0, -1)};

                for (int i = 0;  i < 4; i++)
                {
                    if (IsSpaceWalkable(center, center + v2[i]))
                    {
                        p = node.pos + v2int[i];
                        MapPathNode child;
                        if (chunk.ContainsKey(p))
                        {
                            child = chunk[p];
                        }
                        else
                        {
                            child = new MapPathNode(p);
                            chunk[p] = child;
                            q.Enqueue(child);
                        }

                        if (i == 3) //да, костыль
                        {
                            node.inAir = true;
                        }
                        node.children.Add(child);
                    }
                }
                
            }

            //удаляем все воздушные ноды
            var itemsToRemove = chunk.Where(x => !x.Value.IsOnGround()).ToArray();
            foreach (var item in itemsToRemove)
            {
                foreach (var child in item.Value.children)
                {
                    
                    child.children.RemoveAll(x => x == item.Value);
                }
                chunk.Remove(item.Key);
            }

            //строим связи между платформами
            int current_x = 0;
            Dictionary<int, MapPathNode> lastInRow = new Dictionary<int, MapPathNode>(); //ключ это y-координата ряда
            List<MapPathNode> column = new List<MapPathNode>();
            var comparer = Comparer<KeyValuePair<Vector2Int, MapPathNode>>.Create((x, y) => (x.Key.x - y.Key.x));
            var chunk_as_list = chunk.ToList();
            chunk_as_list.Sort(comparer);
            foreach (var x in chunk_as_list)
            {
                MapPathNode mp_node = x.Value;
                Vector2Int coord = x.Key;
                //TODO: рефактор этой части
                if (coord.x != current_x)
                {
                    foreach (var column_node in column)
                    {
                        foreach (var last_row_node in lastInRow)
                        {
                            if (lastInRow.ContainsKey(column_node.pos.y) && (lastInRow[column_node.pos.y].pos.x + 1 == column_node.pos.x))
                            {
                                if (last_row_node.Value.pos.x + 1 != column_node.pos.x
                                    || column.Any(f => (f.pos.y == last_row_node.Key) && (f.pos.x == last_row_node.Value.pos.x + 1)))
                                {
                                    continue;
                                }
                            }

                            last_row_node.Value.children.Add(column_node);
                            column_node.children.Add(last_row_node.Value);
                        }
                    }

                    foreach (var column_node in column)
                    {
                        lastInRow[column_node.pos.y] = column_node;
                    }

                    column = new List<MapPathNode>();
                    current_x = coord.x;
                }

                column.Add(mp_node);
            }

            foreach (var kv in chunk)
            {
                map[kv.Key] = kv.Value;
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

        return deltaX + deltaY;
    }

    LinkedList<PathNode> ConstructPath(PathNode end, MapPathNode start, Rigidbody2D agentBody, float jumpForce, float maxVelocityX)
    {
        LinkedList<PathNode> path = new LinkedList<PathNode>();
        PathNode jumpDest = end;
        while (end != null)
        {
            path.AddFirst(end);
            if (end.landingNode)
            {
                end.parent.jumpNode = true;
            }
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

            foreach (MapPathNode _neighbour in currentNode.mapPathNode.children)
            {
                PathNode neighbour = (PathNode)_neighbour;

                neighbour.landingNode = !_neighbour.IsAdjacent(currentNode.mapPathNode);

                float ground_modifier = neighbour.landingNode ? 1.0f : 1.2f;

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

                if (neighbour.landingNode)
                {
                    bool success = UpdateOptimalJumpParameters(currentNode, neighbour, agentBody, jumpForce, maxVelocityX);
                    if (!success)
                    {
                        continue;
                    }
                }

                openSet.Add(neighbour);
            }
        }
        return null;
    }
}
