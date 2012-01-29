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
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;


#endif

namespace Squeeze.Entities
{
    
	public partial class FarseerPhysicsEntity
	{
        public static World World { get { return Game1.World; } }
        

        private void CustomInitialize()
        {
            float levelWidth = 3200f;
            float levelHeight = 9600;
            float borderWidth = 50f;

            // create the border around the world
            var leftBorder = new Body(World);
            leftBorder.BodyType = BodyType.Static;
            var leftRectangleVertices = PolygonTools.CreateRectangle(borderWidth, levelHeight / 2f);
            leftBorder.CreateFixture(new PolygonShape(leftRectangleVertices, 1));
            leftBorder.Position = new Vector2(0, -levelHeight / 2f);

            var rightBorder = new Body(World);
            rightBorder.BodyType = BodyType.Static;
            var rightRectangleVertices = PolygonTools.CreateRectangle(borderWidth, levelHeight / 2f);
            rightBorder.CreateFixture(new PolygonShape(rightRectangleVertices, 1));
            rightBorder.Position = new Vector2(levelWidth, -levelHeight / 2f);

            var topBorder = new Body(World);
            topBorder.BodyType = BodyType.Static;
            var topRectangleVertices = PolygonTools.CreateRectangle(levelWidth / 2f, borderWidth);
            topBorder.CreateFixture(new PolygonShape(topRectangleVertices, 1));
            topBorder.Position = new Vector2(levelWidth / 2f, 0);

            var bottomBorder = new Body(World);
            bottomBorder.BodyType = BodyType.Static;
            var bottomRectangleVertices = PolygonTools.CreateRectangle(levelWidth / 2f, borderWidth);
            bottomBorder.CreateFixture(new PolygonShape(bottomRectangleVertices, 1));
            bottomBorder.Position = new Vector2(levelWidth / 2f, -levelHeight);
        }

		private void CustomActivity()
		{
            Game1.World.Step(1);
		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
