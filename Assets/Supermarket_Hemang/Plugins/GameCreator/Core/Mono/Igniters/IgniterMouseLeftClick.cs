namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	[AddComponentMenu("")]
	public class IgniterMouseLeftClick : Igniter, IPointerClickHandler
	{
		#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Left Click";
		public new static bool REQUIRES_COLLIDER = true;
        #endif

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
                this.ExecuteTrigger(gameObject);
			}
		}
	}
}