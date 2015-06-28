using System;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace CJP.Decorators.Services
{
    public class BaseImplementation : IDecoratorTest
    {
        private readonly INotifier _notifier;

        public BaseImplementation(INotifier notifier)
        {
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Test()
        {
            _notifier.Add(NotifyType.Error, T("This message is from the base implementation (This Guid is to ensure uniqueness - {0})", Guid.NewGuid()));
        }
    }
}