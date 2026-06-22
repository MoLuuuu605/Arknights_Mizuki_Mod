using Godot.Bridge;
using Arknights_Mizuki.Scripts.Utils;
using HarmonyLib;

namespace Arknights_Mizuki.Scripts;

public static class Entry
{
    public static void Main()
    {
        new Harmony("arknights_mizuki").PatchAll(typeof(Entry).Assembly);
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        ModAssetPreloader.PreloadCombatAssets();
    }
}
