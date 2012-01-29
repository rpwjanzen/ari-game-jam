using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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

	    private double m_lastPreyKillTime = 0;
        private const double MIN_TIME_BETWEEN_KILLS = 0.5;
        private Vector2 startingOffset = new Vector2(150, -500);

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

        const float halfHeight = 40f / 2f;
        const float halfWidth = 20f / 2f;

        private PolygonShape GetBodyShape()
        {
            var capsuleVertices = PolygonTools.CreateRectangle(halfWidth, halfHeight);
            int c = capsuleVertices.Count;
            var shape = new PolygonShape(capsuleVertices, 1f);
            return shape;
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
            shapes.Add(GetBodyShape());

            List<Body> bodies = PathManager.EvenlyDistributeShapesAlongPath(m_world, m_path, shapes, BodyType.Dynamic, numSegments);
            foreach (var body in bodies)
                body.LinearDamping = 0.005f;

            PathManager.AttachBodiesWithSliderJoint(m_world, bodies,
                new Vector2(0, -halfHeight),
                new Vector2(0, halfHeight),
                false, true, 1, 3);

            var headBody = new Body(m_world);
            headBody.BodyType = BodyType.Dynamic;
            headBody.LinearDamping = 0.005f;
            headBody.CreateFixture(GetBodyShape());
            headBody.Position = new Vector2(0, (numSegments + 1) * -30) + startingOffset;

            Join(bodies[0], headBody);

            m_creatureHead = CreatureHeadFactory.CreateNew();
            m_creatureHead.Body = headBody;

            m_creatureTail = CreatureTailFactory.CreateNew();
            m_creatureTail.Body = bodies[bodies.Count - 1];

            foreach (var body in bodies)
            {
                body.Position += startingOffset;
            }

            for (int i = 0; i < bodies.Count - 1; i++)
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
            var sliderJoint = JointFactory.CreateSliderJoint(m_world, a, b,
                new Vector2(0, -halfHeight),
                new Vector2(0, halfHeight), 1, 3);
            sliderJoint.CollideConnected = true;
        }

        public void AddSegment()
        {
            var lastBodySegment = m_creatureBodies[m_creatureBodies.Count - 1];
            var tailSegment = m_creatureTail;

            Unjoin(lastBodySegment.Body, tailSegment.Body);

            var body = new Body(m_world);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 0.005f;
            body.CreateFixture(GetBodyShape());

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
                m_creatureHead.Body.ApplyForce(forward * 800);

                isKeyPressed = true;
            }
            else if (keyboard.KeyDown(Keys.S))
            {
                Transform t;
                m_creatureHead.Body.GetTransform(out t);

                var rotationMatrix = new Mat22(
                    (float)Math.Cos(t.Angle),
                    (float)Math.Sin(t.Angle),
                    (float)-Math.Sin(t.Angle),
                    (float)Math.Cos(t.Angle));

                var backward = rotationMatrix.Solve(Vector2.UnitY);
                m_creatureHead.Body.ApplyForce(backward * 800);

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

                var left = rotationMatrix.Solve(new Vector2(1,-1));
                m_creatureHead.Body.ApplyForce(left * 120);

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

                var right = rotationMatrix.Solve(new Vector2(-1, -1));

                m_creatureHead.Body.ApplyForce(right * 120);

                isKeyPressed = true;
            }

            if (!isKeyPressed)
            {
                m_creatureHead.Body.ResetDynamics();
                foreach (var body in m_creatureBodies)
                    body.Body.ResetDynamics();
                m_creatureTail.Body.ResetDynamics();
            }

            if (m_lastPreyKillTime + MIN_TIME_BETWEEN_KILLS < TimeManager.CurrentTime  &&
                (m_creatureHead.Body.Position - m_creatureTail.Body.Position).Length() < 60)
            {
                List<Vector2> snakePolygonPoints = new List<Vector2>();
                snakePolygonPoints.Add(m_creatureHead.Body.Position);
                snakePolygonPoints.Add(m_creatureTail.Body.Position);
                foreach (CreatureBody mCreatureBody in m_creatureBodies)
                {
                    snakePolygonPoints.Add(mCreatureBody.Body.Position);
                }

                var prey =
                    PreyGenerator.g_prey.FirstOrDefault(
                        p => IsPointWithin(new Vector2(p.Position.X, p.Position.Y), snakePolygonPoints));

                if (prey != null)
                {
                    AddSegment();
                    prey.Destroy();
                    m_lastPreyKillTime = TimeManager.CurrentTime;
                }
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

        public bool IsPointWithin(Vector2 point, List<Vector2> poly)
        {
            // winding number test for a point in a polygon
            //      Input:   P = a point,
            //               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
            //      Return:  wn = the winding number (=0 only if P is outside V[])

            int Counter = 0;

            // loop through all edges of the polygon
            for (int i = 0; i < poly.Count - 1; i++)
            {   // edge from poly[i] to poly[i+1]
                if (poly[i].Y <= point.Y)
                {
                    // start y <= point.Y
                    if (poly[i + 1].Y > point.Y)  // an upward crossing
                    {
                        if (isLeft(poly[i], poly[i + 1], point) > 0)  // point left of edge
                        {
                            ++Counter;  // have a valid up intersect
                        }
                    }
                }
                else
                {
                    // start y > point.y (no test needed)
                    if (poly[i + 1].Y <= point.Y)     // a downward crossing
                        if (isLeft(poly[i], poly[i + 1], point) < 0)  // point right of edge
                        {
                            --Counter;            // have a valid down intersect
                        }
                }
            }
            if (Counter != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private float isLeft(Vector2 LinePoint1, Vector2 LinePoint2, Vector2 TestPoint)
        {
            // tests if a point is Left|On|Right of an infinite line.
            //    Input:  three points LinePoint1, LinePoint2, and TestPoint
            //    Return: >0 for TestPoint left of the line through LinePoint1 and LinePoint2
            //            =0 for TestPoint on the line
            //            <0 for TestPoint right of the line
            return ((LinePoint2.X - LinePoint1.X) * (TestPoint.Y - LinePoint1.Y) - (TestPoint.X - LinePoint1.X) * (LinePoint2.Y - LinePoint1.Y));
        }
	}
}
