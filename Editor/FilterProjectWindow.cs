using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Yorozu.EditorTool
{
    internal class FilterProjectWindow : EditorWindow
    {
        [MenuItem("Tools/FilterProject")]
        private static void ShowWindow()
        {
            var window = GetWindow<FilterProjectWindow>();

            var icon = EditorGUIUtility.IconContent("d_FilterByType").image as Texture2D;
            window.titleContent = new GUIContent("Filter Project", icon);
            window.Show();
        }
        
        private SearchField _searchField;

        private FilterProjectTreeView _treeView;
        [SerializeField]
        private FilterProjectTreeViewState _state;

        private GUILayoutOption _categoryWidth;
        

        private static class Styles
        {
            public static readonly GUIContent FilterContent = new ("Select Filter");
            public static readonly GUILayoutOption SearchMin = GUILayout.MinWidth(100);
        }
        
        private void Initialize()
        {
            _state ??= new FilterProjectTreeViewState();
            if (_treeView == null)
            {
                _treeView = new FilterProjectTreeView(_state);
            }
            if (_searchField == null)
            {
                _searchField = new SearchField();
                _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
            }
        }

        private void OnGUI()
        {
            Initialize();
            
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    _state.AssetsOnly = GUILayout.Toggle(_state.AssetsOnly, "Assets Only", EditorStyles.toolbarButton);
                    if (check.changed)
                    {
                        _treeView?.Reload();   
                    }
                    var buttonRect = GUILayoutUtility.GetRect(Styles.FilterContent, EditorStyles.toolbarPopup);
                    if (GUI.Button(buttonRect, Styles.FilterContent, EditorStyles.toolbarButton))
                    {
                        var state = new AdvancedDropdownState();
                        var dropdown = new FilterDropdown(state);
                        dropdown.OnSelect += (v, t) =>
                        {
                            _state.FilterContent = new GUIContent(v, t);
                            _state.Width = v.Length * 7 + 16;
                            _treeView?.Reload();
                        };
                        dropdown.Show(buttonRect);
                        GUIUtility.ExitGUI();
                    }
                }
                GUILayout.Space(5);
                EditorGUILayout.LabelField(_state.FilterContent, GUILayout.Width(_state.Width));
                
                GUILayout.FlexibleSpace();
                
                _treeView.searchString =  _searchField.OnToolbarGUI(_treeView.searchString, Styles.SearchMin);
            }
            
            var rect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);
            rect.height -= EditorGUIUtility.singleLineHeight;
            _treeView.OnGUI(rect);
            rect.y += rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            _treeView.OnGUISelectionPath(rect);
        }
    }
}