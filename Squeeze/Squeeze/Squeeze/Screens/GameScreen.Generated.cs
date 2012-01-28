using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall.Math.Geometry;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Input;
using FlatRedBall.IO;
using FlatRedBall.Instructions;
using FlatRedBall.Math.Splines;
using FlatRedBall.Utilities;
using BitmapFont = FlatRedBall.Graphics.BitmapFont;

using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;

#if XNA4
using Color = Microsoft.Xna.Framework.Color;
#else
using Color = Microsoft.Xna.Framework.Graphics.Color;
#endif

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using Microsoft.Xna.Framework.Media;
#endif

// Generated Usings
using FlatRedBall.Broadcasting;
using Squeeze.Entities;
using Squeeze.Factories;
using FlatRedBall;
using FlatRedBall;

namespace Squeeze.Screens
{
	public partial class GameScreen : Screen
	{
		// Generated Fields
		#if DEBUG
		static bool HasBeenLoadedWithGlobalContentManager = false;
		#endif
		private Scene BackgroundSpriteGrid;
		
		private Squeeze.Entities.Creature CreatureInstance;
		private Squeeze.Entities.FarseerPhysicsEntity FarseerPhysicsEntityInstance;
		private Squeeze.Entities.Level1Background BackgroundInstance;
		private Squeeze.Entities.PreyGenerator PreyGeneratorInstance;
		private Scene SpriteGridInstance;

		public GameScreen()
			: base("GameScreen")
		{
		}

        public override void Initialize(bool addToManagers)
        {
			// Generated Initialize
			LoadStaticContent(ContentManagerName);
			if (!FlatRedBallServices.IsLoaded<Scene>(@"content/screens/gamescreen/backgroundspritegrid.scnx", ContentManagerName))
			{
			}
			BackgroundSpriteGrid = FlatRedBallServices.Load<Scene>(@"content/screens/gamescreen/backgroundspritegrid.scnx", ContentManagerName);
			SpriteGridInstance = BackgroundSpriteGrid;
			for (int i = 0; i < SpriteGridInstance.Texts.Count; i++)
			{
				SpriteGridInstance.Texts[i].AdjustPositionForPixelPerfectDrawing = true;
			}
			CreatureInstance = new Squeeze.Entities.Creature(ContentManagerName, false);
			CreatureInstance.Name = "CreatureInstance";
			FarseerPhysicsEntityInstance = new Squeeze.Entities.FarseerPhysicsEntity(ContentManagerName, false);
			FarseerPhysicsEntityInstance.Name = "FarseerPhysicsEntityInstance";
			BackgroundInstance = new Squeeze.Entities.Level1Background(ContentManagerName, false);
			BackgroundInstance.Name = "BackgroundInstance";
			PreyGeneratorInstance = new Squeeze.Entities.PreyGenerator(ContentManagerName, false);
			PreyGeneratorInstance.Name = "PreyGeneratorInstance";
			
			
			PostInitialize();
			base.Initialize(addToManagers);
			if (addToManagers)
			{
				AddToManagers();
			}

        }
        
// Generated AddToManagers
		public override void AddToManagers ()
		{
			AddToManagersBottomUp();
			CustomInitialize();
		}


		public override void Activity(bool firstTimeCalled)
		{
			// Generated Activity
			if (!IsPaused)
			{
				
				CreatureInstance.Activity();
				FarseerPhysicsEntityInstance.Activity();
				BackgroundInstance.Activity();
				PreyGeneratorInstance.Activity();
			}
			else
			{
			}
			base.Activity(firstTimeCalled);
			if (!IsActivityFinished)
			{
				CustomActivity(firstTimeCalled);
			}
			BackgroundSpriteGrid.ManageAll();


				// After Custom Activity
				
            
		}

		public override void Destroy()
		{
			// Generated Destroy
			if (this.UnloadsContentManagerWhenDestroyed)
			{
				BackgroundSpriteGrid.RemoveFromManagers(ContentManagerName != "Global");
			}
			else
			{
				BackgroundSpriteGrid.RemoveFromManagers(false);
			}
			
			if (CreatureInstance != null)
			{
				CreatureInstance.Destroy();
			}
			if (FarseerPhysicsEntityInstance != null)
			{
				FarseerPhysicsEntityInstance.Destroy();
			}
			if (BackgroundInstance != null)
			{
				BackgroundInstance.Destroy();
			}
			if (PreyGeneratorInstance != null)
			{
				PreyGeneratorInstance.Destroy();
			}
			if (SpriteGridInstance != null)
			{
				SpriteGridInstance.RemoveFromManagers(ContentManagerName != "Global");
			}

			base.Destroy();

			CustomDestroy();

		}

		// Generated Methods
		public virtual void PostInitialize ()
		{
		}
		public virtual void AddToManagersBottomUp ()
		{
			BackgroundSpriteGrid.AddToManagers(mLayer);
			CreatureInstance.AddToManagers(mLayer);
			FarseerPhysicsEntityInstance.AddToManagers(mLayer);
			BackgroundInstance.AddToManagers(mLayer);
			PreyGeneratorInstance.AddToManagers(mLayer);
		}
		public virtual void ConvertToManuallyUpdated ()
		{
			BackgroundSpriteGrid.ConvertToManuallyUpdated();
			CreatureInstance.ConvertToManuallyUpdated();
			FarseerPhysicsEntityInstance.ConvertToManuallyUpdated();
			BackgroundInstance.ConvertToManuallyUpdated();
			PreyGeneratorInstance.ConvertToManuallyUpdated();
			SpriteGridInstance.ConvertToManuallyUpdated();
		}
		public static void LoadStaticContent (string contentManagerName)
		{
			#if DEBUG
			if (contentManagerName == FlatRedBallServices.GlobalContentManager)
			{
				HasBeenLoadedWithGlobalContentManager = true;
			}
			else if (HasBeenLoadedWithGlobalContentManager)
			{
				throw new Exception("This type has been loaded with a Global content manager, then loaded with a non-global.  This can lead to a lot of bugs");
			}
			#endif
			Squeeze.Entities.Creature.LoadStaticContent(contentManagerName);
			Squeeze.Entities.FarseerPhysicsEntity.LoadStaticContent(contentManagerName);
			Squeeze.Entities.Level1Background.LoadStaticContent(contentManagerName);
			Squeeze.Entities.PreyGenerator.LoadStaticContent(contentManagerName);
			CustomLoadStaticContent(contentManagerName);
		}
		object GetMember (string memberName)
		{
			switch(memberName)
			{
				case  "BackgroundSpriteGrid":
					return BackgroundSpriteGrid;
					break;
			}
			return null;
		}


	}
}
