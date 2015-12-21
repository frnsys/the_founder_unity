using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using NUnit.Framework;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class SpecialProjectTests
	{
        // TODO fix all this
        //private GameObject gameObj;
        //private GameData gd;
        //private GameManager gm;

        //private Product p = null;
        //private ProductRecipe pr = null;
        //private ProductType pt;
        //private SpecialProject sp;
        //private List<ProductType> pts;
        //private Company c;

        //[SetUp]
        //public void SetUp() {
            //pts = new List<ProductType>() {
                //ScriptableObject.Instantiate(ProductType.Load("Social Network")) as ProductType,
                //ScriptableObject.Instantiate(ProductType.Load("Virtual Reality")) as ProductType
            //};

            //gameObj = new GameObject("Game Manager");
            //gm = gameObj.AddComponent<GameManager>();
            //gd = GameData.New("DEFAULTCORP");
            //gm.Load(gd);

            //c = gd.company;

            //p = ScriptableObject.CreateInstance<Product>();
            //p.Init(pts, 0, 0, 0, c);
            //pr = ProductRecipe.LoadFromTypes(pts);

            //sp = ScriptableObject.CreateInstance<SpecialProject>();
            //sp.requiredProducts = new List<ProductRecipe> { pr };
        //}

        //[TearDown]
        //public void TearDown() {
            //UnityEngine.Object.DestroyImmediate(gameObj);
            //gm = null;
            //p = null;
            //pr = null;
            //pts = null;
            //sp = null;
        //}

        // TODO update
		//[Test]
		//public void IsAvailable() {
            //sp.cost = 0;

            //// Shouldn't be available because the company is missing a prerequisite product.
            //Assert.IsFalse(sp.isAvailable(c));
            //c.products.Add(p);
            //Assert.IsTrue(sp.isAvailable(c));
		//}

    }
}
