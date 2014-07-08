using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Company : HasStats {
    public string name;
    public int sizeLimit = 10;
    public Stat cash = new Stat("Cash", 100000);

    public List<Character> founders = new List<Character>();
    public List<Product> products = new List<Product>();
    private List<Worker> _workers = new List<Worker>();
    public ReadOnlyCollection<Worker> workers {
        get { return _workers.AsReadOnly(); }
    }

    public List<Product> developingProducts {
        get {
            return products.FindAll(p => p.state == Product.State.DEVELOPMENT);
        }
    }

    public List<Item> _items = new List<Item>();
    public ReadOnlyCollection<Item> items {
        get { return _items.AsReadOnly(); }
    }

    public Company(string name_) {
        name = name_;
    }

    public bool HireWorker(Worker worker) {
        if (_workers.Count < sizeLimit) {
            foreach (Item item in _items) {
                worker.ApplyItem(item);
            }

            Debug.Log("Hired " + worker);
            _workers.Add(worker);
            return true;
        }
        return false;
    }
    public void FireWorker(Worker worker) {
        foreach (Item item in _items) {
            worker.RemoveItem(item);
        }

        _workers.Remove(worker);
    }

    public void StartNewProduct(ProductType pt, Industry i, Market m) {
        Product product = new Product(pt, i, m);

        foreach (Item item in _items) {
            product.ApplyItem(item);
        }

        products.Add(product);
    }

    public void DevelopProducts() {
        List<Product> inDevelopment = products.FindAll(p => p.state == Product.State.DEVELOPMENT);
        foreach (Product product in inDevelopment) {
            DevelopProduct(product);
        }
    }

    public void HarvestProducts(float elapsedTime) {
        List<Product> launched = products.FindAll(p => p.state == Product.State.LAUNCHED);

        foreach (Product product in launched) {
            cash.baseValue += product.Revenue(elapsedTime);
        }
    }

    public void DevelopProduct(IProduct product) {
        float charisma = 0;
        float creativity = 0;
        float cleverness = 0;
        float progress = 0;

        foreach (Worker worker in workers) {
            // A bit of randomness to make things more interesting.
            charisma += (worker.charisma.value/2) * Random.Range(0.90f, 1.05f);
            creativity += (worker.creativity.value/2) * Random.Range(0.90f, 1.05f);
            cleverness += (worker.cleverness.value/2) * Random.Range(0.90f, 1.05f);
            progress += (worker.productivity.value/2) * Random.Range(0.90f, 1.05f);
        }

        product.Develop(progress, charisma, creativity, cleverness);
    }

    public void RemoveProduct(Product product) {
        foreach (Item item in _items) {
            product.RemoveItem(item);
        }
        product.Shutdown();
    }

    public void Pay() {
        foreach (Worker worker in workers) {
            cash.baseValue -= worker.salary;
        }
    }

    public bool BuyItem(Item item) {
        if (cash.baseValue - item.cost >= 0) {
            cash.baseValue -= item.cost;
            _items.Add(item);

            List<Product> matchingProducts = FindMatchingProducts(item.productTypes, item.industries, item.markets);

            foreach (Product product in matchingProducts) {
                product.ApplyItem(item);
            }

            foreach (Worker worker in _workers) {
                worker.ApplyItem(item);
            }

            return true;
        }
        return false;
    }

    public void RemoveItem(Item item) {
        _items.Remove(item);

        List<Product> matchingProducts = FindMatchingProducts(item.productTypes, item.industries, item.markets);

        foreach (Product product in matchingProducts) {
            product.RemoveItem(item);
        }

        foreach (Worker worker in _workers) {
            worker.RemoveItem(item);
        }
    }

    public void ApplyEffect(GameEffect effect) {
        foreach (Worker worker in workers) {
            worker.ApplyBuffs(effect.workerBuffs);
        }

        ApplyBuffs(effect.companyBuffs);

        List<Product> matchingProducts = FindMatchingProducts(effect.productTypes, effect.industries, effect.markets);
        foreach (Product product in matchingProducts) {
            product.ApplyBuffs(effect.productBuffs);
        }
    }

    // Given an item, find the list of currently active products that 
    // match the item's industries, product types, or markets.
    private List<Product> FindMatchingProducts(List<ProductType> productTypes, List<Industry> industries, List<Market> markets) {
        // Items which have no product specifications apply to all products.
        if (industries.Count == 0 && productTypes.Count == 0 && markets.Count == 0) {
            return products;

        } else {
            return products.FindAll(p =>
                industries.Exists(i => i == p.industry)
                || productTypes.Exists(pType => pType == p.productType)
                || markets.Exists(m => m == p.market));
        }
    }

    public override Stat StatByName(string name) {
        switch (name) {
            case "Cash":
                return cash;
            default:
                return null;
        }
    }

}


