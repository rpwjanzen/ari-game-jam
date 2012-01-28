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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
			
			BackStack<string> bs = new BackStack<string>();
			bs.Current = string.Empty;

            DebugViewXNA = new DebugViewXNA(World);
            DebugViewXNA.AppendFlags(DebugViewFlags.DebugPanel);
            DebugViewXNA.DefaultShapeColor = Color.White;
            DebugViewXNA.SleepingShapeColor = Color.LightGray;

            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Renderer.UseRenderTargets = false;
            FlatRedBallServices.InitializeFlatRedBall(this, graphics);
			GlobalContent.Initialize();

			Screens.ScreenManager.Start(typeof(Squeeze.Screens.GameScreen).FullName);

            DebugViewXNA.LoadContent(graphics.GraphicsDevice, Content);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            FlatRedBallServices.Update(gameTime);

            ScreenManager.Activity();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            FlatRedBallServices.Draw();

            base.Draw(gameTime);
        }
    }
}
