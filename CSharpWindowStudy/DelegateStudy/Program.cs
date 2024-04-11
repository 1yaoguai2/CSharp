namespace DelegateStudy
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            DelegateClass delegateClass = new DelegateClass();
            DelegateClass.Arithmetic arithmetic = new DelegateClass.Arithmetic(delegateClass.Add);
            DelegateClass.TestA(arithmetic);

            DelegateClass.Arithmetic arithmetic1 = delegateClass.Des;
            DelegateClass.TestA(arithmetic1);

            DelegateClass.Arithmetic arithmetic2 = delegateClass.Mul;
            DelegateClass.TestA(arithmetic2);

            DelegateClass.Arithmetic arithmetic3 = delegateClass.Div;
            DelegateClass.TestA(arithmetic3);

            //泛型委托
            ActionOrFuncClass actionOrFuncClass = new ActionOrFuncClass();
            actionOrFuncClass._action = actionOrFuncClass.WriteResult;
            ActionOrFuncClass.TestB(actionOrFuncClass._action);

            actionOrFuncClass._func = delegateClass.Add;
            ActionOrFuncClass.TestC(actionOrFuncClass._func);
        }
    }
}