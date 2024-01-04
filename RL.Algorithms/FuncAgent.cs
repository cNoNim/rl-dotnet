using System;

namespace RL.Algorithms;

public class FuncAgent<TO, TA>(Func<TO, TA> func) : IAgent<TO, TA>
{
    public TA Predict(TO state) => func(state);
}