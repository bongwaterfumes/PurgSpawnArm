using HarmonyLib;
using PurgatorioCyberGrind.Systems;
using PurgSpawnArm;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static PurgatorioCyberGrind.Systems.CustomCyberGrindEntry;
using static UnityEngine.UIElements.UIR.Implementation.UIRStylePainter;

namespace PurgatorioCyberGrind.Patches
{
	[HarmonyPatch(typeof(EndlessGrid))]
	public static class EndlessGridPatch
	{
		private static bool instanced = false;

		public const int totalVanillaEnemies = 38; //Total EnemyTypes, will most likely get outdated once Fraud releases

		//Add Enemies to the cybergrind enemy pool
		[HarmonyPatch("Start"), HarmonyPostfix]
		public static void EndlessGrid_Start(EndlessGrid __instance, ref PrefabDatabase ___prefabs)
		{
			if (!instanced)
			{
				instanced = true;
				if (CybergrindEntryLoader.entryCount != totalVanillaEnemies)
				{
					for (int i = totalVanillaEnemies; i < CybergrindEntryLoader.entryCount; i++)
					{
						EndlessEnemy[] meleeEnemies = ___prefabs.meleeEnemies;
						EndlessEnemy[] projectileEnemies = ___prefabs.projectileEnemies;
						EndlessEnemy[] uncommonEnemies = ___prefabs.uncommonEnemies;
						EndlessEnemy[] specialEnemies = ___prefabs.specialEnemies;
						int meleeCount = meleeEnemies.Length;
						int projectileCount = projectileEnemies.Length;
						int uncommonCount = uncommonEnemies.Length;
						int specialCount = specialEnemies.Length;

						CustomCyberGrindEntry entry = CybergrindEntryLoader.GetCustomEntry(i);
						if (entry.AddedToTheCybergrind())
						{
							SpawnTypePosition spawnType = entry.SetTypePosition();

							int length = spawnType.spawnType switch
							{
								CybergrindSpawnType.melee => meleeCount + 1,
								CybergrindSpawnType.projectile => projectileCount + 1,
								CybergrindSpawnType.uncommon => uncommonCount + 1,
								CybergrindSpawnType.special => specialCount + 1,
								_ => 0,
							};

							EndlessEnemy[] newEnemyEntry = new EndlessEnemy[length];
							int l = 0;
							for (int j = 0; j < length; j++)
							{
								if (spawnType.spawnTypeIndex + 1 != j)
								{
									newEnemyEntry[j] = spawnType.spawnType switch
									{
										CybergrindSpawnType.melee => meleeEnemies[l],
										CybergrindSpawnType.projectile => projectileEnemies[l],
										CybergrindSpawnType.uncommon => uncommonEnemies[l],
										CybergrindSpawnType.special => specialEnemies[l],
										_ => meleeEnemies[l],
									};
									l++;
								}
								else
								{
									entry.SetEntrySettings(out int spawnCost, out int costIncreasePerSpawn, out int spawnWave, out GameObject prefab);
									EndlessEnemy val = ScriptableObject.CreateInstance<EndlessEnemy>();
									val.name = entry.GetType().Name + "EndlessData";
									val.spawnCost = spawnCost;
									val.spawnWave = spawnWave;
									val.costIncreasePerSpawn = costIncreasePerSpawn;
									val.enemyType = (EnemyType)i;
									val.prefab = prefab;

									newEnemyEntry[j] = val;
								}
							}

							switch (spawnType.spawnType)
							{
								case CybergrindSpawnType.melee:
									___prefabs.meleeEnemies = newEnemyEntry;
									break;
								case CybergrindSpawnType.projectile:
									___prefabs.projectileEnemies = newEnemyEntry;
									break;
								case CybergrindSpawnType.uncommon:
									___prefabs.uncommonEnemies = newEnemyEntry;
									break;
								case CybergrindSpawnType.special:
									___prefabs.specialEnemies = newEnemyEntry;
									break;
							}
							BepInEx.Logging.Logger.CreateLogSource("Purg Spawn Arm").Log(BepInEx.Logging.LogLevel.Info, "Added " + entry.GetType().Name + " to the cybergrind");
						}
					}
				}
				//SpawnMenu.instance.CreateButtons(SpawnMenu.instance.objects.enemies, "ENEMIES");
				//LogCybergrindEnemyEntries(___prefabs);
			}
		}

		//Caps uncommon enemies
		[HarmonyPatch("CapUncommonsAmount"), HarmonyPostfix]
		public static int EndlessGrid_CapUncommonsAmount(int target, int amount, EndlessGrid __instance, ref int __result)
		{
			if (__instance.prefabs.uncommonEnemies[target].enemyType >= (EnemyType)totalVanillaEnemies)
			{
				return CybergrindEntryLoader.GetCustomEntry((int)__instance.prefabs.uncommonEnemies[target].enemyType).CapNonCommonEnemyAmount(__instance.currentWave, amount);
			}
			return __result;
		}

		/// <summary>
		/// Logs the current filled cybergrind entires, useful for whenever Hakita adds new enemies to the entries without needing to dig through the uk assets
		/// </summary>
		/// <param name="prefabs"></param>
		private static void LogCybergrindEnemyEntries(PrefabDatabase prefabs)
		{
			BepInEx.Logging.ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("Purg Spawn Arm");

			log.Log(BepInEx.Logging.LogLevel.Info, "The following logs are each cybergrind type and their enemies listed");
			for (int i = 0; i < prefabs.meleeEnemies.Length; i++)
				BepInEx.Logging.Logger.CreateLogSource("Purg Spawn Arm").Log(BepInEx.Logging.LogLevel.Info, "melee " + i + " type: " + prefabs.meleeEnemies[i].enemyType);
			for (int i = 0; i < prefabs.projectileEnemies.Length; i++)
				BepInEx.Logging.Logger.CreateLogSource("Purg Spawn Arm").Log(BepInEx.Logging.LogLevel.Info, "projectile " + i + " type: " + prefabs.projectileEnemies[i].enemyType);
			for (int i = 0; i < prefabs.uncommonEnemies.Length; i++)
				BepInEx.Logging.Logger.CreateLogSource("Purg Spawn Arm").Log(BepInEx.Logging.LogLevel.Info, "uncommon " + i + " type: " + prefabs.uncommonEnemies[i].enemyType);
			for (int i = 0; i < prefabs.specialEnemies.Length; i++)
				BepInEx.Logging.Logger.CreateLogSource("Purg Spawn Arm").Log(BepInEx.Logging.LogLevel.Info, "special " + i + " type: " + prefabs.specialEnemies[i].enemyType);
		}

		//Add a cap to special enemies
		[HarmonyPatch("GetEnemies"), HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> EndlessGrid_GetEnemies(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher codeMatcher = new CodeMatcher(instructions);

			codeMatcher
				.Start()
				.MatchForward(false,
					new CodeMatch(OpCodes.Ldfld, typeof(PrefabDatabase).GetField(nameof(PrefabDatabase.specialEnemies))),
					new CodeMatch(OpCodes.Ldloc_S),
					new CodeMatch(OpCodes.Ldelem_Ref),
					new CodeMatch(OpCodes.Ldfld, typeof(EndlessGrid).GetField(nameof(EndlessEnemy.spawnCost))),
					new CodeMatch(OpCodes.Conv_R4),
					new CodeMatch(OpCodes.Ldloc_S),
					new CodeMatch(OpCodes.Add),
					new CodeMatch(OpCodes.Blt_Un))
				.ThrowIfInvalid("Could not locate initial special enemies spawn cost check")
				.MatchForward(true,
					new CodeMatch(OpCodes.Ldfld, typeof(PrefabDatabase).GetField(nameof(PrefabDatabase.specialEnemies))),
					new CodeMatch(OpCodes.Ldloc_S))
				.ThrowIfInvalid("Could not locate the getting of the special enemies index loc varnum");
			var locIndexSpecialEnemyIndex = codeMatcher.Operand;
			codeMatcher
				.MatchForward(true,
					new CodeMatch(OpCodes.Ldelem_Ref),
					new CodeMatch(OpCodes.Ldfld, typeof(EndlessGrid).GetField(nameof(EndlessEnemy.spawnCost))),
					new CodeMatch(OpCodes.Conv_R4),
					new CodeMatch(OpCodes.Ldloc_S),
					new CodeMatch(OpCodes.Add),
					new CodeMatch(OpCodes.Blt_Un))
				.ThrowIfInvalid("Could not locate the blt.un jump instruction after the special enemies spawn cost check");
			var IL_054e = codeMatcher.Operand;
			codeMatcher.Advance(1).Insert(
					new CodeInstruction(OpCodes.Ldloc, locIndexSpecialEnemyIndex),
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Call, typeof(EndlessGridPatch).GetMethod(nameof(GetEnemiesCapSpecials), BindingFlags.Static | BindingFlags.NonPublic)),
					new CodeInstruction(OpCodes.Brfalse, IL_054e))
				.ThrowIfInvalid("Could not inject additional special enemies check");

			return codeMatcher.InstructionEnumeration();
		}

		private static bool GetEnemiesCapSpecials(int num8, EndlessGrid self)
		{
			int indexOfEnemyType = self.GetIndexOfEnemyType(self.prefabs.specialEnemies[num8].enemyType);
			return CapSpecialsAmount(num8, self.spawnedEnemyTypes[indexOfEnemyType].amount, ref self.prefabs, self.currentWave) >= self.spawnedEnemyTypes[indexOfEnemyType].amount;
		}

		public static int CapSpecialsAmount(int target, int amount, ref PrefabDatabase prefabs, int currentWave)
		{
			if (prefabs.specialEnemies[target].enemyType >= (EnemyType)totalVanillaEnemies)
			{
				return CybergrindEntryLoader.GetCustomEntry((int)prefabs.specialEnemies[target].enemyType).CapNonCommonEnemyAmount(currentWave, amount);
			}
			return amount;
		}
	}
}
