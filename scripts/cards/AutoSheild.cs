using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class AutoSheild : CustomCardModel
{
	private const int energyCost = 1;

	private const CardType type = CardType.Power;

	private const CardRarity rarity = CardRarity.Uncommon;

	private const TargetType targetType = TargetType.Self;

	private const bool shouldShowInCardLibrary = true;

	public override string PortraitPath => "res://Arknights_Mizuki/images/cards/sheild.png";

	protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[2]
    {
		(DynamicVar)new EnergyVar(2),
		(DynamicVar)new PowerVar<SheildPower>(2m)
	};

	public AutoSheild()
		: base(energyCost, (CardType)3, (CardRarity)3, (TargetType)0, true, true)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay _)
	{
		await PowerCmd.Apply<SheildPower>(choiceContext, ((CardModel)this).Owner.Creature, ((CardModel)this).DynamicVars["SheildPower"].BaseValue, ((CardModel)this).Owner.Creature, (CardModel)(object)this, false);
	}

	protected override void OnUpgrade()
	{
		((CardModel)this).DynamicVars["SheildPower"].UpgradeValueBy(1);
	}
}
