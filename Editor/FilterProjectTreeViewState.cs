using System;
using UnityEditor.IMGUI.Controls;

namespace Yorozu.EditorTool
{
    [Serializable]
    internal class FilterProjectTreeViewState : TreeViewState
    {
        public string FilterType;
    }
}