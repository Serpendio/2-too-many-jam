using System.Collections.Generic;

public class Spell {
    // Spell info
    public string name {get; set;}

    // Spell stats
    public float castSpeed {get; set;}
    public int manaUsage {get; set;}
    public int damage {get; set;}

    // Spell modifiers
    public List<Modifier> modList = new();
    public Element element {get; set;}

    public Spell() 
        : this(0, 0, 0, new List<Modifier>()) {}
    public Spell(float castSpeed, int manaUsage)
        : this(castSpeed, manaUsage, 0, new List<Modifier>()) {}
    public Spell(float castSpeed, int manaUsage, Element element) 
        : this(castSpeed, manaUsage, element, new List<Modifier>()) {}
    public Spell(float castSpeed, int manaUsage, Element element, List<Modifier> modList) {
        this.castSpeed = castSpeed;
        this.manaUsage = manaUsage;
        this.element = element;
        this.modList = modList;
    }

    public void addModifier(Modifier modifier) {
        modList.Add(modifier);
    }

    public static Spell operator +(Spell s1, Spell s2) {
        s1.modList.AddRange(s2.modList);

        Spell rtnSpell = new()
        {
            castSpeed = s1.castSpeed / s2.castSpeed,
            manaUsage = s1.manaUsage / s2.manaUsage,
            element = s1.element,
            modList = s1.modList

        };

        return rtnSpell;
    }

}

public enum Modifier {
    none,
    cool,
    epic
}

public enum Element {
    none,
    Fire,
    Water,
    Lightning,
    Air,
    Earth
}
