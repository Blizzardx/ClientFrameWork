/**
 *	条件函数类
 */
using UnityEngine;
using Config;
using System.Collections.Generic;

public abstract class LimitMethodsBase
{
    private int m_iId;

    public int GetId()
    {
        return m_iId;
    }

    public LimitMethodsBase()
    {
        DecodeId();
    }
    private void DecodeId()
    {
        m_iId = -1;
        string name = this.GetType().ToString();
        if (!name.StartsWith("Limit_"))
        {
            UnityEngine.Debug.LogError("Function method with wrong name " + name);
            return;
        }

        // 
        var tmpname = name.Substring(6);
        int index = tmpname.LastIndexOf("_");
        if (index < 0)
        {
            UnityEngine.Debug.LogError("Function method with wrong name " + name);
            return;
        }
        tmpname = tmpname.Substring(0, index);
        int id = 0;
        if (!int.TryParse(tmpname, out id))
        {
            UnityEngine.Debug.LogError("Function method with wrong name " + name);
            return;
        }
        m_iId = id;
    }
    public abstract bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context); 
}
static public class LimitMethods
{
    static Dictionary<int, LimitMethodsBase> LimitExec;

    static public void InitLimitMethods(List<LimitMethodsBase> sourceData)
    {
        LimitExec = new Dictionary<int, LimitMethodsBase>();
        foreach (var elem in sourceData)
        {
            if (elem.GetId() == -1)
            {
                continue;
            }
            LimitExec.Add(elem.GetId(), elem);
        }
    }
	
	//Limit Exec Func
    static public bool HandleLimitExec(HandleTarget Target, int iLimitGroupId, FuncContext context)
    {
        if (null == Target || 0 == iLimitGroupId)
        {
            return true;
        }

        bool bResult = false;

	    LimitGroup limitdataGroup = ConfigManager.Instance.GetLimitGroup( iLimitGroupId );
	    if (null == limitdataGroup || null == limitdataGroup.LimitDataList)
	    {
	        return bResult;
	    }
		
		//Exec Limit
		LimitData ExecData;
		for( int iLoop = 0; iLoop < limitdataGroup.LimitDataList.Count; ++iLoop )
		{
			ExecData = limitdataGroup.LimitDataList[iLoop];
			if( null == ExecData )
				continue;

            LimitMethodsBase handler = null;
            if( ! LimitExec.TryGetValue(ExecData.Id, out handler) )
			{
				Debug.LogError("limitId:" + ExecData.Id + " is not found.");
				return false;
			}
			
			if( 0 == limitdataGroup.Logic )
			{
                // Or-logic
                bResult = handler.LimitExecHandler(Target, ExecData,context);
			    if (true == bResult)
			    {
			        break;
			    }
			}
			else if( 1 == limitdataGroup.Logic )
			{
                // And-logic
                bResult = handler.LimitExecHandler(Target, ExecData,context);
				if( false == bResult )
				{
					break;
				}
			}
			else
			{
				Debug.LogWarning( "The limitdataGroup's logic is error!" );
				break;
			}
		}
		
		return bResult;
	}
}