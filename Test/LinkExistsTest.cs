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
    public class LinkExists : Test
    {

        private bool added;
        BuildSQLRelationships i = new BuildSQLRelationships();
        List<Link> _links = new List<Link>();

        [SetUp]
        //Setup global data here
        public override void SetUp()
        {
            added = true;
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
            Link link = new Link("callingrule", "calledrule");
            Link linkdiff = new Link("callingrule", "othercalledrule");
            _links.Add(link);
            _links.Add(linkdiff);
            LinkExistsTest(link);
            TearDown();
        }

        [Test]
        [ExpectedExceptionAttribute]
        public void TestPatternFail1()
        {
            SetUp();
            Link link = new Link("callingrule", "calledrule");
            Link linkdiff = new Link("callingrule", "othercalledrule");
            _links.Add(link);
            LinkExistsTest(linkdiff);
            TearDown();
        }



        private void LinkExistsTest(Link link)
        {
            //call the method with the rule pattern matching for each pattern
            added = i.CheckLinkExists(_links, link);
            Assert.AreEqual(true, added);
        }
    }
}
