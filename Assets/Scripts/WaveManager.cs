using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    private int _waveNumber = 0;
    #region Unity Methods
    private void Awake()
    {
        if (Instance == null || Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }

    private void Start()
    {
        _waveNumber = PlayerPrefs.GetInt("WaveNumber");
    } 
    #endregion

    public int WaveNumber
    {
        get => _waveNumber;
        set => _waveNumber = value;
    }

    public void IncreaseWaveNumber() => _waveNumber++;
}
