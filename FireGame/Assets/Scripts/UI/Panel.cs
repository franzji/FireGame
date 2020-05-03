using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour
{

    public GameObject PanelName;
    private Animator anim;
    private bool isIn = false;
    void Start()
    {
        anim = PanelName.GetComponent<Animator>();

        anim.enabled = false;
    }

    public void Slide()
    {
        if (!isIn)
        {
            SlideIn();
        }
        else
        {
            SlideOut();
        }
    }


    private void SlideIn()
    {
        anim.enabled = true;
        transform.SetAsLastSibling();
        anim.Play("Panel1");
    }

    private void SlideOut()
    {
        anim.Play("Panel1R");
    }

    public void Setisintrue()
    {
        isIn = true;
    }
    public void Setinfalse()
    {
        isIn = false;
    }
}