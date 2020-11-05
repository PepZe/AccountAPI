using System.Collections.Generic;

namespace ProcessoSeletivo.Model.Interface
{
    public interface IDao<T>
    {
        public bool Include(T account);

        public List<T> Get();
        public T Search(int id);
    }
}
