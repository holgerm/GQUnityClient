using Code.GQClient.Model.gqml;

namespace Code.GQClient.Model.conditions
{

    public interface ICondition : IParentedXml
	{
		bool IsFulfilled ();

	}

}