using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squeeze.Entities
{
    public static class Settings
    {
        public enum Layers { Tree, CreatureHead, Creature, CreatureTail }

        public static Dictionary<Layers, float> LayersByName = new Dictionary<Layers, float>()
        {
            { Layers.CreatureTail, 0.003f },
            { Layers.Creature, 0.004f },
            { Layers.CreatureHead, 0.005f },
            { Layers.Tree, 0.006f },
        };
    }
}
