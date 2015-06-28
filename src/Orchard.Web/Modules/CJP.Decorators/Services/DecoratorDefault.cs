using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace CJP.Decorators.Services {
    [OrchardDecorator]
    [OrchardFeature("CJP.Decorators.DecoratorDefault")]
    public class DecoratorDefault : IDecoratorTest
    {
        private readonly INotifier _notifier;
        private readonly IDecoratorTest _decoratedService;

        public DecoratorDefault(INotifier notifier, IDecoratorTest decoratedService)
        {
            _notifier = notifier;
            _decoratedService = decoratedService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Test()
        {
            _notifier.Add(NotifyType.Warning, T("This message is from a decorator with the default priority"));
            _decoratedService.Test();
        }
    }
}