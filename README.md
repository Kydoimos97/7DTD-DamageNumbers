# Angel's Damage Numbers v3.0

A floating damage numbers mod for 7 Days to Die that shows visual feedback when you damage enemies. Built with defensive coding principles to enhance your gaming experience without adding complexity or breaking your setup.

## Features

- **Floating Damage Numbers**: Customizable colors for normal hits, headshots, kills, and headshot kills
- **Crosshair Hit Markers**: Screen-center indicators that flash on successful hits
- **TextMeshPro Rendering**: High-quality fonts with outline support for better visibility
- **Dual Configuration**: Traditional XML files + optional in-game UI via Gears mod
- **Performance Optimized**: Rate limiting, smart culling, and efficient memory management
- **No Corpse Spam**: Damage numbers only appear on living targets and damaged sourced by the player
- **Memory Safe**: Proper cleanup prevents performance degradation over time

## Philosophy: Modding as Enrichment

This mod embodies the principle that **modding should enrich gaming, not complicate it**:

- **Zero Learning Curve**: Works perfectly with default settings - no configuration required
- **Graceful Degradation**: Missing dependencies result in simpler functionality, never crashes
- **No Prerequisites**: Core features work without any other mods or special setup
- **Respectful Enhancement**: Adds value without changing how you play the game

The goal is **immediate enrichment with zero friction**. You should get visual feedback that makes combat more satisfying without needing to learn new systems, troubleshoot compatibility issues, or change your gameplay habits.

### Technical Philosophy

- **Defensive by Design**: Comprehensive error handling with automatic fallbacks
- **Dependency Isolation**: Optional features (like Gears integration) are completely isolated
- **Fail-Safe Defaults**: Every setting has a sensible default that works well
- **Progressive Enhancement**: Basic XML config works great, Gears UI adds convenience

## Installation

1. Download compiled binaries from: [Nexus](https://www.nexusmods.com/7daystodie/mods/8478)
1. Extract to `Mods/`
2. Disable EAC (required for all DLL mods)
3. **That's it** - the mod works immediately
4. Optional: Install [Gears](https://www.nexusmods.com/7daystodie/mods/4017) for in-game configuration

## Configuration

### Automatic (Recommended)
The mod creates sensible defaults automatically. Most users never need to change anything.

### In-Game UI (Optional)
With [Gears](https://www.nexusmods.com/7daystodie/mods/4017) installed:
- Go to **Options > Mods > Angel's Damage Numbers**
- Use color pickers, sliders, and toggles
- Changes apply instantly - no restart required

### XML File (Traditional)
Edit `Mods/Angel_DamageNumbers/AngelDamageNumbersConfig.xml` for advanced customization.

## Architecture Highlights

- **Shim Pattern**: Gears integration is completely separate - core mod never depends on it
- **Automatic Fallbacks**: Font loading, camera detection, configuration - everything has fallbacks
- **Error Boundaries**: Exceptions in one area don't crash the entire mod
- **Clean Lifecycle**: Proper initialization and cleanup prevent memory leaks

## Troubleshooting

**Not seeing damage numbers?**
- Enable debug logging in config and check F1 console
- Verify `MinimumDamageThreshold` isn't too high
- Check that `PlayerDamageOnly` matches your use case

**Performance issues?**
- Reduce `TextLifetime` or increase `DamageNumberCooldown`
- The mod is optimized, but high-damage weapons can create many numbers

## License

MIT License

## Contributing

1. Fork the repository
2. Create a feature branch
3. Follow the defensive coding principles
4. Submit a pull request

**Code Philosophy**: Prioritize user experience over technical purity. Graceful degradation, fail-safe defaults, and comprehensive error handling. The mod should make games better, never worse.

## Changelog

### v3.0.0
- Added optional Gears integration with in-game configuration UI
- Implemented TextMeshPro rendering with outline support for better visibility
- Fixed memory leaks and improved long-term performance
- Removed damage numbers on corpses/dead entities (reduces visual noise)
- Added automatic XML configuration migration from older versions
- Enhanced error handling with graceful fallbacks throughout the system
- Improved default settings for better out-of-box experience
- Restructured architecture for better maintainability and extensibility
