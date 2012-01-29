using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squeeze.Entities
{
    public static class Settings
    {
        public enum Layers { Tree, Creature }

        public static Dictionary<Layers, float> LayersByName = new Dictionary<Layers, float>()
        {
            { Layers.Creature, 0.004f },
            { Layers.Tree, 0.005f },
        };
    }
}
