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
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Squeeze.Factories;
using FlatRedBall.Math;


#endif

namespace Squeeze.Entities
{
	public partial class Creature
	{
        private World m_world;

        private CreatureHead m_creatureHead;
        private CreatureTail m_creatureTail;

        private List<CreatureBody> m_creatureBodies = new List<CreatureBody>();

        public Vector3 Centroid
        {
            get
            {
                Vector3 v = m_creatureHead.Position;
                foreach (var bodySegment in m_creatureBodies)
                    v += bodySegment.Position;
                v += m_creatureTail.Position;

                return v / (m_creatureBodies.Count + 2);
            }
        }

		private void CustomInitialize()
		{
            m_world = FarseerPhysicsEntity.World;
            int numSegments = 10;

            Path m_path = new Path();
            m_path.Add(new Vector2(0, numSegments * -30));
            m_path.Add(new Vector2(0, numSegments * 30));
            m_path.Closed = false;

            List<Shape> shapes = new List<Shape>();
            var rectangleVertices = PolygonTools.CreateRectangle(16f, 16f, Vector2.Zero, 0);
            shapes.Add(new PolygonShape(rectangleVertices, 1f));
            shapes.Add(new CircleShape(25f, 1));

            List<Body> bodies = PathManager.EvenlyDistributeShapesAlongPath(m_world, m_path, shapes, BodyType.Dynamic, numSegments);
            foreach (var body in bodies)
                body.LinearDamping = 0.005f;

            PathManager.AttachBodiesWithSliderJoint(m_world, bodies,
                new Vector2(0, -25f),
                new Vector2(0, 25f),
                false, true, 1, 3);

            m_creatureHead = CreatureHeadFactory.CreateNew();
            m_creatureHead.Body = bodies[0];

            m_creatureTail = CreatureTailFactory.CreateNew();
            m_creatureTail.Body = bodies[bodies.Count - 1];

            for (int i = 1; i < bodies.Count - 1; i++)
            {
                var creatureBodySegment = CreatureBodyFactory.CreateNew();
                creatureBodySegment.Body = bodies[i];
                m_creatureBodies.Add(creatureBodySegment);
            }
		}

        private void Unjoin(Body a, Body b)
        {
            var jointEdge = a.JointList;
            var joint = jointEdge.Joint;
            m_world.RemoveJoint(joint);
        }

        private void Join(Body a, Body b)
        {
            var sliderJoint = JointFactory.CreateSliderJoint(m_world, a, b, new Vector2(0, -25f), new Vector2(0, 25f), 1, 3);
            sliderJoint.CollideConnected = true;
        }

        public void AddSegment()
        {
            var lastBodySegment = m_creatureBodies[m_creatureBodies.Count - 1];
            var tailSegment = m_creatureTail;

            Unjoin(lastBodySegment.Body, tailSegment.Body);

            List<Shape> bodyShapes = new List<Shape>();
            var rectangleVertices = PolygonTools.CreateRectangle(16f, 16f, Vector2.Zero, 0);
            bodyShapes.Add(new PolygonShape(rectangleVertices, 1f));
            bodyShapes.Add(new CircleShape(25f, 1));

            var body = new Body(m_world);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 0.005f;
            foreach(var shape in bodyShapes)
                body.CreateFixture(shape);

            var newSegment = CreatureBodyFactory.CreateNew();
            newSegment.Body = body;

            Join(newSegment.Body, lastBodySegment.Body);

            float x = lastBodySegment.Position.X - tailSegment.Position.X;
            float y = lastBodySegment.Position.Y - tailSegment.Position.Y;

            newSegment.Body.Position = new Vector2(
                tailSegment.Position.X + (x / 2.0f),
                tailSegment.Position.Y + (y / 2.0f));

            newSegment.Body.Rotation = tailSegment.Body.Rotation;

            Join(tailSegment.Body, newSegment.Body);

            m_creatureBodies.Add(newSegment);
        }

		private void CustomActivity()
		{
            m_creatureHead.Activity();
            foreach (var bodySegment in m_creatureBodies)
                bodySegment.Activity();

            m_creatureTail.Activity();

            bool isKeyPressed = false;
            var keyboard = InputManager.Keyboard;
            if (keyboard.KeyDown(Keys.W))
            {
                Transform t;
                m_creatureHead.Body.GetTransform(out t);

                var rotationMatrix = new Mat22(
                    (float)Math.Cos(t.Angle),
                    (float)Math.Sin(t.Angle),
                    (float)-Math.Sin(t.Angle),
                    (float)Math.Cos(t.Angle));

                var forward = rotationMatrix.Solve(-Vector2.UnitY);
                m_creatureHead.Body.ApplyForce(forward * 400);

                isKeyPressed = true;
            }
            if (keyboard.KeyDown(Keys.A))
            {
                Transform t;
                m_creatureHead.Body.GetTransform(out t);

                var rotationMatrix = new Mat22(
                    (float)Math.Cos(t.Angle),
                    (float)Math.Sin(t.Angle),
                    (float)-Math.Sin(t.Angle),
                    (float)Math.Cos(t.Angle));

                var left = rotationMatrix.Solve(Vector2.UnitX);
                m_creatureHead.Body.ApplyForce(left * 400);

                isKeyPressed = true;
            }
            else if (keyboard.KeyDown(Keys.D))
            {
                Transform t;
                m_creatureHead.Body.GetTransform(out t);

                var rotationMatrix = new Mat22(
                    (float)Math.Cos(t.Angle),
                    (float)Math.Sin(t.Angle),
                    (float)-Math.Sin(t.Angle),
                    (float)Math.Cos(t.Angle));

                var right = rotationMatrix.Solve(-Vector2.UnitX);

                m_creatureHead.Body.ApplyForce(right * 400);

                isKeyPressed = true;
            }

            if (!isKeyPressed)
            {
                m_creatureHead.Body.ResetDynamics();
                foreach (var body in m_creatureBodies)
                    body.Body.ResetDynamics();
                m_creatureTail.Body.ResetDynamics();
            }

            if (keyboard.KeyReleased(Keys.Space))
            {
                AddSegment();
            }
		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {
            PositionedObjectList<CreatureHead> creatureHeads = new PositionedObjectList<CreatureHead>();
            CreatureHeadFactory.Initialize(creatureHeads, contentManagerName);

            PositionedObjectList<CreatureBody> creatureBodies= new PositionedObjectList<CreatureBody>();
            CreatureBodyFactory.Initialize(creatureBodies, contentManagerName);

            PositionedObjectList<CreatureTail> creatureTails= new PositionedObjectList<CreatureTail>();
            CreatureTailFactory.Initialize(creatureTails, contentManagerName);
        }
	}
}
