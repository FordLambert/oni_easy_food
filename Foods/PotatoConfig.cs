using STRINGS;
using UnityEngine;

namespace EasyFood.Foods
{
    public class PotatoConfig : IEntityConfig
    {
        public const string ID = "Potato";
        public const string NAME = "Potato";
        public const string DESC = "A starchy tuber harvested from a Potato Plant. Can be eaten raw or cooked.";

        // 1200 kcal - between Meal Lice (600) and Bristle Berry (1600)
        public const float CALORIES_PER_UNIT = 1200f * 1000f; // In joules

        // Spoils in 8 cycles (longer than most foods)
        public const float SPOIL_TIME = 8f * 600f; // 600 seconds per cycle

        public string[] GetDlcIds() => null; // Compatible with all versions including Spaced Out!

        public GameObject CreatePrefab()
        {
            var prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: UI.FormatAsLink(NAME, ID),
                desc: DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("basic_plant_food_kanim"), // Using existing Mealwood fruit sprite
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.8f,
                height: 0.4f,
                isPickupable: true
            );

            // Constructor: (id, dlcId, caloriesPerUnit, quality, preserveTemp, rotTemp, spoilTime, canRot)
            var foodInfo = new EdiblesManager.FoodInfo(
                ID,                              // id
                "",                              // dlcId
                CALORIES_PER_UNIT,               // caloriesPerUnit
                TUNING.FOOD.FOOD_QUALITY_MEDIOCRE, // quality
                255.15f,                         // preserveTemperature (-18C)
                277.15f,                         // rotTemperature (4C)
                SPOIL_TIME,                      // spoilTime
                true                             // can_rot
            );

            return EntityTemplates.ExtendEntityToFood(prefab, foodInfo);
        }

        public void OnPrefabInit(GameObject inst) { }
        public void OnSpawn(GameObject inst) { }
    }
}
