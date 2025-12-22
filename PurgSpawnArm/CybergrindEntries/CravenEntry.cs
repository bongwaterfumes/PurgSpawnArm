using PurgatorioCyberGrind.Systems;
using PurgSpawnArm;
using UnityEngine;

namespace PurgatorioCyberGrind.CybergrindEntries
{
	public class CravenEntry : CustomCyberGrindEntry
	{
		public override bool AddedToTheCybergrind()
		{
			return Plugin.CravenInCybergrind.value;
		}

		public override void SetEntrySettings(out int spawnCost, out int costIncreasePerSpawn, out int spawnWave, out GameObject prefab)
		{
			spawnCost = 20;
			costIncreasePerSpawn = 15;
			spawnWave = 18;
			prefab = Plugin.bundle.LoadAsset<GameObject>("Craven");
		}

		public override SpawnTypePosition SetTypePosition()
		{
			return new BeforeAllEnemies(CybergrindSpawnType.uncommon);
		}
	}
}
