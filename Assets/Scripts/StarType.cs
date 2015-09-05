
namespace DajiaGame.Px
{
    /// <summary>
    /// 描述：星星类型
    /// author： 孙广东
    /// </summary>
    public enum StarType           // 粉色、黄、绿、蓝、紫
    {
        Pink,
        Yellow,
        Green,
        Blue,
        Purple
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