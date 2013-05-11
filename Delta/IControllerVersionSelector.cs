using System;

namespace Delta
{
    public interface IControllerVersionSelector
    {
        int GetVersion(Type controllerType);
    }
}