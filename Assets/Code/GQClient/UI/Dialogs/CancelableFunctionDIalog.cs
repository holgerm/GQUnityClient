using System;
using System.Collections;
using System.Collections.Generic;
using GQ.Client.GQEvents;
using UnityEngine;

namespace GQ.Client.UI.Dialogs
{
    public class CancelableFunctionDialog : DialogBehaviour
    {
        private string title { get; set; }
        private string message { get; set; }
        private string doButtonText { get; set; }
        private string cancelText { get; set; }
        private Action doFunction { get; set; }

        public CancelableFunctionDialog(
            string title,
            string message,
            Action cancelableFunction,
            string doButtonText = "Ok",
            string cancelText = "Abbrechen"
        ) : base(null) // 'null' because we do NOT connect a Task, sice cancel dialogs only rely on user interaction

        {
            this.title = title;
            this.message = message;
            this.doButtonText = doButtonText;
            this.cancelText = cancelText;
            this.doFunction = cancelableFunction;
        }

        public override void Start()
        {
            base.Start();

            Dialog.Title.text = title;
            Dialog.Img.gameObject.SetActive(false);
            Dialog.Details.text = message;
            Dialog.SetYesButton(cancelText, CloseDialog);
            Dialog.SetNoButton(
                doButtonText,
                (GameObject sender, EventArgs e) =>
                    {
                        doFunction();
                        CloseDialog(sender, e);
                    });

            // show the dialog:
            Dialog.Show();
        }


    }
}
