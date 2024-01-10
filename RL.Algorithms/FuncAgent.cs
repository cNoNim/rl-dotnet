using System;

namespace RL.Algorithms;

public class FuncAgent<TO, TA>(string name, Func<TO, TA> func) : IAgent<TO, TA>
{
    public string Name => name;
    public TA Predict(TO state) => func(state);
}
