
using UnityEngine;

public class MainContentSearchHandler : MonoBehaviour
{
    public GameObject beachesPanel_Nord;
    public GameObject beachesPanel_MamaiaNord;
    public GameObject beachesPanel_Mamaia;
    public GameObject beachesPanel_Constanta;
    public GameObject beachesPanel_Eforie;
    public GameObject beachesPanel_Costinesti;
    public GameObject beachesPanel_SudLitoral;
    public GameObject beachesPanel_Mangalia;
    public GameObject beachesPanel_VamaVeche;

    void Start()
    {
        Debug.Log("MainContentSearchHandler a pornit");
        Debug.Log("Text primit: " + BeachSearchData.searchText);

        string searchText = BeachSearchData.searchText.ToLower().Trim();

        HideAllPanels();

        if (searchText.Contains("nord") || searchText.Contains("corbu") || searchText.Contains("vadu") || searchText.Contains("midia"))
        {
            Debug.Log("Activez Nord");
            beachesPanel_Nord.SetActive(true);
        }
        else if (searchText.Contains("mamaia nord") || searchText.Contains("navodari"))
        {
            Debug.Log("Activez MamaiaNord");
            beachesPanel_MamaiaNord.SetActive(true);
        }
        else if (searchText.Contains("mamaia"))
        {
            Debug.Log("Activez Mamaia");
            beachesPanel_Mamaia.SetActive(true);
        }
        else if (searchText.Contains("constanta"))
        {
            Debug.Log("Activez Constanta");
            beachesPanel_Constanta.SetActive(true);
        }
        else if (searchText.Contains("eforie"))
        {
            Debug.Log("Activez Eforie");
            beachesPanel_Eforie.SetActive(true);
        }
        else if (searchText.Contains("costinesti"))
        {
            Debug.Log("Activez Costinesti");
            beachesPanel_Costinesti.SetActive(true);
        }
        else if (searchText.Contains("sud litoral") || searchText.Contains("sudlitoral"))
        {
            Debug.Log("Activez SudLitoral");
            beachesPanel_SudLitoral.SetActive(true);
        }
        else if (searchText.Contains("mangalia"))
        {
            Debug.Log("Activez Mangalia");
            beachesPanel_Mangalia.SetActive(true);
        }
        else if (searchText.Contains("vama veche") || searchText.Contains("vamaveche"))
        {
            Debug.Log("Activez VamaVeche");
            beachesPanel_VamaVeche.SetActive(true);
        }
        else
        {
            Debug.Log("Plaja nu a fost gasita.");
        }
    }

    void HideAllPanels()
    {
        beachesPanel_Nord.SetActive(false);
        beachesPanel_MamaiaNord.SetActive(false);
        beachesPanel_Mamaia.SetActive(false);
        beachesPanel_Constanta.SetActive(false);
        beachesPanel_Eforie.SetActive(false);
        beachesPanel_Costinesti.SetActive(false);
        beachesPanel_SudLitoral.SetActive(false);
        beachesPanel_Mangalia.SetActive(false);
        beachesPanel_VamaVeche.SetActive(false);
    }
}