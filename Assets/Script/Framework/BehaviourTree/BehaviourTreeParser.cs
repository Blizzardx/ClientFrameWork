using System.Xml.Linq;
using UnityEngine;
using System.Collections;
using BehaviourTree;
using System.Xml;
using System.Collections.Generic;

namespace BehaviourTree
{
	public class BehaviourTreeParser
	{
        private XElement m_Config;

		private static readonly BehaviourTreeParser g_Instance = new BehaviourTreeParser();
		public static  BehaviourTreeParser Instance
		{
			get
			{
				return g_Instance;
			}
		}
		public BTRoot CreateBehaviourTree( int iId )
		{
			if( iId <= 0 )
			{
				return null;
			}

			BTRoot root = new BTRoot( iId );
		    if (null == m_Config)
		    {
		        m_Config = ConfigManager.Instance.GetAIConfigTable();
		    }
            ParseBTXml(root, m_Config);
			return root;
		}
		private void ParseBTXml( BTRoot root, XElement xml )
		{
			IEnumerable<XElement> behaviorTrees = xml.Elements( BTDataKey.BEHAVIOUR_TREE_ROOT );
			if( null == behaviorTrees )
			{
				return;
			}

			foreach( XElement element in behaviorTrees )
			{
				int iID = 0;
				int.TryParse( element.Attribute( BTDataKey.BEHAVIOUR_TREE_ID ).Value, out iID );
				if( iID != root.ID )
				{
					continue;
				}

				ParseBTNode( root, element );
				break;
			}
		}
		private void ParseBTNode( BTNode root, XElement btNodeE )
		{
			IEnumerable<XElement> nodes = btNodeE.Elements( BTDataKey.NODE_NAME );
			if( null == nodes )
			{
				return;
			}

			foreach( XElement element in nodes )
			{
				BTNode node = GetNode( element );
				if( null != node )
				{
					root.AddChild( node );
				}

				ParseBTNode( node, element );
			}
		}
		private Dictionary<string, string> GetProperty( XElement btNodeE )
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			var propertyEList = btNodeE.Elements( BTDataKey.NODE_PROPERTY );
			if (propertyEList == null )
			{
				return dic;
			}
			foreach(XElement propertyE in propertyEList)
			{
				dic.Add(propertyE.Attribute( BTDataKey.NODE_KEY ).Value, propertyE.Attribute( BTDataKey.NODE_VALUE ).Value);
			}
			return dic;
		}
        private BTNode GetNode( XElement btNodeE)
		{
			string szNodeType = btNodeE.Attribute( BTDataKey.NODE_FIRST_TYPE ).Value;
			Dictionary<string, string> paramsDic = GetProperty( btNodeE );
		    switch (szNodeType)
		    {
		        case BTDataKey.NODE_TYPE_SELECTOR:
		            return ParseSelector(btNodeE, paramsDic);
		        case BTDataKey.NODE_TYPE_SEQUENCE:
		            return ParseSequence(btNodeE, paramsDic);
		        case BTDataKey.NODE_TYPE_ACTION:
		            return ParseAction(btNodeE, paramsDic);
                case BTDataKey.NODE_TYPE_CONDITION:
                    return ParseCondition(btNodeE, paramsDic);
                case BTDataKey.NODE_TYPE_DECORATOR:
		            return ParseDecorator(btNodeE, paramsDic);
		    }

		    return null;
		}
	    #region composites
        private Selector ParseSelector( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new Selector();
		}
		private Sequence ParseSequence( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new Sequence();
		}
        #endregion

        #region Condition
        private BTCondition ParseCondition(XElement btNodeE, Dictionary<string, string> paramsDic)
        {
            return new BTCondition(int.Parse(paramsDic[BTDataKey.NODE_TYPE_CONDITION_LIMIT]));
        }
		#endregion

		#region Action
		private BTAction ParseAction( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			string szNodeName = btNodeE.Attribute( BTDataKey.NODE_SECOND_TYPE ).Value;
			switch( szNodeName )
			{
			case BTDataKey.NODE_NAME_IDLE:
				return ParseA_Idle( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_NAME_MOVETO:
				return ParseA_Moveto( btNodeE, paramsDic );
				break;
			}
			
			return null;
		}
        private BTAMoveTo ParseA_Moveto(XElement btNodeE, Dictionary<string, string> paramsDic)
        {
            int targetid = 0;
            int followPointid = 0;

            string tmpString = string.Empty;

            if (paramsDic.TryGetValue("targetId", out tmpString))
            {
                int.TryParse(tmpString, out targetid);
            }
            if (paramsDic.TryGetValue("followPointId", out tmpString))
            {
                int.TryParse(tmpString, out followPointid);
            }
			return new BTAMoveTo(targetid,followPointid);
		}
		private BTAIdle ParseA_Idle( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTAIdle();
		}
		#endregion

        #region Decorator
        private BTNode ParseDecorator(XElement btNodeE, Dictionary<string, string> paramsDic)
        {
            string szNodeName = btNodeE.Attribute(BTDataKey.NODE_SECOND_TYPE).Value;
            switch (szNodeName)
            {
                case BTDataKey.NODE_NAME_INVERTER:
                    return ParseInverter(btNodeE, paramsDic);
                    break;
            }

            return null;
        }
	    private BTDInverter ParseInverter(XElement btNode, Dictionary<string, string> paramsDic)
	    {
            return new BTDInverter(int.Parse(paramsDic["inverter"]));
	    }
        #endregion
    }
}