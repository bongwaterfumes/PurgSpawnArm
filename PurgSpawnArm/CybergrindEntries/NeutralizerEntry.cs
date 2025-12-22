using PurgatorioCyberGrind.Systems;
using PurgSpawnArm;
using UnityEngine;

namespace PurgatorioCyberGrind.CybergrindEntries
{
	public class NeutralizerEntry : CustomCyberGrindEntry
	{
		public override bool AddedToTheCybergrind()
		{
			return Plugin.NeutralizerInCybergrind.value;
		}

		public override void SetEntrySettings(out int spawnCost, out int costIncreasePerSpawn, out int spawnWave, out GameObject prefab)
		{
			spawnCost = 55;
			costIncreasePerSpawn = 0; //Capped at 1 reguardless
			spawnWave = 19;
			prefab = Plugin.bundle.LoadAsset<GameObject>("Neutralizer");
		}

		public override SpawnTypePosition SetTypePosition()
		{
			return new BeforeAllEnemies(CybergrindSpawnType.special);
		}

		public override int CapNonCommonEnemyAmount(int currentWave, int enemyAmount)
		{
			return 1;
		}
	}
}
