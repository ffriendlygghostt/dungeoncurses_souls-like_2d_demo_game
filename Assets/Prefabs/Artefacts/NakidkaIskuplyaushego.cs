using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Inventory/Effects/NakidkaIskuplyaushego")]
public class NakidkaIskuplyaushego : ItemEffect
{
    public float resistance = 1f; 

    public override void Apply(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.fireResistance += resistance;
            stats.fireResistance = Mathf.Clamp01(stats.fireResistance); 
        }
    }

    public override void Remove(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.fireResistance -= resistance;
            stats.fireResistance = Mathf.Clamp01(stats.fireResistance);
        }
    }
}
