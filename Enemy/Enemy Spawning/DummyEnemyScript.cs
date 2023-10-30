using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyScript : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;

    private void Awake()
    {
        waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
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
    }
}
