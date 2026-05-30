using Godot.Bridge;
using Arknights_Mizuki.Scripts.Utils;

namespace Arknights_Mizuki.Scripts;

public static class Entry
{
    public static void Main()
    {
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        ModAssetPreloader.PreloadCombatAssets();
    }
}
