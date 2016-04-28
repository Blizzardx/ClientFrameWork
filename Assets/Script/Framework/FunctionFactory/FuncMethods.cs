/**
 *	功能函数类
 */
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Config;

public enum EFuncRet
{
    Continue,
    Break,
    Error,
}
public abstract class FuncMethodsBase
{ 
    private int m_iId;

    public int GetId()
    {
        return m_iId;
    }
    public FuncMethodsBase()
    {
        DecodeId();
    }
    private void DecodeId()
    {
        m_iId = -1;
        string name = this.GetType().ToString();
        if (!name.StartsWith("Func_"))
        {
            UnityEngine.Debug.LogError("Function method with wrong name " + name);
            return;
        }

        // 
        var tmpname = name.Substring(5);
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
    public  abstract EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata,FuncContext context);
}
static public class FuncMethods
{
    static Dictionary<int, FuncMethodsBase> FuncExec;
    static public void InitFuncMethods(List<FuncMethodsBase> dataSource)
    {
        FuncExec = new Dictionary<int, FuncMethodsBase>();
        foreach (var elem in dataSource)
        {
            if (elem.GetId() == -1)
            {
                continue;
            }
            FuncExec.Add(elem.GetId(), elem);
        }
    }
    static public int HandleFuncExec(HandleTarget Target, int iFuncGroupId, FuncContext context)
	{
		if( null == Target || 0 == iFuncGroupId)
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

            EFuncRet eRet = func.FuncExecHandler(Target, ExecData,context);
			
			if (EFuncRet.Break == eRet)
			{
				break;
			}
			else if (EFuncRet.Error == eRet)
			{
				UnityEngine.Debug.LogWarning("HandleFuncExec is error, id : " + ExecData.Id.ToString());
			}
		}
		
		if (iLoop == funcdataGroup.FuncDataList.Count)
		{
			return -1;
		}
		
		return iLoop;
	}
	
	
}