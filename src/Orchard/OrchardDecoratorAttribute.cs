using System;

namespace Orchard {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class OrchardDecoratorAttribute : Attribute {

        /// <summary>
        /// Indicates that this class is a decorator
        /// </summary>
        public OrchardDecoratorAttribute() {
            Priority = "10.0.0";
        }

        /// <summary>
        /// Indicates that this class is a decorator
        /// </summary>
        /// <param name="priority">The priority of this Decorator. The lower priority, the closer this decorator will be registered to the original implementation. Defaults to 10.0.0 if not supplied.</param>
        public OrchardDecoratorAttribute(string priority) {
            Priority = priority;
        }

        public string Priority { get; set; }
    }
}