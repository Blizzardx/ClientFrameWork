using UnityEngine;
using System.Collections;

public abstract class StageBase
{
    public GameStateType StageType;

    public StageBase(GameStateType type)
    {
        StageType = type;
    }
    abstract public void StartStage();
    abstract public void EndStage();
}
