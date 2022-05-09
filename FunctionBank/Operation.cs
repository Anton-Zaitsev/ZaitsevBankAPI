namespace ZaitsevBankAPI.FunctionBank
{
    public static class Operation
    {
        public enum OperationNumber
        {
            IncomingTransfer = 0, // Входящий перевод
            OutgoingTransfer = 1, // Исходящий перевод
            TakeCredit = 2,// Взять кредит
            PaymentCredit = 3,// Оплата кредита
            RepaymentCredit = 4,// Погащение кредита
            CurrencyTransfer = 5, // Перевод валюты
            PaymentServices = 6, // Оплата услуг
            ActivationCard = 7, // Активация карты
            DeActivationCard = 8, // Деактивация карты по сроку
            BetweenMyCards = 9
        }
        public static string getNameOperation(OperationNumber Operation)
        {
            return Operation switch
            {
                OperationNumber.IncomingTransfer => "Входящий перевод",
                OperationNumber.OutgoingTransfer => "Клиету ZaitsevBank",
                OperationNumber.TakeCredit => "Взятие кредита",
                OperationNumber.PaymentCredit => "Оплата по кредитному договору",
                OperationNumber.RepaymentCredit => "Погащение кредита",
                OperationNumber.CurrencyTransfer => "Перевод валюты",
                OperationNumber.PaymentServices => "Оплата услуг",
                OperationNumber.ActivationCard => "Активация карты",
                OperationNumber.DeActivationCard => "Деактивация карты по сроку",
                OperationNumber.BetweenMyCards => "Между своими счетами",
                _ => "Не найден тип операции",
            };
        }
    }
}
