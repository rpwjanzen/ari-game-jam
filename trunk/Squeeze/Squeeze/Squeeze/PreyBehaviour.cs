using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;

namespace Squeeze
{
    static class PreyBehaviour
    {
        private const int MAX_ANGULAR_VELOCITY = 1000;
        private const int MIN_ANGULAR_VELOCITY = -1000;
        private const int ANGULAR_VELOCITY_CHANGE = 50;

        private const double TIME_TO_KEEP_HISTORY_FOR = 3; // seconds
        private const float MIN_DISTANCE_REQUIRED = 20; // pixels

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prey"></param>
        /// <param name="preyBody"></param>
        /// <param name="angularVelocity"></param>
        /// <param name="random"></param>
        /// <param name="rotationMatrix"></param>
        /// <returns>The new rotation matrix.</returns>
        public static Matrix? MovePrey(
            PositionedObject prey, 
            Body preyBody, 
            ref int angularVelocity,
            Random random)
        {
            prey.Position.X = preyBody.Position.X;
            prey.Position.Y = preyBody.Position.Y;

            prey.RotationZ = preyBody.Rotation;

            // Move the prey foward
            Transform t;
            preyBody.GetTransform(out t);

            var rotationMatrix = new Mat22(
                (float)Math.Cos(t.Angle),
                (float)Math.Sin(t.Angle),
                (float)-Math.Sin(t.Angle),
                (float)Math.Cos(t.Angle));

            var forward = rotationMatrix.Solve(-Vector2.UnitY);
            preyBody.ApplyForce(forward * 2);

            var left = rotationMatrix.Solve(Vector2.UnitX);

            angularVelocity += random.Next(-ANGULAR_VELOCITY_CHANGE, ANGULAR_VELOCITY_CHANGE);
            if (angularVelocity > MAX_ANGULAR_VELOCITY)
                angularVelocity = MAX_ANGULAR_VELOCITY;
            if (angularVelocity < MIN_ANGULAR_VELOCITY)
                angularVelocity = MIN_ANGULAR_VELOCITY;

            preyBody.ApplyForce(left * angularVelocity);

            return GetRotation(preyBody);
        }

        /// <summary>
        /// Rotate visible representation to match direction of current velocity
        /// </summary>
        private static Matrix? GetRotation(Body preyBody)
        {
            //var velocity = preyBody.LinearVelocity;
            //if (velocity.LengthSquared() > 1 * 1)
            //{
            //    velocity.Normalize();
            //    var rotation = (float)Math.Atan2(velocity.Y, velocity.X) + MathHelper.PiOver2;

            //    return Matrix.CreateRotationZ(rotation);
            //}

            //return null;

            return Matrix.CreateRotationZ(preyBody.Rotation);
        }

        public static Body InitializePreyBody(World world, int preySize, int preyMass)
        {
            var body = BodyFactory.CreateCircle(world, preySize, 1);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 0.15f;
            body.AngularDamping = 0.15f;
            body.Restitution = 0;

            body.Mass = preyMass;
            return body;
        }

        public static void ForceMoveIfStuck(Queue<Tuple<double, Vector2>> PositionalHistory, FarseerPhysics.Dynamics.Body Body)
        {
            PositionalHistory.Enqueue(new Tuple<double, Vector2>(TimeManager.CurrentTime,
                                                        new Vector2(Body.Position.X, Body.Position.Y)));

            bool positionWasSet = false;
            Vector2 oldPosition = Vector2.One;
            while (PositionalHistory.Peek().Item1 + TIME_TO_KEEP_HISTORY_FOR < TimeManager.CurrentTime)
            {
                oldPosition = PositionalHistory.Dequeue().Item2;
                positionWasSet = true;
            }

            if (positionWasSet &&
                (oldPosition - Body.Position).Length() < MIN_DISTANCE_REQUIRED)
            {
                Body.SetTransform(Body.Position, Body.Rotation + (float)Math.PI);
                PositionalHistory.Clear();
            }

        }
    }
}
