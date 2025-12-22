using BepInEx;
using HarmonyLib;
using PluginConfig;
using PluginConfig.API;
using PluginConfig.API.Fields;
using PurgatorioCyberGrind.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PurgSpawnArm
{

    [HarmonyPatch]
    [BepInPlugin(GUID, "Purg Spawn Arm", "1.0.0")]
	[BepInDependency(PluginConfiguratorController.PLUGIN_GUID)]
	public class Plugin : BaseUnityPlugin
    {
        const string GUID = "purg.PurgSpawnArm"; //COPIED CODE, REPLACE STRING

        static Harmony _harmony;

        public static SpawnableObject[] spawnableObjects;

		public static SpawnableObjectsDatabase CGIconsDatabase = new SpawnableObjectsDatabase();

        public static AssetBundle bundle;
        static AssetBundle shadersBundle;

        static List<Shader> loadedShaders;
        public static List<AudioSource> loadedAudioSources;
        
        public const string bundleName = "purg-indulgenceenemies"; // replace with name of bundle

		private PluginConfigurator config;

		public static BoolField CravenInCybergrind; //configs
		public static BoolField NeutralizerInCybergrind;

		void Start()
        {

			/*
			foreach (var loadedAssetBundle in AssetBundle.GetAllLoadedAssetBundles())
			{
				Debug.Log(loadedAssetBundle.name);
				if (loadedAssetBundle.name == "shaders.bundle")
				{
					shadersBundle = loadedAssetBundle;
				}
			}

			if (!shadersBundle)
			{
				string correctPath = Path.Combine(Application.streamingAssetsPath, "aa/StandaloneWindows64/assets_assets_assets/shaders.bundle");
				shadersBundle = AssetBundle.LoadFromFile(correctPath);
			}
			*/
		
			//load config
			config = PluginConfigurator.Create("Purgatorio Enemies", GUID);
			CravenInCybergrind = new BoolField(config.rootPanel, "Craven in the Cybergrind", "CravenInCybergrind", true);
			NeutralizerInCybergrind = new BoolField(config.rootPanel, "Neutralizer in the Cybergrind", "NeutralizerInCybergrind", true);
			config.SetIconWithURL("file://" + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "plugin-icon.png"));

			//load bundle
			bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), bundleName));
            spawnableObjects = bundle.LoadAllAssets<SpawnableObject>();




			/*
            var fuckingTHing = Addressables.LoadAssetAsync<Shader>("9847c6449c4ab6345a943c38df306a85").Result;
            foreach (var mat in bundle.LoadAllAssets<Material>())
            {
                if (mat.shader.name == "ULTRAKILL/Master")
                {
                    SwapShader(mat, fuckingTHing);
                }
            }
            */


			loadedAudioSources = bundle.LoadAllAssets<AudioSource>().ToList();

			//Loads all classes inheriting CustomCyberGrindEntry
			//To add a custom enemy to the grind, simply inherit from CustomCyberGrindEntry
			//Recommended to look at either CravenEntry or NeitralizerEntry as examples of how to use this system
			CybergrindEntryLoader.RegisterAllEntries();

			_harmony = new Harmony(GUID);
            _harmony.PatchAll();

			//Add enemy icons to the CyberGrind Icons mod
			//The database is technically unused, but CGIcons scrapes all databases and uses enemy icons for the type
			//The database used for the spawner arm isnt loaded until the arm is created,
			//And to prevent injecting into another database that might cause issues, we just create a dummy database that the CGIcons can pull from
			CGIconsDatabase.enemies = new SpawnableObject[0];
			var enemiesList = new List<SpawnableObject>();
			foreach (var spawnable in Plugin.spawnableObjects)
				if (spawnable.spawnableObjectType == SpawnableObject.SpawnableObjectDataType.Enemy)
					enemiesList.Add(spawnable);
			CGIconsDatabase.enemies = enemiesList.ToArray();
		}
        
                
        static void SwapShader(Material material, Shader shader) 
        {
            int renderQueue = material.renderQueue;
            material.shader = shader;
            material.renderQueue = renderQueue;
        }

		[HarmonyPatch(typeof(SpawnMenu), "RebuildMenu"), HarmonyPrefix]
		// ReSharper disable once InconsistentNaming
		private static void RebuildMenu(SpawnMenu __instance)
		{
			if (SceneHelper.IsPlayingCustom)
				return;

			var enemiesList = __instance.objects.enemies.ToList();
			var objectsList = __instance.objects.objects.ToList();
			foreach (var spawnable in Plugin.spawnableObjects)
			{
				spawnable.sandboxOnly = false;
				if (spawnable.spawnableObjectType == SpawnableObject.SpawnableObjectDataType.Enemy && !__instance.objects.enemies.Contains(spawnable))
				{
					enemiesList.Add(spawnable);
				}

				if (spawnable.spawnableObjectType == SpawnableObject.SpawnableObjectDataType.Object && !__instance.objects.objects.Contains(spawnable))
				{
					objectsList.Add(spawnable);
				}
			}
			__instance.objects.enemies = enemiesList.ToArray();
			__instance.objects.objects = objectsList.ToArray();
		}

		/*static bool loadedAudio = false;

		[HarmonyPatch(typeof(AudioMixerController), "Start"), HarmonyPrefix]
		private static void AddCheats(AudioMixerController __instance)
		{
			if (!loadedAudio)
				return;

			foreach (var loadAllAsset in Plugin.loadedAudioSources)
			{
				switch (loadAllAsset.outputAudioMixerGroup.name)
				{
					case "AllAudio":
						loadAllAsset.outputAudioMixerGroup = __instance.allGroup;
						break;
					case "GoreAudio":
						loadAllAsset.outputAudioMixerGroup = __instance.goreGroup;
						break;
					case "MusicAudio":
						loadAllAsset.outputAudioMixerGroup = __instance.musicGroup;
						break;
					case "DoorAudio":
						loadAllAsset.outputAudioMixerGroup = __instance.doorGroup;
						break;
					case "UnfreezeableAudio":
						loadAllAsset.outputAudioMixerGroup = __instance.unfreezeableGroup;
						break;
				}
			}

			loadedAudio = true;
		}*/
	}
}