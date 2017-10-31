using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public abstract class AbstractMatching
{
    public abstract IEnumerable<Edge> Matching();
}

public class TrivialMatching : AbstractMatching
{
    private CompleteGraph _graph;
    private Stack<Edge> _bestMatching = null;
    private double _bestValue = double.MaxValue;

    public TrivialMatching(CompleteGraph G)
    {
        _graph = G ?? throw new System.ArgumentNullException(nameof(G));
    }

    private void ComputeMatching(Stack<Edge> partialMatching, bool[] uncoveredVertices, double currentValue)
    {
        Debug.Assert(uncoveredVertices.All(x => !x));

        if (partialMatching.Count == _graph.Order / 2)
        {
            _bestMatching = new Stack<Edge>(partialMatching);
            _bestValue = currentValue;

            return;
        }

        for (int i = 0; i < uncoveredVertices.Length; i++)
        {
            if (uncoveredVertices[i])
            {
                uncoveredVertices[i] = false;

                for (int j = i + 1; j < uncoveredVertices.Length; j++)
                {
                    if (!uncoveredVertices[j])
                        continue;

                    var edge = new Edge(i, j);

                    var newValue = currentValue + _graph[edge];
                    if (newValue > _bestValue)
                        continue; // no room for improvement

                    partialMatching.Push(edge);

                    uncoveredVertices[j] = false;
                    ComputeMatching(partialMatching, uncoveredVertices, newValue);
                    uncoveredVertices[j] = true;

                    partialMatching.Pop();
                }

                uncoveredVertices[i] = true;
            }
        }
    }

    public override IEnumerable<Edge> Matching()
    {
        if (_graph.Order % 2 != 0)
            return null;

        ComputeMatching(new Stack<Edge>(), Enumerable.Repeat(true, _graph.Order).ToArray(), 0);

        return _bestMatching;
    }
}