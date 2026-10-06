using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class HealthBar : MonoBehaviour
{
    public  Slider slider;
    public TextMeshProUGUI healthCounter;
    public States States;
    private float currentHealth, maxHealth;
    private void Start()
    {
        slider=GetComponent<Slider>();

    }
    private void Update()
    {
        currentHealth = States.GetComponent<States>().currentHealth;
        maxHealth = States.GetComponent<States>().maxHealth;
        slider.value = (float)(currentHealth / maxHealth);
        healthCounter.text = currentHealth.ToString() + "/" + maxHealth.ToString(); 
        
      
    }

}
