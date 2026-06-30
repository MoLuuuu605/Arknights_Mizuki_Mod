using System.Runtime.InteropServices;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Godot;
using Arknights_Mizuki.Scripts.Cards;
using Arknights_Mizuki.Scripts.Utils;
using Arknights_Mizuki.Scripts.Relics;

namespace Arknights_Mizuki.Scripts.Characters;

public class Mizuki : PlaceholderCharacterModel
{

    public override Color NameColor => new(0.3f, 0.5f, 1f);

    public override Color EnergyLabelOutlineColor => new(0.1f, 0.2f, 0.7f);

    public override Color MapDrawingColor => new(0.3f, 0.5f, 1f);

    public override CharacterGender Gender => CharacterGender.Masculine;

    public override int StartingHp => 70;

    public override string CustomVisualPath => "res://Arknights_Mizuki/scenes/character.tscn";

    public override string CustomIconTexturePath => "res://Arknights_Mizuki/images/icon.svg";

    public override CreatureAnimator GenerateAnimator(MegaSprite controller) => SpineAnimatorFactory.Create(controller, cast: "Skill_1", buff: "Skill_1", summon: "Skill_1");

    public override string CustomEnergyCounterPath => "res://Arknights_Mizuki/scenes/energy_counter.tscn";

    public override string CustomCharacterSelectBg => "res://Arknights_Mizuki/scenes/bg.tscn";

    public override string CustomCharacterSelectIconPath => "res://Arknights_Mizuki/images/icon.png";

    public override string CustomCharacterSelectLockedIconPath => "res://Arknights_Mizuki/images/icon.png";

    public override string CustomRestSiteAnimPath => "res://Arknights_Mizuki/scenes/rest_site.tscn";

    public override string CustomMerchantAnimPath => "res://Arknights_Mizuki/scenes/test_character_merchant.tscn";

    public override string CharacterSelectSfx => "res://Arknights_Mizuki/audios/select.wav";

    public override string CharacterTransitionSfx => "res://Arknights_Mizuki/audios/pass.wav";

    public override string CustomIconPath => "res://Arknights_Mizuki/scenes/iconpath.tscn";

    // 人物池子
    public override CardPoolModel CardPool => (CardPoolModel)(object)ModelDb.CardPool<Pools.MzkCardPool>();
    public override RelicPoolModel RelicPool => (RelicPoolModel)(object)ModelDb.RelicPool<Pools.MzkRelicPool>();
    public override PotionPoolModel PotionPool => (PotionPoolModel)(object)ModelDb.PotionPool<Pools.MzkPotionPool>();

    // 初始卡组
    public override IEnumerable<CardModel> StartingDeck => [
        (CardModel)ModelDb.Card<MzkStrike>(),
        (CardModel)ModelDb.Card<MzkStrike>(),
        (CardModel)ModelDb.Card<MzkStrike>(),
        (CardModel)ModelDb.Card<MzkStrike>(),
        (CardModel)ModelDb.Card<MzkStrike>(),
        (CardModel)ModelDb.Card<MzkDefence>(),
        (CardModel)ModelDb.Card<MzkDefence>(),
        (CardModel)ModelDb.Card<MzkDefence>(),
        (CardModel)ModelDb.Card<MzkDefence>(),
        (CardModel)ModelDb.Card<Awaken>()
    ];

    // 初始遗物（暂时空，等确认游戏内遗物类名后替换）
    public override IReadOnlyList<RelicModel> StartingRelics => [
        (RelicModel)(object)ModelDb.Relic<MzkTreeBranch>(),
        (RelicModel)(object)ModelDb.Relic<Relics.Key>()
    ];

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}