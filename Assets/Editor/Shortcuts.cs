using UnityEngine;
using UnityEditor;

public class Shortcuts : Editor {
    [MenuItem ("GameObject/ActiveToggle _`")]
    static void ToggleActivationSelection()
    {
        foreach (GameObject go in Selection.gameObjects)
            go.SetActive(!go.activeSelf);
    }
}
