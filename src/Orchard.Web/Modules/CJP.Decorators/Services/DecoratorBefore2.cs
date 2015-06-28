using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace CJP.Decorators.Services {
    [OrchardDecorator(Priority)]
    [OrchardFeature("CJP.Decorators.DecoratorBefore2")]
    public class DecoratorBefore2 : IDecoratorTest
    {
        private const string Priority = "2.0.0:before";

        private readonly INotifier _notifier;
        private readonly IDecoratorTest _decoratedService;

        public DecoratorBefore2(INotifier notifier, IDecoratorTest decoratedService)
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