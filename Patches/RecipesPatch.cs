using HarmonyLib;

namespace EasyFood.Patches
{
    /// <summary>
    /// Adds the Grilled Potato recipe to the Cooking Station (Electric Grill).
    /// </summary>
    [HarmonyPatch(typeof(CookingStationConfig), "ConfigureBuildingTemplate")]
    public class RecipesPatch
    {
        public static void Postfix()
        {
            AddGrilledPotatoRecipe();
        }

        private static void AddGrilledPotatoRecipe()
        {
            // Create the recipe: 1 Potato -> 1 Grilled Potato
            var input = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(Foods.PotatoConfig.ID, 1f)
            };

            var output = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(Foods.GrilledPotatoConfig.ID, 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
            };

            string recipeId = ComplexRecipeManager.MakeRecipeID(
                CookingStationConfig.ID,
                input,
                output
            );

            var recipe = new ComplexRecipe(recipeId, input, output)
            {
                time = 30f, // 30 seconds cooking time
                description = $"Grill a {Foods.PotatoConfig.NAME} to make a delicious {Foods.GrilledPotatoConfig.NAME}.",
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new System.Collections.Generic.List<Tag>
                {
                    CookingStationConfig.ID
                }
            };
        }
    }
}
