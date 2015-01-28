using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Ocean.Communication.Commands
{
    [InheritedExport]
    public interface ICommand
    {
        void Execute(CallerContext callerContext, string[] args, ref string message);
    }
}