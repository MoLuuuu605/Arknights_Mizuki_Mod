using BaseLib.Abstracts;
using Arknights_Mizuki.Scripts.Ancients;
using Arknights_Mizuki.Scripts.Enemies;
using Godot;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace Arknights_Mizuki.Scripts.Acts;

public sealed class EvolutionSingularityAct : CustomActModel
{
    public EvolutionSingularityAct() : base(-1)
    {
    }

    protected override int BaseNumberOfRooms => 3;

    protected override string CustomRestSiteBackgroundPath => ModelDb.Act<Glory>().RestSiteBackgroundPath;

    protected override string CustomMapBotBgPath => ModelDb.Act<Glory>().MapBotBgPath;

    protected override string CustomMapMidBgPath => ModelDb.Act<Glory>().MapMidBgPath;

    protected override string CustomMapTopBgPath => ModelDb.Act<Glory>().MapTopBgPath;

    public override Color MapTraveledColor => ModelDb.Act<Glory>().MapTraveledColor;

    public override Color MapUntraveledColor => ModelDb.Act<Glory>().MapUntraveledColor;

    public override Color MapBgColor => ModelDb.Act<Glory>().MapBgColor;

    public override string[] BgMusicOptions => ModelDb.Act<Glory>().BgMusicOptions;

    public override string[] MusicBankPaths => ModelDb.Act<Glory>().MusicBankPaths;

    public override string AmbientSfx => ModelDb.Act<Glory>().AmbientSfx;

    public override string ChestSpineSkinNameNormal => ModelDb.Act<Glory>().ChestSpineSkinNameNormal;

    public override string ChestSpineSkinNameStroke => ModelDb.Act<Glory>().ChestSpineSkinNameStroke;

    public override string ChestOpenSfx => ModelDb.Act<Glory>().ChestOpenSfx;

    public override IEnumerable<EncounterModel> GenerateAllEncounters()
    {
        return new EncounterModel[]
        {
            ModelDb.Encounter<SingularityEliteEncounter>(),
            ModelDb.Encounter<SingularityBossEncounter>()
        };
    }

    public override IEnumerable<AncientEventModel> AllAncients
    {
        get
        {
            return new AncientEventModel[]
            {
                ModelDb.AncientEvent<LastTidewatcher>()
            };
        }
    }

    public override IEnumerable<EventModel> AllEvents => Array.Empty<EventModel>();

    public override MapPointTypeCounts GetMapPointTypes(Rng mapRng)
    {
        return new MapPointTypeCounts(0, 0);
    }

    protected override ActMap CustomCreateMap(RunState runState, bool replaceTreasureWithElites)
    {
        return new EvolutionSingularityMap();
    }
}

public sealed class EvolutionSingularityMap : ActMap
{
    private readonly MapPoint?[,] grid = new MapPoint?[7, 4];

    public override MapPoint BossMapPoint { get; }

    public override MapPoint StartingMapPoint { get; }

    protected override MapPoint?[,] Grid => grid;

    public EvolutionSingularityMap()
    {
        StartingMapPoint = new MapPoint(3, 0)
        {
            PointType = MapPointType.Ancient,
            CanBeModified = false
        };

        MapPoint rest = CreatePoint(3, 1, MapPointType.RestSite);
        MapPoint shop = CreatePoint(3, 2, MapPointType.Shop);
        MapPoint elite = CreatePoint(3, 3, MapPointType.Elite);
        BossMapPoint = new MapPoint(3, GetRowCount())
        {
            PointType = MapPointType.Boss,
            CanBeModified = false
        };

        StartingMapPoint.AddChildPoint(rest);
        rest.AddChildPoint(shop);
        shop.AddChildPoint(elite);
        elite.AddChildPoint(BossMapPoint);
        startMapPoints.Add(rest);
    }

    private MapPoint CreatePoint(int col, int row, MapPointType type)
    {
        MapPoint point = new MapPoint(col, row)
        {
            PointType = type,
            CanBeModified = false
        };
        grid[col, row] = point;
        return point;
    }
}
