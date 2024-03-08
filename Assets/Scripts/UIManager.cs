using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI foeStateText;

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateStateText(foeState state)
    {
        foeStateText.text = state.ToString();
        foeStateText.color = ColorFetchur.GetColour(state);
    }
}
