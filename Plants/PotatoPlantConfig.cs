using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

namespace EasyFood.Plants
{
    public class PotatoPlantConfig : IEntityConfig
    {
        public const string ID = "PotatoPlant";
        public const string NAME = "Potato Plant";
        public const string DESC = "A hardy plant that produces nutritious Potatoes. Grows well in most conditions.";
        public const string DOMESTICATED_DESC = "A domesticated Potato Plant.";

        public const string SEED_ID = "PotatoSeed";
        public const string SEED_NAME = "Potato Seed";
        public const string SEED_DESC = "The seed of a Potato Plant.";

        private const string TRAIT_ID = "PotatoPlantOriginal";

        // Growth time: 4 cycles (in seconds)
        public const float GROWTH_TIME = 4f * 600f;

        // Harvest amount: 6 potatoes
        public const int CROP_AMOUNT = 6;

        // Water requirement (kg per cycle)
        public const float WATER_RATE = 20f;

        // Safe temperature range (quite tolerant)
        public const float TEMP_MIN = 253.15f; // -20C
        public const float TEMP_MAX = 323.15f; // 50C

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab()
        {
            var prefab = EntityTemplates.CreatePlacedEntity(
                id: ID,
                name: UI.FormatAsLink(NAME, ID),
                desc: DESC,
                width: 1,
                height: 2,
                mass: 1f,
                anim: Assets.GetAnim("oxy_fern_kanim"),
                initialAnim: "idle_empty",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                decor: TUNING.DECOR.BONUS.TIER1,
                defaultTemperature: 293.15f
            );

            EntityTemplates.ExtendEntityToBasicPlant(
                template: prefab,
                temperature_lethal_low: TEMP_MIN - 10f,
                temperature_warning_low: TEMP_MIN,
                temperature_warning_high: TEMP_MAX,
                temperature_lethal_high: TEMP_MAX + 10f,
                pressure_sensitive: false,
                crop_id: Foods.PotatoConfig.ID,
                max_radiation: 2500f,
                baseTraitId: TRAIT_ID,
                baseTraitName: NAME
            );

            // Add maturity modifier to the trait (growth time in cycles)
            var trait = Db.Get().traits.TryGet(TRAIT_ID);
            if (trait != null)
            {
                trait.Add(new AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute.Id, GROWTH_TIME / 600f, NAME));
            }

            prefab.AddOrGet<StandardCropPlant>();

            // Explicitly configure crop
            var crop = prefab.AddOrGet<Crop>();
            crop.cropVal = new Crop.CropVal(Foods.PotatoConfig.ID, GROWTH_TIME, CROP_AMOUNT, true);

            // Create seed for the plant (2 seeds per harvest)
            var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
                plant: prefab,
                id: SEED_ID,
                name: UI.FormatAsLink(SEED_NAME, ID),
                desc: SEED_DESC,
                productionType: SeedProducer.ProductionType.Harvest,
                anim: Assets.GetAnim("muckroot_kanim"),
                numberOfSeeds: 2,
                additionalTags: new List<Tag> { GameTags.CropSeed },
                sortOrder: 3,
                width: 0.33f,
                height: 0.33f
            );

            // Create preview for farm plot
            EntityTemplates.CreateAndRegisterPreviewForPlant(
                seed: seed,
                id: ID + "_preview",
                anim: Assets.GetAnim("oxy_fern_kanim"),
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
