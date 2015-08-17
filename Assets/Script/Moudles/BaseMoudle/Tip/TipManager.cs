using System;
using UnityEngine;
using System.Collections;

public class TipManager : Singleton<TipManager>
{
    #region alert
    public void Alert(string content)
    {
        
    }
    public void Alert(string title, string content)
    {
        
    }
    public void Alert(string title, string content, string correct, Action<bool> callBack)
    {
        
    }
    public void Alert(string title, string content, string correct, string cancle, Action<bool> callBack)
    {

    }
    #endregion


    #region text tip
    public void Tip(string content,LabelFontSizeDefine fontSize = LabelFontSizeDefine.LABEL_SIZE_25,CSS.Styles fontStyle = CSS.Styles.Style_Green_LineBlack)
    {
        
    }
    #endregion

    #region roll tip
    public void RollTip(string content)
    {
        
    }
    #endregion

    #region tip
    #endregion
}
