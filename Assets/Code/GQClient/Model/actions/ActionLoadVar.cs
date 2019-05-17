using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.Model
{
    public class ActionLoadVar : ActionAbstractWithVar
    {
        public override void Execute()
        {
            Variables.LoadVariableFromStore(VarName);
        }
    }
}
