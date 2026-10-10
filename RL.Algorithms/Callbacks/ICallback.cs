namespace RL.Algorithms.Callbacks;

public interface ICallback<TO, TA>
{
    public void Before(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void BeforeCollect(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void BeforeStep(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void Step(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void AfterCollect(IAlgorithm<TO, TA> algorithm)
    {
    }


    public void BeforeTrain(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void TrainStep(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void AfterTrain(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void After(IAlgorithm<TO, TA> algorithm)
    {
    }
}
