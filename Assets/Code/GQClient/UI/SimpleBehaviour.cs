//#define DEBUG_LOG

namespace GQ.Client.UI
{
    public interface SimpleBehaviour : AbstractBehaviour
    {
        void OnProgress(float percent);
    }
}