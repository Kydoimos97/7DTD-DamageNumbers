# Enhanced Floating Damage Numbers

Shows floating damage numbers above targets when you hit them, with special colors for headshots and kills.  
Includes crosshair hit markers and extensive configuration options for fine-tuning the experience.

---

## What it does

- Floating damage numbers that appear above enemies when you damage them
- Different colors for normal hits, headshots, kills, and headshot kills  
- Crosshair hit markers that flash in the center of your screen
- Rate limiting to prevent spam from rapid-fire weapons or bleeding effects
- Position randomization so numbers don't stack on top of each other
- Optional scaling where bigger damage = bigger numbers
- Debug logging to help troubleshoot issues
- Loads config in background so it doesn't slow down game startup

---

## Installation

1. Extract to your Mods directory: `Mods/Angel_DamageNumbers/`
2. Make sure you have:
   ```
   – Mods/Angel_DamageNumbers/AngelDamageNumbers.dll  
   – Config file gets created automatically on first run
   ```
3. **Disable EAC** since this uses a DLL

---

## Configuration Guide

1. Find your config file: `Mods/Angel_DamageNumbers/FloatingDamageNumbersConfig.xml`
2. Open it with Notepad, Notepad++, or any text editor
3. Find the setting you want to change (use Ctrl+F to search)
4. Replace the value between the `> <` brackets
5. Save the file and restart your game

> **Quick Example: To change normal damage color from white to bright red**
> 
> Find this Line
> ```xml
> <NormalDamageColor>#FFFFFF</NormalDamageColor>
> ```
> change it to:
> ```xml
> <NormalDamageColor>#FF0000</NormalDamageColor>
> ```

The config uses normal hex codes: [Color Picker](https://htmlcolorcodes.com/color-picker)

### Colors

**Default Colors**

- Normal damage: **White (#FFFFFF)**
- Headshots: **Gold (#E6B400)** 
- Kills: **Red (#FF4444)**
- Headshot kills: **Dark Red (#8B0000)**

**Better visibility color options:**

- High visibility: **#FF0000 #00FF00 #00FFFF #FFFF00 #FF00FF**
- Gaming style: **#32CD32 #FF6347 #1E90FF #FFA500**
- Easier on eyes: **#FFB6C1 #87CEEB #90EE90**

### Hitmarkers

**Defaults**

- Normal hits: x
- Headshots: x
- Kills: x
- Headshot kills: X

**How to change hitmarkers:**

1. Find lines like: `<NormalHitMarker>x</NormalHitMarker>`
2. Replace the "x" with any symbol from above
3. Example: `<HeadshotKillMarker>☠</HeadshotKillMarker>` for skull on headshot kills

Symbols to copy-paste:
× ☠ ✖ ⚡ ★ ♦ ● ▲ ✓ ➤ • ◆ ◇ ◉ ◎ ✦ ✧ ✶ ✷

**Other Hitmarker Settings:**

- Turn off hitmarkers: Change `<EnableCrosshairMarkers>True</EnableCrosshairMarkers>` to `False`
- Markers flash too fast: Change `<MarkerDuration>0.35</MarkerDuration>` to higher number like `0.5`
- Markers too big/small: Change `<MarkerFontSize>28</MarkerFontSize>` to different number (`20-40 range`)

### Font & Text Settings

**Defaults:**

- Size: **20**
- Visibility time: **1.2** seconds  
- Float speed: **0.85**
- Position: appears **1.5** units above target
- Scaling: **False**

**Common changes:**

- Bigger numbers: Change `<FontSize>20</FontSize>` to **25** or **30**
- Numbers last longer: Change `<TextLifetime>1.2</TextLifetime>` to **2.0**
- Float faster: Change `<FloatSpeed>0.85</FloatSpeed>` to **1.2**
- Appear higher up: Change `<TextOffset>0,1.5,0</TextOffset>` to **0,2.0,0**

**Text Size Damage Scaling:**
To make higher damage show as bigger text

1. Change `<ScaleTextByDamage>False</ScaleTextByDamage>` to `True`
2. Change `<MaxDamageForScale>100</MaxDamageForScale>` to your weapon's max damage
3. For more dramatic size difference, change `<MaxScale>1.3</MaxScale>` to **2.0**

---

## Common Changes

**Visual spam:**

- Hide small damage: Change `<MinimumDamageThreshold>1</MinimumDamageThreshold>` to **5** or **10**
- Less frequent numbers: Change `<DamageNumberCooldown>0.1</DamageNumberCooldown>` to **0.3**

**Shotgun damage not showing all pellets:**

- Change `<DamageNumberCooldown>0.1</DamageNumberCooldown>` to **0.02** or **0**

**Numbers stacking on each other:**

- More spread: Change `<PositionRandomness>0.25</PositionRandomness>` to **0.4**

**Not seeing any numbers:**

- Turn on debug: Change `<EnableDebugLogging>False</EnableDebugLogging>` to `True`
- Check `F1` (in-game) console for error messages
- If you can't figure it out feel free to leave a comment

---

## Compatibility

- Handles rapid-fire weapons and DOT effects without lag
- Shouldn't conflict with other mods unless they also modify damage events  
- Need to disable EAC like all DLL mods
- Config file is created automatically with good defaults

---

## Full Default Config

<details>
<summary>Click to expand full configuration</summary>

```xml
<FloatingDamageNumbersConfig>
    <!--
        Angel's Enhanced Damage Numbers Mod Configuration
        
        Colors: Use hex format like #FFFFFF (white), #FF0000 (red), #e6b400 (gold)
                Can also use 8-digit format #RRGGBBAA for transparency: #FF000080 (semi-transparent red)
        
        Symbols: You can use Unicode symbols like × ☠ ✖ ⚡ ★ ♦ ● ▲ ✓ ➤
        
        Most IDEs will show color previews for hex values and provide color pickers!
       -->
      
     <Debug>
      <!-- Enable debug messages in Unity console - set to true to troubleshoot issues (default: false) -->
      <EnableDebugLogging>False</EnableDebugLogging>
     </Debug>
     
     <DamageNumbers>
      <!-- Minimum damage to show numbers (0 = show all damage) -->
      <MinimumDamageThreshold>1</MinimumDamageThreshold>
      <!-- Minimum time between damage numbers in seconds (prevents spam) -->
      <DamageNumberCooldown>0.1</DamageNumberCooldown>
      <!-- Size of damage text (default: 20) -->
      <FontSize>20</FontSize>
      <!-- How long text is visible in seconds (default: 1.2) -->
      <TextLifetime>1.2</TextLifetime>
      <!-- Speed text floats upward (default: 0.85) -->
      <FloatSpeed>0.85</FloatSpeed>
      <!-- Offset from entity position in X,Y,Z format (default: 0,1.5,0) -->
      <TextOffset>0,1.5,0</TextOffset>
     </DamageNumbers>
     
     <Colors>
      <!-- Normal damage color (default: white) -->
      <NormalDamageColor>#FFFFFF</NormalDamageColor>
      <!-- Headshot damage color (default: gold) -->
      <HeadshotDamageColor>#E6B400</HeadshotDamageColor>
      <!-- Killing blow color (default: red) -->
      <KillDamageColor>#FF4444</KillDamageColor>
      <!-- Headshot kill color (default: dark red) -->
      <HeadshotKillDamageColor>#8B0000</HeadshotKillDamageColor>
     </Colors>
     
     <CrosshairMarkers>
      <!-- Enable/disable crosshair hit markers (default: true) -->
      <EnableCrosshairMarkers>True</EnableCrosshairMarkers>
      <!-- How long markers are visible in seconds (default: 0.35) -->
      <MarkerDuration>0.35</MarkerDuration>
      <!-- Size of crosshair marker symbols (default: 28) -->
      <MarkerFontSize>28</MarkerFontSize>
      <!-- Symbol for normal hits (default: x) -->
      <NormalHitMarker>x</NormalHitMarker>
      <!-- Symbol for kills (default: x) -->
      <KillMarker>x</KillMarker>
      <!-- Symbol for headshots (default: x) -->
      <HeadshotMarker>x</HeadshotMarker>
      <!-- Symbol for headshot kills (default: X) -->
      <HeadshotKillMarker>X</HeadshotKillMarker>
      <!-- Normal hit marker color (default: white) -->
      <NormalMarkerColor>#FFFFFF</NormalMarkerColor>
      <!-- Kill marker color (default: red) -->
      <KillMarkerColor>#FF4444</KillMarkerColor>
      <!-- Headshot marker color (default: gold) -->
      <HeadshotMarkerColor>#E6B400</HeadshotMarkerColor>
      <!-- Headshot kill marker color (default: dark red) -->
      <HeadshotKillMarkerColor>#8B0000</HeadshotKillMarkerColor>
     </CrosshairMarkers>
     
    <Advanced>
      <!-- Only show damage caused by player (default: true) -->
      <PlayerDamageOnly>True</PlayerDamageOnly>
      <!-- Slightly randomize text position to prevent overlap (default: true) -->
      <RandomizePosition>True</RandomizePosition>
      <!-- Amount of position randomization (default: 0.25) -->
      <PositionRandomness>0.25</PositionRandomness>
      <!-- Scale text size based on damage amount (default: false) -->
      <ScaleTextByDamage>False</ScaleTextByDamage>
      <!-- Minimum text scale multiplier when scaling by damage (default: 0.8) -->
      <MinScale>0.8</MinScale>
      <!-- Maximum text scale multiplier when scaling by damage (default: 1.3) -->
      <MaxScale>1.3</MaxScale>
      <!-- Damage amount that gives maximum scale (default: 100) -->
      <MaxDamageForScale>100</MaxDamageForScale>
     </Advanced>
     
</FloatingDamageNumbersConfig>
```

</details>

---
