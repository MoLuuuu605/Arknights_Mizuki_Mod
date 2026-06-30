// using Arknights_Mizuki.Scripts.Pools;
// using BaseLib.Abstracts;
// using BaseLib.Utils;
// using MegaCrit.Sts2.Core.Entities.Relics;
// using MegaCrit.Sts2.Core.Localization.DynamicVars;

// namespace Arknights_Mizuki.Scripts.Relics;

// [Pool(typeof(MzkRelicPool))]
// public sealed class ShrinkingSpine : CustomRelicModel
// {
//     public override RelicRarity Rarity => RelicRarity.Uncommon;

//     protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
//     {
//         new DynamicVar("ShrinkPower", 1m)
//     };

//     public override string PackedIconPath => "res://Arknights_Mizuki/images/relics/shrinking_spine.png";

//     protected override string PackedIconOutlinePath => "res://Arknights_Mizuki/images/relics/shrinking_spine.png";

//     protected override string BigIconPath => "res://Arknights_Mizuki/images/relics/shrinking_spine.png";
// }
