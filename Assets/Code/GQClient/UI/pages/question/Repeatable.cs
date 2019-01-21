using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Repeatable {

    string RepeatButtonText { get; set; }

    string RepeatText { get; set; }

    string RepeatImage { get; set; }

    bool RepeatUntilSuccess { get; set; }


}
