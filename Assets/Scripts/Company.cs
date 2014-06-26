using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Company {
    public string name;
    public float cash = 1000;
    public int sizeLimit = 10;

    public List<Character> founders = new List<Character>();
    public List<Product> products = new List<Products>();
    private List<GameObject> _workers = new List<GameObject>();
    public ReadOnlyCollection<GameObject> workers {
        get { return _workers.AsReadOnly(); }
    }

    public List<Item> _items = new List<Item>();
    public ReadOnlyCollection<Item> items {
        get { return _items.AsReadOnly(); }
    }

    public Company(string name_) {
        name = name_;
    }

    public bool HireWorker(GameObject worker) {
        if (_workers.Count < sizeLimit) {
            _workers.Add(worker);
            return true;
        }
        return false;
    }
    public void FireWorker(GameObject worker) {
        _workers.Remove(worker);
    }

    public void StartNewProduct() {
        ProductType pt = new ProductType("example_ProductType");
        Industry i = new Industry("example_Industry");
        Market m = new Market("example_Market");
        Product product = new Product(pt, i, m);
        products.Add(product);

        // apply item buffs
    }

    public void DevelopProduct(IProduct product) {
        float charisma = 0;
        float creativity = 0;
        float cleverness = 0;
        float progress = 0;

        foreach (GameObject worker in workers) {
            Worker w = worker.GetComponent<Worker>();
            charisma += w.charisma.value;
            creativity += w.creativity.value;
            cleverness += w.cleverness.value;
            progress += w.productivity.value;
        }

        product.Develop(progress, charisma, creativity, cleverness);
    }

    public void Pay() {
        foreach (GameObject worker in workers) {
            cash -= worker.GetComponent<Worker>().salary;
        }
    }

    public bool BuyItem(Item item) {
        if (cash - item.cost >= 0) {
            cash -= item.cost;
            _items.Add(item);

            List<Product> matchingProducts = products.FindAll(p => 
                item.industries.Contains(p.industry) 
                || item.productTypes.Contains(p.productType)
                || item.markets.Contains(p.market)
            );

            foreach (Product product in matchingProducts) {
                product.ApplyItem(item);
            }

            return true;
        }
        return false;
    }

    public bool RemoveItem(Item item) {
        _items.Remove(item);
        
        List<Product> matchingProducts = products.FindAll(p => 
            item.industries.Contains(p.industry) 
            || item.productTypes.Contains(p.productType)
            || item.markets.Contains(p.market)
        );
        
        foreach (Product product in matchingProducts) {
            product.RemoveItem(item);
        }
    }
}


