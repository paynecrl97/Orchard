using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;
using Orchard.Glimpse.Services;
using Orchard.Glimpse.Tabs.Routes;
using Orchard.Mvc.Routes;

namespace Orchard.Glimpse.AlternateImplementation {
    [OrchardFeature(FeatureNames.Routes)]
    internal class GlimpseRoutePublisherDecorator : IRoutePublisher {
        private readonly IRoutePublisher _decoratedService;
        private readonly IGlimpseService _glimpseService;

        public GlimpseRoutePublisherDecorator(IRoutePublisher decoratedService, IGlimpseService glimpseService) {
            _decoratedService = decoratedService;
            _glimpseService = glimpseService;
        }

        public void Publish(IEnumerable<RouteDescriptor> routes, Func<IDictionary<string, object>, Task> pipeline = null) {

            foreach (var route in routes) {
                _glimpseService.PublishMessage(new RoutesMessage {
                    Name = route.Name,
                    Priority = route.Priority
                });
            }

            _decoratedService.Publish(routes, pipeline);
        }
    }
}