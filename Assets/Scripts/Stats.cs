using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public float Damage {get; set;}
    public float ManaUsage {get; set;}
    public float CastSpeed { get; set; }

    public Stats() {
        Damage = 0;
        ManaUsage = 0;
        CastSpeed = 0;
    }

}
