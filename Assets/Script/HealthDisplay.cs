using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HealthDisplay : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public Sprite emptyHeart;
    public Sprite fullHeart;
    public Image[] heartImages; // Array of Image components for each heart display

    public PlayerHealth playerHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < heartImages.Length; i++)
        {
            if (i < playerHealth.currentHealth)
            {
                heartImages[i].sprite = fullHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }
        }
    }
}
