namespace Pjfm.Application.Common.Interfaces
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}