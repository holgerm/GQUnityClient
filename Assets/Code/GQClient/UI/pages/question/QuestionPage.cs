using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.Model
{
    public abstract class QuestionPage : DecidablePage, Repeatable
    {
        public string RepeatButtonText { get; set; }

        public string RepeatText { get; set; }

        public string RepeatImage { get; set; }

        public bool RepeatUntilSuccess { get; set; }
    }
}
