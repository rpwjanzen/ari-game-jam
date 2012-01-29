using Squeeze.Entities;
using System;
using FlatRedBall.Math;
using FlatRedBall.Graphics;
using Squeeze.Performance;

namespace Squeeze.Factories
{
	public static class LogFactory
	{
		static string mContentManagerName;
		static PositionedObjectList<Log> mScreenListReference;
		static PoolList<Log> mPool = new PoolList<Log>();
		public static Action<Log> EntitySpawned;
		public static Log CreateNew ()
		{
			return CreateNew(null);
		}
		public static Log CreateNew (Layer layer)
		{
			if (string.IsNullOrEmpty(mContentManagerName))
			{
				throw new System.Exception("You must first initialize the factory to use it.");
			}
			Log instance = null;
			instance = new Log(mContentManagerName, false);
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
		
		public static void Initialize (PositionedObjectList<Log> listFromScreen, string contentManager)
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
				Log instance = new Log(mContentManagerName, false);
				mPool.AddToPool(instance);
			}
		}
		
		public static void MakeUnused (Log objectToMakeUnused)
		{
			MakeUnused(objectToMakeUnused, true);
		}
		
		public static void MakeUnused (Log objectToMakeUnused, bool callDestroy)
		{
			objectToMakeUnused.Destroy();
		}
		
		
	}
}
