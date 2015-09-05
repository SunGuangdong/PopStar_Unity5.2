
//-----------------------------------------------------------------------
// <copyright file="ComAnimation.cs" company="Game Development Laboratory">
//     Copyright (c) Sprocket Enterprises. All rights reserved.
// </copyright>
// <author> SunGuangDong </author>
//
// <summary>
//     This is the Widget class.
// </summary>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace DajiaGame.Px
{
    /// <summary>
    ///  对定制的  C# 脚本模板拷贝，从 项目Editor到 Unity的安装路径
    /// sunguangdong
    /// </summary>
    public class CopyCSharpScriptTemplates : MonoBehaviour
    {
        [InitializeOnLoad]
        public class Startup
        {
            //  D:/Program Files/Unity5/Editor/Unity.exe
            //  D:\Program Files\Unity5\Editor\Data\Resources\ScriptTemplates
            static Startup()
            {
                string strOriPath = Path.Combine(Application.dataPath, "Editor/81-C# Script-NewBehaviourScript.cs.txt");
                string strDesPath = Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath),
                    "Data/Resources/ScriptTemplates/81-C# Script-NewBehaviourScript.cs.txt");

                File.Copy(strOriPath, strDesPath, true);
            }
        }
    }
}
