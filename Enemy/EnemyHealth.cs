using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;

    [SerializeField] private float maxHP = 20f;
    [SerializeField] private float currHP;

    private void Awake()
    {
        try
        {
            waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
        }
        catch
        {

        }

        currHP = maxHP;
    }
    // decrease current HP by the damage value
    public void TakeDamage(int damage)
    {
        currHP = Mathf.Clamp(currHP - damage, 0, maxHP);

        

        CheckDeath();
    }

    private void CheckDeath()
    {
        if(currHP <= 0)
        {
            try
            {
                int index = waveManager.enemiesAlive.IndexOf(gameObject);
                if (index != -1)
                {
                    //remove the enemy from the list of alive enemies in the wave manager
                    waveManager.enemiesAlive[index] = null;
                    waveManager.enemiesAlive.RemoveAt(index);
                    //update the wave based on the enemy's death as needed

                    waveManager.UpdateWave();
                    Destroy(gameObject);
                }
            }
            catch
            {
                Destroy(gameObject);
            }
        }
    }
}
