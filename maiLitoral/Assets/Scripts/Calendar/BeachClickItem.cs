using UnityEngine;
using UnityEngine.EventSystems;

// Detects click directly on a beach/zone card.
public class BeachClickItem : MonoBehaviour, IPointerClickHandler
{
    public BeachSelectionManager manager;
    public string beachName;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CARD CLICKED: " + beachName);
        manager.SelectBeach(beachName);
    }
}
