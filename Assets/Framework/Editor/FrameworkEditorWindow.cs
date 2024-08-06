using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class FrameworkEditor
    {

        [Button]
        private void CopyDll2Bytes()
        {
            //HybridCLRData/HotUpdateDlls/Android/XXX.dll
            string sourceDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget);
            string targetDir = $"Assets/Resource/HotUpdateDlls/{EditorUserBuildSettings.activeBuildTarget}/";
            for (var i = 0; i < SettingsUtil.HybridCLRSettings.hotUpdateAssemblyDefinitions.Length; i++)
            {
                var assemblyDefinition = SettingsUtil.HybridCLRSettings.hotUpdateAssemblyDefinitions[i];
                string name = assemblyDefinition.name;
                string source = $"{sourceDir}/{name}.dll";
                string target = $"{targetDir}/{name}.dll.bytes";
                if(!System.IO.File.Exists(source))
                {
                    Debug.LogError($"dll not found: {source}");
                    continue;
                }
                
                if(!System.IO.Directory.Exists(targetDir))
                {
                    System.IO.Directory.CreateDirectory(targetDir);
                }
                
                if(!File.Exists(target))
                {
                    File.Copy(source, target);
                }
                else
                {
                    // 覆盖模式写入
                    File.WriteAllBytes(target, File.ReadAllBytes(source));
                }
                
                Debug.Log($"copy {source} to {target}");
            }
            AssetDatabase.Refresh();
        }
        [Button]
        private void BuildHybirdCLR()
        {
            CompileDllCommand.CompileDll(EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();

        }
    }
    public class FrameworkEditorWindow : OdinMenuEditorWindow
    {
        

        [MenuItem("Framework/EditorWindow")]
        private static void OpenWindow()
        {
            GetWindow<FrameworkEditorWindow>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();
            tree.Add("Framework", new FrameworkEditor());
            
            
            return tree;
        }
    }
}