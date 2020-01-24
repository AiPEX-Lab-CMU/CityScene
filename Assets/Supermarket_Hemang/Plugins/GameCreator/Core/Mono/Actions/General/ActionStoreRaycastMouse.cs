namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
    using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Core.Hooks;
    using GameCreator.Variables;

    [AddComponentMenu("")]
	public class ActionStoreRaycastMouse : IAction
	{
        [VariableFilter(Variable.DataType.Vector3)]
        public VariableProperty storePoint = new VariableProperty();

        [Space]
        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeGameObject = new VariableProperty();

        private RaycastHit[] hitBuffer = new RaycastHit[1];

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Ray ray = HookCamera.Instance.Get<Camera>().ScreenPointToRay(Input.mousePosition);

            int hitCount = Physics.RaycastNonAlloc(ray, this.hitBuffer);
            if (hitCount == 0) return true;

            GameObject hitGO = this.hitBuffer[0].collider.gameObject;

            this.storePoint.Set(hitBuffer[0].point, target);
            this.storeGameObject.Set(hitGO, target);

            return true;
        }

        #if UNITY_EDITOR
        public static new string NAME = "Variables/Store Mouse World Position";
        private const string NODE_TITLE = "Store Mouse World Position";

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        #endif
    }
}
