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
using Squeeze.Screens;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework.Audio;


#endif

namespace Squeeze.Entities
{
	public partial class Creature
	{
        private World m_world;

        private CreatureHead m_creatureHead;
        private CreatureTail m_creatureTail;

        private List<CreatureBody> m_creatureBodySegments = new List<CreatureBody>();

	    private double m_lastPreyKillTime = 0;
        private const double MIN_TIME_BETWEEN_KILLS = 0.5;
        private Vector2 startingOffset = new Vector2(800, -500);

	    private const int MOVE_PREY_CLOSER_TRIGGER_DISTANCE = 750;
	    private const int MOVE_PREY_CLOSER_NEW_DISTANCE = 500;

	    private Random m_random = new Random();

        private static SoundEffect m_mouseSquish;

        // We only add one section per time unit so we let you queue them
	    private double m_lastSectionAddTime = 0;
	    private int m_segmentsToAdd = 0;
	    private const double TIME_BETWEEN_SECTION_ADDS = 2;//seconds

        public Vector3 Centroid
        {
            get
            {
                Vector3 v = m_creatureHead.Position;
                return v;

                //foreach (var bodySegment in m_creatureBodies)
                //    v += bodySegment.Position;
                //v += m_creatureTail.Position;

                //return v / (m_creatureBodies.Count + 2);1
            }
        }

	    public Vector2 Vector2Centroid
	    {
            get { return new Vector2(Centroid.X, Centroid.Y); }
	    }
        const int InitialSegmentCount = 40;
        const float HalfHeight = 10f / 2f;
        const float HalfWidth = 15f / 2f;

        const float BodyRestitution = 1;

        const float Frequency = 0;
        const float DampeningRatio = 0;
        const float MinLength = 1;
        const float MaxLength = 1;

        const float LinearDampening = 0.01f;
        const float Friction = 1f;

        const float FastForwardForce = 150;
        const float NormalForwardForce = 150;
        float ForwardForce = NormalForwardForce;
        
        const float BackwardForce = 100;
        const float LateralForce = 100;

        const float Density = 1;

        private PolygonShape GetBodyShape()
        {
            //var capsuleVertices = PolygonTools.CreateRectangle(HalfWidth, HalfHeight);
            var capsuleVertices = PolygonTools.CreateCircle(HalfWidth, 8);
            var shape = new PolygonShape(capsuleVertices, Density);
            return shape;
        }

		private void CustomInitialize()
		{
            m_world = FarseerPhysicsEntity.World;

            Path m_path = new Path();
            m_path.Add(new Vector2(0, InitialSegmentCount * -HalfHeight));
            m_path.Add(new Vector2(0, InitialSegmentCount * HalfHeight));
            m_path.Closed = false;

            List<Shape> shapes = new List<Shape>();
            shapes.Add(GetBodyShape());

            // create bodies and spring joints
            List<Body> bodies = PathManager.EvenlyDistributeShapesAlongPath(m_world, m_path, shapes, BodyType.Dynamic, InitialSegmentCount);
            var springJoints = PathManager.AttachBodiesWithSliderJoint(m_world, bodies,
                new Vector2(0, -HalfHeight),
                new Vector2(0, HalfHeight),
                false, true,
                MinLength, MaxLength);

            // set extra spring joint parameters
            foreach (var joint in springJoints)
            {
                joint.Frequency = Frequency;
                joint.DampingRatio = DampeningRatio;
            }

            // create head
            var headBody = new Body(m_world);
            headBody.BodyType = BodyType.Dynamic;
            headBody.CreateFixture(GetBodyShape());
            headBody.Position = new Vector2(0, (InitialSegmentCount + 1) * -HalfHeight) + startingOffset;
            headBody.LinearDamping = LinearDampening;
            headBody.Restitution = BodyRestitution;
            headBody.Friction = Friction;

            m_creatureHead = CreatureHeadFactory.CreateNew();
            m_creatureHead.Body = headBody;
            Join(bodies[0], headBody);

            // create tail
            m_creatureTail = CreatureTailFactory.CreateNew();
            m_creatureTail.Body = bodies[bodies.Count - 1];

            // offset + set bodies variables
            foreach (var body in bodies)
            {
                body.Position += startingOffset;
                body.LinearDamping = LinearDampening;
                body.Restitution = BodyRestitution;
                body.Friction = Friction;
            }

            // set bodies to body segments
            for (int i = 0; i < bodies.Count - 1; i++)
            {
                var creatureBodySegment = CreatureBodyFactory.CreateNew();
                creatureBodySegment.Body = bodies[i];
                m_creatureBodySegments.Add(creatureBodySegment);
            }

            m_mouseSquish = FlatRedBallServices.Load<SoundEffect>("Content\\MouseSquish");
		}

        private void Unjoin(Body a, Body b)
        {
            var jointEdge = a.JointList;
            var joint = jointEdge.Joint;
            m_world.RemoveJoint(joint);
        }

        private void Join(Body closestToTailBody, Body closestToHeadBody)
        {
            var springJoint = JointFactory.CreateSliderJoint(m_world, closestToTailBody, closestToHeadBody,
                new Vector2(0, -HalfHeight),
                new Vector2(0, HalfHeight),
                MinLength, MaxLength);
            springJoint.CollideConnected = true;
            springJoint.Frequency = Frequency;
            springJoint.DampingRatio = DampeningRatio;
        }

        public void AddSegment()
        {
            var lastBodySegment = m_creatureBodySegments[m_creatureBodySegments.Count - 1];
            var tailSegment = m_creatureTail;

            // detach tail
            Unjoin(lastBodySegment.Body, tailSegment.Body);

            // create new segment body
            var body = new Body(m_world);
            body.BodyType = BodyType.Dynamic;
            body.CreateFixture(GetBodyShape());
            body.LinearDamping = LinearDampening;
            body.Restitution = BodyRestitution;
            body.Friction = Friction;

            var newSegment = CreatureBodyFactory.CreateNew();
            newSegment.Body = body;

            // attach new segment body
            Join(newSegment.Body, lastBodySegment.Body);

            // position new body
            float xDelta = lastBodySegment.Position.X - tailSegment.Position.X;
            float yDelta = lastBodySegment.Position.Y - tailSegment.Position.Y;
            float avgX = tailSegment.Position.X + (xDelta / 2.0f);
            float avgY = tailSegment.Position.Y + (yDelta / 2.0f);
            newSegment.Body.Position = new Vector2(avgX, avgY);
            newSegment.Body.Rotation = tailSegment.Body.Rotation;

            m_creatureBodySegments.Add(newSegment);

            // re-attach tail
            Join(tailSegment.Body, newSegment.Body);
        }

        private Mat22 GetRotationMatrix(float angle)
        {
            var rotationMatrix = new Mat22(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle),
                (float)-Math.Sin(angle),
                (float)Math.Cos(angle));

            return rotationMatrix;
        }

		private void CustomActivity()
		{
            // sync position and rotation of each item
            m_creatureHead.Activity();
            foreach (var bodySegment in m_creatureBodySegments)
                bodySegment.Activity();
            m_creatureTail.Activity();

		    HandleInput();

		    KillPrey();

		    MovePreyCloser();

		    AddQueuedSegments();
		}

        private void AddQueuedSegments()
        {
            if(m_segmentsToAdd > 0 &&
                m_lastSectionAddTime + TIME_BETWEEN_SECTION_ADDS < TimeManager.CurrentTime)
            {
                m_lastSectionAddTime = TimeManager.CurrentTime;
                m_segmentsToAdd--;
                AddSegment();
            }
        }

        private void HandleInput()
        {
            bool isKeyPressed = false;
            var keyboard = InputManager.Keyboard;
            if (keyboard.KeyDown(Keys.LeftShift))
                ForwardForce = FastForwardForce;
            else
                ForwardForce = NormalForwardForce;

            if (keyboard.KeyDown(Keys.W))
            {
                Transform t;
                m_creatureHead.Body.GetTransform(out t);

                var rotationMatrix = GetRotationMatrix(t.Angle);
                var localForward = -Vector2.UnitY;
                var absoluteForward = rotationMatrix.Solve(localForward);
                m_creatureHead.Body.ApplyForce(absoluteForward * ForwardForce);

                ApplyForceToSomeSegments(ForwardForce, localForward);

                isKeyPressed = true;
            }
            else if (keyboard.KeyDown(Keys.S))
            {
                Transform t;
                m_creatureHead.Body.GetTransform(out t);

                var rotationMatrix = GetRotationMatrix(t.Angle);
                var localBackward = Vector2.UnitY;
                var absoluteBackward = rotationMatrix.Solve(localBackward);
                m_creatureHead.Body.ApplyForce(absoluteBackward * BackwardForce);

                ApplyForceToSomeSegments(BackwardForce, localBackward);

                isKeyPressed = true;
            }

            if (keyboard.KeyDown(Keys.A))
            {
                Transform t;
                m_creatureHead.Body.GetTransform(out t);

                var rotationMatrix = GetRotationMatrix(t.Angle);
                
                var localLeft = Vector2.UnitX;
                var absoluteLeft = rotationMatrix.Solve(localLeft);
                m_creatureHead.Body.ApplyForce(absoluteLeft * LateralForce);

                //ApplyForceToSomeSegments(LateralForce, localLeft);

                isKeyPressed = true;
            }
            else if (keyboard.KeyDown(Keys.D))
            {
                Transform t;
                m_creatureHead.Body.GetTransform(out t);

                var rotationMatrix = GetRotationMatrix(t.Angle);
                var localRight = -Vector2.UnitX;
                var absoluteRight = rotationMatrix.Solve(localRight);
                m_creatureHead.Body.ApplyForce(absoluteRight * LateralForce);

                //ApplyForceToSomeSegments(LateralForce, localRight);

                isKeyPressed = true;
            }

            if (!isKeyPressed)
            {
                //m_creatureHead.Body.ResetDynamics();
                //foreach (var body in m_creatureBodySegments)
                //    body.Body.ResetDynamics();
                //m_creatureTail.Body.ResetDynamics();
            }
        }

        private Mat22 GetRotationMatrix(Transform t)
        {
            return new Mat22(
                (float)Math.Cos(t.Angle),
                (float)Math.Sin(t.Angle),
                (float)-Math.Sin(t.Angle),
                (float)Math.Cos(t.Angle));    
        }

        /// <summary>
        /// Kills any encompased prey, maxing out at one per time interval.
        /// </summary>
        private void KillPrey()
        {
            if (m_lastPreyKillTime + MIN_TIME_BETWEEN_KILLS < TimeManager.CurrentTime &&
                (m_creatureHead.Body.Position - m_creatureTail.Body.Position).Length() < 60)
            {
                List<Vector2> snakePolygonPoints = new List<Vector2>();
                snakePolygonPoints.Add(m_creatureHead.Body.Position);
                snakePolygonPoints.Add(m_creatureTail.Body.Position);
                foreach (CreatureBody mCreatureBody in m_creatureBodySegments)
                {
                    snakePolygonPoints.Add(mCreatureBody.Body.Position);
                }

                var prey =
                    PreyGenerator.g_prey.FirstOrDefault(
                        p => IsPointWithin(new Vector2(p.Position.X, p.Position.Y), snakePolygonPoints));

                if (prey != null)
                {
                    m_segmentsToAdd += 1;
                    DeadPrey deadPrey = DeadPreyFactory.CreateNew();
                    deadPrey.Position = prey.Position;
                    deadPrey.RotationMatrix = prey.RotationMatrix;
                    prey.Destroy();
                    m_lastPreyKillTime = TimeManager.CurrentTime;
                    m_mouseSquish.Play();
                }
                else
                {
                    var armadillo = ArmadilloGenerator.g_armadillo.FirstOrDefault(
                                        p => IsPointWithin(new Vector2(p.Position.X, p.Position.Y), snakePolygonPoints));

                    if (armadillo != null)
                    {
                        m_segmentsToAdd += 3;
                        DeadArmadillo deadArmadillo = DeadArmadilloFactory.CreateNew();
                        deadArmadillo.Position = armadillo.Position;
                        deadArmadillo.RotationMatrix = armadillo.RotationMatrix;
                        armadillo.Destroy();
                        m_lastPreyKillTime = TimeManager.CurrentTime;
                        m_mouseSquish.Play();
                    }
                    else
                    {
                        var buffalo = BufalloGenerator.g_buffalo.FirstOrDefault(
                                            p => IsPointWithin(new Vector2(p.Position.X, p.Position.Y), snakePolygonPoints));

                        if (buffalo != null)
                        {
                            m_segmentsToAdd += 6;
                            DeadBuffalo deadBuffalo = DeadBuffaloFactory.CreateNew();
                            deadBuffalo.Position = buffalo.Position;
                            deadBuffalo.RotationMatrix = buffalo.RotationMatrix;
                            buffalo.Destroy();
                            m_lastPreyKillTime = TimeManager.CurrentTime;
                            m_mouseSquish.Play();
                        }
                    }
                }
            }    
        }

        /// <summary>
        /// To give the appearance of a populated area we are going to move units that get far away closer.
        /// </summary>
        private void MovePreyCloser()
        {
            var rats = PreyGenerator.g_prey.Where(r => (r.Position - Centroid).Length() > MOVE_PREY_CLOSER_TRIGGER_DISTANCE);

            foreach(Prey rat in rats)
            {
                MovePositionedObjectCloser(rat.Body);
            }


            var armadillos = ArmadilloGenerator.g_armadillo.Where(r => (r.Position - Centroid).Length() > MOVE_PREY_CLOSER_TRIGGER_DISTANCE);

            foreach (Armadillo armadillo in armadillos)
            {
                MovePositionedObjectCloser(armadillo.Body);
            }

            var buffalos = BufalloGenerator.g_buffalo.Where(r => (r.Position - Centroid).Length() > MOVE_PREY_CLOSER_TRIGGER_DISTANCE);

            foreach (Buffalo buffalo in buffalos)
            {
                MovePositionedObjectCloser(buffalo.Body);
            }
            
        }

        private void MovePositionedObjectCloser(Body body)
        {
            // chose an axis to use to make sure the creature is out of sight.
            if (m_random.Next(2) == 1)
            {
                // x axis
                body.Position = new Vector2(Vector2Centroid.X + (RandOfNegativeOneOrPositiveOne() * MOVE_PREY_CLOSER_NEW_DISTANCE),
                                                            Vector2Centroid.Y + (m_random.Next(-MOVE_PREY_CLOSER_NEW_DISTANCE, MOVE_PREY_CLOSER_NEW_DISTANCE)));
            }
            else
            {
                // y axis
                body.Position = new Vector2(Vector2Centroid.X + (m_random.Next(-MOVE_PREY_CLOSER_NEW_DISTANCE, MOVE_PREY_CLOSER_NEW_DISTANCE)),
                                                            Vector2Centroid.Y + (RandOfNegativeOneOrPositiveOne() * MOVE_PREY_CLOSER_NEW_DISTANCE));
            }
        }

        private int RandOfNegativeOneOrPositiveOne()
        {
            return m_random.Next(2) == 1 ? 1 : -1;
        }

        private void ApplyForceToSomeSegments(float forceAmount, Vector2 localDirection)
        {
            for (int i = 0; i < m_creatureBodySegments.Count; i += 1)
            {
                Transform t;
                
                var bodySegment = m_creatureBodySegments[i];
                var body = bodySegment.Body;
                body.GetTransform(out t);

                var rotationMatrix = new Mat22(
                    (float)Math.Cos(t.Angle),
                    (float)Math.Sin(t.Angle),
                    (float)-Math.Sin(t.Angle),
                    (float)Math.Cos(t.Angle));

                var absoluteDirection = rotationMatrix.Solve(localDirection);

                float falloff = (i / (float)m_creatureBodySegments.Count);
                //body.ApplyForce(absoluteDirection * forceAmount);
                body.ApplyForce(absoluteDirection * forceAmount * falloff);
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
                        if (IsLeft(poly[i], poly[i + 1], point) > 0)  // point left of edge
                        {
                            ++Counter;  // have a valid up intersect
                        }
                    }
                }
                else
                {
                    // start y > point.y (no test needed)
                    if (poly[i + 1].Y <= point.Y)     // a downward crossing
                        if (IsLeft(poly[i], poly[i + 1], point) < 0)  // point right of edge
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

        private float IsLeft(Vector2 LinePoint1, Vector2 LinePoint2, Vector2 TestPoint)
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
