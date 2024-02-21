using System.Collections.Generic;

public class Spell {
    // Spell info
    public string name {get; set;}

    // Spell stats
    public Stats stats { get; set; }

    // Spell modifiers
    public List<SpellModifier> spellMods {get;}
    public Element element {get; set;}

    public Spell() 
        : this(0, 0, 0) {}
    public Spell(float castSpeed, int manaUsage)
        : this(castSpeed, manaUsage, 0) {}
    public Spell(float castSpeed, int manaUsage, Element element) {
        stats.CastSpeed = castSpeed;
        stats.ManaUsage = manaUsage;
        this.element = element;
    }

    // Doesn't work right now
    public static Spell operator +(Spell s1, Spell s2) {
        Spell rtnSpell = new();

        return rtnSpell;
    }

}

public enum Element {
    none,
    Fire,
    Water,
    Lightning,
    Air,
    Earth
}
