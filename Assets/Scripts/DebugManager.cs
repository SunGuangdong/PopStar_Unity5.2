
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DajiaGame.Px
{
    /// <summary>
    /// 描述：
    /// author： 
    /// </summary>
	[AddComponentMenu("DajiaGame/Px/ DebugManager ")]
    public class DebugManager : MonoBehaviour
    {
		#region ===Unity事件===

        // 每帧更新
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("调试用， 输出所有星星内容");
                GameManager.Instance.starMap.PrintStarList();
            }

            // 查看 历史最高纪录
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.LogWarning("查看 历史最高纪录");
                var historyList = JsonReadFileTool.JsonToClasses<int[]>(PlayerPrefs.GetString("history")).ToList();
                if (historyList != null && historyList.Count > 0)
                {
                    for (int i = 0; i < historyList.Count; i++)
                    {
                        Debug.Log("" + (i+1) + ": " + historyList[i]);
                    }
                }
            }
        }

		#endregion
    }
}


/* ===============提示：  特性相关=================
 *  [SerializeField]
 *  [HideInInspector]
 *  [RequireComponent(typeof(Rigidbody))]
 *  [SerializeField, Range(0,5)]    int[] counts;
 *  [SerializeField,TooltipAttribute("说明")]
 *  [SerializeField,HeaderAttribute ("Title")]
 *  [SerializeField,TextAreaAttribute(2, 5)]     string message2;
 *  [DisallowMultipleComponent]             // 不能重复添加脚本
 *  [AddComponentMenu("DajiaGame/Px")]
 *  [ExecuteInEditMode]
 */