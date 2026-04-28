using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//I will create a list for each beach where we will enter all the attributes of each beach and I will also make a 
// method to read the data from the UI
public class BeachManager : MonoBehaviour
{
    // propperties with data 
    public TMP_InputField nameInput;
    public TMP_InputField locationInput;
    public TMP_InputField descriptionInput;
    public TMP_InputField sandTypeInput;

    // yes/no properties, we will implement for every propertie a toggle switch in UI

    public Toggle hasSandToggle;
    public Toggle hasRocksToggle;

    public Toggle HasSunbedsToggle;
    public Toggle HasUmbrellasToggle;
    public Toggle HasCabinsToggle;
    public Toggle HasShowersToggle;
    public Toggle HasToiletsToggle;
    public Toggle HasParkingToggle;
    public Toggle IsEasyAccessToggle;
    public Toggle HasLifeguardToggle;
    public Toggle IsSafeToggle;
    public Toggle IsCleanToggle;


    // the list with every beach and his properties

    public List<BeachProperties> beaches = new List<BeachProperties>();

    public void BeachFromUI()
    {
        BeachProperties beach = new BeachProperties();

        beach.beachName = nameInput.text;
        beach.beachLocation = locationInput.text;
        beach.beachDescription = descriptionInput.text;

        beach.HasCabins = HasCabinsToggle.isOn;
        beach.HasSunbeds = HasSunbedsToggle.isOn;
        beach.HasUmbrellas = HasUmbrellasToggle.isOn;
        beach.HasShowers = HasShowersToggle.isOn;
        beach.HasToilets = HasToiletsToggle.isOn;
        beach.HasParking = HasParkingToggle.isOn;
        beach.IsEasyAccess = IsEasyAccessToggle.isOn;
        beach.HasLifeguard = HasLifeguardToggle.isOn;
        beach.IsSafe = IsSafeToggle.isOn;

        beaches.Add(beach);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}