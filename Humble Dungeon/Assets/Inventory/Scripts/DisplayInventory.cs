using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DisplayInventory : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();
    [Tooltip("The itemSlot prefab")] public GameObject itemSlotPrefab;
    [Tooltip("The inventoryObject that this script should be displaying")] public InventoryObject inventory;
    public float x_Start;
    public float y_Start;
    public int x_Space_Between_Items;
    public int y_Space_Between_Items;
    public int numOfColumns;
    public int numOfRows;
    [Tooltip("A Vector2 where we will place the inventory when it's suppose to be shown")] public Vector2 inventoryPosOnScreen;
    public bool invEnabled = true;

    [HideInInspector]
    public Vector3 objSize;

    private bool invShown;
    private Vector2 invPosStart;
    protected Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();

    void Awake()
    {
        objSize = new Vector3(itemSlotPrefab.GetComponent<RectTransform>().rect.width, itemSlotPrefab.GetComponent<RectTransform>().rect.height, 0);
        AdjustPrefabLocations(transform.gameObject, itemSlotPrefab, new Vector3(x_Space_Between_Items, y_Space_Between_Items, 0));
        GameObject par = transform.parent.gameObject;
        invPosStart.y = par.transform.position.y - par.GetComponent<CanvasScaler>().referenceResolution.y * 2;
    }

    void Start()
    {
        CreateHotBar();
        CreateDisplay();
    }

    void Update()
    {
        UpdateDisplay();
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowHideInventory();
        }
    }

    void CreateHotBar()
    {
        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.hotBarSlots; i++)
        {
            GameObject obj = Instantiate(itemSlotPrefab, Vector3.zero, Quaternion.identity, transform.parent);
            obj.GetComponent<RectTransform>().localPosition = GetHotBarPosition(i, inventory.hotBarSlots);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragBegin(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragExit(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            itemsDisplayed.Add(obj, inventory.container.items[i]);
        }
    }

    void CreateDisplay()
    {
        for (int i = 0; i < inventory.maxItems; i++)
        {
            GameObject obj = Instantiate(itemSlotPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragBegin(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragExit(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            itemsDisplayed.Add(obj, inventory.container.items[i]);
        }
    }

    public void UpdateDisplay()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> slot in itemsDisplayed)
        {
            if (slot.Value.Id >= 0)
            {
                try
                {
                    slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.Value.Id].sprite;
                    slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                    slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = slot.Value.amount == 1 ? "" : slot.Value.amount.ToString("n0");
                }
                catch (KeyNotFoundException)
                {
                    Debug.LogError("You need to add all of your scriptable ItemObjects into a ItemDatabase || Add a ItemDatabase then lock the inspecter then select and drag all of your itemObjects into the Items array");
                    throw;
                }
            }
            else
            {
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    public void AdjustPrefabLocations(GameObject parentObj, GameObject objectPrefab, Vector3 spacing)
    {
        Vector3 parentObjLength = new Vector3(parentObj.GetComponent<RectTransform>().rect.width, parentObj.GetComponent<RectTransform>().rect.height, 0);
        Vector3 spaceTakenByObj = new Vector3(objectPrefab.GetComponent<RectTransform>().rect.width, objectPrefab.GetComponent<RectTransform>().rect.height, 0);

        numOfColumns = Mathf.FloorToInt(parentObjLength.x / (spaceTakenByObj.x + spacing.x));
        numOfRows = Mathf.FloorToInt(parentObjLength.y / (spaceTakenByObj.y + spacing.y));

        float leftSide = parentObjLength.x / -2f;
        leftSide += (spaceTakenByObj.x) / 2; //Add the objects extents so half of it isn't off the parent
        float spaceUsed = (numOfColumns * spaceTakenByObj.x) + ((numOfColumns - 1) * spacing.x);//Spacing is inbetween the objects so there will be one less than the numOfColumns of objects we have
        float spaceRemaining = parentObjLength.x - spaceUsed;
        x_Start = leftSide + (spaceRemaining / 2);

        float topSide = parentObjLength.y / 2f;
        topSide -= (spaceTakenByObj.y) / 2;//Add the objects extents so half of it isn't off the parent
        float verticalSpaceUsed = ((numOfRows * spaceTakenByObj.y) + ((numOfRows - 1) * spacing.y));
        float verticalSpaceRemaining = parentObjLength.y - verticalSpaceUsed;
        y_Start = topSide - (verticalSpaceRemaining / 2);
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        mouseItem.hoverObj = obj;
        if (itemsDisplayed.ContainsKey(obj))
        {
            mouseItem.hoverItem = itemsDisplayed[obj];
        }
    }

    public void OnExit(GameObject obj)
    {
        mouseItem.hoverObj = null;
        mouseItem.hoverItem = null;
    }
    public void OnDragBegin(GameObject obj)
    {
        var mouseObj = new GameObject();
        var rt = mouseObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(objSize.x, objSize.y);
        mouseObj.transform.SetParent(transform.parent);
        if (itemsDisplayed[obj].Id >= 0)
        {
            var img = mouseObj.AddComponent<Image>();
            img.sprite = inventory.database.GetItem[itemsDisplayed[obj].Id].sprite;
            img.raycastTarget = false;
        }
        mouseItem.obj = mouseObj;
        mouseItem.item = itemsDisplayed[obj];
    }
    public void OnDragExit(GameObject obj)
    {
        if (mouseItem.hoverObj)
        {
            inventory.MoveItem(itemsDisplayed[obj], itemsDisplayed[mouseItem.hoverObj]);
        }
        else
        {
            inventory.DropItem(itemsDisplayed[obj], itemsDisplayed[obj].item, itemsDisplayed[obj].amount);
        }
        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }
    public void OnDrag(GameObject obj)
    {
        if (mouseItem.obj != null)
        {
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void ShowHideInventory()
    {
        if (invEnabled)
        {
            Vector3 pos = GetComponent<RectTransform>().localPosition;
            if (invShown)
            {
                GetComponent<RectTransform>().localPosition = new Vector2(inventoryPosOnScreen.x, invPosStart.y);
            }
            else
            {
                GetComponent<RectTransform>().localPosition = new Vector2(inventoryPosOnScreen.x, inventoryPosOnScreen.y);
            }
            invShown = !invShown;
        }
    }

    private Vector3 GetHotBarPosition(int i, int maxSlots)
    {
        Vector3 midScreen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, -Screen.height));
        midScreen.x = objSize.x * (float)(maxSlots / 2);
        return midScreen;
    }

    private Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start + (objSize.x + x_Space_Between_Items) * (i % numOfColumns), y_Start + (-y_Space_Between_Items - objSize.y) * ((i / numOfColumns)), 0f);
    }
}

public class MouseItem
{
    public GameObject obj;
    public GameObject hoverObj;
    public InventorySlot item;
    public InventorySlot hoverItem;
}
