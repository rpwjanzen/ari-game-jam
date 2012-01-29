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
using FarseerPhysics.Factories;
using Squeeze.Factories;
using Microsoft.Xna.Framework;
using FlatRedBall.Math;
using Squeeze.Screens;


#endif

namespace Squeeze.Entities
{
	public partial class DoodadGenerator
	{
        private static readonly PositionedObjectList<Log> g_logs = new PositionedObjectList<Log>();
        private static readonly PositionedObjectList<TreeZero> g_treeZeros = new PositionedObjectList<TreeZero>();
        private static readonly PositionedObjectList<TreeOne> g_treeOnes = new PositionedObjectList<TreeOne>();
        private static readonly PositionedObjectList<StumpOne> g_stumpOnes = new PositionedObjectList<StumpOne>();
        private static readonly PositionedObjectList<StumpZero> g_stumpZeros = new PositionedObjectList<StumpZero>();

        private const int m_logCount = 25;
        private const int m_treeZeroCount = 10;
        private const int m_treeOneCount = 10;
        private const int m_stumpZeroCount = 10;
        private const int m_stumpOneCount = 10;

        private World m_world;

        private Random m_random = new Random(271);

        int levelWidth = 3200;
        int levelHeight = 9600;

		private void CustomInitialize()
		{
            m_world = FarseerPhysicsEntity.World;

            AddLogs(m_logCount);
            AddTreeZeros(m_treeZeroCount);
            AddTreeOnes(m_treeOneCount);
            AddStumpZeros(m_stumpZeroCount);
            AddStumpOnes(m_stumpOneCount);
		}

        private void AddStumpOnes(int stumpOneCount)
        {
            for (int i = 0; i < stumpOneCount; i++)
            {
                float radius = 25;
                float rotation = (float)(m_random.NextDouble() * Math.PI);

                Body body = BodyFactory.CreateCircle(m_world, radius, 1);
                body.BodyType = BodyType.Static;
                var position = GetRandomPosition();
                body.SetTransform(position, rotation);

                var log = StumpOneFactory.CreateNew();
                log.Body = body;
            }
        }

        private void AddStumpZeros(int stumpZeroCount)
        {
            for (int i = 0; i < stumpZeroCount; i++)
            {
                float radius = 25;
                float rotation = (float)(m_random.NextDouble() * Math.PI);

                Body body = BodyFactory.CreateCircle(m_world, radius, 1);
                body.BodyType = BodyType.Static;
                var position = GetRandomPosition();
                body.SetTransform(position, rotation);

                var log = StumpZeroFactory.CreateNew();
                log.Body = body;
            }
        }

        private void AddTreeOnes(int treeCount)
        {
            for (int i = 0; i < treeCount; i++)
            {
                float radius = 80;
                float rotation = (float)(m_random.NextDouble() * Math.PI);

                Body body = BodyFactory.CreateCircle(m_world, radius, 1);
                body.BodyType = BodyType.Static;
                var position = GetRandomPosition();
                body.SetTransform(position, rotation);

                var log = TreeOneFactory.CreateNew();
                log.Body = body;
            }
        }

        private void AddTreeZeros(int treeCount)
        {
            for (int i = 0; i < treeCount; i++)
            {
                float radius = 75;
                float rotation = (float)(m_random.NextDouble() * Math.PI);

                Body body = BodyFactory.CreateCircle(m_world, radius, 1);
                body.BodyType = BodyType.Static;
                var position = GetRandomPosition();
                body.SetTransform(position, rotation);

                var log = TreeZeroFactory.CreateNew();
                log.Body = body;
            }
        }

        private void AddLogs(int logCount)
        {
            for (int i = 0; i < logCount; i++)
            {
                float width = 40;
                float height = 240;
                float rotation = 0;

                bool isHorizontal = i % 2 == 0;
                Body body;
                if (isHorizontal)
                    rotation = (float)(Math.PI / 2.0);

                body = BodyFactory.CreateRectangle(m_world, width, height, 1);
                body.BodyType = BodyType.Static;
                var position = GetRandomPosition();
                body.SetTransform(position, rotation);

                var log = LogFactory.CreateNew();
                log.Body = body;
            }
        }

        private Vector2 GetRandomPosition()
        {
            int x = m_random.Next(0, levelWidth);
            int y = m_random.Next(-levelHeight, 0);

            return new Vector2(x, y);
        }

        bool firstTimeCalled = false;

		private void CustomActivity()
		{
            if (!firstTimeCalled)
            {
                foreach (var log in g_logs)
                    log.Activity();

                foreach (var tree in g_treeOnes)
                    tree.Activity();

                foreach (var tree in g_treeZeros)
                    tree.Activity();

                foreach (var stump in g_stumpZeros)
                    stump.Activity();

                foreach (var stump in g_stumpOnes)
                    stump.Activity();

                firstTimeCalled = true;
            }
		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {
            LogFactory.Initialize(g_logs, contentManagerName);
            TreeZeroFactory.Initialize(g_treeZeros, contentManagerName);
            TreeOneFactory.Initialize(g_treeOnes, contentManagerName);

            StumpOneFactory.Initialize(g_stumpOnes, contentManagerName);
            StumpZeroFactory.Initialize(g_stumpZeros, contentManagerName);
        }
	}
}
