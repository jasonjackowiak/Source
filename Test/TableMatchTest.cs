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
    public class TableMatchTest : Test
    {
        
        RuleDefinition rule = new RuleDefinition();
        private string table;

        private bool tablebool;
        BuildRelationships i = new BuildRelationships();
        List<Link> _links = new List<Link>();

        [SetUp]
        //Setup global data here
        public override void SetUp()
        {
            tablebool = true;
            rule.Name = "callingrule";
            rule.Body = "blabla";
            table = "TA_FILE";
        }

        [TearDown]
        //Clear global data here
        public override void TearDown()
        {
            _links.Clear();
        }

        [Test]
        public void TestTablePattern1()
        {
            SetUp();
            rule.Body = " TA_FILE;";
            TestTableMatch();
            TearDown();
        }

        [Test]
        public void TestTablePattern2()
        {
            SetUp();
            rule.Body = " TA_FILE(";
            TestTableMatch();
            TearDown();
        }

        [Test]
        public void TestTablePattern3()
        {
            SetUp();
            rule.Body = " TA_FILE ";
            TestTableMatch();
            TearDown();
        }

        [Test]
        public void TestTablePattern4()
        {
            SetUp();
            rule.Body = "call TA_FILE.TA_FILE;";
            TestTableMatch();
            TearDown();
        }

                [Test]
        public void TestTablePattern5()
        {
            SetUp();
            rule.Body = "call calledrule('TA_FILE');";
            TestTableMatch();
            TearDown();
        }

        [Test]
        public void TestTablePattern6()
        {
            SetUp();
            rule.Body = "'TA_FILE'";
            TestTableMatch();
            TearDown();
        }

        [Test]
        public void TestTablePattern7()
        {
            SetUp();
            rule.Body = "('TA_FILE')";
            TestTableMatch();
            TearDown();
        }

        [Test]
        public void TestTablePatternGet()
        {
            SetUp();
            rule.Body = "GET TA_FILE(FN_ENV);";
            TestTableMatch();
            TearDown();
        }

        [Test]
        public void TestTablePatternUpdate()
        {
            SetUp();
            rule.Body = "UPDATE TA_FILE;";
            TestTableMatch();
            TearDown();
        }

        [Test]
        public void TestTablePatternForall()
        {
            SetUp();
            rule.Body = "FORALL TA_FILE :";
            TestTableMatch();
            TearDown();
        }

        [Test]
        public void TestTablePatternOn()
        {
            SetUp();
            rule.Body = "ON GETFAIL TA_FILE :";
            TestTableMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestTablePatternFail1()
        {
            SetUp();
            rule.Body = "failtext";
            TestTableMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestTablePatternFail2()
        {
            SetUp();
            rule.Body = ".TA_FILE;";
            TestTableMatch();
            TearDown();
        }

        private void TestTableMatch()
        {
            //call the method with the rule pattern matching for each pattern
            tablebool = i.TableInRuleMatch(table, rule, _links);
            Assert.AreEqual(table, _links[0].CalledEnt.ToString());
            Assert.AreEqual(rule.Name, _links[0].CallingEnt.ToString());
            Assert.AreEqual(true, tablebool);
        }
    }
}
