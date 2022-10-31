using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace wpfProjectNewsReader.Tools
{
    public class AddCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private Action<Object>? actionToInvoke = null;

        public bool CanExecute(object? parameter)
        {
            if (actionToInvoke != null)
                return true;
            else
                return false;
        }

        public void Execute(object? parameter)
        {
            if (actionToInvoke != null)
                this.actionToInvoke(parameter);
        }

        public AddCommand(Action<Object> actionToInvoke)
        {
            this.actionToInvoke = actionToInvoke;
        }
    }
}
