using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.AI; // 필요 시

using Presentation.Views; // MainView가 여기에 있다면

public static class PrefabTools
{
    [MenuItem("Tools/FBX → Prefab (with components)...")]
    public static void CreatePrefabsFromSelectedFbx()
    {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (!path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                continue;

            var fbxRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (fbxRoot == null) continue;

            // 1) 임시 인스턴스 생성(씬에 배치하지 않고 Prefab 저장용)
            var temp = PrefabUtility.InstantiatePrefab(fbxRoot) as GameObject;

            try
            {
                // 2) 표준 구성요소 부착 규칙
                EnsureComponents(temp);

                // 3) 저장 경로 결정 (Assets/Prefabs/ 폴더 자동 생성)
                var prefabDir = "Assets/Prefabs";
                if (!AssetDatabase.IsValidFolder(prefabDir))
                    AssetDatabase.CreateFolder("Assets", "Prefabs");

                var fileName = System.IO.Path.GetFileNameWithoutExtension(path) + ".prefab";
                var savePath = System.IO.Path.Combine(prefabDir, fileName).Replace("\\", "/");

                // 4) Prefab 저장(같은 이름 있으면 덮어쓰기 확인)
                var prefab = PrefabUtility.SaveAsPrefabAsset(temp, savePath, out bool success);
                if (success)
                    Debug.Log($"[PrefabTools] Saved: {savePath}");
            }
            finally
            {
                // 임시 오브젝트 정리
                if (temp != null) Object.DestroyImmediate(temp);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkAllScenesDirty();
    }

    private static void EnsureComponents(GameObject root)
    {
        // A) 콜라이더 규칙(간단 예시)
        // - 스키닝/리깅 없는 단일 메시: BoxCollider
        // - 이름에 "Static" 있으면 MeshCollider(Convex Off)
        // - 이름에 "Dynamic" 있으면 MeshCollider(Convex On) + Rigidbody
        var meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
        foreach (var mf in meshFilters)
        {
            var go = mf.gameObject;
            var nameLower = go.name.ToLower();

            // 이미 콜라이더 있으면 스킵
            if (go.GetComponent<Collider>() != null) continue;

            if (nameLower.Contains("static"))
            {
                var mc = go.AddComponent<MeshCollider>();
                mc.convex = false;
            }
            else if (nameLower.Contains("dynamic"))
            {
                var mc = go.AddComponent<MeshCollider>();
                mc.convex = true;
                if (go.GetComponent<Rigidbody>() == null)
                {
                    var rb = go.AddComponent<Rigidbody>();
                    rb.mass = 1f;
                }
            }
            else
            {
                go.AddComponent<BoxCollider>(); // 기본값
            }
        }

        // B) 캐릭터 컨트롤이 필요하면 CharacterController or NavMeshAgent 추가
        // var controller = root.GetComponent<CharacterController>() ?? root.AddComponent<CharacterController>();

        // C) 네 프로젝트의 이동 로직(MainView) 자동 부착(최상위에)
        var mainView = root.GetComponent<MainView>();
        if (mainView == null)
            mainView = root.AddComponent<MainView>();

        // D) 태그/레이어 설정(정책에 맞게)
        // root.tag = "Interactable";
        // root.layer = LayerMask.NameToLayer("Unit");

        // E) LODGroup, Animator 등 자동화
        // var animator = root.GetComponent<Animator>() ?? root.AddComponent<Animator>();
        // animator.applyRootMotion = false;
    }
}