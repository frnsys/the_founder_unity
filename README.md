# The Founder
-------------

## Development Setup
In the Unity Asset Store, download and then import Unity Test Tools into
the `Assets/Imports/` folder. Make sure you don't import the `Examples`
folder.

You also need to download (and purchase if you haven't) NGUI from the Asset
Store into the `Assets/Imports/` folder. Again, don't import the `Examples`
folder.

## Important Notes

`UIPanel` depth rules:
- use 200+ for menu screens
- HUD uses 10-20
- alerts & notifications use 400+

### Resources

There are many different assets in this game which have to managed carefully.

Broadly speaking, there are two categories of resources in _The Founder_:

- template
- shared

#### Template Resources
Template resources are those which may be modified during gameplay. Thus the actual asset should never be used directly,
a new instance needs to be instantiated from it.

For example: a `Location` is a template resource. It has a certain amount of used infrastructure which is specific to the company
that owns the location.

If we directly used a Location asset, say "San Francisco", we would have a conflict. Company A expands to that location and adds a datacenter.
Company B also expands to that location and adds a datacenter. Since we are using the actual asset, they are sharing a reference to the same location
and thus it looks like Company A and B now have two datacenters at those location. So we must instantiate a separate `Location` for each company:

    Instantiate(myLocation) as Location;

Template resources should inherit from `TemplateResource<T>`, which implements a `Clone()` method which does the above for you. Whenever you are going to be using
a template resource, operate on a clone of it (using `Clone()`), not directly on the asset itself.

#### Shared Resources
Shared resources are those which are never modified and thus should only have a single instance in play. When loading directly from an asset, Unity already handles this for us.

For example:

    ProductType pt  = Resources.Load("Social Network") as ProductType;
    ProductType pt_ = Resources.Load("Social Network") as ProductType;
    pt.GetInstanceID() == pt_.GetInstanceID(); // true

This is great, but when serialization happens, this shared reference might be broken into separate instances if both company A and company B have this product type.

The `Serializer` included with _The Founder_ can automatically preserve these shared references so long as the resource in question inherits from the `SharedResource<T>` class.
The serializer will recognize this inheritance and attempt to load the resource directly from its asset, as demonstrated above.