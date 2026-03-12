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
        public const string DOMESTICATED_DESC = "A domesticated Potato Plant. It produces Potatoes regularly when given proper care.";

        public const string SEED_ID = "PotatoSeed";
        public const string SEED_NAME = "Potato Seed";
        public const string SEED_DESC = "The seed of a Potato Plant.";

        // Growth time: 4 cycles (faster than most crops for "easy food")
        public const float GROWTH_TIME = 4f * 600f;

        // Harvest amount
        public const int CROP_AMOUNT = 6; // 6 potatoes per harvest

        // Water requirement (kg per cycle)
        public const float WATER_RATE = 20f; // 20 kg/cycle (relatively low)

        // Safe temperature range (quite tolerant)
        public const float TEMP_MIN = 253.15f; // -20C
        public const float TEMP_MAX = 323.15f; // 50C

        public string[] GetDlcIds() => null; // Compatible with all versions including Spaced Out!

        public GameObject CreatePrefab()
        {
            var prefab = EntityTemplates.CreatePlacedEntity(
                id: ID,
                name: UI.FormatAsLink(NAME, ID),
                desc: DESC,
                mass: 1f,
                anim: Assets.GetAnim("meallice_kanim"), // Using Mealwood sprite for now
                initialAnim: "idle_empty",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                width: 1,
                height: 2,
                decor: TUNING.DECOR.BONUS.TIER1,
                defaultTemperature: 293.15f // 20C
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

            // Configure harvest amount (6 potatoes per harvest)
            var crop = prefab.AddOrGet<Crop>();
            crop.cropVal = new Crop.CropVal(Foods.PotatoConfig.ID, GROWTH_TIME, CROP_AMOUNT, true);

            // Add water requirement
            EntityTemplates.ExtendPlantToIrrigated(
                template: prefab,
                info: new PlantElementAbsorber.ConsumeInfo
                {
                    tag = SimHashes.Water.CreateTag(),
                    massConsumptionRate = WATER_RATE / 600f // Convert per-cycle to per-second
                }
            );

            prefab.AddOrGet<StandardCropPlant>();

            // Seed production
            var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
                plant: prefab,
                productionType: SeedProducer.ProductionType.Harvest,
                id: SEED_ID,
                name: UI.FormatAsLink(SEED_NAME, SEED_ID),
                desc: SEED_DESC,
                anim: Assets.GetAnim("seed_meallice_kanim"), // Using Mealwood seed sprite
                initialAnim: "object",
                numberOfSeeds: 1,
                additionalTags: new List<Tag> { GameTags.CropSeed },
                planterDirection: SingleEntityReceptacle.ReceptacleDirection.Top,
                replantGroundTag: new Tag(),
                sortOrder: 2,
                domesticatedDescription: DOMESTICATED_DESC,
                collisionShape: EntityTemplates.CollisionShape.CIRCLE,
                width: 0.2f,
                height: 0.2f
            );

            EntityTemplates.CreateAndRegisterPreviewForPlant(
                seed: seed,
                id: ID + "_preview",
                anim: Assets.GetAnim("meallice_kanim"),
                initialAnim: "place",
                width: 1,
                height: 2
            );

            return prefab;
        }

        public void OnPrefabInit(GameObject inst) { }
        public void OnSpawn(GameObject inst) { }
    }
}
