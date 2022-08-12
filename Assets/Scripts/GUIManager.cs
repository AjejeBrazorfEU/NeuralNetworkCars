using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textGUI;
    public SimulationManager simulationManager;

    void Update(){
        textGUI.SetText("Current epoch : " + simulationManager.getCurrentEpoch() + "\nElapsed Time : " + simulationManager.getElapsedTime());
    }
}
