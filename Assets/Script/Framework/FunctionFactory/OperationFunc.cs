/**
 *	操作符类
 */
public enum EFuncOperator
{
	EFO_Set = 0,
	EFO_Inc,
	EFO_Dec,
}
public enum ELimitOperator
{
	ELO_Equal = 0,
	ELO_NotEuqal,
	ELO_MoreThan,
	ELO_LessThan,
	ELO_MoreThanEuqal,
	ELO_LessThanEuqal,
}

public static class OperationFunc
{
	public static long FuncOperatorValue(EFuncOperator eOpt, ref long nValue, long nChange)
	{
		switch (eOpt)
		{
		case EFuncOperator.EFO_Dec:
			nValue -= nChange;
			break;
		case EFuncOperator.EFO_Inc:
			nValue += nChange;
			break;
		case EFuncOperator.EFO_Set:
			nValue = nChange;
			break;
		}
		
		return nValue;
	}
	public static int FuncOperatorValue(EFuncOperator eOpt, ref int nValue, int nChange)
	{
		switch (eOpt)
		{
		case EFuncOperator.EFO_Dec:
			nValue -= nChange;
			break;
		case EFuncOperator.EFO_Inc:
			nValue += nChange;
			break;
		case EFuncOperator.EFO_Set:
			nValue = nChange;
			break;
		}
		
		return nValue;
	}
    public static sbyte FuncOperatorValue(EFuncOperator eOpt, ref sbyte nValue, sbyte nChange)
    {
        switch (eOpt)
        {
            case EFuncOperator.EFO_Dec:
                nValue -= nChange;
                break;
            case EFuncOperator.EFO_Inc:
                nValue += nChange;
                break;
            case EFuncOperator.EFO_Set:
                nValue = nChange;
                break;
        }

        return nValue;
    }
	public static double FuncOperatorValue(EFuncOperator eOpt, ref double dValue, double dChange)
	{
		switch (eOpt)
		{
		case EFuncOperator.EFO_Dec:
			dValue -= dChange;
			break;
		case EFuncOperator.EFO_Inc:
			dValue += dChange;
			break;
		case EFuncOperator.EFO_Set:
			dValue = dChange;
			break;
		}
		
		return dValue;
	}
	
	public static bool LimitOperatorValue(ELimitOperator eOpt, long nCurrValue, long nParam)
	{
		bool bRlt = false;
		switch(eOpt)
		{
		case ELimitOperator.ELO_Equal:
			bRlt = nCurrValue == nParam;
			break;
		case ELimitOperator.ELO_NotEuqal:
			bRlt = nCurrValue != nParam;
			break;
		case ELimitOperator.ELO_MoreThan:
			bRlt = nCurrValue > nParam;
			break;
		case ELimitOperator.ELO_LessThan:
			bRlt = nCurrValue < nParam;
			break;
		case ELimitOperator.ELO_MoreThanEuqal:
			bRlt = nCurrValue >= nParam;
			break;
		case ELimitOperator.ELO_LessThanEuqal:
			bRlt = nCurrValue <= nParam;
			break;
		}
		
		return bRlt;
	}
	
	public static bool LimitOperatorValue(ELimitOperator eOpt, int nCurrValue, int nParam)
	{
		bool bRlt = false;
		switch(eOpt)
		{
		case ELimitOperator.ELO_Equal:
			bRlt = nCurrValue == nParam;
			break;
		case ELimitOperator.ELO_NotEuqal:
			bRlt = nCurrValue != nParam;
			break;
		case ELimitOperator.ELO_MoreThan:
			bRlt = nCurrValue > nParam;
			break;
		case ELimitOperator.ELO_LessThan:
			bRlt = nCurrValue < nParam;
			break;
		case ELimitOperator.ELO_MoreThanEuqal:
			bRlt = nCurrValue >= nParam;
			break;
		case ELimitOperator.ELO_LessThanEuqal:
			bRlt = nCurrValue <= nParam;
			break;
		}
		
		return bRlt;
	}

	public static bool LimitOperatorValue(ELimitOperator eOpt, sbyte nCurrValue, sbyte nParam)
	{
		bool bRlt = false;
		switch(eOpt)
		{
		case ELimitOperator.ELO_Equal:
			bRlt = nCurrValue == nParam;
			break;
		case ELimitOperator.ELO_NotEuqal:
			bRlt = nCurrValue != nParam;
			break;
		case ELimitOperator.ELO_MoreThan:
			bRlt = nCurrValue > nParam;
			break;
		case ELimitOperator.ELO_LessThan:
			bRlt = nCurrValue < nParam;
			break;
		case ELimitOperator.ELO_MoreThanEuqal:
			bRlt = nCurrValue >= nParam;
			break;
		case ELimitOperator.ELO_LessThanEuqal:
			bRlt = nCurrValue <= nParam;
			break;
		}
		
		return bRlt;
	}

	public static bool LimitOperatorValue(ELimitOperator eOpt, bool nCurrValue, bool nParam)
	{
		bool bRlt = false;
		switch(eOpt)
		{
		case ELimitOperator.ELO_Equal:
			bRlt = nCurrValue == nParam;
			break;
		case ELimitOperator.ELO_NotEuqal:
			bRlt = nCurrValue != nParam;
			break;
		}
		
		return bRlt;
	}
}
