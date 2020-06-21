using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public Transform InventoryPanel;
    public Button openInventoryButton;
    public GameObject messageBox;
    public event System.Action<bool> onInventoryChangeActive;

    private void Start()
    {
        openInventoryButton.onClick.AddListener(delegate { OpenOrCloseInventoryPanel(); });
    }

    public void SendGameMessage(string message)
    {
        messageBox.GetComponent<Text>().text = message;
        messageBox.GetComponent<Animator>().SetTrigger("onSendMessage");
    }

    #region Panels

    public void OpenOrCloseInventoryPanel()
    {
        InventoryPanel.gameObject.SetActive(!InventoryPanel.gameObject.activeSelf);
        InventorySystem.inventoryIsOpen = InventoryPanel.gameObject.activeSelf;
        if (GameManager.StoragePanel.activeSelf)
        {
            if(GameManager.CharacterController.interactiveObject is Storage)
            {
                Storage storage = GameManager.CharacterController.interactiveObject as Storage;
                storage.items = GameManager.StorageInventoryContainer.items;
            }

            GameManager.StoragePanel.SetActive(false);
        }

        onInventoryChangeActive?.Invoke(InventorySystem.inventoryIsOpen);
    }

    public void OpenStoragePanel(ItemInspector[] storageItems)
    {
        OpenOrCloseInventoryPanel();

        if (!GameManager.StoragePanel.activeSelf)
        {
            GameManager.StoragePanel.SetActive(true);
            
            for(int i = 0; i < GameManager.StorageInventoryContainer.inventoryCells.Length; i++)
            {
                GameManager.StorageInventoryContainer.AddItem(storageItems[i].item, storageItems[i].itemCount, i);
            }
        }
    }

    #endregion

}
