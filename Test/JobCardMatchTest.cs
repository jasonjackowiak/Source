using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Build;
using Import;

namespace Test
{

    //[TestFixture]
    //public class JobCardInRuleMatchTest : Test
    //{
    //    RuleDefinition rule = new RuleDefinition();
    //    private string jobCard;

    //    private bool added;
    //    BuildRelationships i = new BuildRelationships();
    //    List<Link> _links = new List<Link>();

    //    [SetUp]
    //    //Setup global data here
    //    public override void SetUp()
    //    {
    //        added = true;
    //        rule.Name = "callingrule";
    //        rule.Body = "blabla";
    //        jobCard = "JOBCARD";
    //    }

    //    [TearDown]
    //    //Clear global data here
    //    public override void TearDown()
    //    {
    //        _links.Clear();
    //    }

    //    [Test]
    //    public void TestPattern1()
    //    {
    //        SetUp();
    //        rule.Body = "'JOBCARD'";
    //        TestJobCardMatch();
    //        TearDown();
    //    }

    //    [Test]
    //    [ExpectedExceptionAttribute]
    //    public void TestPatternFail1()
    //    {
    //        SetUp();
    //        rule.Body = "failtext";
    //        TestJobCardMatch();
    //        TearDown();
    //    }

    //    [Test]
    //    [ExpectedExceptionAttribute]
    //    public void TestPatternFail2()
    //    {
    //        SetUp();
    //        rule.Body = " JOBCARD ";
    //        TestJobCardMatch();
    //        TearDown();
    //    }

    //    [Test]
    //    [ExpectedExceptionAttribute]
    //    public void TestPatternFail3()
    //    {
    //        SetUp();
    //        rule.Body = "(JOBCARD)";
    //        TestJobCardMatch();
    //        TearDown();
    //    }

    //    private void TestJobCardMatch()
    //    {
    //        //call the method with the rule pattern matching for each pattern
    //        added = i.JobCardInRuleMatch(jobCard, rule, _links);
    //        Assert.AreEqual(jobCard, _links[0].CalledEnt.ToString());
    //        Assert.AreEqual(rule.Name, _links[0].CallingEnt.ToString());
    //        Assert.AreEqual(true, added);
    //    }

    //}
}
    

