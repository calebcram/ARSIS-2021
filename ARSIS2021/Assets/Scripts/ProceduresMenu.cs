using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System;

public class ProceduresMenu : MonoBehaviour
{
    void Start()
    {
        ListManager list = this.GetComponent<ListManager>(); 
        List<Procedure> procedures = TaskManager.S.allProcedures;
        foreach (Procedure p in procedures)
        {
           for (int j = 0; j < p.Tasks.Length; j++)
           {
                GameObject listItem = list.addListItem(p.Tasks[j].Title);

                Interactable interact = listItem.GetComponent<Interactable>();
                interact.OnClick.AddListener(() =>
                {
                    MenuController.s.currentProcedure = 0;
                    MenuController.s.currentTask = 0;
                    MenuController.s.currentSubTask = 0;
                    VoiceManager.S.generateTaskMenu();
                });  
            
                // TODO: set up needed interaction 
                // voice control, functions it triggers, etc 
           }

            
        }
    }
}
