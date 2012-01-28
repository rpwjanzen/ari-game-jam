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
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;


#endif

namespace Squeeze.Entities
{
	public partial class Prey
	{
        public Body Body { get; private set; }
        private World m_world;

		private void CustomInitialize()
		{
            m_world = FarseerPhysicsEntity.World;

            var body = BodyFactory.CreateRectangle(m_world, 32, 32, 1);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 0.15f;
            body.AngularDamping = 0.15f;

            var rectangleVertices = PolygonTools.CreateRectangle(16f, 16f, Vector2.Zero, 0);
            var shape = new PolygonShape(rectangleVertices, 1f);
            var fixture = body.CreateFixture(shape);
            
            Body = body;
		}

		private void CustomActivity()
		{
            this.Position.X = Body.Position.X;
            this.Position.Y = Body.Position.Y;

            this.RotationZ = Body.Rotation;
		}

		private void CustomDestroy()
		{
            m_world.RemoveBody(Body);

		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
