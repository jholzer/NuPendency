using Ninject.Parameters;

namespace NuPendency.Commons.Interfaces
{
    public interface IInstanceCreator
    {
        T CreateInstance<T>(ConstructorArgument[] arguments);
    }
}