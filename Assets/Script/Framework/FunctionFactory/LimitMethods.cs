/**
 *	条件函数类
 */
using UnityEngine;
using Common.Config;
using System.Collections.Generic;

static public class LimitMethods
{
	static Dictionary<int,LimitExecHandler>	LimitExec;

	static public void  InitLimitMethods(Dictionary<int,LimitExecHandler> sourceData)
	{
	    LimitExec = sourceData;
	}
	
	//Limit Exec Func
	static public bool HandleLimitExec( HandleTarget Target, int iLimitGroupId )
    {
        if (null == Target)
        {
            return false;
        }

        bool bResult = false;
	    
	    LimitGroup limitdataGroup = new LimitGroup();//DataManager.Instance.GetLimitGroup( iLimitGroupId );
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

		    LimitExecHandler handler = null;
            if( ! LimitExec.TryGetValue(ExecData.Id, out handler) )
			{
				Debug.LogError("limitId:" + ExecData.Id + " is not found.");
				return false;
			}
			
			if( 0 == limitdataGroup.Logic )
			{
                // Or-logic
                bResult = handler(Target, ExecData);
			    if (true == bResult)
			    {
			        break;
			    }
			}
			else if( 1 == limitdataGroup.Logic )
			{
                // And-logic
                bResult = handler(Target, ExecData);
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
	
	public delegate bool LimitExecHandler( HandleTarget Target, LimitData Limit ); 
}