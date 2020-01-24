namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ConditionActiveObject : ICondition
	{
        public TargetGameObject target = new TargetGameObject();

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
		{
            GameObject targetValue = this.target.GetGameObject(target);
            if (targetValue == null) return false;
            return targetValue.activeInHierarchy;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/GameObject Active";
		private const string NODE_TITLE = "Is {0} active";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.target);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
