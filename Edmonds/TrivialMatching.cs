using System.Collections.Generic;
using System.Linq;

public abstract class AbstractMatching
{
    public abstract IEnumerable<Edge> Matching();
}

public class TrivialMatching : AbstractMatching
{
    private CompleteGraph _Graph;
    private Stack<Edge> _BestMatching = null;
    private double _bestValue = double.MaxValue;

    public TrivialMatching(CompleteGraph G)
    {
        _Graph = G ?? throw new System.ArgumentNullException(nameof(G));
    }

    private void ComputeMatching(Stack<Edge> partialMatching, bool[] uncoveredVertices)
    {
        if (uncoveredVertices.All(x => !x))
        {
            double result = 0.0;
            foreach (Edge e in partialMatching)
            {
                result += _Graph[e];
            }
            
            if (result < _bestValue)
            {
                _BestMatching = new Stack<Edge>(partialMatching);
                _bestValue = result;
                return;
            }
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

                    partialMatching.Push(new Edge(i, j));

                    uncoveredVertices[j] = false;
                    ComputeMatching(partialMatching, uncoveredVertices);
                    uncoveredVertices[j] = true;

                    partialMatching.Pop();
                }

                uncoveredVertices[i] = true;
            }
        }
    }

    public override IEnumerable<Edge> Matching()
    {
        if (_Graph.Order % 2 != 0)
            return null;

        if (_BestMatching == null)
        {
            ComputeMatching(new Stack<Edge>(), Enumerable.Repeat(true, _Graph.Order).ToArray());
        }
        return _BestMatching;
    }
}