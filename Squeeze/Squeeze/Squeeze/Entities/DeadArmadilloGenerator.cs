using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

using FlatRedBall.Math.Geometry;
using FlatRedBall.Math.Splines;
using BitmapFont = FlatRedBall.Graphics.BitmapFont;
using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using FlatRedBall.Math;
using Squeeze.Factories;


#endif

namespace Squeeze.Entities
{
	public partial class DeadArmadilloGenerator
	{

        public static readonly PositionedObjectList<DeadArmadillo> g_deadArmadillos = new PositionedObjectList<DeadArmadillo>();

		private void CustomInitialize()
		{


		}

		private void CustomActivity()
        {
            foreach (var armadillo in g_deadArmadillos)
                armadillo.Activity();
		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {
            DeadArmadilloFactory.Initialize(g_deadArmadillos, contentManagerName);
        }
	}
}
