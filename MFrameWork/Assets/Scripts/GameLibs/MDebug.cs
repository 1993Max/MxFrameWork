using System;
using UnityEngine;

namespace MFrameWork
{
    public class MDebug : MSingleton<MDebug>
    {
        public void AddLog(string log)
        {
            Debug.LogError(log);
        }

        public void AddErrorLog(string log)
        {
            Debug.LogError(log);
        }

        public void AddGreenLog(string log)
        {

        }

        public void AddErrorLogF(string log, params object[] args)
        {

        }

        public void AddYellowLog(string str)
        {

        }

        public void AddRedLog(string str)
        {

        }

        public void AddWarningLog(string str)
        {

        }
    }
}