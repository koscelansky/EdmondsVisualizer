using ColorMine.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

public class EdmondsVisualizer
{
    private EdmondsMatching _matching;
    private Image _buffer;
    private Graphics _graphics;

    public EdmondsVisualizer(int width, int height, List<Point> points)
    {
        InitGraphics(width, height);
        Points = points;
        InitMatching();
    }

    private void InitGraphics(int width, int height)
    {
        _buffer = new Bitmap(width, height);
        _graphics = Graphics.FromImage(_buffer);
        _graphics.SmoothingMode = SmoothingMode.HighQuality;
        _graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
    }

    private void InitMatching()
    {
        _matching = new EdmondsMatching(new CompleteGraph(Points));
        _matching.Draw = DrawMatching;
    }

    private double BlossomThickness(Blossom blossom, int vertex)
    {
        if (blossom is VertexBlossom)
        {
            Debug.Assert(blossom.Stem == vertex);

            // negative thickness is rather hard to draw
            return Math.Max(blossom.Thickness, 0);
        }

        if (blossom is CompositeBlossom composite)
        {
            foreach (Blossom nextBlossom in composite.SubBlossoms)
            {
                if (nextBlossom.ContainsVertex(vertex))
                {
                    return blossom.Thickness + BlossomThickness(nextBlossom, vertex);
                }
            }
        }

        return 0;
    }

    private int BlossomLevel(Blossom blossom)
    {
        if (blossom is CompositeBlossom composite)
        {
            int max = 0;
            foreach (Blossom nextBlossom in composite.SubBlossoms)
            {
                max = Math.Max(max, BlossomLevel(nextBlossom));
            }
            return max + 1;
        }

        return 0;
    }

    private void DrawBlossom(Blossom blossom)
    {
        int level = BlossomLevel(blossom);
        foreach (int v in blossom.Vertices)
        {
            int x = Points[v].X;
            int y = Points[v].Y;
            float r = (float)BlossomThickness(blossom, v);
            _graphics.FillEllipse(new SolidBrush(PickColor(level)), x - r, y - r, 2 * r, 2 * r);
        }

        if (blossom is CompositeBlossom)
        {
            foreach (Blossom nextBlossom in ((CompositeBlossom)blossom).SubBlossoms)
            {
                DrawBlossom(nextBlossom);
            }
        }
    }

    private void DrawNode(Node n)
    {
        foreach (var v in n._children.Values)
        {
            DrawNode(v);
        }
        DrawBlossom(n._blossom);
    }

    public IEnumerable<Edge> Edges => _matching.Edges;

    public bool NextIter()
    {
        return _matching.NextIter();
    }

    public void DrawMatching(CompleteGraph fullEdges, CompleteGraph partialMatching, List<AbstractTree> forrest)
    {
        _graphics.Clear(Color.White);

        foreach (AbstractTree tree in forrest)
        {
            if (tree is Tree t)
            {
                DrawNode(t.Root);
            }
            else if (tree is Barbell barbell)
            {
                DrawNode(barbell.First);
                DrawNode(barbell.Second);
            }
        }

        foreach (Point p in Points)
        {
            _graphics.DrawEllipse(new Pen(Color.Black, 3), p.X - 2.5f, p.Y - 2.5f, 5f, 5f);
        }

        for (int i = 0; i < partialMatching.Order; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (partialMatching[new Edge(i, j)] == 1)
                {
                    _graphics.DrawLine(new Pen(Color.Black, 5), Points[i], Points[j]);
                }

                if (fullEdges[new Edge(i, j)] == 1)
                {
                    _graphics.DrawLine(new Pen(Color.Red, 1), Points[i], Points[j]);
                }
            }
        }
    }

    public void Draw(Graphics g)
    {
        g.DrawImageUnscaled(_buffer, 0, 0);
    }

    public bool IsMatching { get { return _matching.Graph.IsMatching(_matching.Edges); } }

    private List<Point> Points { get; }

    private static Color PickColor(int level)
    {
        double h = 1 / (level / 2f + 1);

        Hsl hsl = new Hsl { H = h * 360, L = 80, S = 100 };
        Rgb rgb = hsl.To<Rgb>();

        return Color.FromArgb((int)rgb.R, (int)rgb.G, (int)rgb.B);
    }
}