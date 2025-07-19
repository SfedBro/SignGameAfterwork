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
        if (Input.GetKeyDown(KeyCode.J))
        {
            TestSigns3();
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

        testSigns = new List<string> { "Fire1", "Stone1", "Water1" };
        testSigns2 = new List<string> { "Fire1", "Stone1", "Water1", "Air1" };

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


        testSigns = new List<string> { "Fire1", "Stone1", "Water1", "Air1" };

        SignCanvasController.Instance.SetSigns(testSigns);
    }
    private void TestSigns3()
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


        testSigns = new List<string> { };

        SignCanvasController.Instance.SetSigns(testSigns);
    }

}