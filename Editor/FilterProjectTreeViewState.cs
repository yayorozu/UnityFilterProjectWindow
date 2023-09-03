using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Yorozu.EditorTool
{
    [Serializable]
    internal class FilterProjectTreeViewState : TreeViewState
    {
        public string FilterType;
        [SerializeField]
        public Texture2D FilterIcon;
    }
}