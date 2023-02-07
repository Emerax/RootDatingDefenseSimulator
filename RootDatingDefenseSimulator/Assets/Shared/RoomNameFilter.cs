using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class RoomNameFilter : MonoBehaviour {

    private readonly Regex allowedSymbolsRgx = new("[^a-zA-Z]");
    private readonly int maxLength = 5;

    private TMP_InputField inputField;

    private void Awake() {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(OnRoomNameChanged);
    }

    private void Start() {
        // Auto-focus
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void OnRoomNameChanged(string roomName) {
        roomName = roomName.ToUpper();
        roomName = allowedSymbolsRgx.Replace(roomName, "");
        if(roomName.Length > maxLength) {
            roomName = roomName.Substring(0, maxLength);
        }
        inputField.text = roomName;
    }
}
