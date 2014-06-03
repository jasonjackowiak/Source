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
    public class RuleMatchTest : Test
    {
        private string calledRule;
        RuleDefinition rule = new RuleDefinition();

        private bool addNotApplicable;
        BuildRelationships i = new BuildRelationships();
        List<Link> _links = new List<Link>();

        [SetUp]
        //Setup global data here
        public override void SetUp()
        {
            addNotApplicable = true;
            calledRule = "calledrule";
            rule.Body = "calledrule";
            rule.Name = "callingrule";
        }

        [TearDown]
        //Clear global data here
        public override void TearDown()
        {
        _links.Clear();
        }

        [Test]
        public void TestRulePattern1()
        {
            SetUp();
            rule.Body = "calledrule;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern2()
        {
            SetUp();
            rule.Body = "calledrule(";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern3()
        {
            SetUp();
            rule.Body = "(calledrule;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern4()
        {
            SetUp();
            rule.Body = "(calledrule(";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern5()
        {
            SetUp();
            rule.Body = "(calledrule);";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern6()
        {
            SetUp();
            rule.Body = "calledrule =";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern7()
        {
            SetUp();
            rule.Body = "calledrule ^=";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern8()
        {
            SetUp();
            rule.Body = "= calledrule;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern9()
        {
            SetUp();
            rule.Body = "^= calledrule;";
            TestRuleMatch();
            TearDown();
        }

               [Test]
        public void TestRulePattern25()
        {
            SetUp();
            rule.Body = "^= calledrule(";
            TestRuleMatch();
            TearDown();
        }

        
        [Test]
        public void TestRulePattern10()
        {
            SetUp();
            rule.Body = "^calledrule(";
            TestRuleMatch();
            TearDown();
        }

               [Test]
        public void TestRulePattern26()
        {
            SetUp();
            rule.Body = "^calledrule;";
            TestRuleMatch();
            TearDown();
        }
        
        [Test]
        public void TestRulePattern11()
        {
            SetUp();
            rule.Body = "call calledrule('TA_FILE');";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern12()
        {
            SetUp();
            rule.Body = "calledrule >";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern13()
        {
            SetUp();
            rule.Body = "calledrule >=";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern14()
        {
            SetUp();
            rule.Body = "> calledrule(";
            TestRuleMatch();
            TearDown();
        }

                [Test]
        public void TestRulePattern27()
        {
            SetUp();
            rule.Body = "> calledrule;";
            TestRuleMatch();
            TearDown();
        }
        
        [Test]
        public void TestRulePattern15()
        {
            SetUp();
            rule.Body = ">= calledrule(";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern16()
        {
            SetUp();
            rule.Body = "calledrule <";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern17()
        {
            SetUp();
            rule.Body = "calledrule <=";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern18()
        {
            SetUp();
            rule.Body = "< calledrule(";
            TestRuleMatch();
            TearDown();
        }

                [Test]
        public void TestRulePattern29()
        {
            SetUp();
            rule.Body = "< calledrule;";
            TestRuleMatch();
            TearDown();
        }
        
        [Test]
        public void TestRulePattern19()
        {
            SetUp();
            rule.Body = "<= calledrule(";
            TestRuleMatch();
            TearDown();
        }

                [Test]
        public void TestRulePattern30()
        {
            SetUp();
            rule.Body = "<= calledrule;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern20()
        {
            SetUp();
            rule.Body = "arule(calledrule, param);";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern21()
        {
            SetUp();
            rule.Body = "arule(param, calledrule);";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern22()
        {
            SetUp();
rule.Body = "arule(param, calledrule, param);";
            TestRuleMatch();
            TearDown();
        }

                [Test]
        public void TestRulePattern23()
        {
            SetUp();
            rule.Body = "= calledrule(";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestRulePattern24()
        {
            SetUp();
            rule.Body = ">= calledrule(";
            TestRuleMatch();
            TearDown();
        }
        
        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail1()
        {
            SetUp();
            rule.Body = "local var, calledrulet;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail2()
        {
            SetUp();
            rule.Body = "failtext";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail3()
        {
            SetUp();
            rule.Body = "blacalledrule;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail4()
        {
            SetUp();
            rule.Body = "blacalledrule(";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail5()
        {
            SetUp();
            rule.Body = "blacalledrulebla(";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail6()
        {
            SetUp();
            rule.Body = " calledrulebla(";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail7()
        {
            SetUp();
            rule.Body = ", calledrulebla";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail8()
        {
            SetUp();
            rule.Body = "calledrulebla =";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail9()
        {
            SetUp();
            rule.Body = "^calledrule";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail10()
        {
            SetUp();
            rule.Body = "= calledrulebla";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail11()
        {
            SetUp();
            rule.Body = "blacalledrule >=";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail12()
        {
            SetUp();
            rule.Body = "blacalledrule >";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail13()
        {
            SetUp();
            rule.Body = "blacalledrule =<";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail14()
        {
            SetUp();
            rule.Body = "blacalledrule <";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail15()
        {
            SetUp();
            rule.Body = "call blacalledrule(bla, bla";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail16()
        {
            SetUp();
            rule.Body = "rule(calledrulewrong(";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail17()
        {
            SetUp();
            rule.Body = "calledrulefn(param)";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail18()
        {
            SetUp();
            rule.Body = "= calledrulefn(param)";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail19()
        {
            SetUp();
            rule.Body = "= calledrulefn;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail20()
        {
            SetUp();
            rule.Body = "> calledrulefn(param)";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail21()
        {
            SetUp();
            rule.Body = "> calledrulefn;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail22()
        {
            SetUp();
            rule.Body = "< calledrulefn(param)";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail23()
        {
            SetUp();
            rule.Body = "< calledrulefn;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail24()
        {
            SetUp();
            rule.Body = "=> calledrulefn(param)";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail25()
        {
            SetUp();
            rule.Body = "=> calledrulefn;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail26()
        {
            SetUp();
            rule.Body = "<= calledrulefn(param)";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail27()
        {
            SetUp();
            rule.Body = "<= calledrulefn;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestRulePatternFail28()
        {
            SetUp();
            rule.Body = "rulecall ^calledrulefn;";
            TestRuleMatch();
            TearDown();
        }

        [Test]
        public void TestSignalPattern1()
        {
            SetUp();
            rule.Body = "SIGNAL calledrule;";
            TestSignalRuleMatchExclusion();
            TearDown();
        }

        [Test]
        public void TestSignalPattern2()
        {
            SetUp();
            rule.Body = "ON calledrule;";
            TestSignalRuleMatchExclusion();
            TearDown();
        }

        [Test]
        public void TestSignalPattern3()
        {
            SetUp();
            rule.Body = "UNTIL calledrule;";
            TestSignalRuleMatchExclusion();
            TearDown();
        }

        [Test]
        public void TestInvalidData()
        {
            SetUp();
            rule.Body = "randomtext";
            TestSignalRuleMatchExclusion();
            TearDown();
        }




 
   

 
        //Test the patterns from within a rule are recognised
        public void TestRuleMatch()
        {
            //call the method with the rule pattern matching for each pattern
            addNotApplicable = i.RuleInRuleMatch(calledRule, rule, _links, addNotApplicable);
                Assert.AreEqual(calledRule, _links[0].CalledEnt.ToString());
                Assert.AreEqual(rule.Name, _links[0].CallingEnt.ToString());
                Assert.AreEqual(false, addNotApplicable);
        }

        //Test the signal patterns from within a rule are recognised and NOT treated as rules
        public void TestSignalRuleMatchExclusion()
        {
            //call the method with the signal pattern matching for each exclusion pattern
            addNotApplicable = i.RuleInRuleMatch(calledRule, rule, _links, addNotApplicable);

                Assert.IsEmpty(_links);
                Assert.AreEqual(true, addNotApplicable);
        }
    }
}
    

