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
                // Find the printing pod and spawn items near it
                var pod = GameUtil.GetTelepad(ClusterManager.Instance.activeWorldId);
                if (pod != null)
                {
                    Vector3 pos = pod.transform.position;
                    pos.x += 1f;

                    // Spawn 5 potatoes (food)
                    var potatoPrefab = Assets.GetPrefab(Foods.PotatoConfig.ID);
                    if (potatoPrefab != null)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var potato = GameUtil.KInstantiate(potatoPrefab, pos, Grid.SceneLayer.Ore);
                            potato.SetActive(true);
                            pos.x += 0.3f;
                        }
                    }

                    // Spawn 3 potato seeds
                    pos.x += 0.5f;
                    var seedPrefab = Assets.GetPrefab(Plants.PotatoPlantConfig.SEED_ID);
                    if (seedPrefab != null)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var seed = GameUtil.KInstantiate(seedPrefab, pos, Grid.SceneLayer.Ore);
                            seed.SetActive(true);
                            pos.x += 0.3f;
                        }
                    }

                    Debug.Log("EasyFood: Gave 5 potatoes and 3 seeds at start!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"EasyFood: Could not spawn starting items: {e.Message}");
            }
        }
    }
}
