using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] 
    private Health _health;


    [SerializeField] 
    private GameObject _healthBarParent;


    [SerializeField] 
    private Image _healthBarImage;


    private void Awake()
    {
        _health.ClientOnHealthUpdated += HandleHealthUpdated;
        
        _healthBarParent.SetActive(false);
    }


    private void OnDestroy()
    {
        _health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }


    private void OnMouseEnter()
    {
        _healthBarParent.SetActive(true);
    }


    private void OnMouseExit()
    {
        _healthBarParent.SetActive(false);
    }


    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        _healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
