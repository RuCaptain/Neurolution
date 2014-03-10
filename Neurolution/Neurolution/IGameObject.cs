using System.Collections.Generic;

namespace Neurolution
{

    //Interface for objects that based on abstract class (or interface) and having necessary methods and parameters
    public interface IGameObject
    {
        string GetName();
        void PerformAction(string action, List<object> parameters);
        Dictionary<string, object> GetCustomData();
    }
}
