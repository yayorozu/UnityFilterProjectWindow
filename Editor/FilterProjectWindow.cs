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
            window.titleContent = new GUIContent("FilterProject");
            window.Show();
        }
        
        private SearchField _searchField;

        private string[] _filterTypes = new string[]
        {
            "AnimationClip",
            "AudioClip",
            "AudioMixer",
            "ComputeShader",
            "Font",
            "Material",
            "Mesh",
            "Model",
            "Prefab",
            "Scene",
            "Script",
            "ScriptableObject",
            "SpriteAtlas",
            "Texture",
            "VideoClip",
            "VisualEffectAsset",
        };

        private int _filterIndex;
        private FilterProjectTreeView _treeView;
        [SerializeField]
        private FilterProjectTreeViewState _state;

        private void Initialize()
        {
            _state ??= new FilterProjectTreeViewState();
            if (_treeView == null)
            {
                _state.FilterType = _filterTypes[_filterIndex];
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
                    _filterIndex = EditorGUILayout.Popup("Filter", _filterIndex, _filterTypes, EditorStyles.toolbarPopup);
                    if (check.changed)
                    {
                        _state.FilterType = _filterTypes[_filterIndex];
                        Rebuild();
                    }
                }

                _treeView.searchString =  _searchField.OnToolbarGUI(_treeView.searchString);
            }
            
            var rect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);
            _treeView.OnGUI(rect);
        }

        private void Rebuild()
        {
            if (_treeView == null)
                return;
            
            _treeView.Reload();
        }
    }
}