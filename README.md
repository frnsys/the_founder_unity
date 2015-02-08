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

### The Office UI

Most of the UI is accessed through the office environment. Here are some notes about how it is set up.

There is:

- the `Office UI`: this manages the NGUI buttons displayed on top of the office environment.
- the `Office Manager`:
    - manages the addition and removal of employee "avatars" (their in-office representation)
        - also manages the employee "HUDs", i.e. the text that floats up from their head
    - manages what office is currently shown, i.e. apartment, office, campus, etc
    - manages upgrading of the office, which involves:
        - removing the old office
        - instantiating the new office
        - loading the perks into the new office,
        - randomly placing employees so that they are within bounds of the new office
        - sets up the NGUI `Office UI` buttons, linking them with in-office objects to follow
    - manages the process of an office upgrade becoming available
- the `Office`:
    - this script is attached to prefabs which represent certain offices (apartment, office, campus, etc)
    - it manages the addition of perks in the office environment
        - which perks can be shown
        - how those perks are represented (which `GameObject` prefab)
        - where those representations are placed
    - has additional information the `Office Manager` needs, such as its bounds and what UI elements are available and what objects they should follow.
- the `Office Camera Controller`: this is what allows the user to pan around and zoom in and out of the office.
- `Office Area`: this just the `GameObject` which functions as the parent of the actual office object group.

#### Office upgrades

The office can be upgraded throughout the game, which provides access to different systems.

For example, in the starting apartment, the player can only build products, hire employees, and buy perks.

But later on they get access to research, public opinion, and all the other systems in the game.

The access is prevented only by not exposing the UI elements which lead to the menus of those systems.

For development, this is pretty annoying if you want to test all the systems - so there's a "Test" office prefab which includes UI for all the systems.

#### Adding an interactive office object

Office objects which can be interacted with (i.e., can be touched to bring up a menu) should have the `UIOfficeObject` script attached. There you can specify the window prefab to launch on touch. You should check "Enabled" so that it is true. You'll also need to add a `MeshCollider` with "Is Trigger" checked.

Then you will also want to create an NGUI button/label which tracks the object. However, you don't want it directly tracking the object itself since it may not be positioned how you like. So create an empty game object as a child to the office object (I have been naming them "UI Target") and position that where you want the label to be.

The NGUI button/label should be created as a child to `Office UI`  Set the `UIFollowTarget` script's target to the "UI Target" you just created. Also, configure the "On Click" method to your needs.

#### Creating a locked area

Creating a locked area is pretty involved.

You need to attach the `OfficeArea` script to the group which defines the office area and configure it properly. The group should have a "Lock Box" (a semi-transparent dark box surrounding the area). The `Office UI` group will need a button that can unlock the area. Unlocking the area is accomplished through the `Office Area Manager` (see the current methods there) and that area's unlock status is managed by the `GameData`.

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