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

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = base.BuildRows(root);
            if (rootItem.hasChildren)
            {
                foreach (var child in rootItem.children)
                {
                    SetExpanded(child.id, true);
                }
            }
            return rows;
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

            var guids = AssetDatabase.FindAssets($"t:{_filterState.FilterContent.text}", searchInFolders);
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
                                icon = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D,
                            };
                        }
                        else
                        {
                            var asset = AssetDatabase.LoadAssetAtPath<Object>(combinePath);
                            child = new FilterProjectTreeViewItem()
                            {
                                id = asset.GetInstanceID(),
                                icon = AssetDatabase.GetCachedIcon(combinePath) as Texture2D,
                            };
                        }

                        child.depth = prevRoot.depth + 1;
                        child.displayName = name;
                        child.IsFolder = isFolder;
                        child.Path = combinePath;
                        
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

        protected override void DoubleClickedItem(int id)
        {
            var rows = FindRows(new List<int>(){id});
            if (rows.Count <= 0)
                return;

            var item = rows[0] as FilterProjectTreeViewItem;
            var path = item.Path;
            if (AssetDatabase.IsValidFolder(path))
                EditorUtility.RevealInFinder(path);
            else
                AssetDatabase.OpenAsset(EditorUtility.InstanceIDToObject(item.id));
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
            if (selectedIds.Count > 0)
            {
                var row = FindRows(selectedIds)[0] as FilterProjectTreeViewItem;
                var icon = AssetDatabase.GetCachedIcon(row.Path);
                _filterState.SelectionContent = new GUIContent(row.Path, icon);
            }
        }

        internal void OnGUISelectionPath(Rect rect)
        {
            if (_filterState.SelectionContent == null)
                return;
            
            EditorGUI.LabelField(rect, _filterState.SelectionContent);
        }

        protected override bool CanStartDrag(CanStartDragArgs args) => true;

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            if (args.draggedItemIDs.Count <= 0)
                return;

            var dragObjects = args.draggedItemIDs.Select(EditorUtility.InstanceIDToObject).ToArray(); 
                
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.paths = null;
            DragAndDrop.objectReferences = dragObjects;
            DragAndDrop.SetGenericData("Tool", new List<int>(args.draggedItemIDs));
            DragAndDrop.StartDrag(dragObjects.Length > 1 ? "<Multiple>" : dragObjects[0].name);
        }

        protected override void ContextClickedItem(int id)
        {
            var rows = FindRows(new List<int>(){id});
            if (rows.Count <= 0)
                return;

            var item = rows[0] as FilterProjectTreeViewItem;
            
            var @event = Event.current;
            @event.Use();

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy Path"), false, v =>
            {
                EditorGUIUtility.systemCopyBuffer = item.Path;
            }, null);
            menu.AddItem(new GUIContent("Expand"), false, v =>
            {
                SetExpandedRecursive(id, true);
                EditorGUIUtility.systemCopyBuffer = item.Path;
            }, null);
            menu.AddItem(new GUIContent("Collapse"), false, v =>
            {
                SetExpandedRecursive(id, false);
                EditorGUIUtility.systemCopyBuffer = item.Path;
            }, null);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Reimport"), false, v =>
            {
                AssetDatabase.ImportAsset(item.Path, ImportAssetOptions.ImportRecursive);

            }, null);

            // 各所で追加
            menu.ShowAsContext();
        }
    }
}