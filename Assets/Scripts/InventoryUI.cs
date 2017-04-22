using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private Inventory VisInventory;
    GameObject[] ItemsUI;
    [SerializeField]
    private RectTransform Content;
    [SerializeField]
    private GameObject tooltip;
    Canvas cnv;
    
    //size inventory Nx6
    private void Start()
    {
        cnv = FindObjectOfType<Canvas>();
    }

    void DrawItem(int i)
    {
        if (cnv == null)
            cnv = FindObjectOfType<Canvas>();

        RectTransform rect = ItemsUI[i].AddComponent<RectTransform>();
        rect.SetParent(Content);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchorMin = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.localPosition = new Vector3(i%6 * 64f +4.6f, i / 6 * -64f - 4.6f, 0);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 59 * cnv.scaleFactor);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 59 * cnv.scaleFactor);
        Image img = ItemsUI[i].AddComponent<Image>();
        img.sprite = VisInventory.Items[i].Icon;

        //-------------------------Draw Items Count----------------
        GameObject cnt = new GameObject("Count");
        RectTransform cntRect = cnt.AddComponent<RectTransform>();
        cntRect.SetParent(rect);
        cntRect.pivot = new Vector2(1, 0);
        cntRect.anchorMax = new Vector2(1, 0);
        cntRect.anchorMin = new Vector2(1, 0);
        cntRect.anchoredPosition = new Vector3(-2, 0, 0);
        cntRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 50 * cnv.scaleFactor);
        cntRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20 * cnv.scaleFactor);
        Text cntText = cnt.AddComponent<Text>();
        cntText.text = "x"+VisInventory.Items[i].Count.ToString();
        cntText.color = Color.white;
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        cntText.font = ArialFont;
        cntText.resizeTextForBestFit = true;
        cntText.resizeTextMaxSize = 20;
        cntText.resizeTextMinSize = 10;
        cntText.alignment = TextAnchor.LowerRight;
        //-----Add ItemIcon Component
        ItemIcon ic = ItemsUI[i].AddComponent<ItemIcon>();
        ic.tooltip = tooltip;
        ic.item = VisInventory.Items[i];

    }

    void DrawCount(int i)
    {

    }
    public void Activate(Inventory visInventory)
    {
        VisInventory = visInventory;
        ItemsUI = new GameObject[visInventory.Items.Count];
        for (int i = 0; i < visInventory.Items.Count; i++)
        {
            ItemsUI[i] = new GameObject(visInventory.Items[i].Name);
            DrawItem(i);
        }

    }
    public void DeActivate()
    {
        for (int i = 0; i < ItemsUI.Length; i++)
        {
            Destroy(ItemsUI[i]);
        }
    }
}
