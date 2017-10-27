using System.Collections.Generic;

public abstract class AbstractMatching
{
    public abstract IEnumerable<Edge> Matching();
}

public class TrivialMatching : AbstractMatching
{
    private CompleteGraph _Graph;
    private HashSet<Edge> _BestMatching = null;
    private double _bestValue = double.MaxValue;

    public TrivialMatching(CompleteGraph G)
    {
        _Graph = G;
    }

    private void ComputeMatching(HashSet<Edge> partialMatching, List<int> v)
    {
        if (v.Count == 0)
        {
            double temp = _Graph.SumOfWeights(partialMatching);
            if (temp < _bestValue)
            {
                _BestMatching = new HashSet<Edge>(partialMatching);
                _bestValue = temp;
                return;
            }
        }
        for (int j = 1; j < v.Count; j++)
        {
            Edge add = new Edge(v[0], v[j]);
            partialMatching.Add(add);
            List<int> v_new = new List<int>(v);
            v_new.Remove(v[0]);
            v_new.Remove(v[j]);
            ComputeMatching(partialMatching, v_new);
            partialMatching.Remove(add);
        }
    }

    public override IEnumerable<Edge> Matching()
    {
        if (_Graph.Order / 2 != 0)
            return null;

        if (_BestMatching == null)
        {
            List<int> vertices = new List<int>();
            for (int i = 0; i < _Graph.Order; i++)
            {
                vertices.Add(i);
            }
            ComputeMatching(new HashSet<Edge>(), vertices);
        }
        return _BestMatching;
    }
}