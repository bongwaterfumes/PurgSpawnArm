using PurgatorioCyberGrind.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PurgatorioCyberGrind.Systems
{
	internal static class CybergrindEntryLoader
	{
		//Special enemy types, here an explaination for why this is needed:
		//EnemyType is used to track how many of a type is currently in the grind and then
		//applying the "costIncreasePerSpawn" for each enemy using that same type. 
		//Using a vanilla enemy can cause less of that enemy to spawn as it will count both
		//the vanilla and modded enemy to multiply the cost increase by.
		//This type also allows us to set custom spawn caps as well.
		//
		//If another mod uses this system, there wont be any breakage, but some of each
		//modded enemy will spawn less.
		internal static int entryCount = EndlessGridPatch.totalVanillaEnemies;

		//List of all instanced entries
		internal static readonly IList<CustomCyberGrindEntry> entries = new List<CustomCyberGrindEntry>();

		internal static int ReserveEntryID()
		{
			int result = entryCount;
			entryCount++;
			return result;
		}

		public static CustomCyberGrindEntry GetCustomEntry(int type)
		{
			return entries[type - EndlessGridPatch.totalVanillaEnemies];
		}

		//As stated in Plugin.Start, this creates an instance of each class inheriting the CCGEntry
		public static void RegisterAllEntries()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				IEnumerable<Type> derivedTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(CustomCyberGrindEntry)) && !type.IsAbstract && !type.IsGenericType);
				foreach (Type type in derivedTypes)
				{
					((CustomCyberGrindEntry)Activator.CreateInstance(type)).Register();
				}
			}
		}
	}
}
