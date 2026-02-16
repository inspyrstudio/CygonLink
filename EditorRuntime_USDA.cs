using UnityEngine;

public class EditorRuntime_USDA
{
    private string logPrefix = "Cygon Link";

    public void SendLog(string color,  string message)
    {
        Debug.Log($"<b>{logPrefix}</b> <color={color}>{message}</color>");
    }
}
