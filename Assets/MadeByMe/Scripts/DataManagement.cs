using System.Collections.Generic;
using UnityEngine;

public class DataManagement : MonoBehaviour
{
    /// <summary>
    /// The list of NPCs in the 2nd class carriage
    /// </summary>
    [SerializeField]
    private List<string> fCarrage = new List<string>();

    /// <summary>
    /// The list of NPCs in the 3rd class carriage
    /// </summary>
    [SerializeField]
    private List<string> nfCarrage = new List<string>();

    // Start is called before the first frame update
    void Awake()
    {
        //This makes sure the object is not destroyed when the scene changes
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds an NPC to the storage string
    /// </summary>
    /// <param name="npc"></param>
    public void SetFData(string npc)
    {
        fCarrage.Add(npc);
    }

    /// <summary>
    /// Gets the 2nd class carriage storage list and returns it
    /// </summary>
    /// <returns></returns>
    public List<string> ApplyFData()
    {
        if(fCarrage != null)
        {
            return fCarrage;
        }
        return null;
    }
    
    /// <summary>
    /// Gets the 3rd class carriage storage list and returns it
    /// </summary>
    /// <returns></returns>
    public List<string> ApplyNFData()
    {
        if (nfCarrage != null)
        {
            return nfCarrage;
        }
        return null;
    }

    /// <summary>
    /// Adds an NPC to the 3rd class carriage storage list
    /// </summary>
    /// <param name="npc"></param>
    public void SetNFData(string npc)
    {
        nfCarrage.Add(npc);
    }
}
