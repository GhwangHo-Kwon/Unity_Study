using UnityEditor;
using UnityEngine;

public class FbxImportProcessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        if (!assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase)) return;

        var importer = (ModelImporter)assetImporter;

        // --- Model 탭 기본값 강제 ---
        importer.globalScale = 1.0f;
        importer.useFileScale = false;
        importer.importNormals = ModelImporterNormals.Import;
        importer.importTangents = ModelImporterTangents.CalculateMikk;
        importer.isReadable = false;         // 콜라이더 생성 등 필요할 때만 켜세요.
        importer.importCameras = false;
        importer.importLights = false;

        // --- Rig 탭 ---
        // 캐릭터면 Humanoid로 자동 전환하고 싶다면 조건 추가
        // importer.animationType = ModelImporterAnimationType.Generic;

        // --- Materials ---
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
        // 프로젝트 정책 따라 Assign, Import, None로 변경
    }

    void OnPostprocessModel(GameObject g)
    {
        // 필요 시, 임포트 직후 FBX 하위 오브젝트 정리/태그/레이어 지정 자동화 가능
        // 단, 여기서 Prefab 저장까지 하는 건 비추천(분리된 메뉴툴로 처리 권장)
    }
}