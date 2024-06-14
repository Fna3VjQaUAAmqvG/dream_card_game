using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InitLoad : MonoBehaviour
{
    public AssetReference persistent;
    private void Awake() {
        Addressables.LoadSceneAsync(persistent);
    }
}