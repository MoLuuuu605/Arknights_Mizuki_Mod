using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;
using Arknights_Mizuki.Scripts.Acts;

namespace Arknights_Mizuki.Scripts.Enemies;

public sealed class SingularityEliteEncounter : CustomEncounterModel
{
    public override RoomType RoomType => RoomType.Elite;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => new MonsterModel[]
    {
        ModelDb.Monster<ColdDisaster>()
    };

    public override string BossNodePath => "res://Arknights_Mizuki/images/ui/run_history/cold_disaster_elite_icon";

    public override MegaSkeletonDataResource? BossNodeSpineResource => null;

    public SingularityEliteEncounter() : base(RoomType.Elite)
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
            (ModelDb.Monster<ColdDisaster>().ToMutable(), null)
        };
    }
}
