using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class UIBase
{
    protected GameObject m_ObjRoot;
    private bool m_bIsLoading;
    private object m_OpenParam;
    private string m_strResurceName;
    private bool m_bCallOpenOnCompleted;

    #region public interface
    public void DoCreate()
    {
        OnCreate();
        BeginLoadResource();
    }
    public void DoInit()
    {
        OnInit();
    }
    public void DoOpen(object param)
    {
        if (m_bIsLoading)
        {
            // open later
            m_OpenParam = param;
            m_bCallOpenOnCompleted = true;
        }
        else
        {
            m_ObjRoot.SetActive(true);
            OnOpen(param);
        }
    }
    public void DoClose()
    {
        OnClose();
        GameObject.Destroy(m_ObjRoot);
    }
    public void DoHide()
    {
        OnHide();
        m_ObjRoot.SetActive(true);
    }
    #endregion

    #region system function

    private void BeginLoadResource()
    {
        m_bIsLoading = true;
        if (null != m_ObjRoot)
        {
            OnLoadDone();
        }
        else
        {
            // begin load ui window resource from bundle or build in resource
            ResourceManager.Instance.LoadBuildInAssetsAsync(m_strResurceName,AssetType.UI,  (obj) =>
            {
                m_ObjRoot = GameObject.Instantiate(obj) as GameObject;
            });
        }
    }
    protected void SetResourceName(string name)
    {
        m_strResurceName = name;
    }
    protected void SetResource(GameObject root)
    {
        m_ObjRoot = root;
    }
    protected void Hide()
    {
        UIManager.Instance.HideWindow(GetType());
    }
    protected void Close()
    {
        UIManager.Instance.CloseWindow(GetType());
    }
    private void OnLoadDone()
    {
        m_bIsLoading = false;
        if (m_bCallOpenOnCompleted)
        {
            m_bCallOpenOnCompleted = false;
            OnOpen(m_OpenParam);
        }
    }
    #endregion

    protected virtual void OnCreate()
    {
        
    }
    protected virtual void OnInit()
    {
        
    }
    protected virtual void OnOpen(object parma)
    {
        
    }
    protected virtual void OnClose()
    {
        
    }
    protected virtual void OnHide()
    {
        
    }
}
