using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class revMeterHandler: MonoBehaviour
{
    [Header("Rev Meter")]
   
    [Tooltip("The current value of the Rev Meter is O")]
    [SerializeField] private float revVal = 0.0f;

    [Tooltip("The max value of the Rev Meter is O")]
    [SerializeField] private float maxRevVal = 80f;
   
    [Tooltip("The amount at which the rev meter increases on trigger is O")]
    [SerializeField] private float revBuildVal = 10f;
    
    [Tooltip("The amount at which the rev meter increases on parry is O")]
    [SerializeField] private float parryRevBuildVal = 5f;
    
    [Tooltip("The amount the Rev meter will decay is O")]
    [SerializeField] private float revDecayVal = 0f;
    
    [Tooltip("The slider which displays the rev meter")]
    [SerializeField] private Slider revMeterUI;
    
    //the particle system that spawns when rev-ing
    
    [Tooltip("rev p effects")]
    [SerializeField] private ParticleSystem revEffects;

    [SerializeField] private float revEffectsTimer = 0f;
    // timer for the particle effects to despawn after last input
    [SerializeField] private float revEffectsDespawnTime = 1.0f;

    //Determines the different levels of rev meter
    private enum revMeterLevel
    {
        level3 = (int)(80 * 0.75f),
        level2 = (int)(80 * 0.5f), level1 = (int)(80 * 0.25f), level0 = 0
    }

    [Header("The current level of the Rev meter is O")]
    [SerializeField] private revMeterLevel currRevLevel;

    private enum revMoveCost { tap = 5 }
    [SerializeField] private revMoveCost currRevCost;

    // Start is called before the first frame update
    void Awake()
    {
        revMeterUI = GameObject.Find("RevMeter").GetComponent<Slider>();
        //set revmeter Ui, decay value, and current meter level
        revMeterUI.maxValue = maxRevVal;
        revDecayVal = (revMeterUI.maxValue - revMeterUI.minValue) / 15f;
        currRevLevel = revMeterLevel.level0;

        revEffects = GameObject.Find("RevAura_PS").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //update the rev meter
        UpdateRevLevel();

        //clamp the current rev meter value 
        revVal = Mathf.Clamp(revVal - (revDecayVal * Time.deltaTime), (float)currRevLevel, revMeterUI.maxValue);
        revMeterUI.value = Mathf.Clamp(revVal, revMeterUI.minValue, revMeterUI.maxValue);

        //decrement the time for the rev effects to despawn
        revEffectsTimer = Mathf.Clamp(revEffectsTimer - Time.deltaTime, 0, 200);

        // if the timer reaches 0, stop the rev effects
        if (revEffectsTimer <= 0) revEffects.Stop();
    }

    public void playRevEffect()
    {
        if (!revEffects.isPlaying) revEffects.Play();
        revEffectsTimer = revEffectsDespawnTime;
    }

    //Increases the rev meter
    public void BuildRevMeter() //mashing trigger will increase rev meter slightly, stops once the value reaches 100
    {
        //If current rev value is less than max and the player is not moving, increase it
        if (revVal < revMeterUI.maxValue)
        {
            Debug.Log("building rev");
            revVal += revBuildVal;
        }
    }

    //Updates the rev level
   public  void UpdateRevLevel()
    {
        //determine rev level based on current rev value
        if (revVal >= (float)revMeterLevel.level3) { currRevLevel = revMeterLevel.level3; }
        else if (revVal >= (float)revMeterLevel.level2) { currRevLevel = revMeterLevel.level2; }
        else if (revVal >= (float)revMeterLevel.level1) { currRevLevel = revMeterLevel.level1; }
        else { currRevLevel = revMeterLevel.level0; }

        //clamp the current rev meter value 
        revVal = Mathf.Clamp(revVal - (revDecayVal * Time.deltaTime), (float)currRevLevel, revMeterUI.maxValue);
        revMeterUI.value = Mathf.Clamp(revVal, revMeterUI.minValue, revMeterUI.maxValue);
    }

    public IEnumerator DespawnRevEffects()
    {
        yield return new WaitForSeconds(1.5f);
        revEffects.Stop();
    }

    public void setRevCost()
    {
        currRevCost = revMoveCost.tap;
    }

    public void addParryVal()
    {
        if (revVal <= maxRevVal - parryRevBuildVal)
        {
            revVal += parryRevBuildVal;
        }
    }

    public bool checkRevAttackModifier()
    {
        if (revVal >= (int)currRevCost)
        {
            revVal -= (int)currRevCost;
            UpdateRevLevel();
            return true;
            //Debug.Log("doing buffed damage!");
        }
        return false;
    }


}
