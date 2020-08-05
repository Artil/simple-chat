using System.Threading.Tasks;

namespace ChatCore.Factories
{
    public interface IAbstractFactory<out TEntity>
    {
        TEntity GetInstance();
    }

    public interface IAbstractFactoryAsync<TEntity>
    {
        Task<TEntity> GetInstanceAsync();
    }

    public interface IAbstractFactory<out TEntity, in TParam>
    {
        TEntity GetInstance(TParam param);
    }

    public interface IAbstractFactoryAsync<TEntity, in TParam>
    {
        Task<TEntity> GetInstanceAsync(TParam param);
    }

    public interface IAbstractFactory<out TEntity, in TParam1, in TParam2>
    {
        TEntity GetInstance(TParam1 param, TParam2 param2);
    }

    public interface IAbstractFactoryAsync<TEntity, in TParam1, in TParam2>
    {
        Task<TEntity> GetInstanceAsync(TParam1 param, TParam2 param2);
    }

}
