using System.Collections;
using UnityEngine;

public class DynamiteTimer : MonoBehaviour
{
    [SerializeField] Transform _explosionPlacement;
    [SerializeField] GameObject _explosion;

    public void StartTimer() => StartCoroutine(Timer());

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(3f);
        Instantiate(_explosion, _explosionPlacement.position, Quaternion.identity);
    }
}
