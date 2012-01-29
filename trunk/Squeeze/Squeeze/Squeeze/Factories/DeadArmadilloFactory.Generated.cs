using Squeeze.Entities;
using System;
using FlatRedBall.Math;
using FlatRedBall.Graphics;
using Squeeze.Performance;

namespace Squeeze.Factories
{
	public static class DeadArmadilloFactory
	{
		static string mContentManagerName;
		static PositionedObjectList<DeadArmadillo> mScreenListReference;
		static PoolList<DeadArmadillo> mPool = new PoolList<DeadArmadillo>();
		public static Action<DeadArmadillo> EntitySpawned;
		public static DeadArmadillo CreateNew ()
		{
			return CreateNew(null);
		}
		public static DeadArmadillo CreateNew (Layer layer)
		{
			if (string.IsNullOrEmpty(mContentManagerName))
			{
				throw new System.Exception("You must first initialize the factory to use it.");
			}
			DeadArmadillo instance = null;
			instance = new DeadArmadillo(mContentManagerName, false);
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
		
		public static void Initialize (PositionedObjectList<DeadArmadillo> listFromScreen, string contentManager)
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
				DeadArmadillo instance = new DeadArmadillo(mContentManagerName, false);
				mPool.AddToPool(instance);
			}
		}
		
		public static void MakeUnused (DeadArmadillo objectToMakeUnused)
		{
			MakeUnused(objectToMakeUnused, true);
		}
		
		public static void MakeUnused (DeadArmadillo objectToMakeUnused, bool callDestroy)
		{
			objectToMakeUnused.Destroy();
		}
		
		
	}
}
