using System.ComponentModel.Composition;

namespace Ocean.Communication.Commands
{
    [InheritedExport]
    public interface ICommand
    {
        void Execute(CallerContext callerContext, string[] args, ref string message);
    }
}