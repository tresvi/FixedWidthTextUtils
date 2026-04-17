using FixedWidthTextUtils.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FixedWidthTextUtils_NUnit_Test
{
    /// <summary>
    /// Contrato y cobertura de los constructores públicos estándar de las excepciones de FixedWidthTextUtils.
    /// </summary>
    [TestFixture]
    internal class FixedWidthTextExceptions_ConstructorTests
    {
        private static IEnumerable<Type> ConcreteFixedWidthTextExceptionTypes()
        {
            return typeof(FixedWidthTextException).Assembly
                .GetTypes()
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && typeof(FixedWidthTextException).IsAssignableFrom(t));
        }

        [Test]
        [TestCaseSource(nameof(ConcreteFixedWidthTextExceptionTypes))]
        public void Parameterless_ctor_creates_instance_assignable_to_base(Type exceptionType)
        {
            ConstructorInfo? ctor = exceptionType.GetConstructor(Type.EmptyTypes);
            Assert.That(ctor, Is.Not.Null, $"Se esperaba .ctor() público en {exceptionType.Name}");

            var ex = (Exception)ctor!.Invoke(null);

            Assert.That(ex, Is.AssignableTo<FixedWidthTextException>());
        }

        [Test]
        [TestCaseSource(nameof(ConcreteFixedWidthTextExceptionTypes))]
        public void String_ctor_sets_message(Type exceptionType)
        {
            const string message = "test-message-ctor";
            ConstructorInfo? ctor = exceptionType.GetConstructor(new[] { typeof(string) });
            Assert.That(ctor, Is.Not.Null, $"Se esperaba .ctor(string) público en {exceptionType.Name}");

            var ex = (Exception)ctor!.Invoke(new object[] { message });

            Assert.That(ex, Is.AssignableTo<FixedWidthTextException>());
            Assert.That(ex.Message, Is.EqualTo(message));
        }

        [Test]
        [TestCaseSource(nameof(ConcreteFixedWidthTextExceptionTypes))]
        public void String_and_inner_exception_ctor_sets_message_and_inner(Type exceptionType)
        {
            const string message = "outer-message-ctor";
            var inner = new InvalidOperationException("inner-cause");
            ConstructorInfo? ctor = exceptionType.GetConstructor(new[] { typeof(string), typeof(Exception) });
            Assert.That(ctor, Is.Not.Null, $"Se esperaba .ctor(string, Exception) público en {exceptionType.Name}");

            var ex = (Exception)ctor!.Invoke(new object[] { message, inner });

            Assert.That(ex, Is.AssignableTo<FixedWidthTextException>());
            Assert.That(ex.Message, Is.EqualTo(message));
            Assert.That(ex.InnerException, Is.SameAs(inner));
        }
    }
}
