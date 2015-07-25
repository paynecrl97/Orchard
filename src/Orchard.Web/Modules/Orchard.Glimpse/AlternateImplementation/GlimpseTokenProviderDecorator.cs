using Orchard.Environment.Extensions;
using Orchard.Glimpse.Services;
using Orchard.Glimpse.Tabs.Tokens;
using Orchard.Tokens;

namespace Orchard.Glimpse.AlternateImplementation {
    [OrchardDecorator]
    [OrchardFeature(FeatureNames.Tokens)]
    public class GlimpseTokenProviderDecorator : ITokenProvider {
        private readonly ITokenProvider _decoratedProvider;
        private readonly IGlimpseService _glimpseService;

        public GlimpseTokenProviderDecorator(ITokenProvider decoratedProvider, IGlimpseService glimpseService)
        {
            _decoratedProvider = decoratedProvider;
            _glimpseService = glimpseService;
        }

        public void Describe(DescribeContext context) {
             _decoratedProvider.Describe(context);
        }

        public void Evaluate(EvaluateContext context) {
            _glimpseService.PublishTimedAction(() => _decoratedProvider.Evaluate(context), t =>
                new TokenMessage {
                    Context = context,
                    Duration = t.Duration
                }, TimelineCategories.Tokens, "Evaluate", context.Target);
        }
    }
}