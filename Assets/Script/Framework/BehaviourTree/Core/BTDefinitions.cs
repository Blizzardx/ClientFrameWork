

using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 节点状态.
	/// </summary>
	public enum EBTState
	{
		False,
		True,
        Running,
        UnReach,
	}

	/// <summary>
	/// 行为节点状态.
	/// </summary>
	public enum EBTActionState
	{
		Ready,
		Waitting,
		Running,
	}

	/// <summary>
	/// 表格数据.
	/// </summary>
	public class BTDataKey
	{
		public const string BEHAVIOUR_TREE_ROOT = "behaviorTree";
		public const string BEHAVIOUR_TREE_ID = "id";
		public const string NODE_NAME = "btNode";
		public const string NODE_FIRST_TYPE = "nodeType";
		public const string NODE_SECOND_TYPE = "name";
		public const string NODE_PROPERTY = "property";
		public const string NODE_KEY = "key";
		public const string NODE_VALUE = "value";

        public const string NODE_TYPE_CHILDNODE = "childnode";
        public const string NODE_TYPE_DECORATOR = "decorator";

		public const string NODE_TYPE_SELECTOR = "selector";
		public const string NODE_TYPE_SEQUENCE = "sequence";
		public const string NODE_TYPE_PARALLEL = "parallel";
        public const string NODE_TYPE_PARALLEL_SELECTOR = "parallelSelector";
	    public const string NODE_TYPE_PARALLEL_SEQUENCE = "parallelSequence";

		public const string NODE_NAME_INVERTER = "inverter";
		
		
		public const string NODE_TYPE_CONDITION = "condition";
		public const string NODE_TYPE_CONDITION_LIMIT = "limitId";
		
		public const string NODE_TYPE_ACTION = "action";
        public const string NODE_NAME_MOVETO = "moveTo";
		public const string NODE_NAME_PATROL = "patrol";
		public const string NODE_NAME_SEEK = "seek";
		public const string NODE_NAME_POSITION = "position";
		public const string NODE_NAME_RETREAT = "retreat";
		public const string NODE_NAME_IDLE = "idle";
		public const string NODE_NAME_ATTACK = "attack";
		public const string NODE_NAME_ATTACKED = "attacked";

	}

	/// <summary>
	/// 黑板数据key.
	/// </summary>
	public enum EDataBaseKey
	{
		Owner,
		RetreatPos,
		SeekRange,
        IsLock,
	}

	/// <summary>
	/// 寻敌类型.
	/// </summary>
	public enum ESeekType
	{
		Npc,
		Enemy_ExceptNpc,
		All,
	}
}