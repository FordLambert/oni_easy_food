using HarmonyLib;
using static STRINGS.CREATURES;
using static STRINGS.ITEMS;

namespace EasyFood.Patches
{
    [HarmonyPatch(typeof(Localization), "Initialize")]
    public class StringsPatch
    {
        public static void Postfix()
        {
            // Plant strings
            Strings.Add($"STRINGS.CREATURES.SPECIES.{Plants.PotatoPlantConfig.ID.ToUpperInvariant()}.NAME", Plants.PotatoPlantConfig.NAME);
            Strings.Add($"STRINGS.CREATURES.SPECIES.{Plants.PotatoPlantConfig.ID.ToUpperInvariant()}.DESC", Plants.PotatoPlantConfig.DESC);
            Strings.Add($"STRINGS.CREATURES.SPECIES.{Plants.PotatoPlantConfig.ID.ToUpperInvariant()}.DOMESTICATEDDESC", Plants.PotatoPlantConfig.DOMESTICATED_DESC);

            // Potato as seed strings (potato is both food and plantable)
            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{Foods.PotatoConfig.ID.ToUpperInvariant()}.NAME", Foods.PotatoConfig.NAME);
            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{Foods.PotatoConfig.ID.ToUpperInvariant()}.DESC", "Plant a Potato to grow 6 more.");

            // Food strings - Raw Potato
            Strings.Add($"STRINGS.ITEMS.FOOD.{Foods.PotatoConfig.ID.ToUpperInvariant()}.NAME", Foods.PotatoConfig.NAME);
            Strings.Add($"STRINGS.ITEMS.FOOD.{Foods.PotatoConfig.ID.ToUpperInvariant()}.DESC", Foods.PotatoConfig.DESC);

            // Food strings - Grilled Potato
            Strings.Add($"STRINGS.ITEMS.FOOD.{Foods.GrilledPotatoConfig.ID.ToUpperInvariant()}.NAME", Foods.GrilledPotatoConfig.NAME);
            Strings.Add($"STRINGS.ITEMS.FOOD.{Foods.GrilledPotatoConfig.ID.ToUpperInvariant()}.DESC", Foods.GrilledPotatoConfig.DESC);
        }
    }
}
