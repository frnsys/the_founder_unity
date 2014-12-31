using UnityEngine;
using UnityEditor;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class MarketTests
	{
		[Test]
		public void MarketShareCalculation() {
            Company c  = ScriptableObject.CreateInstance<Company>();
            Company c_ = ScriptableObject.CreateInstance<Company>();

            c.publicity.baseValue  = 10;
            c_.publicity.baseValue = 20;
            c.opinion.baseValue  = 0;
            c_.opinion.baseValue = 0;

            // Create two identical products.
            ProductType pt = (ProductType)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestProductType.asset", typeof(ProductType)));
            List<ProductType> pts = new List<ProductType>() { pt };

            Product p  = ScriptableObject.CreateInstance<Product>();
            Product p_ = ScriptableObject.CreateInstance<Product>();

            p.Init(pts, 10, 10, 10, c);
            p_.Init(pts, 10, 10, 10, c_);

            p.Launch();
            p_.Launch();

            c.products.Add(p);
            c_.products.Add(p_);

            MarketManager.Market m = MarketManager.Market.NorthAmerica;
            float marketSize = MarketManager.SizeForMarket(m);
            c.markets.Add(m);
            c_.markets.Add(m);

            List<Company> companies = new List<Company>() { c, c_ };

            Assert.AreEqual(p.marketShare, 0);
            Assert.AreEqual(p_.marketShare, 0);

            MarketManager.CalculateMarketShares(companies);

            Assert.AreEqual(p.marketShare + p_.marketShare, marketSize);
            Assert.IsTrue(p_.marketShare > p.marketShare);
		}
    }
}
