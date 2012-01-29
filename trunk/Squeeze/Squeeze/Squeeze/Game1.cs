using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Utilities;

using Squeeze.Screens;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics;

namespace Squeeze
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        
        public static World World = new World(Vector2.Zero);
        public static DebugViewXNA DebugViewXNA;

        private static SoundEffectInstance m_backgroundMusic;

        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
			
			BackStack<string> bs = new BackStack<string>();
			bs.Current = string.Empty;

            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            DebugViewXNA = new DebugViewXNA(World);
            //DebugViewXNA.AppendFlags(DebugViewFlags.DebugPanel);
            DebugViewXNA.AppendFlags(DebugViewFlags.Shape);
            //DebugViewXNA.AppendFlags(DebugViewFlags.DebugPanel);
            //DebugViewXNA.AppendFlags(DebugViewFlags.PerformanceGraph);
            DebugViewXNA.AppendFlags(DebugViewFlags.Joint);
            DebugViewXNA.AppendFlags(DebugViewFlags.ContactPoints);
            DebugViewXNA.AppendFlags(DebugViewFlags.ContactNormals);
            DebugViewXNA.AppendFlags(DebugViewFlags.Controllers);
            DebugViewXNA.AppendFlags(DebugViewFlags.CenterOfMass);
            DebugViewXNA.AppendFlags(DebugViewFlags.AABB);
            DebugViewXNA.DefaultShapeColor = Color.White;
            DebugViewXNA.SleepingShapeColor = Color.LightGray;
            DebugViewXNA.LoadContent(GraphicsDevice, Content);

            SoundEffect _backgroundMusicSoundEffect = FlatRedBallServices.Load<SoundEffect>("Content\\SnakeBGMusic");
            m_backgroundMusic = _backgroundMusicSoundEffect.CreateInstance();
            m_backgroundMusic.Play();

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Renderer.UseRenderTargets = false;
            FlatRedBallServices.InitializeFlatRedBall(this, graphics);
			GlobalContent.Initialize();

			Screens.ScreenManager.Start(typeof(Squeeze.Screens.GameScreen).FullName);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            m_backgroundMusic.Play();
            FlatRedBallServices.Update(gameTime);

            ScreenManager.Activity();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            FlatRedBallServices.Draw();

            // draw the debug view
            var vp = SpriteManager.Camera.ViewProjection;
            DebugViewXNA.RenderDebugData(ref vp);

            base.Draw(gameTime);
        }
    }
}
