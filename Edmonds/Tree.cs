using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;

public class Node
{
    internal Blossom _blossom;
    internal Node _parent;
    internal Tree _tree;
    internal Dictionary<Edge, Node> _children;

    public Node(Blossom blossom, Node parent)
    {
        _blossom = blossom;
        _parent = parent;
        _tree = _parent?._tree;
        Level = _parent?.Level + 1 ?? 0;
        _children = new Dictionary<Edge, Node>();
    }

    public override string ToString()
    {
        return $"T: {Thickness:F2} V: " + String.Join(" ", Vertices);
    }

    public Node FindNode(int v)
    {
        if (ContainsVertex(v))
            return this;

        foreach (Node n in _children.Values)
        {
            var res = n.FindNode(v);

            if (res != null)
                return res;
        }

        return null;
    }

    public void AddBarbell(Edge e, Barbell barbell)
    {
        int vertexInThis = Vertices.Contains(e.U) ? e.U : e.V;
        int vertexInBarbell = e.OtherEnd(vertexInThis);

        Debug.Assert(!Vertices.Contains(vertexInBarbell));

        Node fst = barbell.First;
        Node snd = barbell.Second;
        if (snd.Vertices.Contains(vertexInBarbell))
        {
            Node temp = fst;
            fst = snd;
            snd = temp;
        }
        
        // from now on vertexInBarbell is in fst

        _children.Add(e, fst);

        fst._parent = this;
        fst.Level = Level + 1;
        fst._tree = _tree;
        fst._children.Clear();

        snd.Level = fst.Level + 1;
        snd._parent = fst;
        snd._tree = _tree;
        snd._children.Clear();

        fst._children.Add(barbell.Edge, snd);
    }

    public void AddCharge(double charge, CompleteGraph actualLoad)
    {
        if (Level % 2 == 0)
        {
            _blossom.Add(charge, actualLoad);
        }
        else
        {
            _blossom.Add(-charge, actualLoad);
        }

        foreach (Node n in _children.Values)
        {
            n.AddCharge(charge, actualLoad);
        }
    }

    public void UpdateLevel()
    {
        Level = _parent == null ? 0 : _parent.Level + 1;

        foreach (Node n in _children.Values)
        {
            n.UpdateLevel();
        }
    }

    public List<AbstractTree> Dissolve(CompleteGraph partialMatching, CompleteGraph actualLoad)
    {
        Node lastParent = _parent;
        Edge lastEdge = lastParent._children.First(x => x.Value == this).Key;

        // vertex in this
        int v = ContainsVertex(lastEdge.V) ? lastEdge.V : lastEdge.U;
        // alternating path in blossom
        HashSet<Edge> path = _blossom.FindAlternatingPath(v, partialMatching);
        CompositeBlossom compositeBlossom = (CompositeBlossom)_blossom;
        int i = compositeBlossom.SubBlossoms.FindIndex(x => x.ContainsVertex(v));

        int dir = path.Contains(compositeBlossom.Edges[i]) ? 1 : -1;

        bool finish = compositeBlossom.SubBlossoms[i].ContainsVertex(compositeBlossom.Stem);
        // create new nodes in tree
        while (!finish)
        {
            Node n = new Node(compositeBlossom.SubBlossoms[i], lastParent);
            lastParent._children[lastEdge] = n;

            if (dir == 1)
            {
                lastEdge = compositeBlossom.Edges[i];
            }
            else
            {
                lastEdge = compositeBlossom.Edges[(i - 1) + ((i - 1) < 0 ? compositeBlossom.SubBlossoms.Count : 0)];
            }
            i = ((i + dir) + ((i + dir) < 0 ? compositeBlossom.SubBlossoms.Count : 0)) % compositeBlossom.SubBlossoms.Count;
            lastParent = n;
            finish = compositeBlossom.SubBlossoms[i].ContainsVertex(compositeBlossom.Stem);
        }
        Node stipe_node = new Node(compositeBlossom.SubBlossoms[i], lastParent);
        lastParent._children[lastEdge] = stipe_node;
        stipe_node._children = _children;
        foreach (Node n in stipe_node._children.Values)
        {
            n._parent = stipe_node;
        }
        // updating level of each node
        stipe_node.UpdateLevel();
        List<AbstractTree> result = new List<AbstractTree>();
        for (int k = 0; k < compositeBlossom.Edges.Count; k++)
        {
            Edge edge = compositeBlossom.Edges[k];
            // edges in matching and not on alternating path are new barbells
            if ((partialMatching[edge] == 1) && (!path.Contains(edge)))
            {
                result.Add(new Barbell(new Node(compositeBlossom.SubBlossoms[k], null), new Node(compositeBlossom.SubBlossoms[(k + 1) % compositeBlossom.SubBlossoms.Count], null), edge));
            }
            // edge not in matching and not on alternating path are no longer full (in L)
            if (partialMatching[edge] == 0)
            {
                if (!path.Contains(compositeBlossom.Edges[k]))
                {
                    actualLoad[edge] = 0;
                }
            }
        }
        return result;
    }

    public bool ContainsVertex(int v) => _blossom.ContainsVertex(v);

    public HashSet<int> Vertices => _blossom.Vertices;

    public bool ContainsVertexInSubtree(int v)
    {
        if (ContainsVertex(v))
            return true;

        foreach (Node i in _children.Values)
        {
            if (i.ContainsVertexInSubtree(v))
                return true;
        }

        return false;
    }

    public double Thickness => _blossom.Thickness;

    public int Level { get; internal set; }
}

public abstract class AbstractTree
{
    public abstract bool ContainsVertex(int v);
}

public class Tree : AbstractTree
{
    private Node _root;

    private Tuple<Node, double, Edge> CriticalValueRecursive(Node root, CompleteGraph graph, CompleteGraph actualLoad, CompleteGraph fullEdges, List<AbstractTree> forrest)
    {
        Tuple<Node, double, Edge> result = null;
        if (root.Level % 2 == 1)
        {
            // we will be subtracting charge
            if (root._blossom is VertexBlossom)
            {
                result = new Tuple<Node, double, Edge>(root, double.MaxValue, null);
            }
            else
            {
                result = new Tuple<Node, double, Edge>(root, root.Thickness, null);
            }
        }
        else
        {
            // we will be adding charge
            foreach (Edge e in root._blossom.Boundary)
            {
                int other_end = root.Vertices.Contains(e.V) ? e.U : e.V;
                AbstractTree other_tree = forrest.Find(tree => tree.ContainsVertex(other_end));
                
                if (fullEdges[e] == 0)
                {
                    double edge_cap = graph[e] - actualLoad[e];
                    if (other_tree is Tree tree)
                    {
                        if (tree.FindNode(other_end).Level % 2 == 1)
                            continue;

                        edge_cap /= 2;
                    }

                    if (result == null || result.Item2 > edge_cap)
                    {
                        result = new Tuple<Node, double, Edge>(root, edge_cap, e);
                    }
                }
            }
        }

        foreach (Node n in root._children.Values)
        {
            Tuple<Node, double, Edge> temp = CriticalValueRecursive(n, graph, actualLoad, fullEdges, forrest);
            if (result.Item2 > temp.Item2)
            {
                result = temp;
            }
        }

        return result;
    }

    private List<AbstractTree> ExtractBarbells(Node root, CompleteGraph partialMatching, CompleteGraph fullEdges)
    {
        // if the tree is merged with other tree, it will break into barbells
        root._blossom.UpdateStem(partialMatching);
        List<AbstractTree> result = new List<AbstractTree>();
        foreach (Node n in root._children.Values)
        {
            result.AddRange(ExtractBarbells(n, partialMatching, fullEdges));
        }
        foreach (var t in root._children)
        {
            if (partialMatching[t.Key] == 1)
            {
                result.Add(new Barbell(root, t.Value, t.Key));
            }
            else
            {
                fullEdges[t.Key] = 0;
            }
        }
        return result;
    }

    public Tree(Node root)
    {
        _root = root;
        _root.Level = 0;
        _root._parent = null;
        _root._tree = this;
    }

    public HashSet<Edge> FindPath(int w, CompleteGraph partialMatching)
    {
        HashSet<Edge> result = new HashSet<Edge>();
        Node node = _root;
        Node nextNode = null;
        Edge edge = null;

        while (!node.Vertices.Contains(w))
        {
            foreach (var t in node._children)
            {
                if (t.Value.ContainsVertexInSubtree(w))
                {
                    nextNode = t.Value;
                    edge = t.Key;
                    break;
                }
            }
            int vertexInThis = node.ContainsVertex(edge.U) ? edge.U : edge.V;
            int vertexInNext = edge.OtherEnd(vertexInThis);
            
            // path inside blossoms
            result.UnionWith(node._blossom.FindAlternatingPath(vertexInThis, partialMatching));
            result.Add(edge);
            result.UnionWith(nextNode._blossom.FindAlternatingPath(vertexInNext, partialMatching));
            node = nextNode;    
        }
        result.UnionWith(node._blossom.FindAlternatingPath(w, partialMatching));

        return result;
    }

    public List<AbstractTree> Merge(Tree other, Edge e, CompleteGraph partialMatching, CompleteGraph fullEdges)
    {
        int vertexInThis = ContainsVertex(e.U) ? e.U : e.V;
        int vertexInOther = e.OtherEnd(vertexInThis);

        Debug.Assert(other.ContainsVertex(vertexInOther));
        
        HashSet<Edge> alternatingPath = new HashSet<Edge>();
        // alternating path
        alternatingPath.UnionWith(FindPath(vertexInThis, partialMatching));
        alternatingPath.UnionWith(other.FindPath(vertexInOther, partialMatching));
        alternatingPath.Add(e);
        // flip the alternating path
        foreach (Edge edge in alternatingPath)
        {
            partialMatching[edge] = 1 - partialMatching[edge];
        }
        // both vertices in both trees are in matching, so they can form barbells
        List<AbstractTree> result = ExtractBarbells(_root, partialMatching, fullEdges);
        result.AddRange(other.ExtractBarbells(other._root, partialMatching, fullEdges));
        Node n1 = FindNode(vertexInThis);
        Node n2 = other.FindNode(vertexInOther);
        // edge between trees
        result.Add(new Barbell(n1, n2, e));
        return result;
    }

    public Node Root
    {
        get { return _root; }
    }

    public Node LCA(Node a, Node b)
    {
        Func<Node, List<Node>> findPathFromRoot = x =>
        {
            var path = new List<Node>(a.Level);

            while (x != _root)
            {
                path.Add(x);
                x = x._parent;
            }

            path.Reverse();
            return path;
        };

        var pathFromRootToA = findPathFromRoot(a);
        var pathFromRootToB = findPathFromRoot(b);

        Node result = _root;
        int commonLength = Math.Min(pathFromRootToA.Count, pathFromRootToB.Count);
        for (int i = 0; i < commonLength; ++i)
        {
            if (pathFromRootToA[i] != pathFromRootToB[i])
                return result;

            result = pathFromRootToA[i];
        }

        return result;
    }

    public void NewBlossom(Node a, Node b, Edge e, CompleteGraph G, CompleteGraph M)
    {
        // e is edge between a and b
        int vertexInA = a.Vertices.Contains(e.U) ? e.U : e.V;
        int vertexInB = e.OtherEnd(vertexInA);

        Debug.Assert(!a.Vertices.Contains(vertexInB) && !b.Vertices.Contains(vertexInA));

        List<Blossom> blossoms = new List<Blossom>();
        List<Edge> edges = new List<Edge>();

        Node lca = LCA(a, b);
        Debug.Assert(lca.ContainsVertexInSubtree(vertexInA));
        Debug.Assert(lca.ContainsVertexInSubtree(vertexInB));

        // path from lca to a
        Node cur = lca;
        Dictionary<Edge, Node> newChildren = new Dictionary<Edge, Node>();
        while (!(cur == a))
        {
            blossoms.Add(cur._blossom);
            foreach (var i in cur._children)
            {
                if (i.Value.ContainsVertexInSubtree(vertexInA))
                {
                    edges.Add(i.Key);
                    cur = i.Value;
                }
                else
                {
                    newChildren.Add(i.Key, i.Value);
                }
            }
        }
        // children from a to new children
        foreach (var i in a._children)
        {
            newChildren.Add(i.Key, i.Value);
        }

        // edge from a to b
        blossoms.Add(cur._blossom);
        edges.Add(e);
        // children from b to new_c
        foreach (var i in b._children)
        {
            newChildren.Add(i.Key, i.Value);
        }
        // path from b to lca
        cur = b;
        while (!(cur == lca))
        {
            blossoms.Add(cur._blossom);
            Node p = cur._parent;
            foreach (var i in p._children)
            {
                if (i.Value == cur)
                {
                    edges.Add(i.Key);
                }
                else
                {
                    if (!newChildren.ContainsKey(i.Key))
                    {
                        newChildren.Add(i.Key, i.Value);
                    }
                }
            }
            cur = p;
        }
        // remove children from lca to a/b
        foreach (var i in lca._children)
        {
            if (i.Value.ContainsVertexInSubtree(vertexInA) || i.Value.ContainsVertexInSubtree(vertexInB))
            {
                newChildren.Remove(i.Key);
            }
        }
        Node new_node = new Node(new CompositeBlossom(blossoms, edges, G, M), lca._parent);
        
        // update parents
        foreach (Node n in newChildren.Values)
        {
            n._parent = new_node;
        }
        new_node._children = newChildren;

        if (lca == _root)
        {
            _root = new_node;
            new_node._tree = this;
        }
        else
        {
            Edge x = null;
            foreach (var i in new_node._parent._children)
            {
                if (i.Value == lca)
                {
                    x = i.Key;
                    break;
                }
            }
            new_node._parent._children.Remove(x);
            new_node._parent._children.Add(x, new_node);
        }
        _root.UpdateLevel();
    }

    public void Add(double charge, CompleteGraph actualLoad)
    {
        _root.AddCharge(charge, actualLoad);
    }

    public Tuple<Node, double, Edge> FindCriticalValue(CompleteGraph graph, CompleteGraph actualLoad, CompleteGraph fullEdges, List<AbstractTree> forrest)
    {
        return CriticalValueRecursive(_root, graph, actualLoad, fullEdges, forrest);
    }

    public override bool ContainsVertex(int v) => _root.ContainsVertexInSubtree(v);

    public Node FindNode(int v)
    {
        return _root.FindNode(v);
    }
}

public class Barbell : AbstractTree
{
    public Barbell(Node fst, Node snd, Edge e)
    {
        First = fst;
        Second = snd;
        Edge = e;
    }

    public Node First { get; }

    public Node Second { get; }

    public Edge Edge { get; }

    public override string ToString()
    {
        return $"Barbell: ({ First }) ({ Edge }) ({ Second })";
    }

    public override bool ContainsVertex(int v)
    {
        return First.ContainsVertex(v) || Second.ContainsVertex(v);
    }
}
