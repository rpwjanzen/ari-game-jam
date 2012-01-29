using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squeeze.Entities
{
    public static class Settings
    {
        public enum Layers { Tree, CreatureHead, CreatureBody, CreatureTail, Buffalo, Rat, Armadillo, Log, Stump }

        public static Dictionary<Layers, float> LayersByName = new Dictionary<Layers, float>()
        {
            { Layers.Stump, 0.001f },
            { Layers.Rat, 0.002f },
            { Layers.CreatureTail, 0.003f },
            { Layers.CreatureBody, 0.004f },
            { Layers.CreatureHead, 0.005f },
            { Layers.Log, 0.006f },
            { Layers.Armadillo, 0.007f },
            { Layers.Buffalo, 0.008f },
            { Layers.Tree, 0.009f },
        };
    }
}
