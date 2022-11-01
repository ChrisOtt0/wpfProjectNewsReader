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
        private Action<Object>? actionToInvokePar = null;
        private Action actionToInvoke = null;

        public bool CanExecute(object? parameter)
        {
            if (actionToInvoke != null)
                return true;

            if (actionToInvokePar != null)
                return true;

            return false;
        }

        public void Execute(object? parameter)
        {
            if (actionToInvokePar != null)
                this.actionToInvokePar(parameter);
            if (actionToInvoke != null)
                this.actionToInvoke();
        }

        public AddCommand(Action<Object> actionToInvoke)
        {
            this.actionToInvokePar = actionToInvoke;
        }

        public AddCommand(Action actionToInvoke)
        {
            this.actionToInvoke = actionToInvoke;
        }
    }
}
