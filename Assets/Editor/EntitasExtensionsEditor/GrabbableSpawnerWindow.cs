using System;
using Code.Gameplay.Interacting.Services;
using Code.Gameplay.Produce.Moulding;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.StaticData;
using UnityEditor;
using UnityEngine;

namespace EntitasExtensionsEditor
{
    public class GrabbableSpawnerWindow : EditorWindow
    {
        private CommonStaticData _commonStaticData;
        private Vector2 _scrollPosition;
        private Vector3 _spawnPosition = Vector3.zero;
        private bool _useSceneViewPosition = false;
        private bool _spawnNearPlayer = true;

        [MenuItem("Tools/Grabbable Spawner")]
        public static void ShowWindow()
        {
            GetWindow<GrabbableSpawnerWindow>("Grabbable Spawner");
        }

        private void OnEnable()
        {
            LoadCommonStaticData();
        }

        private void LoadCommonStaticData()
        {
            string[] guids = AssetDatabase.FindAssets("t:CommonStaticData");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _commonStaticData = AssetDatabase.LoadAssetAtPath<CommonStaticData>(path);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Grabbable Item Spawner", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_commonStaticData == null)
            {
                EditorGUILayout.HelpBox("CommonStaticData not found! Please create one in the project.", MessageType.Error);
                if (GUILayout.Button("Refresh"))
                {
                    LoadCommonStaticData();
                }
                return;
            }

            DrawSpawnSettings();
            EditorGUILayout.Space();
            DrawItemButtons();
        }

        private void DrawSpawnSettings()
        {
            EditorGUILayout.LabelField("Spawn Settings", EditorStyles.boldLabel);

            _spawnNearPlayer = EditorGUILayout.Toggle("Spawn Near Player", _spawnNearPlayer);

            if (!_spawnNearPlayer)
            {
                _useSceneViewPosition = EditorGUILayout.Toggle("Use Scene View Center", _useSceneViewPosition);

                if (!_useSceneViewPosition)
                {
                    _spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", _spawnPosition);
                }
            }
        }

        private void DrawItemButtons()
        {
            EditorGUILayout.LabelField("Spawn Items", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("You need to enter Play Mode to spawn items.", MessageType.Info);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (ItemsEnum item in Enum.GetValues(typeof(ItemsEnum)))
            {
                if (GUILayout.Button($"Spawn {item}", GUILayout.Height(30)))
                {
                    SpawnItem(item);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void SpawnItem(ItemsEnum itemType)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Cannot spawn items in Edit Mode!");
                return;
            }

            try
            {
                // Get factory from Zenject
                var factory = GetGrabbableFactory();

                if (factory == null)
                {
                    Debug.LogError("GrabbableFactory not found! Make sure Zenject is properly set up.");
                    return;
                }

                // Проверяем, является ли предмет формой (Mold)
                MoldEnum? moldEnum = GetMoldEnumFromItem(itemType);

                if (_spawnNearPlayer)
                {
                    // Для SpawnViewNearWithPlayer нужно вызвать SpawnAtPosition с позицией игрока
                    if (!_gameContext.isPlayer)
                    {
                        Debug.LogError("Player not found!");
                        return;
                    }

                    GameEntity playerEntity = _gameContext.playerEntity;
                    Vector3 spawnPosition = playerEntity.transform.Value.position + playerEntity.transform.Value.forward * 2f;
                    factory.SpawnAtPosition(itemType, spawnPosition, true, moldEnum);
                    Debug.Log($"Spawned {itemType} near player" + (moldEnum.HasValue ? $" with mold {moldEnum.Value}" : ""));
                }
                else
                {
                    Vector3 position = _useSceneViewPosition ? GetSceneViewPosition() : _spawnPosition;
                    factory.SpawnAtPosition(itemType, position, true, moldEnum);
                    Debug.Log($"Spawned {itemType} at position {position}" + (moldEnum.HasValue ? $" with mold {moldEnum.Value}" : ""));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to spawn {itemType}: {ex.Message}");
            }
        }

        private MoldEnum? GetMoldEnumFromItem(ItemsEnum itemType)
        {
            string itemName = itemType.ToString();

            // Проверяем, содержит ли название "Mold"
            if (!itemName.Contains("Mold"))
                return null;

            // Пытаемся найти соответствующий MoldEnum
            if (Enum.TryParse<MoldEnum>(itemName, out MoldEnum moldEnum))
            {
                return moldEnum;
            }

            return null;
        }

        private GameContext _gameContext => Contexts.sharedInstance.game;

        private IGrabbableFactory GetGrabbableFactory()
        {
            // Try to get factory from Zenject container
            var context = Zenject.ProjectContext.Instance;
            if (context != null && context.Container != null)
            {
                return context.Container.TryResolve<IGrabbableFactory>();
            }

            Debug.LogError("Zenject ProjectContext or Container is null!");
            return null;
        }

        private Vector3 GetSceneViewPosition()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && sceneView.camera != null)
            {
                // Spawn 5 units in front of the scene view camera
                return sceneView.camera.transform.position + sceneView.camera.transform.forward * 5f;
            }
            return Vector3.zero;
        }
    }
}
