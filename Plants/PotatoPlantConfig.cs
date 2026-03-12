using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace EasyFood.Plants
{
    public class PotatoPlantConfig : IEntityConfig
    {
        public const string ID = "PotatoPlant";
        public const string NAME = "Potato Plant";
        public const string DESC = "A hardy plant that produces nutritious Potatoes. Grows well in most conditions.";
        public const string DOMESTICATED_DESC = "A domesticated Potato Plant. Plant a Potato to grow 6 more.";

        // Growth time: 4 cycles
        public const float GROWTH_TIME = 4f * 600f;

        // Harvest amount: plant 1 potato, get 6 back
        public const int CROP_AMOUNT = 6;

        // Water requirement (kg per cycle)
        public const float WATER_RATE = 20f;

        // Safe temperature range (quite tolerant)
        public const float TEMP_MIN = 253.15f; // -20C
        public const float TEMP_MAX = 323.15f; // 50C

        public string[] GetDlcIds() => null;

        public GameObject CreatePrefab()
        {
            var prefab = EntityTemplates.CreatePlacedEntity(
                id: ID,
                name: UI.FormatAsLink(NAME, ID),
                desc: DESC,
                mass: 1f,
                anim: Assets.GetAnim("meallice_kanim"),
                initialAnim: "idle_empty",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                width: 1,
                height: 2,
                decor: TUNING.DECOR.BONUS.TIER1,
                defaultTemperature: 293.15f
            );

            EntityTemplates.ExtendEntityToBasicPlant(
                template: prefab,
                temperature_lethal_low: TEMP_MIN - 10f,
                temperature_warning_low: TEMP_MIN,
                temperature_warning_high: TEMP_MAX,
                temperature_lethal_high: TEMP_MAX + 10f,
                safe_elements: new SimHashes[] { SimHashes.Oxygen, SimHashes.CarbonDioxide },
                pressure_sensitive: true,
                pressure_lethal_low: 0f,
                pressure_warning_low: 0.15f,
                crop_id: Foods.PotatoConfig.ID,
                max_radiation: 2500f,
                baseTraitId: ID + "Original",
                baseTraitName: NAME
            );

            // Configure harvest: 6 potatoes per harvest
            var crop = prefab.AddOrGet<Crop>();
            crop.cropVal = new Crop.CropVal(Foods.PotatoConfig.ID, GROWTH_TIME, CROP_AMOUNT, true);

            // Add water requirement
            EntityTemplates.ExtendPlantToIrrigated(
                template: prefab,
                info: new PlantElementAbsorber.ConsumeInfo
                {
                    tag = SimHashes.Water.CreateTag(),
                    massConsumptionRate = WATER_RATE / 600f
                }
            );

            prefab.AddOrGet<StandardCropPlant>();

            // No separate seed - the Potato itself is plantable (configured in PotatoConfig)

            return prefab;
        }

        public void OnPrefabInit(GameObject inst) { }
        public void OnSpawn(GameObject inst) { }
    }
}
