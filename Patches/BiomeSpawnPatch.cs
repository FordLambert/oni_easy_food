using HarmonyLib;
using UnityEngine;

namespace EasyFood.Patches
{
    /// <summary>
    /// Gives starting potato seeds when a new game begins.
    /// </summary>
    [HarmonyPatch(typeof(Game), "OnSpawn")]
    public class StartingItemsPatch
    {
        public static void Postfix()
        {
            // Schedule seed delivery after game fully loads
            GameScheduler.Instance.Schedule("GivePotatoSeeds", 5f, (_) => GiveStartingSeeds());
        }

        private static void GiveStartingSeeds()
        {
            try
            {
                // Find the printing pod and spawn seeds near it
                var pod = GameUtil.GetTelepad(ClusterManager.Instance.activeWorldId);
                if (pod != null)
                {
                    Vector3 pos = pod.transform.position;
                    pos.x += 1f;

                    var prefab = Assets.GetPrefab(Plants.PotatoPlantConfig.SEED_ID);
                    if (prefab != null)
                    {
                        // Spawn 5 potato seeds to start
                        for (int i = 0; i < 5; i++)
                        {
                            var seed = GameUtil.KInstantiate(prefab, pos, Grid.SceneLayer.Ore);
                            seed.SetActive(true);
                            pos.x += 0.5f;
                        }
                        Debug.Log("EasyFood: Gave 5 potato seeds at start!");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"EasyFood: Could not spawn starting seeds: {e.Message}");
            }
        }
    }
}
