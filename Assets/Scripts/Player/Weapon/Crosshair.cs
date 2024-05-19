using Game.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField]
    private GameObject Parent;
    [SerializeField]
    private RectTransform Dot;
    [SerializeField]
    private RectTransform Left;
    [SerializeField]
    private RectTransform Right;
    [SerializeField]
    private RectTransform Up;
    [SerializeField]
    private RectTransform Down;

    [Header("Preferences")]
    [SerializeField]
    private bool Visible = true;
    [SerializeField]
    private bool CenterDot = true;
    [SerializeField]
    private int Gap = 10;
    [SerializeField]
    private Vector2 GapMovementFlex;
    [SerializeField]
    private float GapFlexSmoothing = 1;
    [SerializeField]
    private int Width = 3;
    [SerializeField]
    private int Length = 15;
    [SerializeField]
    private Color _Colour = Color.green;

    private Image[] Images;
    [SerializeField]
    private bool Images_found = false;
    private GameObject Player;
    private PlayerMovement Player_Movement;
    private PlayerAnimationScript Player_AnimationScript;
    [SerializeField]
    private float gap_flexOffset = 0f;
    private void FindImages() 
    {
        bool found = true;
        Images = new Image[5];
        Images[0] = Dot.gameObject.GetComponent<Image>();
        Images[1] = Left.gameObject.GetComponent<Image>();
        Images[2] = Right.gameObject.GetComponent<Image>();
        Images[3] = Up.gameObject.GetComponent<Image>();
        Images[4] = Down.gameObject.GetComponent<Image>();
        foreach (Image image in Images) 
        {
            if (image == null) found = false;
            break;
        }
        Images_found = found;
    }

    // Start is called before the first frame update
    void Start()
    {
        FindImages();
        Player = GameObject.Find("Player");
        if(Player) Player_Movement = Player.GetComponent<PlayerMovement>();


    }

    // Update is called once per frame
    void Update()
    {
        if (!Player) 
        {
            Player = GameObject.Find("Player");
            if (!Player) return;
        }
        if (!Player_Movement) 
        {
            Player_Movement = Player.GetComponent<PlayerMovement>();
            if(!Player_Movement) return;
        }
        if (!Images_found) 
        {
            FindImages();
            if (!Images_found) 
            {
                Debugging.Log("Unable to find the image components on one of the crosshair objects");
                return;
            }
        }
        if (!Player_AnimationScript)
        {   
            Player_AnimationScript = Player_Movement.gameObject.GetComponent<PlayerAnimationScript>();
            return;
        }




        //Handle Visability
        if(Player_AnimationScript.weaponOut && !Parent.activeInHierarchy) Parent.SetActive(true);
        if (!Player_AnimationScript.weaponOut && Parent.activeInHierarchy) Parent.SetActive(false);

        //Handle the center dot
        if (CenterDot && !Dot.gameObject.activeInHierarchy) Dot.gameObject.SetActive(true);
        else if (!CenterDot && Dot.gameObject.activeInHierarchy) Dot.gameObject.SetActive(false);
        Vector2 size = new Vector2(Width, Width);
        Dot.sizeDelta = size;

        FlexCrosshair(); //Gets the crosshar flex offset from the players movement. (the amount the "Gap" changes)

        //Handle limbs
        int base_offset = (int)(Length / 2);
        Left.sizeDelta = new Vector2(Width, Length);
        Right.sizeDelta = new Vector2(Width, Length);
        Up.sizeDelta = new Vector2(Width, Length);
        Down.sizeDelta = new Vector2(Width, Length);

       
        Up.anchoredPosition = new Vector2(0, base_offset + Gap + gap_flexOffset);
        Right.anchoredPosition = new Vector2(base_offset + Gap + gap_flexOffset, 0);   
        Down.anchoredPosition = new Vector2(0, -(base_offset + Gap + gap_flexOffset));
        Left.anchoredPosition = new Vector2(-(base_offset + Gap + gap_flexOffset), 0);

        //Set colour
        foreach (Image img in Images) 
        {
            img.color = _Colour;
        }

       
    }

    private void FlexCrosshair() 
    {
        float mag = GameMath.VecXZ(Player_Movement.playerVelocity).magnitude;
        if (mag > 0.05f)
        {
            float ratio = mag / Player_Movement.SprintSpeed;            
            gap_flexOffset = Mathf.Lerp(GapMovementFlex.x, GapMovementFlex.y, ratio);
        }
        else 
        {
            gap_flexOffset = 0f;
        }
    }
    public void ToggleVisible() 
    {
        Visible = !Visible;
    }
    public void SetVisible(bool visible)
    {
        Visible = visible;
    }
    public void SetCenterDot(bool centerDot)
    {
        CenterDot = centerDot;
    }
    public void SetGap(int gap)
    {
        Gap = gap;
    }
    public void SetWidth(int width)
    {
        Width = width;
    }
    public void SetLength(int length)
    {
        Length = length;
    }
    public void SetColour(Color colour)
    {
        _Colour = colour;
    }
  
}
