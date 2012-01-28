using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

using FlatRedBall.Graphics.Model;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Math.Splines;

using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;
using FlatRedBall.Localization;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using FlatRedBall.Graphics.Lighting;
using Microsoft.Xna.Framework;
using FlatRedBall.Graphics;
#endif

namespace Squeeze.Screens
{
	public partial class GameScreen
	{

		void CustomInitialize()
		{
            LightManager.Initialize();
            LightManager.AddAmbientLight(Color.White);
            //LightManager.AddDirectionalLight(Vector3.Backward, Color.White);

            CreatureInstance.Position = new Vector3(0, 0, -10);
		}

		void CustomActivity(bool firstTimeCalled)
		{
            
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
