using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPathFollower : MonoBehaviour
{
    // =========================================================
    // GRID
    // =========================================================

    [Header("Grid")]
    public GridGenerator gridGenerator;
    public RoundScript roundScript;


    // =========================================================
    // AI
    // =========================================================

    [Header("AI Targets")]
    public Transform prey;
    public Transform predator;

    [Header("AI Weights")]

    [Header("AI")]
    private float thinkInterval = 0.05f;
    public float decisionRadius = 5f;


    [Header("Panic Burst")]


    float stuckTimer = 0f;
    Vector3 lastPosition;

    bool edgeEscapeActive = false;
    float edgeEscapeTimer = 0f;

    [SerializeField] float stuckTime = 1f;
    [SerializeField] float edgeEscapeDuration = 2f; 


    // Vector3 wanderTarget;
    bool panicBurstActive = false;
    public float panicEndTime = 0f;

    public float chaseSkill;
    public float escapeSkill;

    public bool isChasing;

    GridNodeData currentGoal;

    [HideInInspector] public Coroutine goCoroutine;

    // =========================================================
    // MOVEMENT
    // =========================================================

    [Header("Movement")]
    Queue<GridNodeData> pathQueue = new Queue<GridNodeData>();


    public GridNodeData currentNode;

    CharacterMovement cm;
    CharacterAnimation ca;


    Coroutine pathFollowCoro;

    // =========================================================
    // START
    // =========================================================
    void Awake()
    {
        gridGenerator = FindObjectOfType<GridGenerator>();
        // roundScript = FindObjectOfType<RoundScript>();

        cm = GetComponent<CharacterMovement>();
        ca = GetComponent<CharacterAnimation>();
    }
    void Start()
    {
        currentNode = GetClosestNode(transform.position);

        chaseSkill = 1f;
        escapeSkill = 1f;

        lastPosition = transform.position;

        if (goCoroutine == null)
        {
            goCoroutine = StartCoroutine(ThinkLoop());
        }
    }


    void Update()
    {
        GridNodeData myNode = GetClosestNode(transform.position);

        if (myNode != null)
        {
            bool nearBlockedBoundary = false;

            int checkRadius = 2;

            for (int x = -checkRadius; x <= checkRadius; x++)
            {
                for (int y = -checkRadius; y <= checkRadius; y++)
                {
                    int checkX = myNode.gridPos.x + x;
                    int checkY = myNode.gridPos.y + y;

                    if (checkX < 0 || checkX >= gridGenerator.width ||
                        checkY < 0 || checkY >= gridGenerator.height)
                        continue;

                    GridNodeData node =
                        gridGenerator.nodes[checkX, checkY];

                    if (node != null && node.isBlocked)
                    {
                        nearBlockedBoundary = true;
                        break;
                    }
                }

                if (nearBlockedBoundary)
                    break;
            }


            // Only check the non-player prey
            if (roundScript.player != this.gameObject &&
                !isChasing &&
                nearBlockedBoundary)
            {
                float movement =
                    Vector3.Distance(
                        transform.position,
                        lastPosition
                    );

                if (movement < 0.05f)
                {
                    stuckTimer += Time.deltaTime;
                }
                else
                {
                    stuckTimer = 0f;
                }

                if (stuckTimer >= stuckTime)
                {
                    edgeEscapeActive = true;
                    edgeEscapeTimer = edgeEscapeDuration;

                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }

            if (edgeEscapeActive)
            {
                edgeEscapeTimer -= Time.deltaTime;

                if (edgeEscapeTimer <= 0f)
                {
                    edgeEscapeActive = false;
                }
            }
        }

        lastPosition = transform.position;
    }

    // =========================================================
    // THINK LOOP
    // =========================================================


    public IEnumerator ThinkLoop()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (CompareTag("Player")) yield break;

        while (true)
        {
            GridNodeData bestNode = GetBestNode();

            if (bestNode != null &&
                bestNode != currentGoal)
            {
                currentGoal = bestNode;

                SetTargetNode(bestNode, false);
            }


            yield return new WaitForSeconds(thinkInterval);
        }
    }
    public void GoToInitialPosition(Vector3 homePos)
    {
        if (roundScript == null || roundScript.player == null)
        {
            return;
        }
        
        GridNodeData homeNode = null;

        if (CompareTag("Player"))
            return;

        if (goCoroutine != null)
        {
            StopCoroutine(goCoroutine);
            goCoroutine = null;
        }
        if(roundScript.player.GetComponent<CharacterMovement>().characterOnThresh)
        {
            homeNode =
            GetClosestNode(homePos + new Vector3(0, -60, 0));
            roundScript.player.GetComponent<CharacterMovement>().currentArea = AreaType.train;
        }
        else if (roundScript.player.GetComponent<CharacterMovement>().currentArea == AreaType.platform
                && GetComponent<CharacterMovement>().currentArea == AreaType.platform)
        {
            homeNode =
            GetClosestNode(homePos);
        }
        else if (roundScript.player.GetComponent<CharacterMovement>().currentArea == AreaType.platform
                && GetComponent<CharacterMovement>().currentArea == AreaType.train)
        {
            homeNode =
            GetClosestNode(homePos);
        }
        else if (roundScript.player.GetComponent<CharacterMovement>().currentArea == AreaType.train
                && GetComponent<CharacterMovement>().currentArea == AreaType.platform)
        {
            homeNode =
            GetClosestNode(homePos);
        }
        else if (roundScript.player.GetComponent<CharacterMovement>().currentArea == AreaType.train
                && GetComponent<CharacterMovement>().currentArea == AreaType.train)
        {
            homeNode =
            GetClosestNode(homePos + new Vector3(0, -60, 0));
        }
        else
        {
            homeNode =
            GetClosestNode(homePos + new Vector3(0, -60, 0));
        }


        if (homeNode != null)
        {
            currentGoal = homeNode;
            SetTargetNode(homeNode, true);
        }
    }



    GridNodeData GetBestNode()
    {
        GridNodeData myNode = GetClosestNode(transform.position);

        if (myNode == null)
            return null;

        GridNodeData best = null;
        float bestScore = float.MinValue;

        for (int x = 0; x < gridGenerator.width; x++)
        {
            for (int y = 0; y < gridGenerator.height; y++)
            {
                GridNodeData node = gridGenerator.nodes[x, y];

                if (node == null || node.isBlocked)
                    continue;

                // Only consider nearby nodes
                float distFromSelf = Vector2Int.Distance(
                    myNode.gridPos,
                    node.gridPos
                );

                if (distFromSelf > decisionRadius)
                    continue;

                float distToPrey = Vector3.Distance(
                    node.worldPos,
                    prey.transform.position
                );

                float distToPredator = Vector3.Distance(
                    node.worldPos,
                    predator.transform.position
                );

                float score;

                if (isChasing)
                {
                    // Chasing:
                    // closer to prey = better
                    score = -distToPrey;
                }
                else
                {
                    // Escaping:
                    // further from predator = better
                    score = distToPredator;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    best = node;
                }
            }
        }

        return best;
    }

    // GridNodeData GetBestNode()
    // {
    //     GridNodeData best = null;
    //     float bestScore = float.MinValue;

    //     GridNodeData myNode = GetClosestNode(transform.position); // gets the closest node

    //     if (myNode == null)
    //         return null;


    //     // --------------------------------------------------
    //     // PLAYER SPEED MODIFICATION
    //     // --------------------------------------------------

    //     currentPredatorDistance =
    //         Vector3.Distance(
    //             transform.position,
    //             predator.position
    //         );

    //     currentPreyDistance =
    //         Vector3.Distance(
    //             transform.position,
    //             prey.position
    //         );

    //     if (roundScript.player != this.gameObject)
    //     {
    //         bool predatorNarrowing =
    //             currentPredatorDistance < previousPredatorDistance;

    //         bool preyNarrowing =
    //             currentPreyDistance < previousPreyDistance;

    //         if (predatorNarrowing &&
    //             predator.gameObject == roundScript.player)
    //         {
    //             // Chosen player is the predator
    //             cm.movementSpeed =
    //                 cm.initialmovementSpeed + Random.Range(25, 36);
    //         }
    //         else if (preyNarrowing &&
    //                 prey.gameObject == roundScript.player)
    //         {
    //             // Chosen player is the prey
    //             cm.movementSpeed =
    //                 cm.initialmovementSpeed - Random.Range(25, 36);
    //         }
    //         else
    //         {
    //             cm.movementSpeed =
    //                 cm.initialmovementSpeed;
    //         }
    //     }

    //     previousPredatorDistance =
    //         currentPredatorDistance;

    //     previousPreyDistance =
    //         currentPreyDistance;


    //     // --------------------------------------------------
    //     // MAP CENTRE AND DISTANCES
    //     // --------------------------------------------------

    //     Vector3 mapCentre =
    //         gridGenerator.nodes[
    //             gridGenerator.width / 2,
    //             gridGenerator.height / 2
    //         ].worldPos;


    //     float stopDistance = 250f;

    //     if (!isChasing && predator.gameObject == roundScript.player)
    //     {
    //         if (currentPredatorDistance >= stopDistance)
    //         {
    //             // Already safely away from the player.
    //             return myNode;
    //         }
    //     }    


    //     // --------------------------------------------------
    //     // FIND BEST NODE
    //     // --------------------------------------------------

    //     for (int x = 0; x < gridGenerator.width; x++)
    //     {
    //         for (int y = 0; y < gridGenerator.height; y++)
    //         {
    //             GridNodeData node =
    //                 gridGenerator.nodes[x, y];

    //             if (node == null || node.isBlocked)
    //                 continue;


    //             // Don't consider nodes too far away
    //             float distFromSelf =
    //                 Vector2Int.Distance(
    //                     myNode.gridPos,
    //                     node.gridPos
    //                 );

    //             if (distFromSelf > decisionRadius)
    //                 continue;


    //             // Distance from this potential node
    //             // to prey and predator
    //             float distToPrey =
    //                 Vector3.Distance(
    //                     node.worldPos,
    //                     prey.position
    //                 );

    //             float distToPred =
    //                 Vector3.Distance(
    //                     node.worldPos,
    //                     predator.position
    //                 );


    //             // --------------------------------------------------
    //             // SCORE NODE
    //             // --------------------------------------------------

    //             float score;

    //             if (edgeEscapeActive)
    //             {
    //                 // NPC is stuck at the edge.
    //                 // Temporarily prioritise getting back
    //                 // toward the centre of the map.

    //                 float distToCentre =
    //                     Vector3.Distance(
    //                         node.worldPos,
    //                         mapCentre
    //                     );

    //                 score =
    //                     distToPred * escapeSkill
    //                     - distToCentre * 2f;
    //             }
    //             else if (isChasing)
    //             {
    //                 // Chase prey.
    //                 // Closer = better.

    //                 score =
    //                     -distToPrey * chaseSkill;
    //             }
    //             else
    //             {
    //                 // Evade predator.
    //                 // Further away = better.

    //                 float edgeDistanceX = Mathf.Min(
    //                     node.gridPos.x,
    //                     gridGenerator.width - 1 - node.gridPos.x
    //                 );

    //                 float edgeDistanceY = Mathf.Min(
    //                     node.gridPos.y,
    //                     gridGenerator.height - 1 - node.gridPos.y
    //                 );

    //                 float distanceFromEdge = Mathf.Min(
    //                     edgeDistanceX,
    //                     edgeDistanceY
    //                 );

    //                 // Discourage the NPC from getting too close
    //                 // to the edge of the map.

    //                 float safeEdgeDistance = 12f;
    //                 float edgePenalty = 0f;

    //                 if (distanceFromEdge < safeEdgeDistance)
    //                 {
    //                     edgePenalty =
    //                         Mathf.Pow(
    //                             safeEdgeDistance - distanceFromEdge,
    //                             2f
    //                         ) * 5f;
    //                 }

    //                 score =
    //                     distToPred * escapeSkill
    //                     - edgePenalty;
    //             }


    //             // --------------------------------------------------
    //             // BEST NODE
    //             // --------------------------------------------------

    //             if (score > bestScore)
    //             {
    //                 bestScore = score;
    //                 best = node;
    //             }
    //         }
    //     }

    //     return best;
    // }

    // =========================================================
    // PATHFINDING
    // =========================================================

    public void SetTargetNode(GridNodeData target, bool initializing = false)
    {
        if (target == null)
            return;


        currentNode =
            GetClosestNode(transform.position);

        if (currentNode == null)
            return;


        bool[,] blockedGrid =
            new bool[gridGenerator.width, gridGenerator.height];

        for (int x = 0; x < gridGenerator.width; x++)
        {
            for (int y = 0; y < gridGenerator.height; y++)
            {
                blockedGrid[x, y] =
                    gridGenerator.nodes[x, y] == null ||
                    gridGenerator.nodes[x, y].isBlocked;
            }
        }

        List<Vector2Int> pathIndices =
            Pathfinding.FindPath(
                blockedGrid,
                gridGenerator.width,
                gridGenerator.height,
                currentNode.gridPos,
                target.gridPos
            );

        if (pathIndices == null ||
            pathIndices.Count == 0)
            return;

        pathIndices = ExtractCorners(pathIndices);
        pathIndices = SmoothPath(pathIndices);

        if (pathIndices.Count > 1)
            pathIndices.RemoveAt(0);

        pathQueue.Clear();

        foreach (Vector2Int pos in pathIndices)
        {
            if (pos.x < 0 ||
                pos.y < 0 ||
                pos.x >= gridGenerator.width ||
                pos.y >= gridGenerator.height)
            {
                continue;
            }

            GridNodeData node =
                gridGenerator.nodes[pos.x, pos.y];

            if (node != null)
            {
                pathQueue.Enqueue(node);
            }
        }

        if (pathFollowCoro != null)
        {
            StopCoroutine(pathFollowCoro);
        }

        pathFollowCoro =
            StartCoroutine(FollowPath(initializing));
    }

    // =========================================================
    // FOLLOW PATH
    // =========================================================

    IEnumerator FollowPath(bool initializing = false)
    {
        while (pathQueue.Count > 0)
        {
            GridNodeData node =
                pathQueue.Dequeue();

            Vector3 targetPos =
                node.worldPos;

            while (
                Vector3.Distance(
                    transform.position,
                    targetPos
                ) > 3f
            )
            {
                Vector3 dir = (targetPos - transform.position).normalized;

                // cm.movementSpeed = initializing ? 150 : cm.initialmovementSpeed;

                cm.change = dir;

                yield return null;
            }

            transform.position = targetPos;

            cm.change = Vector3.zero;
            
            // cm.movementSpeed = cm.initialmovementSpeed;

            currentNode = node;

            yield return null;
        }
    }

    // =========================================================
    // CLOSEST NODE
    // =========================================================

    GridNodeData GetClosestNode(Vector3 worldPos)
    {
        GridNodeData closest = null;

        float minDist = float.MaxValue;

        for (int x = 0; x < gridGenerator.width; x++)
        {
            for (int y = 0; y < gridGenerator.height; y++)
            {
                GridNodeData node =
                    gridGenerator.nodes[x, y];

                if (node == null)
                    continue;

                float dist =
                    Vector3.Distance(
                        worldPos,
                        node.worldPos
                    );

                if (dist < minDist)
                {
                    minDist = dist;
                    closest = node;
                }
            }
        }

        return closest;
    }

    // =========================================================
    // EXTRACT CORNERS
    // =========================================================

    List<Vector2Int> ExtractCorners(
        List<Vector2Int> path
    )
    {
        List<Vector2Int> result =
            new List<Vector2Int>();

        if (path.Count == 0)
            return result;

        result.Add(path[0]);

        Vector2Int prevDir = Vector2Int.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int dir =
                path[i] - path[i - 1];

            if (dir != prevDir)
            {
                result.Add(path[i - 1]);
            }

            prevDir = dir;
        }

        result.Add(path[path.Count - 1]);

        return result;
    }

    // =========================================================
    // SMOOTH PATH
    // =========================================================

    List<Vector2Int> SmoothPath(
        List<Vector2Int> path
    )
    {
        if (path.Count <= 2)
            return path;

        List<Vector2Int> result =
            new List<Vector2Int>();

        result.Add(path[0]);

        int currentIndex = 0;

        while (currentIndex < path.Count - 1)
        {
            int nextIndex = currentIndex + 1;

            for (
                int i = path.Count - 1;
                i > nextIndex;
                i--
            )
            {
                if (
                    HasLineOfSight(
                        path[currentIndex],
                        path[i]
                    )
                )
                {
                    nextIndex = i;
                    break;
                }
            }

            result.Add(path[nextIndex]);

            currentIndex = nextIndex;
        }

        return result;
    }

    // =========================================================
    // LINE OF SIGHT
    // =========================================================

    bool HasLineOfSight(
        Vector2Int a,
        Vector2Int b
    )
    {
        int dx = Mathf.Abs(b.x - a.x);
        int dy = Mathf.Abs(b.y - a.y);

        int sx = a.x < b.x ? 1 : -1;
        int sy = a.y < b.y ? 1 : -1;

        int err = dx - dy;

        int x = a.x;
        int y = a.y;

        while (true)
        {
            if (
                x < 0 ||
                y < 0 ||
                x >= gridGenerator.width ||
                y >= gridGenerator.height
            )
            {
                return false;
            }

            GridNodeData node =
                gridGenerator.nodes[x, y];

            if (node == null || node.isBlocked)
            {
                return false;
            }

            if (x == b.x && y == b.y)
            {
                break;
            }

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }

        return true;
    }


    public void ResetForNewRound()
    {
        currentGoal = null;

        pathQueue.Clear();

        if (pathFollowCoro != null)
        {
            StopCoroutine(pathFollowCoro);
            pathFollowCoro = null;
        }

        // if(goCoroutine != null)
        // {
        //     StopCoroutine(goCoroutine);
        //     goCoroutine = null;
        // }
        // goCoroutine = StartCoroutine(ThinkLoop());
        currentNode = GetClosestNode(transform.position);



        edgeEscapeActive = false;
    }

    public void StartThinking()
    {
        if (goCoroutine == null)
        {
            goCoroutine = StartCoroutine(ThinkLoop());
        }
    }

    

}