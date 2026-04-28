
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SearchBarScript : MonoBehaviour
{
    public TMP_InputField nameInput;

    public void SearchBeach()
    {
        string searchText = nameInput.text.ToLower().Trim();

        Debug.Log("Text introdus in FirstPage: " + searchText);

        if (string.IsNullOrEmpty(searchText))
        {
            Debug.Log("Nu ai introdus nimic.");
            return;
        }

        BeachSearchData.searchText = searchText;
        Debug.Log("Salvez in BeachSearchData: " + BeachSearchData.searchText);

        SceneManager.LoadScene("MainContent");
    }
}