using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Xml.Serialization;
using GQ.Client.Err;
using System.Xml;

namespace GQ.Client.Model
{

    public abstract class ActionAbstractWithVar : ActionAbstract
    {
        #region Structure

        private string varName = null;

        public string VarName
        {
            get
            {
                return varName;
            }
            protected set
            {
                varName = value;
            }
        }

        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            VarName = GQML.GetStringAttribute(GQML.VARIABLE, reader);
        }

        #endregion

    }
}
