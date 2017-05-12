using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Activities;
using System.Activities.Presentation;
using System.IO;
using System.Activities.Statements;
namespace Workflow.CustomerDesigner
{
    public class CommandView
    {
        internal CommandView()
        {
            //this.NewCommand = new ReplayCommand(ExecuteNew, null);
        }
       
    }

    public class ReplayCommand : ICommand 
    {
        private Action<object> execute;
        private Predicate<object> canExecute;

        public  ReplayCommand(Action<object> execute, Predicate<object> canPredicate)
        {
            this.execute = execute;
            this.canExecute = canPredicate;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
