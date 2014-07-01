using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Company {
    public string name;
    public float cash = 1000;
    public int sizeLimit = 10;

    public List<Character> founders = new List<Character>();
    public List<Product> products = new List<Product>();
    private List<Worker> _workers = new List<Worker>();
    public ReadOnlyCollection<Worker> workers {
        get { return _workers.AsReadOnly(); }
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

    public void StartNewProduct() {
        ProductType pt = ProductType.Social_Network;
        Industry i = Industry.Space;
        Market m = Market.Millenials;
        Product product = new Product(pt, i, m);

        foreach (Item item in _items) {
            product.ApplyItem(item);
        }

        products.Add(product);
    }

    public void DevelopProduct(IProduct product) {
        float charisma = 0;
        float creativity = 0;
        float cleverness = 0;
        float progress = 0;

        foreach (Worker worker in workers) {
            charisma += worker.charisma.value;
            creativity += worker.creativity.value;
            cleverness += worker.cleverness.value;
            progress += worker.productivity.value;
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
            cash -= worker.salary;
        }
    }

    public bool BuyItem(Item item) {
        if (cash - item.cost >= 0) {
            cash -= item.cost;
            _items.Add(item);

            List<Product> matchingProducts;

            // Items which have no product specifications apply to all products.
            if (item.industries.Count == 0 && item.productTypes.Count == 0 && item.markets.Count == 0) {
                matchingProducts = products;
            } else {
                matchingProducts = products.FindAll(p =>
                    item.industries.Exists(i => i == p.industry)
                    || item.productTypes.Exists(pType => pType == p.productType)
                    || item.markets.Exists(m => m == p.market)
                );
            }

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

        List<Product> matchingProducts;
        if (item.industries.Count == 0 && item.productTypes.Count == 0 && item.markets.Count == 0) {
            matchingProducts = products;
        } else {
            matchingProducts = FindMatchingProducts(item);
        }

        foreach (Product product in matchingProducts) {
            product.RemoveItem(item);
        }

        foreach (Worker worker in _workers) {
            worker.RemoveItem(item);
        }
    }

    // Given an item, find the list of currently active products that 
    // match the item's industries, product types, or markets.
    private List<Product> FindMatchingProducts(Item item) {
        return products.FindAll(p =>
            item.industries.Exists(i => i == p.industry)
            || item.productTypes.Exists(pType => pType == p.productType)
            || item.markets.Exists(m => m == p.market)
        );
    }
}


