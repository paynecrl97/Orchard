using System;
using System.Web;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;
using Glimpse.Core.Message;
using Orchard.Glimpse.Models;
using Orchard.Glimpse.Tabs;
using Orchard.Localization;
using ILogger = Orchard.Logging.ILogger;
using NullLogger = Orchard.Logging.NullLogger;

namespace Orchard.Glimpse.Services {
    public class DefaultGlimpseService : IGlimpseService
    {
        public DefaultGlimpseService()
        {
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; private set; }

        public TimerResult Time(Action action)
        {
            var executionTimer = GetTimer();

            if (executionTimer == null) {
                action();
                return null;
            }

            return executionTimer.Time(action);
        }

        public TimedActionResult<T> Time<T>(Func<T> action)
        {
            var result = default(T);

            var executionTimer = GetTimer();

            if (executionTimer == null) {
                return new TimedActionResult<T> { ActionResult = action() };
            }

            var duration = executionTimer.Time(() => { result = action(); });

            return new TimedActionResult<T>
            {
                ActionResult = result,
                TimerResult = duration
            };
        }

        public TimerResult PublishTimedAction(Action action, TimelineCategoryItem category, string eventName, string eventSubText = null)
        {
            var timedResult = Time(action);
            PublishMessage(new TimelineMessage { EventName = eventName, EventCategory = category, EventSubText = eventSubText }.AsTimedMessage(timedResult));

            return timedResult;
        }

        public TimerResult PublishTimedAction<TMessage>(Action action, Func<TMessage> messageFactory, TimelineCategoryItem category, string eventName, string eventSubText = null)
        {
            var timedResult = PublishTimedAction(action, category, eventName, eventSubText);
            PublishMessage(messageFactory());

            return timedResult;
        }

        public TimerResult PublishTimedAction<TMessage>(Action action, Func<TimerResult, TMessage> messageFactory, TimelineCategoryItem category, string eventName, string eventSubText = null)
        {
            var timedResult = PublishTimedAction(action, category, eventName, eventSubText);
            PublishMessage(messageFactory(timedResult));

            return timedResult;
        }

        public TimedActionResult<T> PublishTimedAction<T>(Func<T> action, TimelineCategoryItem category, string eventName, string eventSubText = null)
        {
            var timedResult = Time(action);
            PublishMessage(new TimelineMessage { EventName = eventName, EventCategory = category, EventSubText = eventSubText }.AsTimedMessage(timedResult.TimerResult));

            return timedResult;
        }

        public TimedActionResult<T> PublishTimedAction<T>(Func<T> action, TimelineCategoryItem category, Func<T, string> eventNameFactory, Func<T, string> eventSubTextFactory = null)
        {
            var timedResult = Time(action);

            string eventSubText = null;
            if (eventSubTextFactory != null)
            {
                eventSubText = eventSubTextFactory(timedResult.ActionResult);
            }

            PublishMessage(new TimelineMessage { EventName = eventNameFactory(timedResult.ActionResult), EventCategory = category, EventSubText = eventSubText }.AsTimedMessage(timedResult.TimerResult));

            return timedResult;
        }

        public TimedActionResult<T> PublishTimedAction<T, TMessage>(Func<T> action, Func<T, TimerResult, TMessage> messageFactory, TimelineCategoryItem category, string eventName, string eventSubText = null)
        {
            var actionResult = PublishTimedAction(action, category, eventName, eventSubText);
            PublishMessage(messageFactory(actionResult.ActionResult, actionResult.TimerResult));

            return actionResult;
        }

        public TimedActionResult<T> PublishTimedAction<T, TMessage>(Func<T> action, Func<T, TimerResult, TMessage> messageFactory, TimelineCategoryItem category, Func<T, string> eventNameFactory, Func<T, string> eventSubTextFactory = null)
        {
            var actionResult = PublishTimedAction(action, category, eventNameFactory, eventSubTextFactory);
            PublishMessage(messageFactory(actionResult.ActionResult, actionResult.TimerResult));

            return actionResult;
        }

        public void PublishMessage<T>(T message) {
            GetMessageBroker().Publish(message);
        }

        private IExecutionTimer GetTimer() { //todo: make this more robust and  make public
            var context = HttpContext.Current;
            if (context == null)
            {
                return null;
            }

            return ((GlimpseRuntime)context.Application.Get("__GlimpseRuntime")).Configuration.TimerStrategy.Invoke();
        }

        private IMessageBroker GetMessageBroker() //todo: make this more robust and  make public
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                return null;
            }

            return ((GlimpseRuntime)context.Application.Get("__GlimpseRuntime")).Configuration.MessageBroker;
        }
    }
}