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
        private PositionedObject cameraCenter = new PositionedObject();

		void CustomInitialize()
		{
            SpriteManager.Camera.UsePixelCoordinates();
            SpriteManager.Camera.AttachTo(cameraCenter, true);
            //SpriteGridInstance.AddToManagers();
            //BackgroundInstance.AddToManagers();
		}

		void CustomActivity(bool firstTimeCalled)
		{
            cameraCenter.Position = CreatureInstance.Centroid;

            // Floor so that we do not have flickering lines between map tiles.
            cameraCenter.Position.X = (float)Math.Floor(cameraCenter.Position.X);
            cameraCenter.Position.Y = (float)Math.Floor(cameraCenter.Position.Y);
            cameraCenter.Position.Z = (float)Math.Floor(cameraCenter.Position.Z);

            
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
