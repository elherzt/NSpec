﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSpec;
using NSpec.Domain;
using NSpec.Domain.Extensions;

namespace NSpecNUnit
{
    [TestFixture]
    public class when_after_contains_exception
    {
        private class SpecClass : nspec
        {
            public void method_level_context()
            {
                after = () => { throw new InvalidOperationException(); };

                it["should fail this example because of after"] = () => "1".should_be("1");

                it["should also fail this example because of after"] = () => "1".should_be("1");
            }
        }

        private Context classContext;
        private Context methodContext;

        [SetUp]
        public void setup()
        {
            classContext = new Context(typeof(SpecClass));

            var method = typeof(SpecClass).Methods().Single(m => m.Name == "method_level_context");

            methodContext = new Context(method);

            classContext.AddContext(methodContext);

            classContext.Run();
        }

        [Test]
        public void it_should_fail_all_examples_in_after()
        {
            TheExample("should fail this example because of after").Exception.GetType().should_be(typeof(InvalidOperationException));
            TheExample("should also fail this example because of after").Exception.GetType().should_be(typeof(InvalidOperationException));
        }

        private Example TheExample(string name)
        {
            return classContext.Contexts.First().AllExamples().Single(s => s.Spec == name);
        }
    }
}
