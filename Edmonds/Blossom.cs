using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public abstract class Blossom
{
    public abstract void UpdateStem(CompleteGraph M);

    public abstract HashSet<Edge> FindAlternatingPath(int v, CompleteGraph M);

    public void Add(double charge, CompleteGraph actualLoad)
    {
        Thickness += charge;

        foreach (Edge e in Boundary)
        {
            actualLoad[e] += charge;
        }
    }

    public bool ContainsVertex(int v) => Vertices.Contains(v);

    public HashSet<Edge> Boundary { get; } = new HashSet<Edge>();

    public HashSet<int> Vertices { get; } = new HashSet<int>();

    public int Stem { get; protected set; }

    public double Thickness { get; protected set; } = 0;
}

public class CompositeBlossom : Blossom
{
    public CompositeBlossom(List<Blossom> blossoms, List<Edge> edges, CompleteGraph graph, CompleteGraph partialMatching)
    {
        Debug.Assert(blossoms.Count == edges.Count);
        Debug.Assert(blossoms.Count % 2 == 1);

        // edges[0] between blossoms[0], blossoms[1]... 
        SubBlossoms = blossoms;
        Edges = edges;

        foreach (Blossom b in blossoms)
        {
            Boundary.SymmetricExceptWith(b.Boundary);
            Vertices.UnionWith(b.Vertices);
        }

        Stem = FindStem(partialMatching);
    }

    private int FindStem(CompleteGraph partialMatching)
    {
        for (int i = 0; i < Edges.Count; i++)
        {
            Edge e = Edges[i];
            Edge f = Edges[i == 0 ? Edges.Count - 1 : i - 1];

            if ((partialMatching[e] == 0) && (partialMatching[f] == 0))
            {
                return SubBlossoms[i].Stem;
            }
        }

        throw new InvalidOperationException("Stem cannot be located.");
    }

    public override HashSet<Edge> FindAlternatingPath(int v, CompleteGraph partialMatching)
    {
        Debug.Assert(ContainsVertex(v));

        HashSet<Edge> result = new HashSet<Edge>();
        if (Stem == v)
            return result;

        int blossomId = LocateVertex(v);

        // alt path from first to stem
        // first edge must be from matching M
        int offset, direction;
        if (partialMatching[Edges[blossomId]] == 1)
        {
            offset = 0;
            direction = 1;
        }
        else
        {
            offset = -1;
            direction = -1;
        }

        result.UnionWith(SubBlossoms[blossomId].FindAlternatingPath(v, partialMatching));

        while (!SubBlossoms[blossomId].ContainsVertex(Stem))
        {
            int i = (blossomId + offset) + ((blossomId + offset) < 0 ? SubBlossoms.Count : 0);
            int next = ((blossomId + direction) + ((blossomId + direction) < 0 ? SubBlossoms.Count : 0)) % SubBlossoms.Count;
            Edge e = Edges[i];
            if (SubBlossoms[blossomId].ContainsVertex(e.U))
            {
                result.UnionWith(SubBlossoms[blossomId].FindAlternatingPath(e.U, partialMatching));
                result.UnionWith(SubBlossoms[next].FindAlternatingPath(e.V, partialMatching));
            }
            else
            {
                result.UnionWith(SubBlossoms[blossomId].FindAlternatingPath(e.V, partialMatching));
                result.UnionWith(SubBlossoms[next].FindAlternatingPath(e.U, partialMatching));
            }

            result.Add(e);
            blossomId = next;
        }

        return result;
    }

    private int LocateVertex(int v)
    {
        for (int i = 0; i < SubBlossoms.Count; i++)
        {
            if (SubBlossoms[i].ContainsVertex(v))
            {
                return i;
            }
        }

        throw new ArgumentException("Vertex is not in any subblossom.");
    }

    public override void UpdateStem(CompleteGraph partialMatching)
    {
        foreach (Blossom b in SubBlossoms)
        {
            b.UpdateStem(partialMatching);
        }

        Stem = FindStem(partialMatching);
    }

    public List<Blossom> SubBlossoms { get; }

    public List<Edge> Edges { get; }
}

public class VertexBlossom : Blossom
{
    public VertexBlossom(int v, CompleteGraph G)
    {
        Stem = v;
        Vertices.Add(v);

        for (int i = 0; i < G.Order; i++)
        {
            if (i != v)
            {
                Boundary.Add(new Edge(i, v));
            }
        }
    }

    public override string ToString() => $"T: {Thickness} V: {Stem}";

    public override HashSet<Edge> FindAlternatingPath(int v, CompleteGraph M)
    {
        return new HashSet<Edge>();
    }

    public override void UpdateStem(CompleteGraph partialMatching) { }
}