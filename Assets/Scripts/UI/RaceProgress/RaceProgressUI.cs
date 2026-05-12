using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceProgressUI : MonoBehaviour
{
    public GameObject PinPrefab;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var skater in Skater.All)
        {
            _AddPin(skater);
        }

        Skater.OnSkaterSpawn += skater =>
        {
            _AddPin(skater);
        }; 
    }
    private void _AddPin(Skater TargetSkater)
    {
        var newPin = GameObject.Instantiate(PinPrefab);
        newPin.transform.SetParent(this.transform);
        newPin.GetComponent<RaceProgressPin>().SetTargetSkater(TargetSkater);
    }
}
