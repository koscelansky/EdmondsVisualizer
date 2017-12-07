using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Node
{
    private Node _parent;
    internal Tree _tree;
    private Dictionary<Edge, Node> _children = new Dictionary<Edge, Node>();

    public Node(Blossom blossom)
    {
        Blossom = blossom;
    }

    public override string ToString()
    {
        return $"T: {Thickness:F2} V: " + String.Join(" ", Vertices);
    }

    public Node FindNode(int v)
    {
        if (ContainsVertex(v))
            return this;

        foreach (Node n in Children.Values)
        {
            var res = n.FindNode(v);

            if (res != null)
                return res;
        }

        return null;
    }

    public void AddBarbell(Edge e, Barbell barbell)
    {
        int vertexInThis = ContainsVertex(e.U) ? e.U : e.V;
        int vertexInBarbell = e.OtherEnd(vertexInThis);

        Debug.Assert(!ContainsVertex(vertexInBarbell));

        Node fst = barbell.First;
        Node snd = barbell.Second;
        if (snd.ContainsVertex(vertexInBarbell))
        {
            Node temp = fst;
            fst = snd;
            snd = temp;
        }

        // from now on vertexInBarbell is in fst

        fst._children.Clear();
        AddChild(e, fst);

        snd._children.Clear();
        fst.AddChild(barbell.Edge, snd);
    }

    public void AddCharge(double charge, CompleteGraph actualLoad)
    {
        Blossom.Add(Level % 2 == 0 ? charge : -charge, actualLoad);

        foreach (Node n in Children.Values)
        {
            n.AddCharge(charge, actualLoad);
        }
    }

    private void UpdateLevel()
    {
        Level = _parent?.Level + 1 ?? 0;

        foreach (Node n in Children.Values)
        {
            n.UpdateLevel();
        }
    }

    public List<Edge> FindPath(int v, CompleteGraph partialMatching)
    {
        if (ContainsVertex(v))
            return new List<Edge>(Blossom.FindAlternatingPath(v, partialMatching));

        foreach (var i in Children)
        {
            List<Edge> path = i.Value.FindPath(v, partialMatching);
            if (path != null)
            {
                Edge edge = i.Key;

                int vertexInThis = ContainsVertex(edge.U) ? edge.U : edge.V;
                int vertexInNext = edge.OtherEnd(vertexInThis);

                // path inside blossoms
                path.AddRange(Blossom.FindAlternatingPath(vertexInThis, partialMatching));
                path.Add(edge);
                path.AddRange(i.Value.Blossom.FindAlternatingPath(vertexInNext, partialMatching));
                    
                return path;
            }
        }

        return null;
    }

    public List<AbstractTree> Dissolve(CompleteGraph partialMatching, CompleteGraph actualLoad)
    {
        Node lastParent = _parent;
        Edge lastEdge = lastParent.Children.First(x => x.Value == this).Key;

        // vertex in this
        int v = ContainsVertex(lastEdge.V) ? lastEdge.V : lastEdge.U;
        // alternating path in blossom
        HashSet<Edge> path = Blossom.FindAlternatingPath(v, partialMatching);
        CompositeBlossom compositeBlossom = (CompositeBlossom)Blossom;
        int index = compositeBlossom.SubBlossoms.FindIndex(x => x.ContainsVertex(v));

        int dir = path.Contains(compositeBlossom.Edges[index]) ? 1 : -1;

        bool finish = compositeBlossom.SubBlossoms[index].ContainsVertex(compositeBlossom.Stem);
        // create new nodes in tree
        while (!finish)
        {
            Node n = new Node(compositeBlossom.SubBlossoms[index]);
            lastParent.AddChild(lastEdge, n);

            if (dir == 1)
            {
                lastEdge = compositeBlossom.Edges[index];
            }
            else
            {
                lastEdge = compositeBlossom.Edges[(index - 1) + ((index - 1) < 0 ? compositeBlossom.SubBlossoms.Count : 0)];
            }
            index = ((index + dir) + ((index + dir) < 0 ? compositeBlossom.SubBlossoms.Count : 0)) % compositeBlossom.SubBlossoms.Count;
            lastParent = n;
            finish = compositeBlossom.SubBlossoms[index].ContainsVertex(compositeBlossom.Stem);
        }
        Node stipeNode = new Node(compositeBlossom.SubBlossoms[index]);
        lastParent.AddChild(lastEdge, stipeNode);

        foreach (var i in Children)
        {
            stipeNode.AddChild(i.Key, i.Value);
        }

        List<AbstractTree> result = new List<AbstractTree>();
        for (int k = 0; k < compositeBlossom.Edges.Count; k++)
        {
            Edge edge = compositeBlossom.Edges[k];
            // edges in matching and not on alternating path are new barbells
            if ((partialMatching[edge] == 1) && (!path.Contains(edge)))
            {
                result.Add(new Barbell(new Node(compositeBlossom.SubBlossoms[k]), new Node(compositeBlossom.SubBlossoms[(k + 1) % compositeBlossom.SubBlossoms.Count]), edge));
            }

            // edge not in matching and not on alternating path are no longer full (in L)
            if (partialMatching[edge] == 0)
            {
                if (!path.Contains(edge))
                {
                    actualLoad[edge] = 0;
                }
            }
        }
        return result;
    }

    public void AddChild(Edge edge, Node node)
    {
        _children[edge] = node;
        node.Parent = this;
    }

    public void RemoveChild(Edge edge)
    {
        _children.Remove(edge);
    }

    public bool ContainsVertex(int v) => Blossom.ContainsVertex(v);

    public HashSet<int> Vertices => Blossom.Vertices;

    public bool ContainsVertexInSubtree(int v)
    {
        if (ContainsVertex(v))
            return true;

        foreach (Node i in Children.Values)
        {
            if (i.ContainsVertexInSubtree(v))
                return true;
        }

        return false;
    }

    public static Node LCA(Node a, Node b)
    {
        if (a.Level > b.Level)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        Debug.Assert(a.Level <= b.Level);

        while (a.Level < b.Level)
        {
            b = b.Parent;
        }

        while (a != b)
        {
            a = a.Parent;
            b = b.Parent;
        }

        return a;
    }

    public Blossom Blossom { get; private set; }

    public double Thickness => Blossom.Thickness;

    public int Level { get; private set; }

    public IReadOnlyDictionary<Edge, Node> Children { get => _children; }

    public Node Parent
    {
        get { return _parent; }
        set
        {
            _parent = value;
            _tree = _parent?._tree;
            UpdateLevel();
        }
    }
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
            if (root.Blossom is VertexBlossom)
            {
                result = new Tuple<Node, double, Edge>(root, Double.MaxValue, null);
            }
            else
            {
                result = new Tuple<Node, double, Edge>(root, root.Thickness, null);
            }
        }
        else
        {
            // we will be adding charge
            foreach (Edge e in root.Blossom.Boundary)
            {
                int otherEnd = root.Vertices.Contains(e.V) ? e.U : e.V;
                AbstractTree otherTree = forrest.Find(tree => tree.ContainsVertex(otherEnd));
                
                if (fullEdges[e] == 0)
                {
                    double edgeCap = graph[e] - actualLoad[e];
                    if (otherTree is Tree tree)
                    {
                        if (tree.FindNode(otherEnd).Level % 2 == 1)
                            continue;

                        edgeCap /= 2;
                    }

                    if (result == null || result.Item2 > edgeCap)
                    {
                        result = new Tuple<Node, double, Edge>(root, edgeCap, e);
                    }
                }
            }
        }

        foreach (Node n in root.Children.Values)
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
        root.Blossom.UpdateStem(partialMatching);
        List<AbstractTree> result = new List<AbstractTree>();
        foreach (Node n in root.Children.Values)
        {
            result.AddRange(ExtractBarbells(n, partialMatching, fullEdges));
        }
        foreach (var t in root.Children)
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
        _root._tree = this;
    }

    public List<AbstractTree> Merge(Tree other, Edge e, CompleteGraph partialMatching, CompleteGraph fullEdges)
    {
        int vertexInThis = ContainsVertex(e.U) ? e.U : e.V;
        int vertexInOther = e.OtherEnd(vertexInThis);

        Debug.Assert(other.ContainsVertex(vertexInOther));
        
        HashSet<Edge> alternatingPath = new HashSet<Edge>();
        // alternating path
        alternatingPath.UnionWith(_root.FindPath(vertexInThis, partialMatching));
        alternatingPath.UnionWith(other._root.FindPath(vertexInOther, partialMatching));
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

    public void NewBlossom(Node a, Node b, Edge e, CompleteGraph G, CompleteGraph M)
    {
        // e is edge between a and b
        int vertexInA = a.Vertices.Contains(e.U) ? e.U : e.V;
        int vertexInB = e.OtherEnd(vertexInA);

        Debug.Assert(!a.Vertices.Contains(vertexInB) && !b.Vertices.Contains(vertexInA));

        List<Blossom> blossoms = new List<Blossom>();
        List<Edge> edges = new List<Edge>();

        Node lca = Node.LCA(a, b);
        Debug.Assert(lca.ContainsVertexInSubtree(vertexInA));
        Debug.Assert(lca.ContainsVertexInSubtree(vertexInB));

        // path from lca to a
        Node cur = lca;
        Dictionary<Edge, Node> newChildren = new Dictionary<Edge, Node>();
        while (cur != a)
        {
            blossoms.Add(cur.Blossom);
            foreach (var i in cur.Children)
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
        foreach (var i in a.Children)
        {
            newChildren.Add(i.Key, i.Value);
        }

        // edge from a to b
        blossoms.Add(cur.Blossom);
        edges.Add(e);

        // children from b to newChildren
        foreach (var i in b.Children)
        {
            newChildren.Add(i.Key, i.Value);
        }

        // path from b to lca
        cur = b;
        while (cur != lca)
        {
            blossoms.Add(cur.Blossom);
            Node p = cur.Parent;
            foreach (var i in p.Children)
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
        foreach (var i in lca.Children)
        {
            if (i.Value.ContainsVertexInSubtree(vertexInA) || i.Value.ContainsVertexInSubtree(vertexInB))
            {
                newChildren.Remove(i.Key);
            }
        }
        Node newNode = new Node(new CompositeBlossom(blossoms, edges, G, M));

        if (lca == _root)
        {
            _root = newNode;
            newNode._tree = this;
        }
        else
        {
            Edge x = null;
            foreach (var i in lca.Parent.Children)
            {
                if (i.Value == lca)
                {
                    x = i.Key;
                    break;
                }
            }
            lca.Parent.RemoveChild(x);
            lca.Parent.AddChild(x, newNode);
        }

        // update parents
        foreach (var i in newChildren)
        {
            newNode.AddChild(i.Key, i.Value);
        }
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
