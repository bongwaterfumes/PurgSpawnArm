namespace PurgatorioCyberGrind.Systems
{
	/// <summary>
	/// This class contains each spawn type's enemies in order from where they appear in the list
	/// </summary>
	public class CybergrindEnemyCatagories
	{
		/// <summary>
		/// Melee enemies in order of where they appear in the melee enemies array.
		/// </summary>
		public enum MeleeEnemies
		{
			Filth = 0,
			Schism = 1,
			Streetcleaner = 2,
			Cerberus = 3,
			Swordsmachine = 4,
			Mannequin = 5
		}

		/// <summary>
		/// Projectile enemies in order of where they appear in the projectile enemies array.
		/// </summary>
		public enum ProjectileEnemies
		{
			Stray = 0,
			Drone = 1,
			Filth = 2, //Double check?, output logs repeat this but this may be a corrupted enemy entry
			MaliciousFace = 3,
			Gutterman = 4
		}

		/// <summary>
		/// Uncommon enemies in order of where they appear in the uncommon enemies array.
		/// </summary>
		public enum UncommonEnemies
		{
			Virtue = 0,
			Stalker = 1,
			Sentry = 2, //sentry
			Idol = 3,
			Guttertank = 4
		}

		/// <summary>
		/// special enemies in order of where they appear in the melee enemies array.
		/// </summary>
		public enum SpecialEnemies
		{
			Mindflayer = 0,
			Insurrectionist = 1,
			Ferryman = 2
		}
	}
}
