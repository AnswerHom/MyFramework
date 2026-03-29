using System;
using UnityEngine;

namespace Framework.Manager
{
    /// <summary>
    /// 管理器驱动类
    /// 用来驱动执行Mono生命周期函数
    /// </summary>
    public class ManagerDriver:MonoBehaviour
    {

        private void Update()
        {
            GameManager.Instance.OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            GameManager.Instance.OnFixedUpdate();
        }

        private void LateUpdate()
        {
            GameManager.Instance.OnLateUpdate();
        }
    }
}