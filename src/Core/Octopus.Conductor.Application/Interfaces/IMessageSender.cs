using System.Threading.Tasks;

namespace Octopus.Conductor.Application.Interfaces
{
    public interface IMessageSender
    {
        Task SendMessage(object message);
    }
}
