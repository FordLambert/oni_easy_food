using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace EasyFood.Foods
{
    public class PotatoConfig : IEntityConfig
    {
        public const string ID = "Potato";
        public const string NAME = "Potato";
        public const string DESC = "A starchy tuber that can be eaten raw, cooked, or planted to grow more potatoes.";

        // 1200 kcal - between Meal Lice (600) and Bristle Berry (1600)
        public const float CALORIES_PER_UNIT = 1200f * 1000f;

        // Spoils in 8 cycles (longer than most foods)
        public const float SPOIL_TIME = 8f * 600f;

        public string[] GetDlcIds() => null;

        public GameObject CreatePrefab()
        {
            var prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: UI.FormatAsLink(NAME, ID),
                desc: DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("meallicegrain_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.8f,
                height: 0.4f,
                isPickupable: true
            );

            // Make it edible
            var foodInfo = new EdiblesManager.FoodInfo(
                ID,
                "",
                CALORIES_PER_UNIT,
                TUNING.FOOD.FOOD_QUALITY_MEDIOCRE,
                255.15f,
                277.15f,
                SPOIL_TIME,
                true
            );

            EntityTemplates.ExtendEntityToFood(prefab, foodInfo);

            // Make it plantable (like a seed)
            prefab.AddOrGet<KPrefabID>().AddTag(GameTags.CropSeed, false);
            var plantableSeed = prefab.AddOrGet<PlantableSeed>();
            plantableSeed.PlantID = new Tag(Plants.PotatoPlantConfig.ID);
            plantableSeed.domesticatedDescription = "Plant a Potato to grow 6 more.";

            return prefab;
        }

        public void OnPrefabInit(GameObject inst) { }
        public void OnSpawn(GameObject inst) { }
    }
}
