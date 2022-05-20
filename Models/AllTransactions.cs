namespace ZaitsevBankAPI.Models
{
    public class AllTransactions
    {
        public int TypeTransaction { get; set; } // Тип транзакции
        public string NameTransaction { get; set; } // Название транзакции
        public DateTime DateTime { get; set; } // Время транзакции
        public Guid TransactionId { get; set; } // ID транзакции 
        public TransactionTransfer? TransactionTransfer { get; set; } // Для исходящих и выходящих переводов

        public TransactionValuteBetween? TransactionValuteBetween { get; set; } // Между своими счетами и перевод валюты

        public TransactionCardOrCredit? TransactionCardOrCredit { get; set; } // Активация карты или кредита, а так же его закрытие

        public TransactionPaymentServices? TransactionPaymentServices { get; set; } // Оплата товаров или услуг

        public TransactionCredit? TransactionCredit { get; set; } // Погащения кредита
    }

    public class TransactionTransfer // Класс для входящих и исходящий переводов
    {  
        public bool IncomingTransfer { get; set; } // Входящяя ли транзакции, если нет, то исходящяя

        public TransactionPaymentServices TransactionPaymentServices { get; set; }

        public TransactionValute? TransactionValute { get; set; } // Была ли транзакция на перевод валюты 

    }

    public class TransactionValuteBetween // Между своими счетами и перевод валюты
    {
        public string FromCardName { get; set; } // Название карты из которой произошла транзакция денег
        public string ToCardName { get; set; } // Карта куда пришли деньги

        // Внизу могут быть nil, потому что может быть транзакция на перевод валюту, тогда они не нужны
        public double? CountValute { get; set; } 

        public string? ValuteType { get; set; } // Так же может быть опущен, так как на транзакцию перевода валюты он не играет роли
        public TransactionValute? TransactionValute { get; set; }
    }

    public class TransactionValute // Перевод валюты 
    {
        public bool BuySale { get; set; } // true покупка и false продажа
        public string ValuteA { get; set; } // Из валюты А
        public string ValuteB { get; set; } // В валюту B

        public double CountValuteA { get; set; }
        public double CountValuteB { get; set; }
    }

    public class TransactionCardOrCredit
    {
        public string Name { get; set; }

        public bool Activation { get; set; } // true Если Успешно активирована, false если заблокирована
    }

    public class TransactionPaymentServices // Оплата товаров или услуг и перевод валюты
    {
        public string NameClient { get; set; }

        public double CountMoney { get; set; }

        public string ValuteType { get; set; }

        public TransactionValute? TransactionValute { get; set; }

    }

    public class TransactionCredit
    {
        public string NameClient { get; set; }

        public double CountMoney { get; set; }

        public float Progress { get; set; }
    }
}
