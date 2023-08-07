using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RunesTree : MonoBehaviour
{
    [SerializeField] private Vector2 spacing;

    [SerializeField] private float expandDuration;
    [SerializeField] private float collapseDuration;
    [SerializeField] private Ease expandEase;
    [SerializeField] private Ease collapseEase;

    [SerializeField] private float expandFadeDuration;
    [SerializeField] private float collapseFadeDuration;

    [SerializeField] private bool useSpiral = false;

    private Button mainButton;
    private Rune[] menuItems;
    private Rune[] secondaryMenuItems;
    private Vector2[] initialPositions;

    private bool isExpanded = false;
    private Vector2 mainButtonPosition;
    private Vector2 secondaryMainButtonPosition;
    private int itemsCount;
    private int secondaryItemsCount;
    private Image mainButtonImage;

    private int mainRuneIndex = 0;
    private int previousRuneIndex = 0;

    private void Start()
    {
        itemsCount = transform.childCount - 1;
        menuItems = new Rune[itemsCount];
        initialPositions = new Vector2[itemsCount];

        for (int i = 0; i < itemsCount; i++)
        {
            menuItems[i] = transform.GetChild(i + 1).GetComponent<Rune>();
            initialPositions[i] = menuItems[i].rectTrans.anchoredPosition;
        }

        secondaryItemsCount = transform.childCount - itemsCount - 1;
        secondaryMenuItems = new Rune[secondaryItemsCount];
        for (int i = 0; i < secondaryItemsCount; i++)
        {
            secondaryMenuItems[i] = transform.GetChild(itemsCount + i + 2).GetComponent<Rune>();
        }

        mainButton = transform.GetChild(0).GetComponent<Button>();
        mainButton.onClick.AddListener(ToggleMenu);
        mainButton.transform.SetAsLastSibling();

        mainButtonPosition = mainButton.GetComponent<RectTransform>().anchoredPosition;
        mainButtonImage = mainButton.GetComponent<Image>();

        secondaryMainButtonPosition = menuItems[0].rectTrans.anchoredPosition;

        ResetPositions();
    }

    private void ResetPositions()
    {
        for (int i = 0; i < itemsCount; i++)
        {
            menuItems[i].rectTrans.anchoredPosition = mainButtonPosition;
        }

        for (int i = 0; i < secondaryItemsCount; i++)
        {
            secondaryMenuItems[i].rectTrans.anchoredPosition = secondaryMainButtonPosition;
        }
    }

    private void ToggleMenu()
    {
        isExpanded = !isExpanded;

        if (isExpanded)
        {
            if (useSpiral)
            {
                Spiral();
            }

            for (int i = 0; i < itemsCount; i++)
            {
                menuItems[i].rectTrans.DOAnchorPos(initialPositions[i], expandDuration).SetEase(expandEase);
                menuItems[i].img.DOFade(1f, expandFadeDuration).From(0f);
            }

            for (int i = 0; i < secondaryItemsCount; i++)
            {
                secondaryMenuItems[i].rectTrans.DOAnchorPos(secondaryMainButtonPosition + spacing * (i + 1), expandDuration).SetEase(expandEase);
                secondaryMenuItems[i].img.DOFade(1f, expandFadeDuration).From(0f);
            }
        }
        else
        {
            for (int i = 0; i < itemsCount; i++)
            {
                menuItems[i].rectTrans.DOAnchorPos(mainButtonPosition, collapseDuration).SetEase(collapseEase);
                menuItems[i].img.DOFade(0f, collapseFadeDuration);
            }

            for (int i = 0; i < secondaryItemsCount; i++)
            {
                secondaryMenuItems[i].rectTrans.DOAnchorPos(secondaryMainButtonPosition, collapseDuration).SetEase(collapseEase);
                secondaryMenuItems[i].img.DOFade(0f, collapseFadeDuration);
            }
        }
    }

    private void Spiral()
    {
        if (!useSpiral || itemsCount <= 1)
            return;

        float angleIncrement = 360f / itemsCount;
        float radius = spacing.magnitude;

        for (int i = 0; i < itemsCount; i++)
        {
            float angle = i * angleIncrement;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            menuItems[i].rectTrans.anchoredPosition = mainButtonPosition + new Vector2(x, y);
        }
    }

    public void ToggleSpiral(bool value)
    {
        useSpiral = value;

        if (!isExpanded)
            return;

        ResetPositions();
        ToggleMenu();
    }

    public void OnItemClick(int index, Sprite newSprite)
    {
        if (isExpanded)
        {
            if (index >= 0 && index < itemsCount)
            {
                Vector2 clickedRunePosition = menuItems[index].rectTrans.anchoredPosition;
                Vector2 previousRunePosition = menuItems[previousRuneIndex].rectTrans.anchoredPosition;
                menuItems[previousRuneIndex].rectTrans.anchoredPosition = clickedRunePosition;
                menuItems[index].rectTrans.anchoredPosition = previousRunePosition;

                Sprite mainButtonSprite = mainButtonImage.sprite;
                mainButtonImage.sprite = newSprite;
                menuItems[index].img.sprite = mainButtonSprite;

                mainRuneIndex = index;
                previousRuneIndex = mainRuneIndex;
            }

            ToggleMenu();
        }
    }

    private void OnDestroy()
    {
        mainButton.onClick.RemoveListener(ToggleMenu);
    }
}
