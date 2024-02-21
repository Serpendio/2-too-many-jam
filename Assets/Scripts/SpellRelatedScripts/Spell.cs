using System.Collections.Generic;

public class Spell {
    // Spell info
    public string name {get; set;}

    // Spell stats
    public float castSpeed {get; set;}
    public int manaUsage {get; set;}
    public int damage {get; set;}

    // Spell modifiers
    public Element element {get; set;}

    public Spell() 
        : this(0, 0, 0) {}
    public Spell(float castSpeed, int manaUsage)
        : this(castSpeed, manaUsage, 0) {}
    public Spell(float castSpeed, int manaUsage, Element element) {
        this.castSpeed = castSpeed;
        this.manaUsage = manaUsage;
        this.element = element;
    }

    public static Spell operator +(Spell s1, Spell s2) {
        Spell rtnSpell = new()
        {
            castSpeed = s1.castSpeed / s2.castSpeed,
            manaUsage = s1.manaUsage / s2.manaUsage,
            element = s1.element,

        };

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
