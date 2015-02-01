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

`GameEvent` images must be 800x440px.

When importing textures for the Browser mesh into Unity, make sure filtering is set to bilinear,
and maximize the aniso level. This is so that these flat-object textures look ok when rotating.

For incorporating 3D objects into the NGUI UI, we are using [the render queue approach](http://spaceandtim.es/posts/clipping-3d-objects-in-ngui).

The following values are rules of thumb, you may need to tweak them for the particular use case.
The `UIPanel` with the 3D objects should have its Render Q set to Start At 3200.
The 3D objects should have the `SetRenderQueue.cs` script attached, with the render queue set to 4000, use the Self-Illumin/Diffuse material, and be on the PlayArea layer.
If the `UIPanel` hosting the 3D objects has a parent `UIPanel`, e.g. the window with the header/navigation items, the header panel (or the parent panel itself) should have its Render Q set to Start At 4000.
All alerts/popups should have their `UIPanel` Render Q set to Start At 5000.

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

#### Special case: Workers

Workers are a special case since they are both templates (they are modified throughout the game) and shared (there is a "canonical" instance for each worker). So they can't simply be reloaded from
their source asset because they may have changes. And we can't create a new instance for each company as we do with template resources, since there is only supposed to be one instance of a given worker.

For workers, we accept multiple instances for a single worker but have some rules which let us find the "canonical" instance:

- If a worker is employed at a company, the employed instance is canonical.
- If the worker is not employed, the instance in `data.unemployed` is canonical.

We don't eliminate extraneous references. Instead, all other instances, such as those held by `UnlockSet`s, are only for reference - we never use them directly. We lookup the canonical worker with the same name and just use that instance.

A `WorkerManager` on the `GameManager` keeps track of these canonical instances, so always move workers around (i.e. hire and fire them) through the `WorkerManager`:

    // Defaults to the player company.
    GameManager.Instance.workerManager.HireWorker(worker<, company>);
    GameManager.Instance.workerManager.FireWorker(worker<, company>);

The `WorkerManager` makes sure workers are not employed at multiple places and removes/adds them to the `data.unemployed` list as necessary.