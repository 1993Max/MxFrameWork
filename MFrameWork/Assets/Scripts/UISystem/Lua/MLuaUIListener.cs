using System;
using System.Collections;
using System.Collections.Generic;
using MFrameWork;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MFrameWork
{
    public class MLuaUIListener : EventTrigger
    {
        public float longClickTime = 3.5f;
        private float _currentLongClickTime = 0;
        private Vector2 _longClickDownPos = Vector2.zero;
        //private CoroutineTaskManager.CoroutineTask _longClickTask;

        public delegate void UIEventDelegate(GameObject go, PointerEventData eventData);
        public UIEventDelegate onClick;
        public UIEventDelegate onDown;
        public UIEventDelegate onEnter;
        public UIEventDelegate onExit;
        public UIEventDelegate onUp;
        public UIEventDelegate initDrag;
        public UIEventDelegate beginDrag;
        public UIEventDelegate onDrag;
        public UIEventDelegate endDrag;
        public UIEventDelegate onDrop;
        public UIEventDelegate onScroll;
        public UIEventDelegate onLongClick;

        public static MLuaUIListener Get(GameObject go)
        {
            if (!go)
            {
                return null;
            }
            MLuaUIListener listener = go.GetComponent<MLuaUIListener>();
            if (!listener)
            {
                listener = go.AddComponent<MLuaUIListener>();
            }
            return listener;
        }

        public static void Destroy(GameObject go)
        {
            if (!go)
            {
                return;
            }
            MLuaUIListener listener = go.GetComponent<MLuaUIListener>();
            if (listener)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    UnityEngine.Object.DestroyImmediate(listener);
                }
                else
                {
                    UnityEngine.Object.Destroy(listener);
                }
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            //EventSystem.current.RaycastAll()
            base.OnPointerClick(eventData);
            //MAudioMgr.singleton.Play("event:/DefualtButtonSound");
            //var anyClickArgs = MEventPool<MAnyClickEventArgs>.GetEvent();
            //anyClickArgs.clickObj = gameObject;
           // MEventMgr.singleton.FireGlobalEvent(MEventType.MGlobalEvent_Any_Click, anyClickArgs);
            if (onClick != null) onClick(gameObject, eventData);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onLongClick != null)
            {
                //if (_longClickTask != null)
                    //_longClickTask.Stop();
                //_longClickTask = CoroutineTaskManager.Instance.AddTask(LongClick(eventData), (succ) => { _currentLongClickTime = 0; });
            }
            base.OnPointerDown(eventData);
            if (onDown != null) onDown(gameObject, eventData);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (onEnter != null) onEnter(gameObject, eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (onExit != null) onExit(gameObject, eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onLongClick != null)
            {
                //if (_longClickTask != null)
                    //_longClickTask.Stop();
            }
            base.OnPointerUp(eventData);
            if (onUp != null) onUp(gameObject, eventData);
        }
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
            if (initDrag != null) initDrag(gameObject, eventData);
        }
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (beginDrag != null) beginDrag(gameObject, eventData);
        }
        public override void OnDrag(PointerEventData eventData)
        {
            if (onLongClick != null && // 拖动位移超过一定范围，停止长按
                Vector2.Distance(_longClickDownPos, eventData.position) > 4f * Screen.height / 640f)
            {
                //if (_longClickTask != null)
                    //_longClickTask.Stop();
            }
            base.OnDrag(eventData);
            if (onDrag != null) onDrag(gameObject, eventData);
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (endDrag != null) endDrag(gameObject, eventData);
        }
        public override void OnDrop(PointerEventData eventData)
        {
            base.OnDrop(eventData);
            if (onDrop != null) onDrop(gameObject, eventData);
        }
        public override void OnScroll(PointerEventData eventData)
        {
            base.OnScroll(eventData);
            if (onScroll != null) onScroll(gameObject, eventData);
        }

        IEnumerator LongClick(PointerEventData eventData)
        {
            while (true)
            {
                _currentLongClickTime += Time.deltaTime;
                yield return null;
                if (gameObject.Equals(null) || gameObject.activeSelf == false)
                    break;
                if (_currentLongClickTime > longClickTime)
                {
                    if (onLongClick != null)
                        onLongClick(gameObject, eventData);
                    _currentLongClickTime = 0;
                    break;
                }
            }
        }
    }

}
