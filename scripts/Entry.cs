using Godot.Bridge;

namespace Arknights_Mizuki.Scripts;

public static class Entry
{
    public static void Main()
    {
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
    }
}
