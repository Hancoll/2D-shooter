using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CharacterController Character;
    public Vector3 defaultOffset;
    public Vector2 inventoryPlayerOffset;//Смещение пресонажа при открытии инвентаря
    public float cameraSpeed;
    Vector3 offset;
    public bool lightingStatus = true;
    //public GameObject lighting;

    private void Start()
    {
        offset = defaultOffset;
        GameManager.MotionController.eventOnChangeInputDirection += SetMotionOffset;
        GameManager.MotionController.eventOnPointerUp += ClearMotionOffset;
        GameManager.GameUIManager.onInventoryChangeActive += OnInventoryChangeActive;
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = Character.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, newPosition, cameraSpeed * Time.fixedDeltaTime);
    }

    public void OnChangeLightingStatus(bool lightingStatus)
    {
        /* НА БУДУЩЕЕ . . .
        if(this.lightingStatus != lightingStatus)
        {
            lighting.SetActive(!lightingStatus);            
            this.lightingStatus = lightingStatus;
        }
        */
    }

    void OnInventoryChangeActive(bool isActive)
    {
        if (isActive)
            offset = defaultOffset + (Vector3)inventoryPlayerOffset;
        else
            ClearMotionOffset();
    }

    #region Motion

    void SetMotionOffset(Vector2 direction) => offset = defaultOffset + (Vector3)direction;

    void ClearMotionOffset() => offset = defaultOffset;

    #endregion
}
