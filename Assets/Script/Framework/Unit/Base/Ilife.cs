using BehaviourTree;
using UnityEngine;
using System.Collections.Generic;

public interface Ilife
{
    int GetInstanceId();
}
public interface IStateMachineBehaviour : Ilife
{
    StateMachine GetStateController();
}
public interface IAIBehaviour : Ilife
{
    AIAgent GetAIAgent();
}
public interface ICountBehaviour : Ilife
{
    CountData GetCountData();
}
public interface ITransformBehaviour : Ilife
{
    TransformData GetTransformData();
}
