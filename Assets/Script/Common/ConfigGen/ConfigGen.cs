using System.Collections.Generic;
using System.Text;
using System.Threading;
using Assets.Scripts.Framework.Network;
using NetFramework.Auto;
using Thrift.Protocol;
using UnityEngine;
using System.Collections;

public class ConfigGen : MonoBehaviour 
{

	// Use this for initialization
	void Start ()
	{
	    LogManager.Instance.Initialize();

        ArmyInfo tmp = new ArmyInfo();
	    tmp.Id = 10;
	    tmp.MemberInfoList = new List<ArmyMemberInfo>();
	    for (int i = 0; i < 200; ++i)
	    {
	        ArmyMemberInfo elem = new ArmyMemberInfo();
	        elem.CharId = 100;
	        elem.JoinTime = 2;
	        elem.Level = 52;
            tmp.MemberInfoList.Add(elem);
	    }
	    ThriftGen.SaveData(tmp, Application.streamingAssetsPath + "/armyinfo.byte");

        
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (Input.GetKeyDown(KeyCode.A))
	    {
            ArmyInfo tmp = ResourceManager.Instance.DecodeTemplate<ArmyInfo>(Application.streamingAssetsPath + "/armyinfo.byte");
	        Debuger.Log(tmp.Id);
            for (int i = 0; i < tmp.MemberInfoList.Count; ++i)
            {
                ArmyMemberInfo elem = tmp.MemberInfoList[i];
                Debuger.Log(elem.CharId );
                Debuger.Log(elem.JoinTime );
                Debuger.Log(elem.Level);
            }
	    }
	}
}
