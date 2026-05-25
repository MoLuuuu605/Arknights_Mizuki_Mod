using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Arknights_Mizuki.Scripts.keywords;

public class AutoPlay
{
    // 自定义枚举的名字。最终会变成{前缀}-{枚举值大写}的形式，例如TEST-UNIQUE
    [CustomEnum("AutoPlay")]
    // 放在原版卡牌描述的位置，这里是卡牌描述的前面
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Autoplay;

}
public class KWSeaCreature
{
    [CustomEnum("KWSeaCreature")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword SeaCreature;

}
public class KWHuman
{
    [CustomEnum("KWHuman")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Human;

}

public class Echo1
{
    [CustomEnum("Echo1")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Echo;

}

public class Echo2
{
    [CustomEnum("Echo2")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Echo;

}

public class Echo3
{
    [CustomEnum("Echo3")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Echo;

}
public class Sputter
{
    [CustomEnum("Sputter")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword sputter;
}