using UnityEngine;

namespace PurgatorioCyberGrind.Systems
{
	/// <summary>
	/// Inherit this class to add custom enemies to the cybergrind.
	/// </summary>
	public abstract class CustomCyberGrindEntry
	{
		/// <summary>
		/// What spawn type the enemy is and where in the spawn type array the enemy should be placed.
		/// <br/> NOTE: this can effect how likely an enemy is chosen, 
		/// <br/> with being higher than other enemies can cause it to be chosen less and being lower than other enemies causing the enemy to be a fallback if others aren't chosen/able to spawn.
		/// <br/> Use AfterSpawnTypeEnemy or BeforeSpawnTypeEnemy to specify where the enemy is placed in the context to the spawn type listed
		/// </summary>
		public abstract class SpawnTypePosition
		{
			public virtual int spawnTypeIndex { get; internal set; }

			public virtual CybergrindSpawnType spawnType { get; internal set; }
		}

		/// <summary>
		/// What spawn type the enemy is and where in the spawn type array the enemy should be placed.
		/// <br/> Used to place the enemy after a vanilla enemy.
		/// <br/> Example: new AfterSpawnTypeEnemy(CybergrindEnemyCatagories.UncommonEnemies.Stalker) will list the enemy as 'Uncommon' and will place the enemy after stalkers in the list
		/// </summary>
		public class AfterSpawnTypeEnemy : SpawnTypePosition
		{
			public AfterSpawnTypeEnemy(CybergrindEnemyCatagories.MeleeEnemies afterMeleeEnemy)
			{
				spawnType = CybergrindSpawnType.melee;
				spawnTypeIndex = (int)afterMeleeEnemy;
			}

			public AfterSpawnTypeEnemy(CybergrindEnemyCatagories.ProjectileEnemies afterMeleeEnemy)
			{
				spawnType = CybergrindSpawnType.projectile;
				spawnTypeIndex = (int)afterMeleeEnemy;
			}

			public AfterSpawnTypeEnemy(CybergrindEnemyCatagories.UncommonEnemies afterMeleeEnemy)
			{
				spawnType = CybergrindSpawnType.uncommon;
				spawnTypeIndex = (int)afterMeleeEnemy;
			}

			public AfterSpawnTypeEnemy(CybergrindEnemyCatagories.SpecialEnemies afterMeleeEnemy)
			{
				spawnType = CybergrindSpawnType.special;
				spawnTypeIndex = (int)afterMeleeEnemy;
			}
		}

		/// <summary>
		/// What spawn type the enemy is and where in the spawn type array the enemy should be placed.
		/// <br/> Used to place the enemy before a vanilla enemy.
		/// <br/> Example: new BeforeSpawnTypeEnemy(CybergrindEnemyCatagories.ProjectileEnemies.MaliciousFace) will list the enemy as 'Projectile common' and will place the enemy before mal faces in the list
		/// </summary>
		public class BeforeSpawnTypeEnemy : SpawnTypePosition
		{
			public BeforeSpawnTypeEnemy(CybergrindEnemyCatagories.MeleeEnemies afterMeleeEnemy)
			{
				spawnType = CybergrindSpawnType.melee;
				spawnTypeIndex = (int)afterMeleeEnemy - 1;
			}

			public BeforeSpawnTypeEnemy(CybergrindEnemyCatagories.ProjectileEnemies afterMeleeEnemy)
			{
				spawnType = CybergrindSpawnType.projectile;
				spawnTypeIndex = (int)afterMeleeEnemy - 1;
			}

			public BeforeSpawnTypeEnemy(CybergrindEnemyCatagories.UncommonEnemies afterMeleeEnemy)
			{
				spawnType = CybergrindSpawnType.uncommon;
				spawnTypeIndex = (int)afterMeleeEnemy - 1;
			}

			public BeforeSpawnTypeEnemy(CybergrindEnemyCatagories.SpecialEnemies afterMeleeEnemy)
			{
				spawnType = CybergrindSpawnType.special;
				spawnTypeIndex = (int)afterMeleeEnemy - 1;
			}
		}

		/// <summary>
		/// What spawn type the enemy is and where in the spawn type array the enemy should be placed.
		/// <br/> Used to place the enemy after all vanilla enemies 
		/// </summary>
		public class AfterAllEnemies : SpawnTypePosition
		{
			public AfterAllEnemies(CybergrindSpawnType SpawnType)
			{
				spawnType = SpawnType;
				spawnTypeIndex = SpawnType switch
				{
					CybergrindSpawnType.melee => (int)CybergrindEnemyCatagories.MeleeEnemies.Mannequin,
					CybergrindSpawnType.projectile => (int)CybergrindEnemyCatagories.ProjectileEnemies.Gutterman,
					CybergrindSpawnType.uncommon => (int)CybergrindEnemyCatagories.UncommonEnemies.Guttertank,
					CybergrindSpawnType.special => (int)CybergrindEnemyCatagories.SpecialEnemies.Ferryman,
					_ => 0,
				};
			}
		}

		/// <summary>
		/// What spawn type the enemy is and where in the spawn type array the enemy should be placed.
		/// <br/> Used to place the enemy before all vanilla enemies 
		/// </summary>
		public class BeforeAllEnemies : SpawnTypePosition
		{
			public BeforeAllEnemies(CybergrindSpawnType SpawnType)
			{
				spawnType = SpawnType;
				spawnTypeIndex = SpawnType switch
				{
					CybergrindSpawnType.melee => (int)CybergrindEnemyCatagories.MeleeEnemies.Filth,
					CybergrindSpawnType.projectile => (int)CybergrindEnemyCatagories.ProjectileEnemies.Stray,
					CybergrindSpawnType.uncommon => (int)CybergrindEnemyCatagories.UncommonEnemies.Virtue,
					CybergrindSpawnType.special => (int)CybergrindEnemyCatagories.SpecialEnemies.Mindflayer,
					_ => 0,
				} - 1;
			}
		}

		internal virtual void Register()
		{
			CybergrindEntryLoader.ReserveEntryID();
			CybergrindEntryLoader.entries.Add(this);
		}

		/// <summary>
		/// Used to specify whether an enemy is added to the cybergrind.
		/// <br/> Main usecases for this is to add configs to prevent certain enemies from being added
		/// </summary>
		/// <returns></returns>
		public virtual bool AddedToTheCybergrind()
		{
			return true;
		}

		/// <summary>
		/// Caps enemies that are either Uncommon or Special.
		/// <br/> Vanilla enemies such as Stalkers use this to prevent large waves from spawning unmanagable or unbalanced amounts of themselves from spawning
		/// <br/>Returns enemyAmount by default.
		/// </summary>
		/// <param name="currentWave">The current cybergrind wave being spawned.</param>
		/// <param name="enemyAmount">The amount of the enemy attempted to be spawned.</param>
		/// <returns>The smallest amount of the enemy that can spawn.</returns>
		public virtual int CapNonCommonEnemyAmount(int currentWave, int enemyAmount)
		{
			return enemyAmount;
		}

		/// <summary>
		/// The settings for the enemy in the cybergrind.
		/// <br/> Called when setting up the cybergrind prefabs.
		/// </summary>
		/// <param name="spawnCost">The cost of the enemy in points.</param>
		/// <param name="costIncreasePerSpawn">The additional cost each extra enemy of this type will cost in points.</param>
		/// <param name="spawnWave">The initial wave the enemy can spawn at.</param>
		/// <param name="prefab">The gameobject that is spawned.</param>
		public abstract void SetEntrySettings(out int spawnCost, out int costIncreasePerSpawn, out int spawnWave, out GameObject prefab);

		/// <summary>
		/// Used to set what spawn type (common(melee, projectile), uncommon, special) the enemy.
		/// <br/> Also used to set where in the list the enemy is added at. 
		/// <br/> Being higher than some enemies can cause the enemy to be chosen less
		/// <br/> Being lower than some enemies can cause the enemy to be chosen more as a fallback if other enemies aren't chosen/cost too much
		/// </summary>
		/// <returns>The position the enemy is positioned at in the list of enemies for the spawn type.</returns>
		public abstract SpawnTypePosition SetTypePosition();
	}
}
