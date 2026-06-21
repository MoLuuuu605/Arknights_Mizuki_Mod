using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Arknights_Mizuki.Scripts.Enemies;

public class KnightBossEncounter : CustomEncounterModel
{
    private const string BossNodeIconPath = "res://Arknights_Mizuki/images/ui/run_history/knight_boss_encounter_icon";
    private const string RunHistoryIconPath = BossNodeIconPath + ".png";
    private const string RunHistoryIconOutlinePath = BossNodeIconPath + "_outline.png";

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Knight>()];

    public override bool IsValidForAct(ActModel act) => act.ActNumber() == 3;

    public override string BossNodePath => BossNodeIconPath;

    public override MegaSkeletonDataResource? BossNodeSpineResource => null;

    public override string CustomRunHistoryIconPath => RunHistoryIconPath;

    public override string CustomRunHistoryIconOutlinePath => RunHistoryIconOutlinePath;

    public override float GetCameraScaling() => 0.9f;

    public KnightBossEncounter() : base(RoomType.Boss)
    {
    }

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<Knight>().ToMutable(), null)
    ];
}
