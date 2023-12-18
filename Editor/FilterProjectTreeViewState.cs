using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Yorozu.EditorTool
{
    [Serializable]
    internal class FilterProjectTreeViewState : TreeViewState
    {
        [SerializeField]
        public GUIContent FilterContent;
        public float Width;
        public bool AssetsOnly;
        public GUIContent SelectionContent;
    }
}