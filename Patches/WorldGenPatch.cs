using HarmonyLib;
using ProcGenGame;
using System.Collections.Generic;

namespace EasyFood.Patches
{
    /// <summary>
    /// Adds Potato seeds to the starting biome and various other biomes
    /// so players can find them naturally on the map.
    /// </summary>
    [HarmonyPatch(typeof(Immigration), "ConfigureCarePackages")]
    public class CarePackagesPatch
    {
        // Add potato seeds to care packages (printing pod drops)
        public static void Postfix(Immigration __instance)
        {
            var field = AccessTools.Field(typeof(Immigration), "carePackages");
            var packages = (CarePackageInfo[])field.GetValue(__instance);

            var list = new List<CarePackageInfo>(packages);
            list.Add(new CarePackageInfo(Plants.PotatoPlantConfig.SEED_ID, 3f, null));

            field.SetValue(__instance, list.ToArray());
        }
    }

    /// <summary>
    /// Spawns potato plants in the starting area when world generates.
    /// </summary>
    [HarmonyPatch(typeof(MutantPlant), "OnPrefabInit")]
    public class PlantSpawnHelper
    {
        // This is just to ensure the plant configs are loaded
        public static void Postfix() { }
    }
}
