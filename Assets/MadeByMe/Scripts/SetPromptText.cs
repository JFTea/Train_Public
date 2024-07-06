using UnityEngine;
using TMPro;

public class SetPromptText : MonoBehaviour
{
    //The amount of NPCs that can be given tickets
    public int promptAmount = 0;

    public void AddAmount()
    {
        promptAmount++;
        GetComponent<TMP_Text>().text = "Remaining: " + promptAmount;
    }

    public void RemoveAmount()
    {
        promptAmount--;
        GetComponent<TMP_Text>().text = "Remaining: " + promptAmount;
    }

}
