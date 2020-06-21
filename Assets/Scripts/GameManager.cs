using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [Header("Game")]
    [SerializeField] Transform gameSpace;//Parent всех объектов в игре
    public static Transform GameSpace { get { return gameManager.gameSpace; } }

    [SerializeField] LayerMask invisibleLayerMask;//Слой за которым монстры не видят игрока
    public static LayerMask InvisibleLayerMask { get { return gameManager.invisibleLayerMask; } }

    [SerializeField] GradualLoadingMapManager gradualLoadingMapManager;
    public static GradualLoadingMapManager GradualLoadingMapManager { get { return gameManager.gradualLoadingMapManager; } }

    [SerializeField] int loadingDistance;
    public static int LoadingDistance { get { return gameManager.loadingDistance; } }

    [SerializeField] int diedLayerMask;
    public static int DiedLayerMask { get { return gameManager.diedLayerMask; } }

    [Header("Monsters")]
    [SerializeField] GameObject markerPrefab;
    public static GameObject MarkerPrefab { get { return gameManager.markerPrefab; } }

    [SerializeField] Transform markersContainer;
    public static Transform MarkersContainer { get { return gameManager.markersContainer; } }

    public static int MinSpawnDistance { get { return gameManager.minSpawnDistance; } }
    [SerializeField] int minSpawnDistance;

    [Header("Inventory")]
    [SerializeField] InventorySystem inventorySystem;
    public static InventorySystem InventorySystem { get { return gameManager.inventorySystem; } }

    [SerializeField] InventoryController inventoryController;
    public static InventoryController InventoryController { get { return gameManager.inventoryController; } }

    [SerializeField] InventoryContainer inventoryContainer;
    public static InventoryContainer InventoryContainer { get { return gameManager.inventoryContainer; } }

    [SerializeField] InventoryContainer activeInventoryContainer;
    public static InventoryContainer ActiveInventoryContainer { get { return gameManager.activeInventoryContainer; } }

    [SerializeField] InventoryContainer storageInventoryContainer;
    public static InventoryContainer StorageInventoryContainer { get { return gameManager.storageInventoryContainer; } }

    [SerializeField] GameObject storagePanel;
    public static GameObject StoragePanel { get { return gameManager.storagePanel; } }

    [SerializeField] GameObject itemNamePanel;
    public static GameObject ItemNamePanel { get { return gameManager.itemNamePanel; } }

    [Header("Character")]
    [SerializeField] Transform characterTransform;
    public static Transform CharacterTransform { get { return gameManager.characterTransform; } }

    [SerializeField] CharacterStats characterStats;
    public static CharacterStats CharacterStats { get { return gameManager.characterStats; } }

    [SerializeField] CharacterController characterController;
    public static CharacterController CharacterController { get { return gameManager.characterController; } }

    [SerializeField] MobileController motionController;
    public static MobileController MotionController { get { return gameManager.motionController; } }

    [SerializeField] LayerMask characterLayerMask;
    public static LayerMask CharacterLayerMask { get { return gameManager.characterLayerMask; } }

    [Header("Camera")]
    [SerializeField] CameraController cameraController;
    public static CameraController CameraController { get { return gameManager.cameraController; } }

    [SerializeField] GameObject loadingAreaSpriteMask;
    public static GameObject LoadingAreaSpriteMask { get { return gameManager.loadingAreaSpriteMask; } }

    [Header("UI")]
    [SerializeField] GameUIManager gameUIManager;
    public static GameUIManager GameUIManager { get { return gameManager.gameUIManager; } }

    private static GameManager gameManager;//instance

    private void Awake() => gameManager = this;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(cameraController.transform.position + transform.position, minSpawnDistance);
        Gizmos.DrawWireSphere(cameraController.transform.position + transform.position, loadingDistance);
        loadingAreaSpriteMask.transform.localScale = loadingDistance * Vector2.one * 2;
    }
}
