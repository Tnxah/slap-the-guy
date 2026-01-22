using System;

public interface IRewardedAdUnit : IAdUnit
{
    public abstract bool TryShow(Action onRewarded = null);
}
