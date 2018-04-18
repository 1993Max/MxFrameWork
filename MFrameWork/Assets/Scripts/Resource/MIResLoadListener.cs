using System;

namespace MFrameWork
{
    public interface MIResLoadListener
    {
        //资源加载成功 ResLoad Finish
        void Finished();
        //资源加载失败 ResLoad Fail
        void Fail();
    }
}
