using System;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Yorozu.EditorTool
{
    internal class FilterDropdown : AdvancedDropdown
    {
        private static readonly string[] _defaultfilterTypes = new string[]
        {
            "MonoScript",
            "RuntimeAnimatorController",
            "AudioClip",
            "Animator",
            "AnimationClip",
            "AudioMixer",
            "AudioMixerGroup",
            "Mesh",
            "SceneAsset",
            "Shader",
            "Material",
            "Texture",
            "GameObject",
            "ScriptableObject",
            "TextAsset",
            "ComputeShader",
            "Sprite",
            "SpriteAtlas",
            "Font",
            "VisualEffectObject",
            "VideoClip",
            "Preset",
            "Avatar",
        };
        
#if false
        private static readonly string[] _ignorefilterTypes = new string[]
        {
            "Motion",
            "ShaderVariantCollection",
            "PhysicMaterial",
            "PhysicsMaterial2D",
            "TerrainData",
            "TerrainLayer",
            "NavMeshData",
            "AvatarMask",
            "Transition",
            "StateMachine",
            "State",
            "AssetBundle",
            "AssetBundleManifest",
            "AudioMixerSnapshot",
            "LightingSettings",
            "BillboardAsset",
            "LightmapSettings",
            "LightProbes",
            "QualitySettings",
            "RenderSettings",
            "Flare",
            "Component",
            "FailedToLoadScriptObject",
            "LowerResBlitTexture",
            "PreloadData",
            "GraphicsSettings",
            "LocalizationAsset",
            "SpeedTreeWindAsset",
            "UnityConnectSettings",
            "ProjectSettingsBase",
            "AssetImportInProgressProxy",
            "AssetImporter",
            "DefaultAsset",
            "EditorBuildSettings",
            "EditorSettings",
            "EditorUserBuildSettings",
            "EditorUserSettings",
            "LightingDataAsset",
            "LightmapParameters",
            "LightmapSnapshot",
            "PlayerSettings",
            "SceneVisibilityState",
            "VersionControlSettings",
            "HumanTemplate",
            "VisualEffectResource",
            "ShaderContainer",
            "EditorProjectAccess",
            "BuildReport",
            "PackedAssets",
            "ScenesUsingAssets",
            "AudioMixerEffectController",
            "ImportLog",
            "GameObjectRecorder",
            "AnimatorTransitionBase",
            "AnimatorState",
            "AnimatorStateMachine",
            "SpriteAtlasAsset",
            "RayTracingShader",
        };
#endif
        internal event Action<string, Texture2D> OnSelect;
        
        internal FilterDropdown(AdvancedDropdownState state) : base(state)
        {
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var targets = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(UnityEngine.Object)))
                .Where(t => !t.IsSubclassOf(typeof(UnityEngine.Component)))
                .Where(t => !t.IsSubclassOf(typeof(AssetImporter)))
                ;
            var root = new AdvancedDropdownItem("root");
            var extend = new AdvancedDropdownItem("Others");

            var prefab = new AdvancedDropdownItem("Prefab")
            {
                icon = EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D,
            };
            
            root.AddChild(prefab);   
            foreach (var target in targets)
            {
                var name = target.Name; 
                if (name == "MonoScript")
                {
                    name = "Script";
                }
                if (name == "SceneAsset")
                {
                    name = "Scene";
                }
                var item = new AdvancedDropdownItem(name);
                item.icon = EditorGUIUtility.ObjectContent(null, target).image as Texture2D;
                if (item.icon == null)
                    continue;

                if (item.icon.name.EndsWith("DefaultAsset Icon") || item.icon.name.EndsWith("GameManager Icon"))
                    continue;

                if (target.Name == "MonoScript")
                {
                    item.icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                }
                
                if (_defaultfilterTypes.Contains(target.Name))
                {
                    root.AddChild(item);
                }
                else
                {
                    extend.AddChild(item);
                }
            }
            root.AddChild(extend);

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            OnSelect?.Invoke(item.name, item.icon);
            base.ItemSelected(item);
        }
    }
}