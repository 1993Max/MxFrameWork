// **************************************
//
// 文件名(MClassObjectPool.cs):
// 功能描述("处理资源的离线数据的基类):
// 游戏中资源会用到对象池管理 使用者会对对象操作 该函数的目的是使用者在使用对象的时候 确保初始化基本信息的正确性
// 作者(Max1993):
// 日期(2019/5/16  16:09):
//
// **************************************
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MFrameWork
{
    public class MResOffLineDataBase : MonoBehaviour
    {
        public int m_allCount;
        public Transform[] m_allTransforms;
        public bool[] m_allActiveState;
        public int[] m_allPointChildCount;
        public Vector3[] m_allPostions;
        public Vector3[] m_allScale;
        public Quaternion[] m_allRot;

        [ContextMenu("BindBasicData")]
        public virtual void BindBasicData()
        {
            //包含隐藏节点
            m_allTransforms = gameObject.GetComponentsInChildren<Transform>(true);
            m_allCount = m_allTransforms.Length;
            m_allActiveState = new bool[m_allCount];
            m_allPointChildCount = new int[m_allCount];
            m_allPostions = new Vector3[m_allCount];
            m_allScale = new Vector3[m_allCount];
            m_allRot = new Quaternion[m_allCount];

            for (int i = 0; i < m_allCount; i++)
            {
                Transform tempTransform = m_allTransforms[i];
                m_allActiveState[i] = tempTransform.gameObject.activeSelf;
                m_allPointChildCount[i] = tempTransform.childCount;
                m_allPostions[i] = tempTransform.localPosition;
                m_allScale[i] = tempTransform.localScale;
                m_allRot[i] = tempTransform.localRotation;
            }
        }

        public virtual void ResetBasicData()
        {
            for (int i = 0; i < m_allCount; i++)
            {
                Transform transform = m_allTransforms[i];
                if (transform != null)
                {
                    transform.localPosition = m_allPostions[i];
                    transform.localScale = m_allScale[i];
                    transform.localRotation = m_allRot[i];
                    transform.gameObject.SetActiveEx(m_allActiveState[i]);
                    //ToDo 多余的节点处理
                    if (transform.childCount > m_allPointChildCount[i])
                    {
                        int childCount = transform.childCount;
                    }
                }
            }
        }
    }
}
