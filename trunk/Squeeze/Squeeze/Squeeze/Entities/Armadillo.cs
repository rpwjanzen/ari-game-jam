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
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;


#endif

namespace Squeeze.Entities
{
	public partial class Armadillo
	{

        public Body Body { get; private set; }

        private World m_world;

        private int m_angularVelocity = 0;

        private Random m_random = new Random();

        // do this so that pray which spawn at the same time have different seeds.
        public void SetupRandomGenerator(int seed)
        {
            m_random = new Random(seed);
        }

		private void CustomInitialize()
        {
            m_world = FarseerPhysicsEntity.World;

		    Body = PreyBehaviour.InitializePreyBody(m_world, 32, 20);
        }

		private void CustomActivity()
		{
            this.RotationMatrix = PreyBehaviour.MovePrey(
                this,
                Body,
                ref m_angularVelocity,
                m_random);
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
