﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeSettingsMenuManager : MenuManager
{
    [SerializeField] private GameObject colourSegment; 
    [SerializeField] private Image colourSample;

    private InteractiveNode node;
    
    private Slider[] _colourSliders;
    private TextMeshProUGUI[] _colourValues;
    private TMP_InputField _textField;

    /// <summary>
    /// Method <c>Start</c> closes the menu.
    /// </summary>
    private void Start()
    {
        CloseMenu();
        _colourSliders = colourSegment.GetComponentsInChildren<Slider>();
        _colourValues = FindObjectsOfType<TextMeshProUGUI>()
            .ToList().FindAll( x=>x.name.EndsWith("Value"))
            .OrderByDescending(x=>x.name).ToArray();
        _textField = GetComponentInChildren<TMP_InputField>();
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        base.OnEnable();
        NodeSideMenu.Editing += ToggleActive;
        InteractiveNode.NodeSelected += ChangeNode;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        base.OnDisable();
        NodeSideMenu.Editing -= ToggleActive;
        InteractiveNode.NodeSelected -= ChangeNode;
    }



    /// <summary>
    /// Method <c>ToggleActive</c> updates the menu for the new node.
    /// <param name="id_selected">The id of the selected node.</param>
    /// <param name="new_pos">The new position of the selected node.</param>
    /// <param name="game_obj">The selected node game object.</param>
    /// </summary>
    private void ChangeNode(int id_selected, Vector3 new_pos, GameObject game_obj)
    {
        node = game_obj.GetComponent<InteractiveNode>();
        UpdateColour(node.GetComponent<SpriteRenderer>().color);
    }

    /// <summary>
    /// Method <c>ToggleActive</c> toggles the active state of the menu.
    /// <param name="new_node">The node the menu is working for.</param>
    /// </summary>
    private void ToggleActive(InteractiveNode new_node)
    {
        node = new_node;
        base.ToggleActive();
        UpdateColour(node.GetComponent<SpriteRenderer>().color);
    }



    /// <summary>
    /// Method <c>ApplyColour</c> applies the chosen colour to the current node.
    /// </summary>
    public void ApplyColour()
    {
        node.ChangeColour(colourSample.color);
    }
    
    /// <summary>
    /// Method <c>OnColourAdjusted</c> changes the colour based on the values entered,
    /// and updates text boxes to express it's value in both decimal and hexadecimal.
    /// </summary>
    public void OnColourAdjusted()
    {
        if (_colourSliders.Length == 0) return;
        
        colourSample.color = new Color(_colourSliders[0].value/255, _colourSliders[1].value/255, 
            _colourSliders[2].value/255);
        for(var i = 0; i < _colourSliders.Length; i++)
        {
            var slider_val = (int) _colourSliders[i].value;
            _colourValues[i].text = $"{slider_val} ({slider_val:X2})";
        }
    }

    /// <summary>
    /// Method <c>ColourHexSelected</c> marks inputs as typing for the input manager.
    /// </summary>
    public void ColourHexSelected()
    {
        InputManager.isTyping = true;
    }
    
    /// <summary>
    /// Method <c>ColourHexInput</c> ensures only valid hex inputs are entered into the hex input field.
    /// </summary>
    public void ColourHexInput()
    {
        var text_val = _textField.text;
        text_val = "#" + Regex.Replace(text_val, "[^0-9ABCDEFabcdef]", "").ToUpper();
        
        _textField.text = text_val.Substring(0, Math.Min(text_val.Length, 7));
    }
    
    /// <summary>
    /// Method <c>OnColourHexAdjusted</c> converts the inputted hex value to decimal, and calls OnColourAdjusted.
    /// </summary>
    public void OnColourHexAdjusted()
    {
        InputManager.isTyping = false;
        for (var i = 0; i < 3; i++)
        {
            _colourSliders[i].value = int.Parse(_textField.text.Substring(1 + i * 2, 2),
                System.Globalization.NumberStyles.HexNumber);
        }
        OnColourAdjusted();
    }

    /// <summary>
    /// Method <c>UpdateColour</c> updates the current sample colour, as well as the input methods' current values.
    /// <param name="colour">The new colour for the sample.</param>
    /// </summary>
    private void UpdateColour(Color colour)
    {
        if (!IsActive) return;
        
        _textField.text = $"#{((int) colour.r*255).ToString("X2")}" +
                          $"{((int)colour.g*255).ToString("X2")}{((int)colour.b*255).ToString("X2")}";
        Debug.Log(_textField.text);
        OnColourHexAdjusted();
    }
}