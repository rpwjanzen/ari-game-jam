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
using FarseerPhysics.Dynamics;


#endif

namespace Squeeze.Entities
{
	public partial class PreyGenerator
	{
        public static readonly PositionedObjectList<Prey> g_prey = new PositionedObjectList<Prey>();
        private readonly Random m_random = new Random(271);

        public Body HeadBody { get; set; }

		private void CustomInitialize()
		{
		}

		private void CustomActivity()
		{
            while (g_prey.Count < 10)
            {
                var prey = PreyFactory.CreateNew();
                prey.SetupRandomGenerator(m_random.Next(int.MaxValue));
                prey.HeadBody = HeadBody;
                int x = m_random.Next(0 + 64, 800 - 64);
                int y = m_random.Next(-600 - 64 , - (0 + 64));

                prey.Body.Position = new Microsoft.Xna.Framework.Vector2(x, y);
            }

            foreach (var prey in g_prey)
                prey.Activity();
		}

		private void CustomDestroy()
		{

		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {
            PreyFactory.Initialize(g_prey, contentManagerName);
        }
	}
}
