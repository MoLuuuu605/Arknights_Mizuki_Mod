import os
import re

cards_dir = r"scripts/cards"

# CardType mapping
card_type_map = {
    "CardType.Attack": "攻击",
    "CardType.Skill": "技能",
    "CardType.Power": "能力",
    "(CardType)3": "能力",
}

# CardRarity mapping
rarity_map = {
    "CardRarity.Basic": "基础",
    "CardRarity.Common": "普通",
    "CardRarity.Uncommon": "罕见",
    "CardRarity.Rare": "稀有",
    "CardRarity.Ancient": "远古",
    "(CardRarity)4": "远古",
}

# CardTag mapping
tag_map = {
    "CardTag.Strike": "打击",
    "CardTag.Defend": "防御",
}

# CardKeyword mapping
keyword_map = {
    "CardKeyword.Exhaust": "消耗",
    "CardKeyword.Ethereal": "虚无",
    "CardKeyword.Innate": "固有",
    "CardKeyword.Retain": "保留",
    "AutoPlay.Autoplay": "自驱动",
}

def parse_file(filepath):
    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()
    
    # Class name
    class_match = re.search(r'public class (\w+)\s*:\s*CustomCardModel', content)
    class_name = class_match.group(1) if class_match else ""
    
    # CanonicalTags
    tags = []
    tags_match = re.search(r'CanonicalTags\s*=>\s*new HashSet<CardTag>\s*\{([^}]+)\}', content)
    if tags_match:
        for tag in re.findall(r'CardTag\.\w+', tags_match.group(1)):
            tags.append(tag_map.get(tag, tag))
    
    # CanonicalKeywords
    keywords = []
    kw_match = re.search(r'CanonicalKeywords\s*=>\s*(.+?);', content, re.DOTALL)
    if kw_match:
        kw_text = kw_match.group(1)
        for kw in re.findall(r'CardKeyword\.\w+|AutoPlay\.\w+', kw_text):
            keywords.append(keyword_map.get(kw, kw))
    
    # OnUpgrade body
    upgrade_lines = []
    in_upgrade = False
    brace_count = 0
    for line in content.split('\n'):
        if 'protected override void OnUpgrade()' in line:
            in_upgrade = True
            continue
        if in_upgrade:
            if '{' in line:
                brace_count += line.count('{')
            if '}' in line:
                brace_count -= line.count('}')
            if brace_count <= 0:
                break
            stripped = line.strip()
            if stripped and not stripped.startswith('//'):
                upgrade_lines.append(stripped)
    
    upgrade_text = "; ".join(upgrade_lines) if upgrade_lines else ""
    
    # Parse upgrade effect into readable Chinese
    upgrade_chinese = parse_upgrade(upgrade_text)
    
    return class_name, tags, keywords, upgrade_chinese

def parse_upgrade(text):
    if not text:
        return "无升级"
    
    effects = []
    
    # UpgradeValueBy
    for m in re.finditer(r'DynamicVars\[?"?(\w+)"?\]?\)?\.UpgradeValueBy\((\d+(?:\.\d+)?)m?\)', text):
        var_name = m.group(1)
        val = m.group(2)
        var_map = {
            "Damage": "伤害", "Block": "格挡", "Cards": "抽牌数",
            "SanityPower": "损伤层数", "SanityBuffPower": "损伤附加层数",
            "SanityThornsPower": "毒素棘刺层数", "StealthPower": "潜行层数",
            "AttackApplySanityPower": "创伤性癔症层数",
            "ErodeTidePower": "侵蚀层数", "SheildPower": "格挡",
            "VulnerablePower": "易伤层数", "WeakPower": "虚弱层数",
            "VigorPower": "伤害提高", "DiscardPicks": "可选牌数",
            "Repeat": "重复次数", "SanityUnlimitPower": "损伤解限层数",
            "Times": "次数",
        }
        name = var_map.get(var_name, var_name)
        effects.append(f"{name}+{val}")
    
    # EnergyCost.UpgradeBy
    for m in re.finditer(r'EnergyCost\.UpgradeBy\((-?\d+)\)', text):
        val = int(m.group(1))
        if val < 0:
            effects.append(f"耗能{val}")
        else:
            effects.append(f"耗能+{val}")
    
    # AddKeyword
    for m in re.finditer(r'AddKeyword\(CardKeyword\.(\w+)\)', text):
        kw = m.group(1)
        kmap = {"Innate": "获得固有", "Retain": "获得保留"}
        effects.append(kmap.get(kw, f"获得{kw}"))
    
    # RemoveKeyword
    for m in re.finditer(r'RemoveKeyword\(CardKeyword\.(\w+)\)', text):
        kw = m.group(1)
        kmap = {"Exhaust": "移除消耗", "Ethereal": "移除虚无"}
        effects.append(kmap.get(kw, f"移除{kw}"))
    
    return ", ".join(effects) if effects else "无变化"

def main():
    files = sorted([f for f in os.listdir(cards_dir) if f.endswith('.cs')])
    
    results = []
    for fname in files:
        fpath = os.path.join(cards_dir, fname)
        class_name, tags, keywords, upgrade = parse_file(fpath)
        if class_name:
            results.append((fname, class_name, tags, keywords, upgrade))
    
    # Print for inspection
    for fname, cname, tags, keywords, upgrade in results:
        print(f"{fname:<30} {cname:<20} tags={tags} kw={keywords} upgrade={upgrade}")

if __name__ == "__main__":
    main()