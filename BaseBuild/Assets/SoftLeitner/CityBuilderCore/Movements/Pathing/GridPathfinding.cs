using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// simple a* implementation for non diagonal pathfinding between equidistant points
    /// </summary>
    public class GridPathfinding : IRoadPathfinder, IRoadPathfinderBlocked, IMapGridPathfinder
    {
        private class Node
        {
            public GridPathfinding Grid { get; private set; }
            public Vector2Int Point { get; private set; }

            public int GCost;
            public int HCost;
            public Node Parent;

            public int FCost => GCost + HCost;

            public Node(GridPathfinding grid, Vector2Int point)
            {
                Grid = grid;
                Point = point;
            }

            public virtual IEnumerable<Node> GetNeighbours()
            {
                Node neighbour;

                neighbour = addNodeNeighbour(1, 0);
                if (neighbour != null)
                    yield return neighbour;
                neighbour = addNodeNeighbour(0, 1);
                if (neighbour != null)
                    yield return neighbour;
                neighbour = addNodeNeighbour(-1, 0);
                if (neighbour != null)
                    yield return neighbour;
                neighbour = addNodeNeighbour(0, -1);
                if (neighbour != null)
                    yield return neighbour;
            }

            protected virtual bool isValidNeighbour(Vector2Int point) => true;

            private Node addNodeNeighbour(int x, int y)
            {
                if (x == 0 && y == 0)
                    return null;

                if (!Grid._nodes.TryGetValue(new Vector2Int(Point.x + x, Point.y + y), out Node neighbour))
                    return null;

                if (!neighbour.isValidNeighbour(Point))
                    return null;

                return neighbour;
            }
        }

        private class SwitchNode : Node
        {
            public bool IsDirectional { get; private set; }
            public Vector2Int Entry { get; private set; }
            public Vector2Int Exit { get; private set; }
            public GridPathfinding ExitGrid { get; private set; }

            public SwitchNode(GridPathfinding entryGrid, Vector2Int point, GridPathfinding exitGrid) : base(entryGrid, point)
            {
                IsDirectional = false;
                ExitGrid = exitGrid;
            }
            public SwitchNode(GridPathfinding entryGrid, Vector2Int entry, Vector2Int point, Vector2Int exit, GridPathfinding exitGrid) : base(entryGrid, point)
            {
                IsDirectional = true;
                Entry = entry;
                Exit = exit;
                ExitGrid = exitGrid;
            }

            public override IEnumerable<Node> GetNeighbours()
            {
                if (IsDirectional)
                {
                    if (Grid._nodes.TryGetValue(Entry, out Node entryNeighbour))
                        yield return entryNeighbour;
                    if (ExitGrid._nodes.TryGetValue(Exit, out Node exitNeighbour))
                        yield return exitNeighbour;
                }
                else
                {
                    Node neighbour;

                    neighbour = addNodeNeighbour(1, 0, Grid);
                    if (neighbour != null)
                        yield return neighbour;
                    neighbour = addNodeNeighbour(0, 1, Grid);
                    if (neighbour != null)
                        yield return neighbour;
                    neighbour = addNodeNeighbour(-1, 0, Grid);
                    if (neighbour != null)
                        yield return neighbour;
                    neighbour = addNodeNeighbour(0, -1, Grid);
                    if (neighbour != null)
                        yield return neighbour;

                    neighbour = addNodeNeighbour(1, 0, ExitGrid);
                    if (neighbour != null)
                        yield return neighbour;
                    neighbour = addNodeNeighbour(0, 1, ExitGrid);
                    if (neighbour != null)
                        yield return neighbour;
                    neighbour = addNodeNeighbour(-1, 0, ExitGrid);
                    if (neighbour != null)
                        yield return neighbour;
                    neighbour = addNodeNeighbour(0, -1, ExitGrid);
                    if (neighbour != null)
                        yield return neighbour;
                }
            }

            protected override bool isValidNeighbour(Vector2Int point) => IsDirectional ? Entry == point : true;

            private Node addNodeNeighbour(int x, int y, GridPathfinding grid)
            {
                if (x == 0 && y == 0)
                    return null;

                if (grid._nodes.TryGetValue(new Vector2Int(Point.x + x, Point.y + y), out Node neighbour))
                    return neighbour;
                else
                    return null;
            }
        }

        private Dictionary<Vector2Int, Node> _nodes = new Dictionary<Vector2Int, Node>();

        public void Clear()
        {
            _nodes.Clear();
        }
        public void Add(IEnumerable<Vector2Int> points) => points.ForEach(p => Add(p));
        public void Add(Vector2Int point)
        {
            if (_nodes.ContainsKey(point))
                return;
            _nodes.Add(point, new Node(this, point));
        }
        public void Remove(IEnumerable<Vector2Int> points) => points.ForEach(p => Remove(p));
        public void Remove(Vector2Int point)
        {
            _nodes.Remove(point);
        }
        public void AddSwitch(Vector2Int point, GridPathfinding grid)
        {
            if (_nodes.ContainsKey(point))
                return;
            _nodes.Add(point, new SwitchNode(this, point, grid));
        }
        public void AddSwitch(Vector2Int entry, Vector2Int point, Vector2Int exit, GridPathfinding grid)
        {
            if (_nodes.ContainsKey(point))
                return;
            _nodes.Add(point, new SwitchNode(this, entry, point, exit, grid));
        }

        public IEnumerable<Vector2Int> GetPositions() => _nodes.Keys;

        public bool HasPoint(Vector2Int point, object tag = null) => _nodes.ContainsKey(point);

        public WalkingPath FindPath(Vector2Int start, Vector2Int target, object tag = null) => FindPath(new Vector2Int[] { start }, new Vector2Int[] { target });
        public WalkingPath FindPath(Vector2Int[] starts, Vector2Int[] targets)
        {
            var nodes = findNodePath(starts, targets);
            if (nodes == null)
                return null;
            return new WalkingPath(nodes.Select(n => n.Point).ToArray());
        }

        private List<Node> findNodePath(Vector2Int[] startPositions, Vector2Int[] targetPositions)
        {
            List<Node> startNodes = new List<Node>();
            List<Node> targetNodes = new List<Node>();

            foreach (var startPosition in startPositions)
            {
                if (_nodes.TryGetValue(startPosition, out Node startNode))
                    startNodes.Add(startNode);
            }

            foreach (var targetPosition in targetPositions)
            {
                if (_nodes.TryGetValue(targetPosition, out Node targetNode))
                    targetNodes.Add(targetNode);
            }

            return findNodePath(startNodes, targetNodes);
        }
        private List<Node> findNodePath(List<Node> startNodes, List<Node> targetNodes)
        {
            if (startNodes.Count == 0 || targetNodes.Count == 0)
                return null;

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.AddRange(startNodes);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (targetNodes.Contains(currentNode))
                {
                    return retraceNodePath(startNodes, currentNode);
                }

                foreach (Node neighbour in currentNode.GetNeighbours())
                {
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.GCost + getNodeDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = getNodeDistance(neighbour, targetNodes);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        private static List<Node> retraceNodePath(List<Node> startNodes, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (!startNodes.Contains(currentNode))
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Add(currentNode);
            path.Reverse();
            return path;
        }

        private static int getNodeDistance(Node nodeA, Node nodeB)
        {
            int dstX = System.Math.Abs(nodeA.Point.x - nodeB.Point.x);
            int dstY = System.Math.Abs(nodeA.Point.y - nodeB.Point.y);
            return (dstX > dstY) ?
                14 * dstY + 10 * (dstX - dstY) :
                14 * dstX + 10 * (dstY - dstX);
        }
        private static int getNodeDistance(Node node, List<Node> nodes)
        {
            return nodes.Min(n => getNodeDistance(node, n));
        }

    }
}