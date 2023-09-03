using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Yorozu.EditorTool
{
    [EditorWindowTitle(title = "FilterProject", icon = "Project")]
    internal class FilterProjectWindow : EditorWindow
    {
        [MenuItem("Tools/FilterProject")]
        private static void ShowWindow()
        {
            var window = GetWindow<FilterProjectWindow>();
            window.titleContent = new GUIContent("FilterProject");
            window.Show();
        }
        
        private SearchField _searchField;

        private FilterProjectTreeView _treeView;
        [SerializeField]
        private FilterProjectTreeViewState _state;
        
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
                var filterContent = new GUIContent(_state.FilterType, _state.FilterIcon);
                EditorGUILayout.LabelField(filterContent, EditorStyles.label);
                
                var content = new GUIContent("Select Filter");
                var buttonRect = GUILayoutUtility.GetRect(content, EditorStyles.toolbarButton);
                if (GUI.Button(buttonRect, content, EditorStyles.toolbarButton))
                {
                    var state = new AdvancedDropdownState();
                    var dropdown = new FilterDropdown(state);
                    dropdown.OnSelect += (v, t) =>
                    {
                        _state.FilterType = v;
                        _state.FilterIcon = t;
                        
                        if (_treeView != null)
                            _treeView.Reload();
                    };
                    dropdown.Show(buttonRect);
                    GUIUtility.ExitGUI();
                }

                GUILayout.FlexibleSpace();
                _treeView.searchString =  _searchField.OnToolbarGUI(_treeView.searchString);
            }
            
            var rect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);
            _treeView.OnGUI(rect);
        }
    }
}