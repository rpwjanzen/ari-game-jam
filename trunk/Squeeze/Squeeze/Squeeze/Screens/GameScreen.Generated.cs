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
		private Squeeze.Entities.PreyGenerator PreyGeneratorInstance;
		private Squeeze.Entities.LevelOne LevelOneInstance;
		private Squeeze.Entities.ArmadilloGenerator ArmadilloGeneratorInstance;
		private Squeeze.Entities.DoodadGenerator DoodadGeneratorInstance;
		private Squeeze.Entities.DeadPreyGenerator DeadPreyGeneratorInstance;
		private Squeeze.Entities.BufalloGenerator BuffaloGeneratorInstance;
		private Squeeze.Entities.DeadArmadilloGenerator DeadArmadilloGeneratorInstance;
		private Squeeze.Entities.DeadBuffaloGenerator DeadBuffaloGeneratorInstance;

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
			CreatureInstance = new Squeeze.Entities.Creature(ContentManagerName, false);
			CreatureInstance.Name = "CreatureInstance";
			FarseerPhysicsEntityInstance = new Squeeze.Entities.FarseerPhysicsEntity(ContentManagerName, false);
			FarseerPhysicsEntityInstance.Name = "FarseerPhysicsEntityInstance";
			PreyGeneratorInstance = new Squeeze.Entities.PreyGenerator(ContentManagerName, false);
			PreyGeneratorInstance.Name = "PreyGeneratorInstance";
			LevelOneInstance = new Squeeze.Entities.LevelOne(ContentManagerName, false);
			LevelOneInstance.Name = "LevelOneInstance";
			ArmadilloGeneratorInstance = new Squeeze.Entities.ArmadilloGenerator(ContentManagerName, false);
			ArmadilloGeneratorInstance.Name = "ArmadilloGeneratorInstance";
			DoodadGeneratorInstance = new Squeeze.Entities.DoodadGenerator(ContentManagerName, false);
			DoodadGeneratorInstance.Name = "DoodadGeneratorInstance";
			DeadPreyGeneratorInstance = new Squeeze.Entities.DeadPreyGenerator(ContentManagerName, false);
			DeadPreyGeneratorInstance.Name = "DeadPreyGeneratorInstance";
			BuffaloGeneratorInstance = new Squeeze.Entities.BufalloGenerator(ContentManagerName, false);
			BuffaloGeneratorInstance.Name = "BuffaloGeneratorInstance";
			DeadArmadilloGeneratorInstance = new Squeeze.Entities.DeadArmadilloGenerator(ContentManagerName, false);
			DeadArmadilloGeneratorInstance.Name = "DeadArmadilloGeneratorInstance";
			DeadBuffaloGeneratorInstance = new Squeeze.Entities.DeadBuffaloGenerator(ContentManagerName, false);
			DeadBuffaloGeneratorInstance.Name = "DeadBuffaloGeneratorInstance";
			
			
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
				PreyGeneratorInstance.Activity();
				LevelOneInstance.Activity();
				ArmadilloGeneratorInstance.Activity();
				DoodadGeneratorInstance.Activity();
				DeadPreyGeneratorInstance.Activity();
				BuffaloGeneratorInstance.Activity();
				DeadArmadilloGeneratorInstance.Activity();
				DeadBuffaloGeneratorInstance.Activity();
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
			if (PreyGeneratorInstance != null)
			{
				PreyGeneratorInstance.Destroy();
			}
			if (LevelOneInstance != null)
			{
				LevelOneInstance.Destroy();
			}
			if (ArmadilloGeneratorInstance != null)
			{
				ArmadilloGeneratorInstance.Destroy();
			}
			if (DoodadGeneratorInstance != null)
			{
				DoodadGeneratorInstance.Destroy();
			}
			if (DeadPreyGeneratorInstance != null)
			{
				DeadPreyGeneratorInstance.Destroy();
			}
			if (BuffaloGeneratorInstance != null)
			{
				BuffaloGeneratorInstance.Destroy();
			}
			if (DeadArmadilloGeneratorInstance != null)
			{
				DeadArmadilloGeneratorInstance.Destroy();
			}
			if (DeadBuffaloGeneratorInstance != null)
			{
				DeadBuffaloGeneratorInstance.Destroy();
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
			PreyGeneratorInstance.AddToManagers(mLayer);
			LevelOneInstance.AddToManagers(mLayer);
			ArmadilloGeneratorInstance.AddToManagers(mLayer);
			DoodadGeneratorInstance.AddToManagers(mLayer);
			DeadPreyGeneratorInstance.AddToManagers(mLayer);
			BuffaloGeneratorInstance.AddToManagers(mLayer);
			DeadArmadilloGeneratorInstance.AddToManagers(mLayer);
			DeadBuffaloGeneratorInstance.AddToManagers(mLayer);
		}
		public virtual void ConvertToManuallyUpdated ()
		{
			BackgroundSpriteGrid.ConvertToManuallyUpdated();
			CreatureInstance.ConvertToManuallyUpdated();
			FarseerPhysicsEntityInstance.ConvertToManuallyUpdated();
			PreyGeneratorInstance.ConvertToManuallyUpdated();
			LevelOneInstance.ConvertToManuallyUpdated();
			ArmadilloGeneratorInstance.ConvertToManuallyUpdated();
			DoodadGeneratorInstance.ConvertToManuallyUpdated();
			DeadPreyGeneratorInstance.ConvertToManuallyUpdated();
			BuffaloGeneratorInstance.ConvertToManuallyUpdated();
			DeadArmadilloGeneratorInstance.ConvertToManuallyUpdated();
			DeadBuffaloGeneratorInstance.ConvertToManuallyUpdated();
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
			Squeeze.Entities.PreyGenerator.LoadStaticContent(contentManagerName);
			Squeeze.Entities.LevelOne.LoadStaticContent(contentManagerName);
			Squeeze.Entities.ArmadilloGenerator.LoadStaticContent(contentManagerName);
			Squeeze.Entities.DoodadGenerator.LoadStaticContent(contentManagerName);
			Squeeze.Entities.DeadPreyGenerator.LoadStaticContent(contentManagerName);
			Squeeze.Entities.BufalloGenerator.LoadStaticContent(contentManagerName);
			Squeeze.Entities.DeadArmadilloGenerator.LoadStaticContent(contentManagerName);
			Squeeze.Entities.DeadBuffaloGenerator.LoadStaticContent(contentManagerName);
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
