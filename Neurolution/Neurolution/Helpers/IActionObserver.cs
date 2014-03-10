using System.Collections.Generic;

namespace Neurolution.Helpers
{
    //Used for sending request (implemented only in Game class)
    public interface IActionObserver
    {
        void RequestAction(string action, List<object> parameters);
    }
}
