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


#endif

namespace Squeeze.Entities
{
	public partial class DeadArmadillo
    {
        private double m_spawnTime = 0;
        private const double DEATH_DISPLAY_TIME = 1;

		private void CustomInitialize()
		{
            m_spawnTime = TimeManager.CurrentTime;
		}

		private void CustomActivity()
		{
            if (m_spawnTime + DEATH_DISPLAY_TIME < TimeManager.CurrentTime)
                this.Destroy();
		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
