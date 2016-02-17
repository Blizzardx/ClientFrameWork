using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Assets.Scripts.Core.Utils;
using BehaviourTree;
using UnityEngine;
using System.Collections.Generic;

public class UIWindow_BTViewPanel : MonoBehaviour
{
    public UIWindow_BTEditorPanel   m_EditorWindow;
    public UIWindow_SelectPanel     m_SelectPanel;
    public GameObject               m_NodeTempalte;
    public AIDebugerTreeRoot        m_Root;
    public AIDebugerTreeParser      m_TreeParser = new AIDebugerTreeParser();
    private static UIWindow_BTViewPanel m_Instance;
    public static string m_strDataPath;
    public UIInput m_InputEditorConfigPath;
    public GameObject m_PanelSettingRoot;

    public static UIWindow_BTViewPanel GetInstance
    {
        get
        {
            return m_Instance;
        }
    }
    public void OnClickSave()
    {
        XElement tmproot = new XElement("root");
        //add current edit root
        m_TreeParser.GenXML(tmproot,m_Root);

        //check other content and check id
        Check(tmproot, m_Root.ID);

        //ready to save
        using (FileStream fs = new FileStream(m_strDataPath, FileMode.Create))
        {
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "\t";
            setting.NewLineChars = "\n";
            setting.Encoding = Encoding.UTF8;
            using (XmlWriter xw = XmlWriter.Create(fs, setting))
            {
                tmproot.WriteTo(xw);
            }
        }
    }
    public void OnClickOpen()
    {
        m_SelectPanel.SelectPlane((ID) =>
        {
            Clear();
            m_Root = m_TreeParser.CreateBehaviourTree(ID, m_NodeTempalte, ComponentTool.FindChild("TreeRoot", null), LoadAIConfig());
            m_Root.Render(0);
        });
    }
    public void OnClickUp()
    {
        var child = m_Root.GetSelectedNode();
        if (null != child)
        {
            var tmpRoot = m_Root.GetNodeRoot(child);
            if (tmpRoot == null)
            {
                return;
            }
            for (int i = 0; i < tmpRoot.m_ChildList.Count; ++i)
            {
                if (tmpRoot.m_ChildList[i] == child)
                {
                    if (i != 0)
                    {
                        var tmp = tmpRoot.m_ChildList[i - 1];
                        tmpRoot.m_ChildList[i - 1] = child;
                        tmpRoot.m_ChildList[i] = tmp;
                        m_Root.Render(0);
                    }
                    break;
                }
            }
        }
    }
    public void OnClickDown()
    {
        var child = m_Root.GetSelectedNode();
        if (null != child)
        {
            var tmpRoot = m_Root.GetNodeRoot(child);
            if (tmpRoot == null)
            {
                return;
            }
            for (int i = 0; i < tmpRoot.m_ChildList.Count; ++i)
            {
                if (tmpRoot.m_ChildList[i] == child)
                {
                    if (i != tmpRoot.m_ChildList.Count-1)
                    {
                        var tmp = tmpRoot.m_ChildList[i + 1];
                        tmpRoot.m_ChildList[i + 1] = child;
                        tmpRoot.m_ChildList[i] = tmp;
                        m_Root.Render(0);
                    }
                    break;
                }
            }
        }
    }
    public void OnClickCreateRoot()
    {
        m_SelectPanel.CreateRoot((id,desc) =>
        {
            if (null != desc)
            {
                Clear();
                AIDebugerTreeRoot root = new AIDebugerTreeRoot(id,m_NodeTempalte);
                root.Desc = desc;
                ComponentTool.Attach(ComponentTool.FindChild("TreeRoot", null).transform, root.m_ObjRoot.transform);
                m_Root = root;
            }
        });
    }
    public void OnClickAddNode()
    {
        var root = m_Root.GetSelectedNode();
        if (null == root)
        {
            return;
        }
        m_EditorWindow.OnCreateNode((node) =>
        {
            if (null != root)
            {
                root.AddChild(node);

                m_Root.Render(0);
            }
        });
    }
    public void OnClickDelNode()
    {
        var child = m_Root.GetSelectedNode();
        if (null != child)
        {
            var tmpRoot = m_Root.GetNodeRoot(child);
            if (null != tmpRoot)
            {
                tmpRoot.RemoveChild(child);
                GameCamera.Destroy(child.m_ObjRoot);
            }
        }
        m_Root.Render(0);
    }
    public void OnClickEditNode()
    {
        var root = m_Root.GetSelectedNode();
        if (root == null)
        {
            return;
        }
        m_EditorWindow.OnEditNode(root,() =>
        {
            
        });
    }
    public void OnDoubleClick(GameObject sender)
    {
        m_Root.SetActive(sender);
        m_Root.Render(0);
    }
    public void OnClick(GameObject sender)
    {
        m_Root.SetSelected(sender);
    }

    public void OnClickPanelSetting()
    {
        m_PanelSettingRoot.SetActive(true);
    }
    public void OnClickSavePanelSetting()
    {
        m_strDataPath = m_InputEditorConfigPath.value;
        if (File.Exists(m_strDataPath))
        {
            PlayerPrefs.SetString("AIConfigPath", m_strDataPath);
            m_PanelSettingRoot.SetActive(false);
        }
    }
    private void Start()
    {
        LogManager.Instance.Initialize();
        m_Instance = this;

        //check
        m_strDataPath = PlayerPrefs.GetString("AIConfigPath", string.Empty);
        if (!File.Exists(m_strDataPath))
        {
            m_PanelSettingRoot.SetActive(true);
        }
    }
    private void Clear()
    {
        GameObject root = ComponentTool.FindChild("TreeRoot", null);
        for (int i = 0; i < root.transform.childCount; ++i)
        {
            GameObject.Destroy(root.transform.GetChild(i).gameObject);
        }
    }
    private void Check(XElement root,int currentRootId)
    {
        var xml = UIWindow_BTViewPanel.LoadAIConfig();
        IEnumerable<XElement> behaviorTrees = xml.Elements(BTDataKey.BEHAVIOUR_TREE_ROOT);
        if (null == behaviorTrees)
        {
            return;
        }

        foreach (XElement element in behaviorTrees)
        {
            int iID = 0;
            int.TryParse(element.Attribute(BTDataKey.BEHAVIOUR_TREE_ID).Value, out iID);
            if (iID != currentRootId)
            {
                root.Add(element);
            }
        }
    }
    public static XElement LoadAIConfig()
    {
        string xmlConfig = FileUtils.ReadStringFile(m_strDataPath);
        return XElement.Parse(xmlConfig);
   
    }
}