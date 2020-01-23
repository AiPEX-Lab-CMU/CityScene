namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionAdventureCamera : IAction
	{
        public bool mainCameraMotor = false;
        public CameraMotor cameraMotor;

        public bool allowOrbitInput = true;
        public bool allowZoom = true;
        public bool avoidWallClip = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            CameraMotor motor = (this.mainCameraMotor ? CameraMotor.MAIN_MOTOR : this.cameraMotor);
            if (motor != null && motor.cameraMotorType.GetType() == typeof(CameraMotorTypeAdventure))
            {
                CameraMotorTypeAdventure adventureMotor = (CameraMotorTypeAdventure)motor.cameraMotorType;
                adventureMotor.allowOrbitInput = this.allowOrbitInput;
                adventureMotor.allowZoom = this.allowZoom;
                adventureMotor.avoidWallClip = this.avoidWallClip;
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Camera/Adventure Camera Settings";
        private const string NODE_TITLE = "Change {0} Adventure Camera settings";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spMainCameraMotor;
        private SerializedProperty spCameraMotor;

        private SerializedProperty spAllowOrbitInput;
        private SerializedProperty spAllowZoom;
        private SerializedProperty spAvoidWallClip;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            string motor = (this.mainCameraMotor
                ? "[Main Camera]"
                : (this.cameraMotor == null ? "none" : this.cameraMotor.gameObject.name)
            );
            
			return string.Format(NODE_TITLE, motor);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spMainCameraMotor = this.serializedObject.FindProperty("mainCameraMotor");
            this.spCameraMotor = this.serializedObject.FindProperty("cameraMotor");
            this.spAllowOrbitInput = this.serializedObject.FindProperty("allowOrbitInput");
            this.spAllowZoom = this.serializedObject.FindProperty("allowZoom");
            this.spAvoidWallClip = this.serializedObject.FindProperty("avoidWallClip");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spMainCameraMotor = null;
            this.spCameraMotor = null;
            this.spAllowOrbitInput = null;
            this.spAllowZoom = null;
            this.spAvoidWallClip = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spMainCameraMotor);
            EditorGUI.BeginDisabledGroup(this.spMainCameraMotor.boolValue);
            EditorGUILayout.PropertyField(this.spCameraMotor);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spAllowOrbitInput);
            EditorGUILayout.PropertyField(this.spAllowZoom);
            EditorGUILayout.PropertyField(this.spAvoidWallClip);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
