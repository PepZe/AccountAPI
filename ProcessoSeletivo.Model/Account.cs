namespace ProcessoSeletivo.Model
{
    public class Account
    {
        public int Id { get; private set; }

        public double Balance { get; private set; }

        public Account(int id, double balance)
        {
            Id = id;
            Balance = balance;
        }

        public bool Withdraw(double value)
        {
            if (Balance > value)
            {
                Balance -= value;
                return true;
            }
            return false;
        }

        public bool Deposit(double value)
        {
            if (value > 0)
            {
                Balance += value;
                return true;
            }

            return false;
        }

        public bool Transfer(Account destiny, double value)
        {
            if (Balance > value)
            {
                destiny.Deposit(value);
                Balance -= value;
                return true;
            }

            return false;
        }
    }
}
