

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FuncContext 
{
	private readonly Dictionary<ContextKey, object> DIC = new Dictionary<ContextKey, object>();


	public enum ContextKey {
		ShowMsg,
        TargetId,
        LimitId,
        FunctionId,
        User,
        Target,
	}


	private FuncContext()
	{

	}

	public static FuncContext Create()
	{
		FuncContext context = new FuncContext ();
		return context;
	}

	public void Put(ContextKey key, object value)
	{
		if (!DIC.ContainsKey (key)) 
		{
			DIC.Add(key, value);
		}
		else
		{
			DIC[key] = value;
		}
	}

	public object Get(ContextKey key)
	{
		if(DIC.ContainsKey(key))
		{
			return DIC[key];
		}
		return null;
	}

    public bool ContainsKey(ContextKey key)
    {
        return DIC.ContainsKey(key);
    }
}