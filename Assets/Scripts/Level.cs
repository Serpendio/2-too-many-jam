using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Level : MonoBehaviour
{
    private int currentLevel;
    private int currentXP; //Gets reset to 0 when levelling up
    private int xpToLevelUp;

    private void Awake() {
        currentLevel = 1;
        currentXP = 0;
        xpToLevelUp = 20;
    }

    private void Update() {
        if (currentXP >= xpToLevelUp) {
            LevelUp();
        }
    }

    int getCurrentLevel() {
        return currentLevel;
    }

    int getXPToLevelUp() {
        return xpToLevelUp;
    }

    void addXP(int val) {
        currentXP += val;
    }

    void LevelUp() {
        currentLevel += 1;
        currentXP = 0;
        xpToLevelUp += currentLevel * 10;
    }
}
