namespace Pjfm.WebClient.Services
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}