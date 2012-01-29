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
using FarseerPhysics.Dynamics;
using Squeeze.Factories;


#endif

namespace Squeeze.Entities
{
	public partial class ArmadilloGenerator
	{
        public static readonly PositionedObjectList<Armadillo> g_armadillo = new PositionedObjectList<Armadillo>();
        private readonly Random m_random = new Random(382);
        
		private void CustomInitialize()
		{


		}

		private void CustomActivity()
		{
            while (g_armadillo.Count < 2)
            {
                var armadillo = ArmadilloFactory.CreateNew();
                armadillo.SetupRandomGenerator(m_random.Next(int.MaxValue));
                int x = m_random.Next(0 + 64, 800 - 64);
                int y = m_random.Next(-600 - 64, -(0 + 64));

                armadillo.Body.Position = new Microsoft.Xna.Framework.Vector2(x, y);
            }

            foreach (var prey in g_armadillo)
                prey.Activity();
		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {
            ArmadilloFactory.Initialize(g_armadillo, contentManagerName);
        }
	}
}
