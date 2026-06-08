using UnityEngine;
using UnityEngine.UI;

public class EnergyDisplay : MonoBehaviour
{
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField] private Image[] energyImages;
    [SerializeField] private Color fullColor = new Color(0.2f, 0.75f, 1f, 1f);
    [SerializeField] private Color emptyColor = new Color(0.08f, 0.1f, 0.14f, 0.55f);

    void Start()
    {
        if (playerEnergy == null)
            playerEnergy = FindAnyObjectByType<PlayerEnergy>();
    }

    public void SetTarget(PlayerEnergy energy, Image[] images)
    {
        playerEnergy = energy;
        energyImages = images;
    }

    void Update()
    {
        if (playerEnergy == null || energyImages == null) return;

        for (int i = 0; i < energyImages.Length; i++)
        {
            if (energyImages[i] == null) continue;
            energyImages[i].color = i < playerEnergy.currentEnergy ? fullColor : emptyColor;
        }
    }
}
