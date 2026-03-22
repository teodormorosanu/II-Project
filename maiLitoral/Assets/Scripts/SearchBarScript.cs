using TMPro;
using UnityEngine;
//search bar and search button, when the name of a beach is written, it will be displayed the data of that beach
public class SearchBarScript : MonoBehaviour
{
    public TMP_InputField nameInput;
    public BeachManager beachManager;

    public TMP_Text resultText; // for output

    public void SearchBeach()
    {
        string searchText = nameInput.text.ToLower();

        foreach (var beach in beachManager.beaches)
        {
            if (beach.beachName.ToLower().Contains(searchText))
            {
                string result = beach.GetBeachInfo() + "\n" + beach.GetFacilities();

                Debug.Log("Found: " + result);

                if (resultText != null)
                    resultText.text = result;

                return;
            }
        }

        Debug.Log("Beach not found");

        if (resultText != null)
            resultText.text = "Beach not found";
    }
}