﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

public class Edge : IComparable<Edge>
{
    public Edge(int u, int v)
    {
        if (u == v)
            throw new IndexOutOfRangeException("Loops are not supported");

        U = Math.Max(u, v);
        V = Math.Min(u, v);
    }

    public override bool Equals(Object obj)
    {
        return obj is Edge e ? this == e : false;
    }

    public override int GetHashCode()
    {
        return ((U << 16) | (V & 0xffff));
    }

    public override string ToString() => $"({U}, {V})";

    public int OtherEnd(int v)
    {
        Debug.Assert(v == V || v == U);

        return v == V ? U : V;
    }

    public int CompareTo(Edge other)
    {
        return U == other.U ? V - other.V : U - other.U;
    }

    public static bool operator ==(Edge e, Edge f)
    {
        return e.U == f.U && e.V == f.V;
    }

    public static bool operator !=(Edge e, Edge f)
    {
        return !(e == f);
    }
    
    public int U { get; }

    public int V { get; }
}

public class CompleteGraph
{
    double[][] _weights;

    public CompleteGraph(int n)
    {
        _weights = new double[n][];
        for (int i = 0; i < _weights.Length; i++)
        {
            _weights[i] = new double[i];
        }
    }

    public CompleteGraph(List<Point> points)
        : this(points.Count)
    {
        for (int i = 0; i < _weights.Length; i++)
        {
            for (int j = 0; j < _weights[i].Length; j++)
            {
                double dx = points[i].X - points[j].X;
                double dy = points[i].Y - points[j].Y;
                _weights[i][j] = Math.Sqrt((dx * dx) + (dy * dy));
            }
        }
    }

    public bool IsMatching(IEnumerable<Edge> matching)
    {
        BitArray marked = new BitArray(Order, false);

        foreach (Edge e in matching)
        {
            if (marked[e.U] || marked[e.V])
                return false;

            marked.Set(e.U, true);
            marked.Set(e.V, true);
        }
        return (from bool b in marked where b select b).Count() == marked.Count;
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < Order; i++)
        {
            for (int j = 0; j < Order; j++)
            {
                builder.AppendLine(j == i ? "X\t" : _weights[i][j].ToString() + "\t");
            }
        }
        return builder.ToString();
    }

    public double this[Edge e]
    {
        get { return _weights[e.U][e.V]; }
        set { _weights[e.U][e.V] = value; }
    }

    public int Order => _weights.Length;

    public int NonZeroEdges => _weights.SelectMany(x => x).Where(x => x != 0).Count();
}