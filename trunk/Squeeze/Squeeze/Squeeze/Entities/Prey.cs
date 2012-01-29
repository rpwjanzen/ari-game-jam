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
using FarseerPhysics.Dynamics.Contacts;


#endif

namespace Squeeze.Entities
{
	public partial class Prey
	{
        public Body Body { get; private set; }
        public Body HeadBody { get; set; }

        private World m_world;

        private int m_angularVelocity = 0;
        private const int MAX_ANGULAR_VELOCITY = 1000;
        private const int MIN_ANGULAR_VELOCITY = -1000;
	    private const int ANGULAR_VELOCITY_CHANGE = 50;

        private Random m_random = new Random();

        // do this so that pray which spawn at the same time have different seeds.
        public void SetupRandomGenerator(int seed)
        {
            m_random = new Random(seed);
        }

		private void CustomInitialize()
		{
            m_world = FarseerPhysicsEntity.World;

            var body = BodyFactory.CreateCircle(m_world, 16, 1);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 0.15f;
            body.AngularDamping = 0.15f;

            body.Mass = 20;
            Body = body;
		}

		private void CustomActivity()
		{
            this.Position.X = Body.Position.X;
            this.Position.Y = Body.Position.Y;

            this.RotationZ = Body.Rotation;

            // Move the prey foward
            Transform t;
            Body.GetTransform(out t);

            var rotationMatrix = new Mat22(
                (float)Math.Cos(t.Angle),
                (float)Math.Sin(t.Angle),
                (float)-Math.Sin(t.Angle),
                (float)Math.Cos(t.Angle));

            var forward = rotationMatrix.Solve(-Vector2.UnitY);
            Body.ApplyForce(forward * 200);

            var left = rotationMatrix.Solve(Vector2.UnitX);

            m_angularVelocity += m_random.Next(-ANGULAR_VELOCITY_CHANGE, ANGULAR_VELOCITY_CHANGE);
            if (m_angularVelocity > MAX_ANGULAR_VELOCITY)
                m_angularVelocity = MAX_ANGULAR_VELOCITY;
            if (m_angularVelocity < MIN_ANGULAR_VELOCITY)
                m_angularVelocity = MIN_ANGULAR_VELOCITY;

            Body.ApplyForce(left * m_angularVelocity);

            SetRotation();
		}

        /// <summary>
        /// Rotate visible representation to match direction of current velocity
        /// </summary>
        private void SetRotation()
        {
            var velocity = this.Body.LinearVelocity;
            if (velocity.LengthSquared() > 1 * 1)
            {
                velocity.Normalize();
                var rotation = (float)Math.Atan2(velocity.Y, velocity.X) + MathHelper.PiOver2;

                this.RotationMatrix = Matrix.CreateRotationZ(rotation);
            }
        }

		private void CustomDestroy()
		{
		    this.m_world.RemoveBody(this.Body);
		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
