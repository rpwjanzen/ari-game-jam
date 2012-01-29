using Squeeze.Entities;
using System;
using FlatRedBall.Math;
using FlatRedBall.Graphics;
using Squeeze.Performance;

namespace Squeeze.Factories
{
	public static class BuffaloFactory
	{
		static string mContentManagerName;
		static PositionedObjectList<Buffalo> mScreenListReference;
		static PoolList<Buffalo> mPool = new PoolList<Buffalo>();
		public static Action<Buffalo> EntitySpawned;
		public static Buffalo CreateNew ()
		{
			return CreateNew(null);
		}
		public static Buffalo CreateNew (Layer layer)
		{
			if (string.IsNullOrEmpty(mContentManagerName))
			{
				throw new System.Exception("You must first initialize the factory to use it.");
			}
			Buffalo instance = null;
			instance = new Buffalo(mContentManagerName, false);
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
		
		public static void Initialize (PositionedObjectList<Buffalo> listFromScreen, string contentManager)
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
				Buffalo instance = new Buffalo(mContentManagerName, false);
				mPool.AddToPool(instance);
			}
		}
		
		public static void MakeUnused (Buffalo objectToMakeUnused)
		{
			MakeUnused(objectToMakeUnused, true);
		}
		
		public static void MakeUnused (Buffalo objectToMakeUnused, bool callDestroy)
		{
			objectToMakeUnused.Destroy();
		}
		
		
	}
}
