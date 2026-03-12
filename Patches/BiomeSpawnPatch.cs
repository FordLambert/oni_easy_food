using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace EasyFood.Patches
{
    /// <summary>
    /// Gives starting potato seeds when a new game begins.
    /// </summary>
    [HarmonyPatch(typeof(CharacterContainer), "OnSpawn")]
    public class StartingItemsPatch
    {
        private static bool seedsGiven = false;

        public static void Postfix(CharacterContainer __instance)
        {
            // Reset flag on new game
            if (GameClock.Instance != null && GameClock.Instance.GetCycle() == 0 && !seedsGiven)
            {
                // Schedule seed delivery after game fully loads
                GameScheduler.Instance.Schedule("GivePotatoSeeds", 1f, (_) => GiveStartingSeeds());
                seedsGiven = true;
            }
        }

        private static void GiveStartingSeeds()
        {
            // Find the printing pod and spawn seeds near it
            var pod = GameUtil.GetTelepad(ClusterManager.Instance.activeWorldId);
            if (pod != null)
            {
                Vector3 pos = pod.transform.position;
                pos.x += 1f;

                // Spawn 5 potato seeds to start
                for (int i = 0; i < 5; i++)
                {
                    var seed = GameUtil.KInstantiate(
                        Assets.GetPrefab(Plants.PotatoPlantConfig.SEED_ID),
                        pos,
                        Grid.SceneLayer.Ore
                    );
                    seed.SetActive(true);
                }

                Debug.Log("EasyFood: Gave 5 potato seeds at start!");
            }
        }
    }

    /// <summary>
    /// Resets the seeds given flag when returning to main menu.
    /// </summary>
    [HarmonyPatch(typeof(Game), "DestroyInstances")]
    public class ResetSeedsFlag
    {
        public static void Postfix()
        {
            AccessTools.Field(typeof(StartingItemsPatch), "seedsGiven").SetValue(null, false);
        }
    }
}
