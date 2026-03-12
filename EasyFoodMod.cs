using HarmonyLib;
using KMod;

namespace EasyFood
{
    public class EasyFoodMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            Debug.Log("EasyFood: Mod loaded successfully!");
        }
    }
}
