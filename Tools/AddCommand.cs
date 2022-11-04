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
        private Action<Object> actionToInvokePar = null;
        private Action<Object, Object> actionToInvokePar2 = null;
        private Action actionToInvoke = null;

        public bool CanExecute(object? parameter)
        {
            if (actionToInvoke != null)
                return true;

            if (actionToInvokePar != null)
                return true;

            if (actionToInvokePar2 != null)
                return true;

            return false;
        }

        public void Execute()
        {
            if (actionToInvoke != null)
                this.actionToInvoke();
        }

        public void Execute(Object p)
        {
            if (actionToInvokePar != null)
                this.actionToInvokePar(p);
        }

        public void Execute(Object p1, Object p2)
        {
            if (actionToInvokePar2 != null)
                this.actionToInvokePar2(p1, p2);
        }

        public AddCommand(Action<Object> actionToInvoke)
        {
            this.actionToInvokePar = actionToInvoke;
        }

        public AddCommand(Action actionToInvoke)
        {
            this.actionToInvoke = actionToInvoke;
        }

        public AddCommand(Action<Object, Object> actionToInvoke)
        {
            this.actionToInvokePar2 = actionToInvoke;
        }
    }
}
