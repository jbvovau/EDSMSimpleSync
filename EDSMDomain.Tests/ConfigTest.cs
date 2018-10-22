using EDLogWatcher.Filter;
using EDSMSimpleSync.Utils;
using EDSync.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Tests
{
    [TestFixture]
    class ConfigTest
    {
        [Test]
        public void TestMemoryStorage()
        {
            var mem = new MemoryStorage();

            var key = "Key1";
            var obj = "Object1";

            var k2 = "Key2";
            var obj2 = new List<string>();
            obj2.Add("Toto");
            obj2.Add("Titi");

            mem.Set(key, obj);

            var t1 = mem.Get(key);
            Assert.AreEqual(obj, t1);
            var t2 = mem.Get(k2);
            Assert.IsNull(t2);

            mem.Set(k2, obj2);

            t2 = mem.Get(k2);
            Assert.IsNotNull(t2);

            Assert.AreSame(t2, obj2);
        }

        [Test]
        public void TestConfigMemoryStorage()
        {
            EDConfig.Instance.Storage = new MemoryStorage();

            Assert.IsNull(EDConfig.Instance.Get("test"));

            EDConfig.Instance.Set("test", "test");
            Assert.IsNotNull(EDConfig.Instance.Get("test"));

            Assert.AreEqual("test", EDConfig.Instance.Get("test"));
        }


        [Test]
        public void TestDateFilter()
        {
            var mem = new MemoryStorage();

            var filter = new DateEntryFilter(mem, "test");
            var data = "{\"test\":\"test\"}";

            Assert.IsTrue(filter.Accepted(data));

            filter.Discard(data);

            Assert.IsFalse(filter.Accepted(data));

            var dataWTime1 = "{\"timestamp\":\"2018-10-21T18:36:26Z\",\"test\":\"test\"}";
            var dataWTime2 = "{\"timestamp\":\"2018-10-21T18:36:26Z\",\"test\":\"other\"}"; // same time
            var dataWTime3 = "{\"timestamp\":\"2018-10-21T18:36:39Z\",\"test\":\"blob\"}";

            Assert.IsTrue(filter.Accepted(dataWTime1));
            Assert.IsTrue(filter.Accepted(dataWTime2));
            Assert.IsTrue(filter.Accepted(dataWTime3));

            // add first event
            filter.Discard(dataWTime1);

            // only first event is discarded
            Assert.IsFalse(filter.Accepted(dataWTime1));
            Assert.IsTrue(filter.Accepted(dataWTime2));
            Assert.IsTrue(filter.Accepted(dataWTime3));

            // second event
            filter.Discard(dataWTime2);

            Assert.IsFalse(filter.Accepted(dataWTime1));
            Assert.IsFalse(filter.Accepted(dataWTime2));
            Assert.IsTrue(filter.Accepted(dataWTime3));

            // third event
            filter.Discard(dataWTime3);

            Assert.IsFalse(filter.Accepted(dataWTime1));
            Assert.IsFalse(filter.Accepted(dataWTime2));
            Assert.IsFalse(filter.Accepted(dataWTime3));

            // anterior event
            var dataWTime4 = "{\"timestamp\":\"2018-10-21T18:36:12Z\",\"test\":\"test blob arg\"}";
            Assert.IsFalse(filter.Accepted(dataWTime4));

            // should confirm to store
            filter.Confirm();

            // try with a new filter but same storage
            var filter2 = new DateEntryFilter(mem, "test");

            Assert.IsFalse(filter2.Accepted(dataWTime1));
            Assert.IsFalse(filter2.Accepted(dataWTime2));
            Assert.IsTrue(filter2.Accepted(dataWTime3)); // warning : same time = ok
            Assert.IsFalse(filter2.Accepted(dataWTime4));


        }
    }
}
