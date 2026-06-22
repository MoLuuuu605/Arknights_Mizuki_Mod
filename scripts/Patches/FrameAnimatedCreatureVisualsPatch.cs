using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Arknights_Mizuki.Scripts.Patches;

[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class FrameAnimatedCreatureVisualsTriggerPatch
{
    public static void Postfix(NCreature __instance, string trigger)
    {
        PlayTrigger(__instance, trigger);
    }

    public static void PlayTrigger(NCreature creature, string trigger)
    {
        AnimatedSprite2D? sprite = creature.Visuals.GetNodeOrNull<AnimatedSprite2D>("%Visuals");
        if (sprite == null || !sprite.HasMethod("play_trigger"))
            return;

        sprite.Call("play_trigger", trigger);
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class FrameAnimatedCreatureVisualsDeathPatch
{
    public static void Prefix(NCreature __instance)
    {
        FrameAnimatedCreatureVisualsTriggerPatch.PlayTrigger(__instance, "Dead");
    }
}
