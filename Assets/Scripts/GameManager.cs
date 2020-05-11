using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourSingleton<GameManager>
{

    protected override void OnAwake()
    {
    }

    // void Update()
    // {
    //     currentTurn.text = string.Format("Turn {0}", 0);
    //     if (Selection != null && Selection.CurrentAbility != null)
    //         currentSelection.text = string.Format("{0}\n {1}", Selection, Selection.CurrentAbility.Type);
    //     if (Selection.CurrentAbility == null)
    //     {
    //         moveButton.GetComponent<Image>().color = Color.white;
    //         actionButton.GetComponent<Image>().color = Color.white;
    //     }
    //     else
    //     {
    //         if (Selection.CurrentAbility.Type == AbilityType.Move)
    //         {
    //             moveButton.GetComponent<Image>().color = Color.green;
    //             actionButton.GetComponent<Image>().color = Color.white;
    //         }
    //         else if (Selection.CurrentAbility.Type == AbilityType.Action)
    //         {
    //             moveButton.GetComponent<Image>().color = Color.white;
    //             actionButton.GetComponent<Image>().color = Color.magenta;
    //         }
    //     }

    //     Vector2Int mousePos;
    //     Utilities.MousePositionOnMap(out mousePos);

    //     if (Input.GetMouseButtonDown(0)
    //         && MapManager.Instance.IsPositionOnMap(mousePos)
    //         && Selection != null
    //         && Selection.CurrentAbility != null
    //         && Selection.CurrentAbility.Type != AbilityType.None)
    //     {
    //         if (MapManager.Instance.VisualPath == null)
    //             Debug.LogFormat("Visual path is null, cannot cast {0}", Selection.CurrentAbility.Type);
    //         else if (MapManager.Instance.VisualPath.Contains(MapManager.Instance.CellAt(mousePos)))
    //         {
    //             bool cast = Selection.CurrentAbility.Cast(
    //                 MapManager.Instance.CellAt(Selection.Position),
    //                 MapManager.Instance.CellAt(mousePos));
    //             if (cast)
    //             {
    //                 if (Selection.CurrentAbility.Type == AbilityType.Move)
    //                 {
    //                     selectionLocked = true;
    //                 }
    //                 else if (Selection.CurrentAbility.Type == AbilityType.Action)
    //                 {
    //                     StartCoroutine(EndTurn());
    //                 }
    //             }
    //         }
    //     }

    //     MapManager.Instance.UpdateVisualPath();
    // }

    // public void SetCurrentAbility(AbilityType type)
    // {
    //     if (Selection != null
    //         && Selection.CurrentAbility != null
    //         && Selection.CurrentAbility.Type == type)
    //     {
    //         type = AbilityType.None;
    //     }
    //     Selection.SetCurrentAbility(type);
    // }
}