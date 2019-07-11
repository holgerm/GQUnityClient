namespace GQ.Client.Model
{

    public interface ICondition : IParentedXml
	{
		bool IsFulfilled ();

	}

}