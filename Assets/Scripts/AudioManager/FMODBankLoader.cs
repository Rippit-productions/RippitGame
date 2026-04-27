using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class FMODBankLoader : MonoBehaviour
{
    public static FMODBankLoader Instance => FindFirstObjectByType<FMODBankLoader>();
    [FMODUnity.BankRef]
    public List<string> BankReferene = new List<string>();
    private static Task _LoadingTask;

    public static bool Loading => !_LoadingTask.IsCompleted;
    // Start is called before the first frame update
    void Awake()
    {
        _LoadingTask = LoadBanks();
    }


    private void Update()
    {
        
    }

    public async Task LoadBanks()
    {
        if (Loading) return;
        foreach (var bank in BankReferene)
        {
            FMODUnity.RuntimeManager.LoadBank(bank, true);
        }

        // Keep yielding the co-routine until all the bank loading is done
        while (!FMODUnity.RuntimeManager.HaveAllBanksLoaded)
        {
            await Task.Yield();
        }

        // Keep yielding the co-routine until all the sample data loading is done
        while (FMODUnity.RuntimeManager.AnySampleDataLoading())
        {
            await Task.Yield();
        }

        // Only added to test loading screen class
        await Task.Delay(2000);
        
    }
}
