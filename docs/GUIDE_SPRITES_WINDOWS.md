# Guide : Créer les sprites custom sous Windows

Ce guide explique comment convertir les images PNG en format Kanim pour Oxygen Not Included.

## Fichiers sources

Les images PNG sont dans `images/potatoes/` :
- `potato.png.png` → pour la Potato (aliment)
- `baked_potato.png` → pour la Grilled Potato
- `growing_potato.png` → pour la Potato Plant (3 étapes de croissance)

## Outil nécessaire

**Kanim Explorer** : https://github.com/romen-h/kanim-explorer/releases

Télécharge la dernière version (fichier .zip) et extrais-la.

## Étape 1 : Créer le kanim pour la Potato (aliment)

1. Lance `KanimExplorer.exe`
2. Va dans **File → New**
3. Importe l'image :
   - Clique sur **Add Sprite** ou glisse `potato.png.png` dans la fenêtre
4. Configure :
   - **Build name** : `potato`
   - **Animation name** : `object`
5. L'animation doit avoir au moins 1 frame avec le sprite
6. **File → Export → Kanim**
7. Choisis le dossier de sortie : `anim/assets/potato_kanim/`

Tu dois obtenir 3 fichiers :
```
potato_kanim/
├── potato_0.png
├── potato_anim.bytes
└── potato_build.bytes
```

## Étape 2 : Créer le kanim pour la Grilled Potato

Répète les mêmes étapes avec :
- **Image** : `baked_potato.png`
- **Build name** : `grilledpotato`
- **Animation name** : `object`
- **Dossier de sortie** : `anim/assets/grilledpotato_kanim/`

## Étape 3 : Créer le kanim pour la Potato Plant

C'est plus complexe car il y a 3 étapes de croissance.

1. Lance `KanimExplorer.exe`
2. **File → New**
3. L'image `growing_potato.png` contient 3 sprites côte à côte. Tu dois :
   - Soit découper l'image en 3 fichiers séparés avant
   - Soit utiliser l'outil de découpe de Kanim Explorer

4. Crée **3 animations** :
   - `idle_empty` → petit sprite (étape 1)
   - `grow` → sprite moyen (étape 2)
   - `idle_full` → grand sprite avec fleurs (étape 3)

5. Configure :
   - **Build name** : `potatoplant`

6. **File → Export → Kanim**
7. Dossier de sortie : `anim/assets/potatoplant_kanim/`

## Étape 4 : Intégrer dans le mod

### Structure finale du mod

```
EasyFood/
├── anim/
│   └── assets/
│       ├── potato_kanim/
│       │   ├── potato_0.png
│       │   ├── potato_anim.bytes
│       │   └── potato_build.bytes
│       ├── grilledpotato_kanim/
│       │   ├── grilledpotato_0.png
│       │   ├── grilledpotato_anim.bytes
│       │   └── grilledpotato_build.bytes
│       └── potatoplant_kanim/
│           ├── potatoplant_0.png
│           ├── potatoplant_anim.bytes
│           └── potatoplant_build.bytes
├── EasyFood.dll
└── mod_info.yaml
```

### Modifier le code

Dans les fichiers C#, change les noms des animations :

**Foods/PotatoConfig.cs** (ligne ~28) :
```csharp
// Avant :
anim: Assets.GetAnim("meallicegrain_kanim")
// Après :
anim: Assets.GetAnim("potato_kanim")
```

**Foods/GrilledPotatoConfig.cs** (ligne ~28) :
```csharp
// Avant :
anim: Assets.GetAnim("gristleberry_kanim")
// Après :
anim: Assets.GetAnim("grilledpotato_kanim")
```

**Plants/PotatoPlantConfig.cs** (lignes ~40 et ~101) :
```csharp
// Avant :
anim: Assets.GetAnim("meallice_kanim")
// Après :
anim: Assets.GetAnim("potatoplant_kanim")
```

### Recompiler

```bash
dotnet build
```

### Réinstaller le mod

Copie le contenu vers le dossier du mod :
```
~/.config/unity3d/Klei/Oxygen Not Included/mods/Local/EasyFood/
```

Copie :
- `bin/Debug/net472/EasyFood.dll`
- `mod_info.yaml`
- Le dossier `anim/` complet

## Dépannage

### Le sprite n'apparaît pas
- Vérifie que le nom du kanim correspond exactement dans le code
- Les noms sont sensibles à la casse

### Le jeu crash au chargement
- Vérifie que les 3 fichiers (.png, _anim.bytes, _build.bytes) sont présents
- Vérifie que l'animation "object" existe pour les aliments

### La plante n'a pas d'animation de croissance
- Vérifie que les animations `idle_empty`, `grow`, `idle_full` existent

## Ressources

- Kanim Explorer : https://github.com/romen-h/kanim-explorer
- Documentation ONI Modding : https://forums.kleientertainment.com/forums/forum/204-oxygen-not-included-mods-and-tools/
- Kanimal-SE (CLI alternatif) : https://github.com/skairunner/kanimal-SE
