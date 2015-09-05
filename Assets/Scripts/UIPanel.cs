
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DajiaGame.Px
{
    /// <summary>
    /// 描述：
    /// author： 
    /// </summary>
	[AddComponentMenu("DajiaGame/Px/ UIPanel ")]
    public class UIPanel : MonoBehaviour
    {
		#region ===字段===

        public Text StarCount;
        public Text PredictPoint;
        public Text TotalPoint;
        public Text HistoryPoint;

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