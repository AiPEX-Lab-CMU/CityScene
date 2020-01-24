namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	[AddComponentMenu("")]
	public class IgniterMouseMiddleClick : Igniter, IPointerClickHandler
	{
		#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Middle Click";
		public new static bool REQUIRES_COLLIDER = true;
        #endif

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Middle)
			{
                this.ExecuteTrigger(gameObject);
			}
		}
	}
}