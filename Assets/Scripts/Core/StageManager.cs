using Core;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

//Keeps track of user's current stage - increases when user beats a boss. Stage determines enemy difficulty.
public class StageManager : MonoBehaviour
{
    private int stage;
    private int levelsPerStage;
    private int maxStage;

    private void Awake()
    {
        Locator.ProvideStageManager(this);
        stage = 1;
        levelsPerStage = 10;
        maxStage = 3;
    }

    public int getStage() { return stage; }
    public int getLevelsPerStage() { return levelsPerStage; }
    public int getMaxStage() { return maxStage; }
    public void NextStage() => stage += 1;
}
