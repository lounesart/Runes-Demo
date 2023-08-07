using UnityEngine;
using UnityEngine.UI;

public class Rune : MonoBehaviour
{
    [HideInInspector] public Image img;
    [HideInInspector] public RectTransform rectTrans;

    private RunesTree runesTree;
    private Button button;
    private int index;

    private void Awake()
    {
        img = GetComponent<Image>();
        rectTrans = GetComponent<RectTransform>();

        runesTree = rectTrans.parent.GetComponent<RunesTree>();

        index = rectTrans.GetSiblingIndex() - 1;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnItemClick);
    }

    private void OnItemClick()
    {
        runesTree.OnItemClick(index, img.sprite);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnItemClick);
    }
}
