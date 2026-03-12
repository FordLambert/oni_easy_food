# Easy Food - Potato Mod

A mod for Oxygen Not Included that adds an easy-to-grow Potato plant and a cooked recipe.

**Version:** 0.1.0

## Features

### Potato Plant
- **Growth time:** 4 cycles
- **Harvest:** 6 potatoes per harvest
- **Water requirement:** 20 kg/cycle
- **Temperature tolerance:** -20°C to +50°C
- **No fertilizer required**

### Potato (Raw)
- **Calories:** 1,200 kcal
- **Quality:** Mediocre (-1)
- **Shelf life:** 8 cycles

### Grilled Potato
- **Calories:** 1,500 kcal
- **Quality:** Good (0)
- **Shelf life:** 12 cycles
- **Recipe:** 1 Potato → 1 Grilled Potato (Cooking Station, 30s)

### Obtaining Potatoes
- 5 Potatoes are given at the start of each new game
- Plant a Potato to grow 6 more (no separate seeds needed!)

## Installation

### From Release
1. Download the latest release
2. Extract the contents to your ONI mods folder:
   - **Windows:** `%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\EasyFood\`
   - **Linux:** `~/.config/unity3d/Klei/Oxygen Not Included/mods/Local/EasyFood/`
3. Launch the game and enable the mod in the Mods menu

### Required Files
```
EasyFood/
├── EasyFood.dll
└── mod_info.yaml
```

## Building from Source

### Prerequisites
- .NET SDK 6.0 or higher
- Game DLLs from `OxygenNotIncluded_Data/Managed/`

### Setup
1. Clone the repository
2. Copy the following DLLs from your game installation to `lib/`:
   - `Assembly-CSharp.dll`
   - `Assembly-CSharp-firstpass.dll`
   - `0Harmony.dll`
   - `UnityEngine.dll`
   - `UnityEngine.CoreModule.dll`
3. Build with `dotnet build`
4. Output will be in `bin/Debug/net472/`

## Compatibility

- **Game version:** U57-707956 and above
- **DLC:** Compatible with base game and Spaced Out!

## Changelog

### 0.1.0
- Initial release
- Added Potato Plant
- Added Potato (raw food)
- Added Grilled Potato (cooked food)
- Added Cooking Station recipe

## License

MIT License

## Custom Sprites

Custom sprite images are available in `images/potatoes/`. To convert them to Kanim format, see the guide: [docs/GUIDE_SPRITES_WINDOWS.md](docs/GUIDE_SPRITES_WINDOWS.md)

## Credits

- Sprites: Currently using placeholder sprites from base game
- Custom sprite PNGs included for future use
