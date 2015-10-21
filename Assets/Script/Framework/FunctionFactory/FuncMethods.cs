/**
 *	功能函数类
 */
using System.Collections.Generic;
using Common.Config;
public enum EFuncRet
{
    Continue,
    Break,
    Error,
}
public class HandleTarget
{
    
}
static public class FuncMethods
{
	public delegate EFuncRet FuncExecHandler( HandleTarget Target, FuncData funcdata);
	
	static Dictionary<int,FuncExecHandler>		FuncExec;
	static public void InitFuncMethods(Dictionary<int,FuncExecHandler> dataSource)
	{
	    FuncExec = dataSource;

	}
	static public int HandleFuncExec( HandleTarget Target, int iFuncGroupId)
	{
		if( null == Target )
			return 0;
		FuncGroup funcdataGroup = new FuncGroup();//.Instance.GetFuncGroup( iFuncGroupId );
		if( null == funcdataGroup || null == funcdataGroup.FuncDataList )
			return 0;
		
		//Exec Func
		FuncData ExecData = null;
		int iLoop = 0;
		
		for( ; iLoop < funcdataGroup.FuncDataList.Count; ++iLoop )
		{
			ExecData = funcdataGroup.FuncDataList[iLoop];
		    if (null == ExecData)
		    {
		        continue;
		    }
			
		    FuncExecHandler func = null;
		    FuncExec.TryGetValue(ExecData.Id, out func);
			//
		    if (null == func)
		    {
		        continue;
		    }

            EFuncRet eRet = func(Target, ExecData);
			
			if (EFuncRet.Break == eRet)
			{
				break;
			}
			else if (EFuncRet.Error == eRet)
			{
				Debuger.LogWarning("HandleFuncExec is error, id : " + ExecData.Id.ToString());
			}
		}
		
		if (iLoop == funcdataGroup.FuncDataList.Count)
		{
			return -1;
		}
		
		return iLoop;
	}
	
	
}