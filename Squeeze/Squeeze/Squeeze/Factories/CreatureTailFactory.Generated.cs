using Squeeze.Entities;
using System;
using FlatRedBall.Math;
using FlatRedBall.Graphics;
using Squeeze.Performance;

namespace Squeeze.Factories
{
	public static class CreatureTailFactory
	{
		static string mContentManagerName;
		static PositionedObjectList<CreatureTail> mScreenListReference;
		static PoolList<CreatureTail> mPool = new PoolList<CreatureTail>();
		public static Action<CreatureTail> EntitySpawned;
		public static CreatureTail CreateNew ()
		{
			return CreateNew(null);
		}
		public static CreatureTail CreateNew (Layer layer)
		{
			if (string.IsNullOrEmpty(mContentManagerName))
			{
				throw new System.Exception("You must first initialize the factory to use it.");
			}
			CreatureTail instance = null;
			instance = new CreatureTail(mContentManagerName, false);
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
		
		public static void Initialize (PositionedObjectList<CreatureTail> listFromScreen, string contentManager)
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
				CreatureTail instance = new CreatureTail(mContentManagerName, false);
				mPool.AddToPool(instance);
			}
		}
		
		public static void MakeUnused (CreatureTail objectToMakeUnused)
		{
			MakeUnused(objectToMakeUnused, true);
		}
		
		public static void MakeUnused (CreatureTail objectToMakeUnused, bool callDestroy)
		{
			objectToMakeUnused.Destroy();
		}
		
		
	}
}
