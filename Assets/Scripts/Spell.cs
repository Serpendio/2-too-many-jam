public class Spell {
    public string name {get; set;}
    public float _castSpeed {get; set;}
    public int _manaUsage {get; set;}
    public Element _element {get; set;}

    public Spell(float castSpeed, int manaUsage, Element element) 
    {
        _castSpeed = castSpeed;
        _manaUsage = manaUsage;
        _element = element;
    }
}

public enum Element {
    Fire,
    Water,
    Lightning,
    Air,
    Earth
}
