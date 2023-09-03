using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Yorozu.EditorTool
{
    internal class FilterProjectTreeView : TreeView
    {
        private FilterProjectTreeViewState _filterState => state as FilterProjectTreeViewState;
        
        internal FilterProjectTreeView(TreeViewState state) : base(state)
        {
            showBorder = true;
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem()
            {
                id = 0,
                displayName = "root",
                depth = -1,
            };

            var searchInFolders = _filterState.AssetsOnly ? new string[] {"Assets"} : null;

            var guids = AssetDatabase.FindAssets($"t:{_filterState.FilterType}", searchInFolders);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prevRoot = root;
                var splitPaths = path.Split("/");
                var combinePath = "";
                for (var j = 0; j < splitPaths.Length; j++)
                {
                    var splitPath = splitPaths[j];
                    if (string.IsNullOrEmpty(combinePath))
                    {
                        combinePath += $"{splitPath}";    
                    }
                    else
                    {
                        combinePath += $"/{splitPath}";
                    }
                    
                    var next = prevRoot.hasChildren ? prevRoot.children.FirstOrDefault(c => c.displayName == splitPath) : null;
                    if (next == null)
                    {
                        var isFolder = AssetDatabase.IsValidFolder(combinePath);
                        var name = isFolder ? 
                            splitPath :
                            System.IO.Path.GetFileNameWithoutExtension(splitPath);

                        FilterProjectTreeViewItem child = null;
                        // Packages は取得できないので
                        if (splitPath == "Packages")
                        {
                            child = new FilterProjectTreeViewItem()
                            {
                                id = 0,
                                depth = prevRoot.depth + 1,
                                displayName = name,
                                icon = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D,
                                IsFolder = isFolder,
                            };
                        }
                        else
                        {
                            var asset = AssetDatabase.LoadAssetAtPath<Object>(combinePath);
                            child = new FilterProjectTreeViewItem()
                            {
                                id = asset.GetInstanceID(),
                                depth = prevRoot.depth + 1,
                                displayName = name,
                                icon = AssetDatabase.GetCachedIcon(combinePath) as Texture2D,
                                IsFolder = isFolder,
                            };
                        }
                        
                        prevRoot.AddChild(child);
                        prevRoot = child;
                    }
                    else
                    {
                        prevRoot = next;
                    }
                }
            }
            
            if (!root.hasChildren)
            {
                root.children = new List<TreeViewItem>();
            }

            return root;
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            var valid = base.DoesItemMatchSearch(item, search);
            if (!valid)
                return false;
            
            var filter = item as FilterProjectTreeViewItem;
            return !filter.IsFolder;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            Selection.instanceIDs = selectedIds.ToArray();
        }
    }
}