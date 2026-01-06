using System;

namespace LendingApp.UI.CashierUI
{
    public static class ReceiptEvents
    {
        // Fired when a new receipt is created by payment processing.
        // Handlers run on the UI thread where they are subscribed.
        public static event Action<ReceiptDto> ReceiptCreated;

        public static void RaiseReceiptCreated(ReceiptDto d)
        {
            ReceiptCreated?.Invoke(d);
        }
    }
}
