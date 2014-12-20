using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
    [TestFixture]
    internal class InfrastructureTests
    {
        Infrastructure id;
        Infrastructure id_;

        [SetUp]
        public void SetUp() {
            id  = new Infrastructure();
            id[Infrastructure.Type.Datacenter] = 5;
            id[Infrastructure.Type.Studio]     = 2;

            id_ = new Infrastructure();
            id_[Infrastructure.Type.Datacenter] = 6;
            id_[Infrastructure.Type.Factory]    = 1;
        }

        [TearDown]
        public void TearDown() {
            id  = null;
            id_ = null;
        }

        [Test]
        public void Equals() {
            Assert.IsFalse(id.Equals(id_));

            id_[Infrastructure.Type.Datacenter] = 5;
            id_[Infrastructure.Type.Factory]    = 0;
            id_[Infrastructure.Type.Studio]     = 2;

            Assert.IsTrue(id.Equals(id_));
        }

        [Test]
        public void GreaterThan() {
            Assert.IsFalse(id > id_);

            id[Infrastructure.Type.Factory]     = 2;
            id_[Infrastructure.Type.Datacenter] = 1;

            Assert.IsTrue(id > id_);
        }

        [Test]
        public void LessThan() {
            Assert.IsFalse(id < id_);

            id_[Infrastructure.Type.Datacenter] = 10;
            id_[Infrastructure.Type.Studio]     = 10;

            Assert.IsTrue(id < id_);
        }

        [Test]
        public void Addition() {
            Infrastructure result = new Infrastructure();
            result[Infrastructure.Type.Datacenter] = 11;
            result[Infrastructure.Type.Studio]     = 2;
            result[Infrastructure.Type.Factory]    = 1;

            Assert.IsTrue(result.Equals(id + id_));
        }

        [Test]
        public void Subtract() {
            Infrastructure result = new Infrastructure();
            result[Infrastructure.Type.Datacenter] = -1;
            result[Infrastructure.Type.Studio]     = 2;
            result[Infrastructure.Type.Factory]    = -1;

            Assert.IsTrue(result.Equals(id - id_));
        }
    }
}
