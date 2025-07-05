using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Framework.Attribute;
using Framework.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CustomEditor(typeof(UIObject), true)]
public class UIObjectEditorTools : Editor
{
    /// <summary>
    /// 重写Inspector面板
    /// 在下方增加绑定SerializeField和绑定Button OnClick的按钮
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        if (GUILayout.Button("Bind Serialize Field")) BindPropertyField();
        // 每次改完代码还得找到脚本重新绑定，比较麻烦,所以暂时不启用
        // if (GUILayout.Button("Bind Button OnClick")) BindButtonOnClick();
    }

    #region 绑定属性

    /// <summary>
    /// 绑定SerializeField
    /// </summary>
    private void BindPropertyField()
    {
        // TODO: 处理同名问题
        var uiObject = (UIObject)target;
        var iterator = serializedObject.GetIterator();
        // 提取所有Unity节点
        var transforms = new Dictionary<string, Transform>();
        foreach (var transform in uiObject.gameObject.GetComponentsInChildren<Transform>())
        {
            transforms.TryAdd(transform.name.ToLower(), transform);
        }

        // 遍历所有SerializedProperty
        while (iterator.Next(true))
        {
            switch (iterator.propertyType)
            {
                // 绑定数组对象
                case SerializedPropertyType.Generic:
                {
                    // 临时方案,如果不是PPtr<>类型,代表不能绑定到Transform上
                    var elementType = GetPropertyType(iterator.arrayElementType);
                    if (elementType == iterator.arrayElementType) break;

                    // 匹配规则 xxxList
                    var match = Regex.Match(iterator.name, "(.+)(List+)$");
                    if (match.Success)
                    {
                        var prefix = match.Groups[1].Value;
                        var size = 0;
                        iterator.arraySize = 0;
                        while (true)
                        {
                            var hierarchyName = $"{prefix}{size}";
                            if (transforms.TryGetValue(hierarchyName.ToLower(), out var transform))
                            {
                                iterator.arraySize += 1;
                                var type = GetPropertyType(iterator.GetArrayElementAtIndex(size).type);
                                var elemIterator = iterator.GetArrayElementAtIndex(size);
                                SetPropertyType(elemIterator, type, transform);
                                size++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    break;
                }
                // 绑定单个对象
                case SerializedPropertyType.ObjectReference:
                {
                    if (transforms.TryGetValue(iterator.name.ToLower(), out var transform))
                    {
                        var type = GetPropertyType(iterator.type);
                        SetPropertyType(iterator, type, transform);
                    }

                    break;
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
        
        Debug.Log($"{uiObject.GetType().Name}: Bind property field success");
    }

    /// <summary>
    /// 通过正则表达式提取SerializedProperty的类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetPropertyType(string type)
    {
        var match = Regex.Match(type, @"PPtr<\$(.*?)>");
        if (match.Success)
        {
            type = match.Groups[1].Value;
        }

        return type;
    }

    /// <summary>
    /// 设置SerializedProperty的类型
    /// </summary>
    /// <param name="iterator"></param>
    /// <param name="type"></param>
    /// <param name="transform"></param>
    private static void SetPropertyType(SerializedProperty iterator, string type, Transform transform)
    {
        switch (type)
        {
            case "GameObject":
                iterator.objectReferenceValue = transform.gameObject;
                break;
            case "Transform":
                iterator.objectReferenceValue = transform;
                break;
            default:
                var com = transform.GetComponent(type);
                if (com != null) iterator.objectReferenceValue = com;
                break;
        }
    }

    #endregion

    #region 绑定按钮

    /// <summary>
    /// 绑定Button OnClick
    /// </summary>
    private void BindButtonOnClick()
    {
        // TODO: 处理同名问题
        var uiObject = (UIObject)target;
        var buttons = uiObject.gameObject.GetComponentsInChildren<Button>();
        var methods = uiObject.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            .Where(item => Attribute.IsDefined(item, typeof(UIButton)));

        // 清空无效事件
        foreach (var button in buttons)
        {
            for (var i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                var targetName = button.onClick.GetPersistentTarget(i);
                var methodName = button.onClick.GetPersistentMethodName(i);
                // 如果target method 或者 函数不存在,删除常驻回调
                if (!targetName || string.IsNullOrEmpty(methodName) || targetName.GetType().GetMethod(methodName) == null)
                {
                    UnityEventTools.RemovePersistentListener(button.onClick, i);
                }
            }
        }
        
        // 遍历所有带 [UIButton] 方法
        foreach (var method in methods)
        {
            if (method.GetParameters().Length != 0)
            {
                Debug.LogWarning($"{uiObject.GetType().Name}.{method.Name} method parameters more than one");
                continue;
            }

            // [UIButton] 允许多个,遍历所有的特性
            var attributes = method.GetCustomAttributes<UIButton>();
            foreach (var attribute in attributes)
            {
                var callback = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), uiObject, method);
                foreach (var button in buttons)
                {
                    // 按钮名字和特性参数一致
                    if (button.name != attribute.Name) continue;
                    
                    var isSet = false; // 是否已经设置
                    for (var i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                    {
                        var targetName = button.onClick.GetPersistentTarget(i);
                        var methodName = button.onClick.GetPersistentMethodName(i);
                        if (!targetName) continue;
                        if (targetName.name != uiObject.GetType().Name || methodName != method.Name) continue;
                        isSet = true; // 有相同回调
                    }
                    if (!isSet)
                    {
                        UnityEventTools.AddPersistentListener(button.onClick, callback);
                    }
                }
            }
        }
        
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
        
        Debug.Log($"{uiObject.GetType().Name}: Bind button success");
    }

    #endregion
}