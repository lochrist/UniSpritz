using UnityEditor;
using UnityEngine;

public class ListenToDomainReloadWindow : EditorWindow
{
    [MenuItem("Test/Show ListenToDomainReload")]
    static void Init()
    {
        GetWindow<ListenToDomainReloadWindow>();
    }

    void OnEnable()
    {
        AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    void OnDisable()
    {
        // AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        // AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    public void OnBeforeAssemblyReload()
    {
        Debug.Log("Before Assembly Reload");
    }

    public void OnAfterAssemblyReload()
    {
        Debug.Log("After Assembly Reload");
    }
}