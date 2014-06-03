using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Build;
using Import;

namespace Test
{
    [TestFixture]
    public class TriggerMatchTest : Test
    {
        
        RuleDefinition rule = new RuleDefinition();
        private string triggerName;
        TriggerDefinition trigger = new TriggerDefinition();
        BuildRelationships i = new BuildRelationships();
        List<Link> _links = new List<Link>();

        [SetUp]
        //Setup global data here
        public override void SetUp()
        {
            rule.Name = "callingrule";
            rule.Body = "blabla";
            trigger.TableName = "table";
            trigger.RuleName = "calledrule";
            trigger.Access = "I";
            triggerName = trigger.RuleName + trigger.TableName + trigger.Access;
        }

        [TearDown]
        //Clear global data here
        public override void TearDown()
        {
            _links.Clear();
        }

        [Test]
        public void TestPattern1()
        {
            SetUp();
            rule.Body = "INSERT " + trigger.TableName + ";";
            TestTriggerMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestPatternFail1()
        {
            SetUp();
            rule.Body = "failtext";
            TestTriggerMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestPatternFail2()
        {
            SetUp();
            rule.Body = " SCREEN_SN ";
            TestTriggerMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestPatternFail3()
        {
            SetUp();
            rule.Body = "(SCREEN_SN)";
            TestTriggerMatch();
            TearDown();
        }

        private void TestTriggerMatch()
        {
            //call the method with the rule pattern matching for each pattern
            i.TriggerInRuleMatch(trigger, rule, _links);
            Assert.AreEqual(triggerName, _links[0].CalledEnt.ToString());
            Assert.AreEqual(rule.Name, _links[0].CallingEnt.ToString());
            //Assert.AreEqual(true, added);
        }
    }
}
