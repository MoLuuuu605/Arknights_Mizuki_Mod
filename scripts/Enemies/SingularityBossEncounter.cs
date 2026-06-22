using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;
using Arknights_Mizuki.Scripts.Acts;

namespace Arknights_Mizuki.Scripts.Enemies;

public sealed class SingularityBossEncounter : CustomEncounterModel
{
    private const string IconPath = "res://Arknights_Mizuki/images/ui/run_history/izumik_boss_encounter_icon";

    public override RoomType RoomType => RoomType.Boss;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => new MonsterModel[]
    {
        ModelDb.Monster<Izumik>(),
        ModelDb.Monster<IzumikOffspring>()
    };

    public override string BossNodePath => IconPath;

    public override string? CustomRunHistoryIconPath => IconPath + ".png";

    public override string? CustomRunHistoryIconOutlinePath => IconPath + "_outline.png";

    public override MegaSkeletonDataResource? BossNodeSpineResource => null;

    public SingularityBossEncounter() : base(RoomType.Boss)
    {
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act is EvolutionSingularityAct;
    }

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        return new (MonsterModel, string?)[]
        {
            (ModelDb.Monster<Izumik>().ToMutable(), null)
        };
    }
}
