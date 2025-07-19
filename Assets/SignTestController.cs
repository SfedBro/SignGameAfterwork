using UnityEngine;
using System.Collections.Generic;

public class SignTestController : MonoBehaviour
{
    private bool isTestActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TestSigns();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            TestSigns2();
        }
    }

    private void TestSigns()
    {
        if (SignCanvasController.Instance == null)
        {
            Debug.LogError("SignCanvasController instance not found!");
            return;
        }

        if (SignCanvasController.Instance.IsAnimating())
        {
            Debug.Log("SignCanvasController is currently animating, skipping test.");
            return;
        }

        List<string> testSigns;

        List<string> testSigns2;

        if (!isTestActive)
        {
            testSigns = new List<string> { "Fire1", "Stone1", "Water1" };
            testSigns2 = new List<string> { "Fire1", "Stone1", "Water1", "Air1" };
        }
        else
        {
            testSigns = new List<string>();
            Debug.Log("Clearing all signs");
        }

        SignCanvasController.Instance.SetSigns(testSigns);
    }

    private void TestSigns2()
    {
        if (SignCanvasController.Instance == null)
        {
            Debug.LogError("SignCanvasController instance not found!");
            return;
        }

        if (SignCanvasController.Instance.IsAnimating())
        {
            Debug.Log("SignCanvasController is currently animating, skipping test.");
            return;
        }

        List<string> testSigns;


        if (!isTestActive)
        {
            testSigns = new List<string> { "Fire1", "Stone1", "Water1", "Air1" };
        }
        else
        {
            testSigns = new List<string>();
            Debug.Log("Clearing all signs");
        }

        SignCanvasController.Instance.SetSigns(testSigns);
    }

}