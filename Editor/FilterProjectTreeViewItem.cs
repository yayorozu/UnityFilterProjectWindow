using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Yorozu.EditorTool
{
    internal class FilterProjectTreeViewItem : TreeViewItem
    {
        internal bool IsFolder;
        internal string Path;
    }
}