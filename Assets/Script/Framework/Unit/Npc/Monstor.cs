using BehaviourTree;
using UnityEngine;
using System.Collections;

public class Monstor : Npc,IAIBehaviour
{
    private AIAgent m_AiAgent;

    public AIAgent GetAIAgent()
    {
        return m_AiAgent;
    }
}
