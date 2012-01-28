using Squeeze.Entities;
using System;
using FlatRedBall.Math;
using FlatRedBall.Graphics;
using Squeeze.Performance;

namespace Squeeze.Factories
{
	public static class CreatureBodyFactory
	{
		static string mContentManagerName;
		static PositionedObjectList<CreatureBody> mScreenListReference;
		static PoolList<CreatureBody> mPool = new PoolList<CreatureBody>();
		public static Action<CreatureBody> EntitySpawned;
		public static CreatureBody CreateNew ()
		{
			return CreateNew(null);
		}
		public static CreatureBody CreateNew (Layer layer)
		{
			if (string.IsNullOrEmpty(mContentManagerName))
			{
				throw new System.Exception("You must first initialize the factory to use it.");
			}
			CreatureBody instance = null;
			instance = new CreatureBody(mContentManagerName, false);
			instance.AddToManagers(layer);
			if (mScreenListReference != null)
			{
				mScreenListReference.Add(instance);
			}
			if (EntitySpawned != null)
			{
				EntitySpawned(instance);
			}
			return instance;
		}
		
		public static void Initialize (PositionedObjectList<CreatureBody> listFromScreen, string contentManager)
		{
			mContentManagerName = contentManager;
			mScreenListReference = listFromScreen;
		}
		
		public static void Destroy ()
		{
			mContentManagerName = null;
			mScreenListReference = null;
			mPool.Clear();
			EntitySpawned = null;
		}
		
		private static void FactoryInitialize ()
		{
			const int numberToPreAllocate = 20;
			for (int i = 0; i < numberToPreAllocate; i++)
			{
				CreatureBody instance = new CreatureBody(mContentManagerName, false);
				mPool.AddToPool(instance);
			}
		}
		
		public static void MakeUnused (CreatureBody objectToMakeUnused)
		{
			MakeUnused(objectToMakeUnused, true);
		}
		
		public static void MakeUnused (CreatureBody objectToMakeUnused, bool callDestroy)
		{
			objectToMakeUnused.Destroy();
		}
		
		
	}
}
