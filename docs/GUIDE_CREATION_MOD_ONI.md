# Guide complet : Créer un mod pour Oxygen Not Included

Ce guide documente tout le processus de création du mod "Easy Food - Potato", incluant les erreurs rencontrées et leurs solutions.

## Table des matières

1. [Structure du projet](#1-structure-du-projet)
2. [Configuration initiale](#2-configuration-initiale)
3. [Créer une plante](#3-créer-une-plante)
4. [Créer un aliment](#4-créer-un-aliment)
5. [Créer une recette de cuisson](#5-créer-une-recette-de-cuisson)
6. [Les patches Harmony](#6-les-patches-harmony)
7. [Les traductions](#7-les-traductions)
8. [Compilation et installation](#8-compilation-et-installation)
9. [Erreurs fréquentes et solutions](#9-erreurs-fréquentes-et-solutions)
10. [Ressources utiles](#10-ressources-utiles)

---

## 1. Structure du projet

```
MonMod/
├── lib/                      # DLLs du jeu (gitignore)
│   ├── Assembly-CSharp.dll
│   ├── Assembly-CSharp-firstpass.dll
│   ├── 0Harmony.dll
│   ├── UnityEngine.dll
│   └── UnityEngine.CoreModule.dll
├── Foods/                    # Configs des aliments
│   └── MonAlimentConfig.cs
├── Plants/                   # Configs des plantes
│   └── MaPlantConfig.cs
├── Patches/                  # Patches Harmony
│   ├── StringsPatch.cs
│   └── RecipesPatch.cs
├── MonMod.csproj             # Projet C#
├── MonModMod.cs              # Point d'entrée
├── mod_info.yaml             # IMPORTANT : config DLC
└── README.md
```

---

## 2. Configuration initiale

### 2.1 Récupérer les DLLs du jeu

Copie ces fichiers depuis `OxygenNotIncluded_Data/Managed/` vers `lib/` :
- `Assembly-CSharp.dll` (code du jeu)
- `Assembly-CSharp-firstpass.dll`
- `0Harmony.dll` (bibliothèque de patching)
- `UnityEngine.dll`
- `UnityEngine.CoreModule.dll`

### 2.2 Fichier .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <AssemblyName>MonMod</AssemblyName>
    <RootNamespace>MonMod</RootNamespace>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="0Harmony">
      <HintPath>lib/0Harmony.dll</HintPath>
      <Private>false</Private>
    </Reference>
    <Reference Include="Assembly-CSharp">
      <HintPath>lib/Assembly-CSharp.dll</HintPath>
      <Private>false</Private>
    </Reference>
    <Reference Include="Assembly-CSharp-firstpass">
      <HintPath>lib/Assembly-CSharp-firstpass.dll</HintPath>
      <Private>false</Private>
    </Reference>
    <Reference Include="UnityEngine">
      <HintPath>lib/UnityEngine.dll</HintPath>
      <Private>false</Private>
    </Reference>
    <Reference Include="UnityEngine.CoreModule">
      <HintPath>lib/UnityEngine.CoreModule.dll</HintPath>
      <Private>false</Private>
    </Reference>
  </ItemGroup>
</Project>
```

### 2.3 mod_info.yaml (CRITIQUE)

**IMPORTANT** : C'est `mod_info.yaml`, PAS `mod.yaml` !

```yaml
supportedContent: ALL
minimumSupportedBuild: 707956
version: 0.1.0
APIVersion: 2
```

**Erreurs évitées :**
- ❌ `mod.yaml` seul → "Incompatible DLC configuration"
- ❌ `minimumSupportedBuild` trop ancien → "Mod out-of-date"
- ✅ Trouve le build actuel dans `Player.log` : cherche "Build: U57-707956-V"

### 2.4 Point d'entrée du mod

```csharp
using HarmonyLib;
using KMod;

namespace MonMod
{
    public class MonModMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            Debug.Log("MonMod: Loaded!");
        }
    }
}
```

---

## 3. Créer une plante

### 3.1 Structure de base

```csharp
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace MonMod.Plants
{
    public class MaPlantConfig : IEntityConfig
    {
        public const string ID = "MaPlant";
        public const string NAME = "Ma Plante";
        public const string DESC = "Description de la plante.";

        public string[] GetDlcIds() => null; // Compatible avec tout

        public GameObject CreatePrefab()
        {
            // Voir ci-dessous
        }

        public void OnPrefabInit(GameObject inst) { }
        public void OnSpawn(GameObject inst) { }
    }
}
```

### 3.2 CreatePrefab complet pour une plante

```csharp
public GameObject CreatePrefab()
{
    // 1. Créer l'entité de base
    var prefab = EntityTemplates.CreatePlacedEntity(
        id: ID,
        name: UI.FormatAsLink(NAME, ID),
        desc: DESC,
        mass: 1f,
        anim: Assets.GetAnim("meallice_kanim"), // Animation existante
        initialAnim: "idle_empty",
        sceneLayer: Grid.SceneLayer.BuildingFront,
        width: 1,
        height: 2,
        decor: TUNING.DECOR.BONUS.TIER1,
        defaultTemperature: 293.15f
    );

    // 2. Configurer comme plante basique
    // IMPORTANT : inclure baseTraitId et baseTraitName !
    EntityTemplates.ExtendEntityToBasicPlant(
        template: prefab,
        temperature_lethal_low: 243.15f,    // -30°C
        temperature_warning_low: 253.15f,   // -20°C
        temperature_warning_high: 323.15f,  // +50°C
        temperature_lethal_high: 333.15f,   // +60°C
        safe_elements: new SimHashes[] { SimHashes.Oxygen, SimHashes.CarbonDioxide },
        pressure_sensitive: true,
        pressure_lethal_low: 0f,
        pressure_warning_low: 0.15f,
        crop_id: Foods.MonAlimentConfig.ID,
        max_radiation: 2500f,
        baseTraitId: ID + "Original",       // REQUIS !
        baseTraitName: NAME                  // REQUIS !
    );

    // 3. Configurer la récolte
    var crop = prefab.AddOrGet<Crop>();
    crop.cropVal = new Crop.CropVal(
        Foods.MonAlimentConfig.ID,  // ID de l'aliment produit
        4f * 600f,                  // Temps de croissance (4 cycles)
        6,                          // Quantité récoltée
        true                        // Produit des graines ?
    );

    // 4. Ajouter irrigation (optionnel)
    EntityTemplates.ExtendPlantToIrrigated(
        template: prefab,
        info: new PlantElementAbsorber.ConsumeInfo
        {
            tag = SimHashes.Water.CreateTag(),
            massConsumptionRate = 20f / 600f  // 20 kg/cycle
        }
    );

    prefab.AddOrGet<StandardCropPlant>();

    return prefab;
}
```

**Erreur évitée :**
- ❌ Oublier `baseTraitId` et `baseTraitName` → "base trait wasn't specified"

---

## 4. Créer un aliment

### 4.1 Aliment simple (mangeable)

```csharp
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

    // ATTENTION : Utiliser les paramètres positionnels !
    var foodInfo = new EdiblesManager.FoodInfo(
        ID,                                    // id
        "",                                    // dlcId
        1200f * 1000f,                        // calories (en joules)
        TUNING.FOOD.FOOD_QUALITY_MEDIOCRE,    // qualité
        255.15f,                              // preserveTemperature
        277.15f,                              // rotTemperature
        8f * 600f,                            // spoilTime
        true                                  // canRot
    );

    return EntityTemplates.ExtendEntityToFood(prefab, foodInfo);
}
```

### 4.2 Aliment plantable (comme la Potato)

Pour qu'un aliment soit aussi plantable :

```csharp
// Après ExtendEntityToFood
prefab.AddOrGet<KPrefabID>().AddTag(GameTags.CropSeed, false);
var plantableSeed = prefab.AddOrGet<PlantableSeed>();
plantableSeed.PlantID = new Tag(Plants.MaPlantConfig.ID);
plantableSeed.domesticatedDescription = "Description quand planté.";
```

### 4.3 Qualités de nourriture

```csharp
TUNING.FOOD.FOOD_QUALITY_AWFUL      // -3 (Gristle Berry)
TUNING.FOOD.FOOD_QUALITY_TERRIBLE   // -2
TUNING.FOOD.FOOD_QUALITY_MEDIOCRE   // -1 (Meal Lice)
TUNING.FOOD.FOOD_QUALITY_GOOD       // 0  (Pickled Meal)
TUNING.FOOD.FOOD_QUALITY_GREAT      // +1
TUNING.FOOD.FOOD_QUALITY_AMAZING    // +2
TUNING.FOOD.FOOD_QUALITY_WONDERFUL  // +3
```

---

## 5. Créer une recette de cuisson

```csharp
using HarmonyLib;

namespace MonMod.Patches
{
    [HarmonyPatch(typeof(CookingStationConfig), "ConfigureBuildingTemplate")]
    public class RecipesPatch
    {
        public static void Postfix()
        {
            var input = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(Foods.MonAlimentConfig.ID, 1f)
            };

            var output = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(
                    Foods.MonAlimentCuitConfig.ID,
                    1f,
                    ComplexRecipe.RecipeElement.TemperatureOperation.Heated
                )
            };

            string recipeId = ComplexRecipeManager.MakeRecipeID(
                CookingStationConfig.ID,
                input,
                output
            );

            var recipe = new ComplexRecipe(recipeId, input, output)
            {
                time = 30f,
                description = "Description de la recette.",
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new System.Collections.Generic.List<Tag>
                {
                    CookingStationConfig.ID  // Cooking Station
                    // Ou GourmetCookingStationConfig.ID pour Gas Range
                }
            };
        }
    }
}
```

---

## 6. Les patches Harmony

### 6.1 Patch simple (Postfix)

```csharp
[HarmonyPatch(typeof(ClasseCible), "NomMethode")]
public class MonPatch
{
    public static void Postfix(ClasseCible __instance)
    {
        // Exécuté APRÈS la méthode originale
    }
}
```

### 6.2 Patch avec gestion d'erreurs

```csharp
public static void Postfix()
{
    try
    {
        // Code qui peut échouer
    }
    catch (System.Exception e)
    {
        Debug.LogWarning($"MonMod: Erreur - {e.Message}");
    }
}
```

---

## 7. Les traductions

```csharp
[HarmonyPatch(typeof(Localization), "Initialize")]
public class StringsPatch
{
    public static void Postfix()
    {
        // Plante
        Strings.Add($"STRINGS.CREATURES.SPECIES.MAPLANT.NAME", "Ma Plante");
        Strings.Add($"STRINGS.CREATURES.SPECIES.MAPLANT.DESC", "Description.");
        Strings.Add($"STRINGS.CREATURES.SPECIES.MAPLANT.DOMESTICATEDDESC", "Domestiquée.");

        // Graine (si séparée)
        Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.MASEED.NAME", "Graine");
        Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.MASEED.DESC", "Description.");

        // Aliment
        Strings.Add($"STRINGS.ITEMS.FOOD.MONALIMENT.NAME", "Mon Aliment");
        Strings.Add($"STRINGS.ITEMS.FOOD.MONALIMENT.DESC", "Description.");
    }
}
```

**Note :** Les clés sont en MAJUSCULES et correspondent à l'ID de l'entité.

---

## 8. Compilation et installation

### 8.1 Compiler

```bash
dotnet build
```

Le DLL sera dans `bin/Debug/net472/MonMod.dll`

### 8.2 Trouver le dossier des mods

```bash
# Linux (Steam normal)
~/.config/unity3d/Klei/Oxygen Not Included/mods/Local/

# Linux (Steam Snap)
~/snap/steam/common/.config/unity3d/Klei/Oxygen Not Included/mods/Local/

# Windows
%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\
```

### 8.3 Installer

```bash
mkdir -p "[DOSSIER_MODS]/MonMod"
cp bin/Debug/net472/MonMod.dll "[DOSSIER_MODS]/MonMod/"
cp mod_info.yaml "[DOSSIER_MODS]/MonMod/"
```

Structure finale :
```
mods/Local/MonMod/
├── MonMod.dll
└── mod_info.yaml
```

---

## 9. Erreurs fréquentes et solutions

### "Incompatible DLC configuration"

**Cause :** Mauvais fichier de config ou `GetDlcIds()` incorrect.

**Solution :**
1. Utiliser `mod_info.yaml` (pas `mod.yaml`)
2. Ajouter `supportedContent: ALL`
3. Dans le code : `public string[] GetDlcIds() => null;`

### "Mod out-of-date"

**Cause :** `minimumSupportedBuild` trop ancien.

**Solution :**
1. Cherche le build actuel dans `Player.log` :
   ```
   grep "Build:" Player.log
   # Résultat : Build: U57-707956-V
   ```
2. Met à jour `mod_info.yaml` :
   ```yaml
   minimumSupportedBuild: 707956
   ```

### "First anim file needs to be non-null"

**Cause :** Le nom de l'animation n'existe pas.

**Solution :**
1. Trouve les vrais noms d'animations :
   ```bash
   strings OxygenNotIncluded_Data/sharedassets0.assets | grep "_kanim" | sort -u
   ```
2. Exemples valides :
   - `meallicegrain_kanim` (Meal Lice)
   - `gristleberry_kanim` (Gristle Berry grillé)
   - `meallice_kanim` (plante Mealwood)
   - `bristleberry_kanim` (Bristle Berry)

### "base trait wasn't specified"

**Cause :** Paramètres manquants dans `ExtendEntityToBasicPlant`.

**Solution :** Ajouter `baseTraitId` et `baseTraitName` :
```csharp
EntityTemplates.ExtendEntityToBasicPlant(
    // ... autres paramètres ...
    baseTraitId: ID + "Original",
    baseTraitName: NAME
);
```

### "InvalidCastException" dans un patch

**Cause :** L'API du jeu a changé.

**Solution :**
1. Vérifie les types avec la décompilation du jeu
2. Ou désactive le patch problématique temporairement
3. Utilise `try/catch` pour éviter les crashs

### Le jeu crash au lancement d'une partie

**Cause :** Erreur dans un patch qui s'exécute au chargement.

**Solution :**
1. Regarde `Player.log` pour l'erreur exacte
2. Ajoute des `try/catch` dans les patches
3. Désactive les patches un par un pour isoler le problème

---

## 10. Ressources utiles

### Outils

- **dnSpy** : Décompiler le jeu pour voir le code source
- **Kanim Explorer** : Créer des sprites (Windows)
- **Kanimal-SE** : Convertir sprites (CLI, Linux/Mac/Windows)

### Documentation

- [Forums Klei - Mods & Tools](https://forums.kleientertainment.com/forums/forum/204-oxygen-not-included-mods-and-tools/)
- [ONI Mod Template](https://github.com/O-n-y/OxygenNotIncludedModTemplate)
- [PeterHan's Mods](https://github.com/peterhaneve/ONIMods) (exemples)
- [Cairath's Mods](https://github.com/Cairath/ONI-Mods) (exemples)

### Trouver les noms d'animations

```bash
# Dans le dossier du jeu
strings OxygenNotIncluded_Data/sharedassets0.assets | grep "_kanim" | grep -i "mot_clé"
```

### Trouver le numéro de build

```bash
grep "Build:" ~/.config/unity3d/Klei/Oxygen\ Not\ Included/Player.log
```

### Logs du jeu

```
# Linux
~/.config/unity3d/Klei/Oxygen Not Included/Player.log

# Linux (Snap)
~/snap/steam/common/.config/unity3d/Klei/Oxygen Not Included/Player.log

# Windows
%USERPROFILE%\AppData\LocalLow\Klei\Oxygen Not Included\Player.log
```

---

## Checklist avant de publier

- [ ] `mod_info.yaml` avec le bon `minimumSupportedBuild`
- [ ] `supportedContent: ALL` pour compatibilité DLC
- [ ] Tous les `GetDlcIds()` retournent `null`
- [ ] Noms d'animations valides (testés)
- [ ] Pas d'erreurs dans `Player.log`
- [ ] Testé sur une nouvelle partie
- [ ] README à jour
