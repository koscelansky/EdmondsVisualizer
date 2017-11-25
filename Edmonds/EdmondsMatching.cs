using MoreLinq;
using System;
using System.Collections.Generic;

public class EdmondsMatching : AbstractMatching
{
    public delegate void DrawEventHandler(CompleteGraph L, CompleteGraph M, List<AbstractTree> H);
    public DrawEventHandler Draw = null;

    private CompleteGraph _actualLoad = null;
    private CompleteGraph _fullEdges = null;
    private CompleteGraph _partialMatching = null;
    List<AbstractTree> _hungarianForest = null;

    public EdmondsMatching(CompleteGraph graph)
    {
        if (graph.Order % 2 != 0)
            throw new ArgumentException("Graph with odd number of vertices cannot have 1-factor.");

        Graph = graph;
        _actualLoad = new CompleteGraph(graph.Order);
        _fullEdges = new CompleteGraph(graph.Order);
        _partialMatching = new CompleteGraph(graph.Order);
        _hungarianForest = new List<AbstractTree>();
        for (int i = 0; i < graph.Order; i++)
        {
            _hungarianForest.Add(new Tree(new Node(new VertexBlossom(i, Graph))));
        }
    }

    public IEnumerable<Edge> Edges
    {
        get
        {
            HashSet<Edge> result = new HashSet<Edge>();
            for (int i = 0; i < Graph.Order; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    var e = new Edge(i, j);

                    if (_partialMatching[e] == 1)
                    {
                        result.Add(e);
                    }
                }
            }
            return result;
        }
    }

    private void OnDraw(CompleteGraph L, CompleteGraph M, List<AbstractTree> H)
    {
        Draw?.Invoke(L, M, H);
    }

    public bool NextIter()
    {
        OnDraw(_fullEdges, _partialMatching, _hungarianForest);

        if (_partialMatching.NonZeroEdges >= (Graph.Order / 2))
            return false;
        
        List<Edge> work_edges = new List<Edge>();
        List<Tuple<Node, double, Edge>> work = new List<Tuple<Node, double, Edge>>();

        // finding the critical value
        foreach (AbstractTree i in _hungarianForest)
        {
            if (i is Tree tree)
            {
                work.Add(tree.FindCriticalValue(Graph, _actualLoad, _fullEdges, _hungarianForest));
            }
        }

        Tuple<Node, double, Edge> min = work.MinBy(workItem => workItem.Item2);

        // thickness is updated
        foreach (AbstractTree i in _hungarianForest)
        {
            if (i is Tree tree)
            {
                tree.Add(min.Item2, _actualLoad);
            }
        }

        // we found critical value, node and edge associated with it
        Tree work_tree = min.Item1._tree;
        _hungarianForest.Remove(work_tree);
        double work_value = min.Item2;
        Node work_node = min.Item1;
        Edge work_edge = min.Item3;

        if ((work_node.Level % 2) == 0)
        {
            // new full edge
            _fullEdges[work_edge] = 1;
            int otherEnd = work_node.ContainsVertex(work_edge.U) ? work_edge.V : work_edge.U;
            // find tree with other end of work_edge
            AbstractTree other_tree = null;
            for (int i = 0; i < _hungarianForest.Count; i++)
            {
                if (_hungarianForest[i].ContainsVertex(otherEnd))
                {
                    other_tree = _hungarianForest[i];
                    _hungarianForest.RemoveAt(i);
                    break;
                }
            }
            // edge is between two trees
            if (other_tree != null)
            {
                // test of other_tree is barbell
                if (other_tree is Barbell barbell)
                {
                    // add barbell to node
                    work_node.AddBarbell(work_edge, barbell);
                    _hungarianForest.Add(work_tree);
                }
                else if (other_tree is Tree tree)
                {
                    // merging two trees together
                    List<AbstractTree> new_barbells = work_tree.Merge(tree, work_edge, _partialMatching, _fullEdges);
                    _hungarianForest.AddRange(new_barbells);
                }
                else
                {
                    throw new InvalidCastException("Unsupported type.");
                }
            }
            else
            {
                // edge in one tree, new blossom
                Node another_node = work_tree.FindNode(otherEnd);
                work_tree.NewBlossom(work_node, another_node, work_edge, Graph, _partialMatching);
                _hungarianForest.Add(work_tree);
            }
        }
        else
        {
            // dissolve blossom
            List<AbstractTree> new_barbells = work_node.Dissolve(_partialMatching, _fullEdges);
            _hungarianForest.AddRange(new_barbells);
            _hungarianForest.Add(work_tree);
                    
        }
        return true;
    }

    public override IEnumerable<Edge> Matching()
    {
        while (NextIter())
            ;

        return Edges;
    }

    public CompleteGraph Graph { get; }
}