﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Build;
using Import;

namespace Test
{
    [TestFixture]
    public class ReportMatchTest : Test
    {
        
        RuleDefinition rule = new RuleDefinition();
        private string report;

        private bool added;
        BuildSQLRelationships i = new BuildSQLRelationships();
        List<Link> _links = new List<Link>();

        [SetUp]
        //Setup global data here
        public override void SetUp()
        {
            added = true;
            rule.Name = "callingrule";
            rule.Body = "blabla";
            report = "REPORT_RP";
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
            rule.Body = "'REPORT_RP'";
            TestReportMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestPatternFail1()
        {
            SetUp();
            rule.Body = "failtext";
            TestReportMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestPatternFail2()
        {
            SetUp();
            rule.Body = " REPORT_RP ";
            TestReportMatch();
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestPatternFail3()
        {
            SetUp();
            rule.Body = "(REPORT_RP)";
            TestReportMatch();
            TearDown();
        }

        private void TestReportMatch()
        {
            //call the method with the rule pattern matching for each pattern
            added = i.ReportInRuleMatch(report, rule, _links);
            Assert.AreEqual(report, _links[0].CalledEnt.ToString());
            Assert.AreEqual(rule.Name, _links[0].CallingEnt.ToString());
            Assert.AreEqual(true, added);
        }
    }
}
