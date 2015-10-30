/**
 *	功能函数类
 */
using System.Collections.Generic;
using Config;

public enum EFuncRet
{
    Continue,
    Break,
    Error,
}
public abstract class FuncMethodsBase
{
    public  abstract EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata);
}
static public class FuncMethods
{
    static Dictionary<int, FuncMethodsBase> FuncExec;
    static public void InitFuncMethods(Dictionary<int, FuncMethodsBase> dataSource)
	{
	    FuncExec = dataSource;
	}
	static public int HandleFuncExec( HandleTarget Target, int iFuncGroupId)
	{
		if( null == Target )
			return 0;
		FuncGroup funcdataGroup = ConfigManager.Instance.GetFuncGroup( iFuncGroupId );
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

            FuncMethodsBase func = null;
		    FuncExec.TryGetValue(ExecData.Id, out func);
			//
		    if (null == func)
		    {
		        continue;
		    }

            EFuncRet eRet = func.FuncExecHandler(Target, ExecData);
			
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