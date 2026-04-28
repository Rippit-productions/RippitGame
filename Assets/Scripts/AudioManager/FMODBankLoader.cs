using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODBankLoader : MonoBehaviour
{
#if UNITY_ADDRESSABLES_EXIST
   // List of asset references to load if using Addressables
    public List<AssetReference> AssetReferenceBanks = new List<AssetReference>();
#else
    // List of Banks to load
    [FMODUnity.BankRef]
    public List<string> Banks = new List<string>();
#endif
    public FMODBankLoader Instance => FindFirstObjectByType<FMODBankLoader>();

    public static bool Loading = false;

    public static Action OnLoadingFinished = () => { };
    public void Awake()
    {
        if (!Loading)
        {
            StartCoroutine(LoadBanks());
        }
    }

    public IEnumerator LoadBanks()
    {
        if (Loading) yield break; 
#if UNITY_ADDRESSABLES_EXIST
        // Iterate all the asset references and start loading their studio banks
        // in the background, including their audio sample data
        foreach (var bank in AssetReferenceBanks)
        {
            FMODUnity.RuntimeManager.LoadBank(bank, true);
        }
#else
        // Iterate all the Studio Banks and start them loading in the background
        // including the audio sample data
        foreach (var bank in Banks)
        {
            FMODUnity.RuntimeManager.LoadBank(bank, true);
        }
#endif

        // (for platforms with asynchronous bank loading)
        while (!FMODUnity.RuntimeManager.HaveAllBanksLoaded)
        {
            yield return null;

        }

        while (FMODUnity.RuntimeManager.AnySampleDataLoading())
        {
            yield return null;
        }

        OnLoadingFinished.Invoke();
    }
}
