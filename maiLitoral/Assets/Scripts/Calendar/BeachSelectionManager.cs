using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Manages beach selection, shows the calendar and updates the selected beach title.
public class BeachSelectionManager : MonoBehaviour
{
    public GameObject zoneListPanel;
    public GameObject beachCalendarPanel;
    public TMP_Text selectedBeachTitle;


    // Called when a beach/zone panel is pressed.
    public void SelectBeach(string beachName)
    {
        Debug.Log("CLICK WORKED: " + beachName);

        selectedBeachTitle.text = beachName;

        zoneListPanel.SetActive(false);
        beachCalendarPanel.SetActive(true);
    }

    public void BackToZoneList()
    {
        Debug.Log("SEARCH BAR CLICKED - BACK TO LIST");

        beachCalendarPanel.SetActive(false);
        zoneListPanel.SetActive(true);
    }
}

