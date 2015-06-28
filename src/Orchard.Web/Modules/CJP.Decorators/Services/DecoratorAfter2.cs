using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace CJP.Decorators.Services {
    [OrchardDecorator(Priority)]
    [OrchardFeature("CJP.Decorators.DecoratorAfter2")]
    public class DecoratorAfter2 : IDecoratorTest
    {
        private const string Priority = "2.0.0:after";

        private readonly INotifier _notifier;
        private readonly IDecoratorTest _decoratedService;

        public DecoratorAfter2(INotifier notifier, IDecoratorTest decoratedService)
        {
            _notifier = notifier;
            _decoratedService = decoratedService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Test()
        {
            _notifier.Add(NotifyType.Information, T("This message is from a decorator with priority {0}", Priority));
            _decoratedService.Test();
        }
    }
}