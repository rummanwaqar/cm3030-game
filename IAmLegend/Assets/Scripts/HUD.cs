using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] RectTransform PlayerHealthBar;
    [SerializeField] RectTransform DogHealthBar;
    [SerializeField] TMP_Text tmpScore;
    [SerializeField] TMP_Text tmpTime;
    [SerializeField] TMP_Text tmpAmmo;

    public enum Damage
    {
        ZombieDamage,
        BossDamage
    };

    // Update the health bar of the player based on the damage type
    public void updateHealthBarPlayer(Damage damageType)
    {
        if( damageType == Damage.ZombieDamage )
            healthBarUpdate(PlayerHealthBar, 25);
        else if( damageType == Damage.BossDamage )
            healthBarUpdate(PlayerHealthBar, 50);
    }

    // Update the health bar of the dog based on the damage type
    public void updateHealthBarDog( Damage damageType )
    {
        if( damageType == Damage.ZombieDamage )
            healthBarUpdate(DogHealthBar, 25);
        else if( damageType == Damage.BossDamage )
            healthBarUpdate(DogHealthBar, 50);
    }

    // Update the size of a given health bar
    void healthBarUpdate( RectTransform bar, float damageAmount)
    {
        // Shrink the width of the health bar. The Y value stays the same
        float currentHealth = bar.sizeDelta.x;
        if(currentHealth > 0)
            bar.sizeDelta = new Vector2(currentHealth - damageAmount, 7);
    }

    // ONLY EXAMPLES, THERE IS NO NEED OF THIS FUNCTION
    void Start()
    {
        updateHealthBarDog(Damage.ZombieDamage);
        updateHealthBarPlayer(Damage.ZombieDamage);

        int score = 1;
        score++;
        tmpScore.text = score.ToString();
    }
}
