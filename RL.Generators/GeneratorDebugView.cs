using System.Diagnostics;
using RL.Core;

namespace RL.Generators;

public class GeneratorDebugView(IGenerator generator)
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public object?[] Items
    {
        get
        {
            var array = new object?[generator.Count];
            foreach (var index in Generator.Range<int>(generator.Count))
                array[index] = generator[index];
            return array;
        }
    }
}