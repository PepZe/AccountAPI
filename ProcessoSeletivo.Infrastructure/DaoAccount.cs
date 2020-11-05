using ProcessoSeletivo.Model;
using ProcessoSeletivo.Model.Interface;
using System.Collections.Generic;
using System.Linq;

namespace ProcessoSeletivo.Infrastructure
{
    public class DaoAccount : IDao<Account>
    {
        private readonly List<Account> _accounts;

        public DaoAccount()
        {
            _accounts = new List<Account>();
        }
        public bool Include(Account account)
        {
            _accounts.Add(account);
            return true;
        }

        public List<Account> Get()
        {
            return _accounts;
        }

        public Account Search(int id)
        {
            return _accounts.Where(ac => ac.Id == id).FirstOrDefault();
        }
    }
}
