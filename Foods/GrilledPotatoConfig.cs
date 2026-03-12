using STRINGS;
using UnityEngine;

namespace EasyFood.Foods
{
    public class GrilledPotatoConfig : IEntityConfig
    {
        public const string ID = "GrilledPotato";
        public const string NAME = "Grilled Potato";
        public const string DESC = "A perfectly grilled potato with a crispy exterior. Much more appetizing than raw.";

        // 1500 kcal - bonus from cooking
        public const float CALORIES_PER_UNIT = 1500f * 1000f;

        // Spoils in 12 cycles (longer than raw)
        public const float SPOIL_TIME = 12f * 600f;

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab()
        {
            var prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: UI.FormatAsLink(NAME, ID),
                desc: DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("cookedmeat_kanim"), // Using cooked meat sprite for now
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.8f,
                height: 0.4f,
                isPickupable: true
            );

            // Constructor: (id, dlcId, caloriesPerUnit, quality, preserveTemp, rotTemp, spoilTime, canRot)
            var foodInfo = new EdiblesManager.FoodInfo(
                ID,
                "",
                CALORIES_PER_UNIT,
                TUNING.FOOD.FOOD_QUALITY_GOOD, // Good quality (0)
                255.15f,  // -18C preserve
                277.15f,  // 4C rot
                SPOIL_TIME,
                true
            );

            return EntityTemplates.ExtendEntityToFood(prefab, foodInfo);
        }

        public void OnPrefabInit(GameObject inst) { }
        public void OnSpawn(GameObject inst) { }
    }
}
