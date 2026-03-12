using UnityEngine;

namespace EasyFood.Plants
{
    public class PotatoPlantPreviewConfig : IEntityConfig
    {
        public const string ID = "PotatoPlantPreview";

        public string[] GetDlcIds() => null;

        public GameObject CreatePrefab()
        {
            var prefab = EntityTemplates.CreatePlacedEntity(
                id: ID,
                name: PotatoPlantConfig.NAME,
                desc: PotatoPlantConfig.DESC,
                mass: 1f,
                anim: Assets.GetAnim("oxy_fern_kanim"),
                initialAnim: "idle_empty",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                width: 1,
                height: 2,
                decor: default,
                defaultTemperature: 293.15f
            );

            // Add EntityPreview component required for farm plot preview
            prefab.AddOrGet<EntityPreview>();

            return prefab;
        }

        public void OnPrefabInit(GameObject inst) { }
        public void OnSpawn(GameObject inst) { }
    }
}
