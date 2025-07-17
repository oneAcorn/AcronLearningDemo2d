using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acorn.Tools
{
    public interface IPoolable
    {
        void OnGetFromPool();    // 从池中取出时调用
        void OnReturnToPool();   // 返回池中时调用
    }
}