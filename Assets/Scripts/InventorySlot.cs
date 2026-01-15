using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI Bilesenleri")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    [HideInInspector] public int myIndex;
    [HideInInspector] public List<InventoryItem> myOwnerList;
    [HideInInspector] public bool isChestSlot;

    private Canvas canvas;
    public static GameObject globalGhostIcon; // Global Hayalet

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void Setup(List<InventoryItem> ownerList, int index, bool isChest)
    {
        myOwnerList = ownerList;
        myIndex = index;
        isChestSlot = isChest;

        if (myIndex < myOwnerList.Count && myOwnerList[myIndex] != null && myOwnerList[myIndex].data != null)
        {
            SetItem(myOwnerList[myIndex]);
        }
        else
        {
            SetItem(null);
        }
    }

    public void SetItem(InventoryItem item)
    {
        if (item != null && item.data != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = item.data.icon;
            amountText.enabled = true;
            amountText.text = item.quantity > 1 ? item.quantity.ToString() : ""; // 1 ise sayi yazmasin

            Color c = iconImage.color; c.a = 1f; iconImage.color = c;
        }
        else
        {
            iconImage.enabled = false;
            amountText.enabled = false;
        }
    }

    // --- SURUKLEME ISLEMLERI (Degismedi) ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!iconImage.enabled) return;
        if (globalGhostIcon != null) Destroy(globalGhostIcon);

        globalGhostIcon = new GameObject("GhostIcon");
        globalGhostIcon.transform.SetParent(canvas.transform);
        globalGhostIcon.transform.SetAsLastSibling();

        Image ghostImage = globalGhostIcon.AddComponent<Image>();
        ghostImage.sprite = iconImage.sprite;
        ghostImage.raycastTarget = false;

        Color gc = ghostImage.color; gc.a = 0.6f; ghostImage.color = gc;
        globalGhostIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (globalGhostIcon != null) globalGhostIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (globalGhostIcon != null) Destroy(globalGhostIcon);
    }

    // --- KRITIK BOLGE: BIRAKMA ANI ---
    // --- KRITIK BOLGE: BIRAKMA ANI (GUVENLI VERSIYON) ---
    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag.GetComponent<InventorySlot>();

        if (draggedSlot != null)
        {
            // 1. Kaynagi (Elimizdeki esyayi) al
            InventoryItem sourceItem = draggedSlot.myOwnerList[draggedSlot.myIndex];

            // 2. Hedefi (Biraktigimiz kutuyu) GUVENLI sekilde al
            InventoryItem targetItem = null;

            // Eger biraktigimiz kutunun numarasi, listenin boyutundan kucukse (Veri varsa)
            if (myIndex < myOwnerList.Count)
            {
                targetItem = myOwnerList[myIndex];
            }
            // Eger myIndex buyukse, targetItem "null" kalir (Yani orasi bostur)


            // 3. Karar Ani: Birlestir mi, Degistir mi?

            // Eger hedef bos degilse (null degil) 
            // VE datasi varsa 
            // VE ayni esyaysa 
            // VE yiginlanabilirse -> STACK
            if (targetItem != null && targetItem.data != null &&
                sourceItem.data == targetItem.data &&
                targetItem.data.isStackable)
            {
                StackItems(draggedSlot, sourceItem, targetItem);
            }
            // Diger tum durumlarda (Bos yere koyma veya farkli esyayla takas) -> SWAP
            else
            {
                SwapItems(draggedSlot.myOwnerList, draggedSlot.myIndex, myOwnerList, myIndex);
            }
        }
    }

    // --- YENI: BIRLESTIRME FONKSIYONU ---
    void StackItems(InventorySlot sourceSlot, InventoryItem sourceItem, InventoryItem targetItem)
    {
        if (globalGhostIcon != null) Destroy(globalGhostIcon);

        int maxStack = sourceItem.data.maxStackSize;
        int totalCount = sourceItem.quantity + targetItem.quantity;

        // Senaryo A: Hepsi Sigiyor (2 + 1 = 3, Limit 99)
        if (totalCount <= maxStack)
        {
            targetItem.quantity = totalCount;
            // Kaynak slotu tamamen bosalt
            sourceSlot.myOwnerList[sourceSlot.myIndex] = new InventoryItem();
        }
        // Senaryo B: Tasma Var (98 + 5 = 103, Limit 99)
        else
        {
            targetItem.quantity = maxStack; // Hedefi fulle
            sourceItem.quantity = totalCount - maxStack; // Artani kaynakta birak
        }

        RefreshAllUI();
    }

    // --- YER DEGISTIRME FONKSIYONU ---
    void SwapItems(List<InventoryItem> sourceList, int sourceIndex, List<InventoryItem> targetList, int targetIndex)
    {
        if (globalGhostIcon != null) Destroy(globalGhostIcon);

        // Liste guvenligi
        while (sourceList.Count <= sourceIndex) sourceList.Add(new InventoryItem());
        while (targetList.Count <= targetIndex) targetList.Add(new InventoryItem());

        InventoryItem temp = sourceList[sourceIndex];
        sourceList[sourceIndex] = targetList[targetIndex];
        targetList[targetIndex] = temp;

        RefreshAllUI();
    }

    void RefreshAllUI()
    {
        if (InventoryUI.instance != null) InventoryUI.instance.UpdateUI();
        if (ChestUI.instance != null) ChestUI.instance.UpdateUI();
    }
}