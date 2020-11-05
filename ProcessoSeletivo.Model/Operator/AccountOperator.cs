using ProcessoSeletivo.Model.Enum;

namespace ProcessoSeletivo.Model.Operator
{
    public class AccountOperator
    {
        public string Type { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public double Amount { get; set; }
    }
}
