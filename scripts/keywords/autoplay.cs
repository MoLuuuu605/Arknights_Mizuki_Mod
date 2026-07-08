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

public class Monster1des
{
    [CustomEnum("Monster1des")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword monster1des;
}
public class Monster1
{
    [CustomEnum("Monster1")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword monster1;
}
public class Monster2
{
    [CustomEnum("Monster2")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword monster2;
}
public class Monster2des
{
    [CustomEnum("Monster2des")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword monster2des;
}