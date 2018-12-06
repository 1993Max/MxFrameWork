using System;
using System.Collections.Generic;
using MFrameWork;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MFrameWork
{
    public class MLuaUISound : MonoBehaviour, IPointerClickHandler
    {
        public delegate void UIEventDelegate(GameObject go, PointerEventData eventData);

        public int onClickId = 4;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onClickId != 0) { }
                //MAudioMgr.singleton.Play(onClickId);
        }

    }
}
