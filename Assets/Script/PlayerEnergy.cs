using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour
{
    public int maxEnergy = 5;
    public int currentEnergy;
    [SerializeField] private bool createRuntimeDisplay = true;
    [SerializeField] private Vector2 displayAnchoredPosition = new Vector2(36f, -72f);

    public bool IsFull => currentEnergy >= maxEnergy;

    void Start()
    {
        if (createRuntimeDisplay)
            CreateRuntimeDisplay();
    }

    public void AddEnergy(int amount)
    {
        if (amount <= 0) return;

        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
        Debug.Log("Player Energy: " + currentEnergy + "/" + maxEnergy);
    }

    public bool CanHeal(PlayerHealth playerHealth)
    {
        return IsFull && playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth;
    }

    public bool UseHeal(PlayerHealth playerHealth)
    {
        if (!CanHeal(playerHealth)) return false;

        bool healed = playerHealth.Heal(1);
        if (!healed) return false;

        currentEnergy = 0;
        Debug.Log("Energy heal used.");
        return true;
    }

    public void ResetEnergy()
    {
        currentEnergy = 0;
    }

    private void CreateRuntimeDisplay()
    {
        if (FindAnyObjectByType<EnergyDisplay>() != null) return;

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        Sprite sprite = CreateDisplaySprite();
        GameObject root = new GameObject("EnergyDisplay", typeof(RectTransform), typeof(EnergyDisplay));
        root.transform.SetParent(canvas.transform, false);

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0f, 1f);
        rootRect.anchorMax = new Vector2(0f, 1f);
        rootRect.pivot = new Vector2(0f, 1f);
        rootRect.anchoredPosition = displayAnchoredPosition;
        rootRect.sizeDelta = new Vector2(150f, 22f);

        Image[] images = new Image[maxEnergy];
        for (int i = 0; i < maxEnergy; i++)
        {
            GameObject cell = new GameObject("Energy " + (i + 1), typeof(RectTransform), typeof(Image));
            cell.transform.SetParent(root.transform, false);

            RectTransform cellRect = cell.GetComponent<RectTransform>();
            cellRect.anchorMin = new Vector2(0f, 0.5f);
            cellRect.anchorMax = new Vector2(0f, 0.5f);
            cellRect.pivot = new Vector2(0f, 0.5f);
            cellRect.anchoredPosition = new Vector2(i * 24f, 0f);
            cellRect.sizeDelta = new Vector2(18f, 18f);

            Image image = cell.GetComponent<Image>();
            image.sprite = sprite;
            image.color = new Color(0.08f, 0.1f, 0.14f, 0.55f);
            images[i] = image;
        }

        EnergyDisplay display = root.GetComponent<EnergyDisplay>();
        display.SetTarget(this, images);
    }

    private Sprite CreateDisplaySprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));
    }
}
