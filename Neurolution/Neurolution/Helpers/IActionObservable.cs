namespace Neurolution.Helpers
{
    //Used for registering classes which need to send requests to Game class.
    public interface IActionObservable
    {
        void Register(IActionObserver observer);
    }
}
