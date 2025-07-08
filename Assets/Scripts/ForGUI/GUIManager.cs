using System.Collections;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    public Animator panelAnimator;
    public bool IsPanelActive { get; private set; } = false;

    public void PanelActivate(bool flag)
    {
        if (flag)
        {
            panel.SetActive(true);
            panelAnimator.SetBool("slide", true);
            IsPanelActive = true;
        }
        else
        {
            panelAnimator.SetBool("slide", false);
            StartCoroutine(DeactivatePanel());
        }
    }

    private IEnumerator DeactivatePanel()
    {
        yield return new WaitForSeconds(1.2f);
        panel.SetActive(false);
        IsPanelActive = false;
    }
}