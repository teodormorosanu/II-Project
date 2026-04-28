using UnityEngine;

public class TESTSCRIPT : MonoBehaviour {
    [SerializeField] private GameObject zoneList;
    public void Searching() {
        zoneList.SetActive(true);
    }
    public void StoppedSearching() {
        zoneList.SetActive(false);
    }
}
