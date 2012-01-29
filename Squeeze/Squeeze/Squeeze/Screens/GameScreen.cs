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
using Squeeze.Entities;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Squeeze.Factories;
using FlatRedBall.Math;
#endif

namespace Squeeze.Screens
{
	public partial class GameScreen
	{
        private PositionedObject cameraCenter = new PositionedObject();

        private int m_logCount = 100;

        private World m_world;

        private Random m_random = new Random(271);

        int levelWidth = 3200;
        int levelHeight = 9600;
        float borderWidth = 50f;

        private static readonly PositionedObjectList<Log> g_logs = new PositionedObjectList<Log>();

		void CustomInitialize()
		{
            SpriteManager.Camera.UsePixelCoordinates();
            SpriteManager.Camera.AttachTo(cameraCenter, true);
            m_world = FarseerPhysicsEntity.World;
            
            AddLogs(m_logCount);
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
                    rotation = (float)Math.PI;

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

        void CustomActivity(bool firstTimeCalled)
		{
            cameraCenter.Position = CreatureInstance.Centroid;

            // Floor so that we do not have flickering lines between map tiles.
            cameraCenter.Position.X = (float)Math.Floor(cameraCenter.Position.X);
            cameraCenter.Position.Y = (float)Math.Floor(cameraCenter.Position.Y);
            cameraCenter.Position.Z = (float)Math.Floor(cameraCenter.Position.Z);
            
            if (firstTimeCalled)
            {
                foreach (var log in g_logs)
                    log.Activity();
            }
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {
            LogFactory.Initialize(g_logs, contentManagerName);
        }
	}
}
