﻿using System;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;
using Orchard.Glimpse.Models;

namespace Orchard.Glimpse.Services {
    public interface IGlimpseService : IDependency {
        /// <summary>
        /// Executes an action, and times how long the execution takes
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <returns>An instance of TimerResult which contains the measured time taken to execute the action</returns>
        TimerResult Time(Action action);

        /// <summary>
        /// Executes an action, times how long the exectution takes, and returns the result of the timer and the result of the action
        /// </summary>
        /// <typeparam name="T">The return type of the action to execute</typeparam>
        /// <param name="action">The action to execute</param>
        /// <returns>A TimedActionResult containing an instance of TimerResult depicting the measured time taken to execute the action, and the result of the action that was executed</returns>
        TimedActionResult<T> Time<T>(Func<T> action);

        TimerResult PublishTimedAction(Action action, TimelineCategoryItem category, string eventName, string eventSubText = null);
        TimerResult PublishTimedAction<TMessage>(Action action, Func<TMessage> messageFactory, TimelineCategoryItem category, string eventName, string eventSubText = null);
        TimerResult PublishTimedAction<TMessage>(Action action, Func<TimerResult, TMessage> messageFactory, TimelineCategoryItem category, string eventName, string eventSubText = null);
        TimedActionResult<T> PublishTimedAction<T>(Func<T> action, TimelineCategoryItem category, string eventName, string eventSubText = null);
        TimedActionResult<T> PublishTimedAction<T>(Func<T> action, TimelineCategoryItem category, Func<T, string> eventNameFactory, Func<T, string> eventSubTextFactory = null);
        TimedActionResult<T> PublishTimedAction<T, TMessage>(Func<T> action, Func<T, TimerResult, TMessage> messageFactory, TimelineCategoryItem category, string eventName, string eventSubText = null);
        TimedActionResult<T> PublishTimedAction<T, TMessage>(Func<T> action, Func<T, TimerResult, TMessage> messageFactory, TimelineCategoryItem category, Func<T, string> eventNameFactory, Func<T, string> eventSubTextFactory = null);

        /// <summary>
        /// Places a message into the Glimpse Broker
        /// </summary>
        /// <typeparam name="T">The type of the message to be published</typeparam>
        /// <param name="message">The message to be published</param>
        void PublishMessage<T>(T message);
    }
}