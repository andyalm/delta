﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouteAttribExplorer
{
    public interface IControllerVersionSelector
    {
        int GetVersion(Type controllerType);
    }

    public class NamespaceControllerVersionSelector : IControllerVersionSelector
    {
        public int GetVersion(Type controllerType)
        {
            var ns = controllerType.Namespace;
            var segment = ns.Substring(ns.LastIndexOf('.') + 1);
            var version = int.Parse(segment.Substring(1));
            return version;
        }
    }
}
