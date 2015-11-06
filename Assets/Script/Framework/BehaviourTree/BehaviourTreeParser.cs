

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
		    ParseBTXml(root, null);//DataManager.Instance.GetAIXml() );
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

		private BTNode GetNode( XElement btNodeE )
		{
			string szNodeType = btNodeE.Attribute( BTDataKey.NODE_FIRST_TYPE ).Value;
			Dictionary<string, string> paramsDic = GetProperty( btNodeE );
			switch( szNodeType )
			{
			case BTDataKey.NODE_TYPE_SELECTOR:
				return ParseSelector( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_TYPE_PRIORITY_SELECTOR:
				return ParsePropilySelector( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_TYPE_PARALLEL:
				return ParseParallel( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_TYPE_SEQUENCE:
				return ParseSequence( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_TYPE_DECORATOR:
				return ParseDecorator( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_TYPE_CONDITION:
				return ParseCondition( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_TYPE_ACTION:
				return ParseAction( btNodeE, paramsDic );
				break;
			}

			return null;
		}

		private Selector ParseSelector( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new Selector();
		}

		private PropilySelector ParsePropilySelector( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new PropilySelector();
		}

		private Parallel ParseParallel( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new Parallel();
		}

		private Sequence ParseSequence( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new Sequence();
		}

		private BTCondition ParseCondition( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTCondition( int.Parse( paramsDic[BTDataKey.NODE_TYPE_CONDITION_TARGET] ), int.Parse( paramsDic[BTDataKey.NODE_TYPE_CONDITION_LIMIT] ) );
		}

		#region Decorator
		private BTDecorator ParseDecorator( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			string szNodeName = btNodeE.Attribute( BTDataKey.NODE_SECOND_TYPE ).Value;
			switch( szNodeName )
			{
			case BTDataKey.NODE_NAME_INVERTER:
				return ParseD_Inverter( btNodeE, paramsDic );
				break;
			}

			return null;
		}

		private BTDInverter ParseD_Inverter( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTDInverter( int.Parse( paramsDic["interval"] ) );
		}
		#endregion


		#region Action
		private BTAction ParseAction( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			string szNodeName = btNodeE.Attribute( BTDataKey.NODE_SECOND_TYPE ).Value;
			switch( szNodeName )
			{
			/*case BTDataKey.NODE_NAME_MOVE:
				return ParseA_Move( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_NAME_PATROL:
				return ParseA_Patrol( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_NAME_SEEK:
				return ParseA_Seek( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_NAME_POSITION:
				return ParseA_Position( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_NAME_RETREAT:
				return ParseA_Retreat( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_NAME_IDLE:
				return ParseA_Idle( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_NAME_ATTACK:
				return ParseA_Attack( btNodeE, paramsDic );
				break;
			case BTDataKey.NODE_NAME_ATTACKED:
				return ParseA_Attacked( btNodeE, paramsDic );
				break;*/
			}
			
			return null;
		}
/*
		private BTAMove ParseA_Move( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTAMove();
		}

		private BTAPatrol ParseA_Patrol( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTAPatrol( int.Parse( paramsDic["range"] ) );
		}

		private BTASeek ParseA_Seek( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTASeek( int.Parse( paramsDic["range"] ), int.Parse( paramsDic["type"] ) );
		}

		private BTAPosition ParseA_Position( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTAPosition( int.Parse( paramsDic["range"] ) );
		}

		private BTARetreat ParseA_Retreat( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTARetreat( int.Parse( paramsDic["maxCount"] ), int.Parse( paramsDic["range"] ) );
		}

		private BTAIdle ParseA_Idle( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTAIdle();
		}

		private BTAAttack ParseA_Attack( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTAAttack();
		}

		private BTAAttacked ParseA_Attacked( XElement btNodeE, Dictionary<string, string> paramsDic )
		{
			return new BTAAttacked();
		}*/
		#endregion
	}
}