# The Founder
-------------

![Think happy.](http://www.thefounder.biz/assets/art/ad.png)

<http://thefounder.biz/>
<http://www.thefounder.biz/art/>

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

In the code, there is a stat called "publicity", but note that this is only used internally. In-game, the term "Hype" is used instead.

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

#### Rearranging the office

If you add or remove objects to the office, make sure you re-bake the nav textures.

### Shared Resources
Shared resources are those which are never modified and thus should only have a single instance in play. When loading directly from an asset, Unity already handles this for us.

For example:

    ProductType pt  = Resources.Load("Social Network") as ProductType;
    ProductType pt_ = Resources.Load("Social Network") as ProductType;
    pt.GetInstanceID() == pt_.GetInstanceID(); // true

This is great, but when serialization happens, this shared reference might be broken into separate instances if both company A and company B have this product type.

The `Serializer` included with _The Founder_ can automatically preserve these shared references so long as the resource in question inherits from the `SharedResource<T>` class.
The serializer will recognize this inheritance and attempt to load the resource directly from its asset, as demonstrated above.

#### The `A` Classes

To make serialization/deserialization (game saving/loading) less insane, we try to take advantage of the `SharedResource` system wherever possible. However, some classes, such as `Worker`s, are modified throughout the game. We do not want to permanently affect its underlying asset. For these classes, we have the `SharedResource` version of the class, e.g. `Worker`, which contains static (as in unchanging) data. That way, we do not need to serialize or deserialize each individual field; rather we just re-load the asset as needed. But we do not directly interact with this `SharedResource` class. Rather, we have an associated class, in the case of `Worker`, it is `AWorker`, which wraps around the `SharedResource` and manages the data which does change throughout the course of the game. This is so that we save only the absolute minimum.

## License

<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/"><img alt="Creative Commons License" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/80x15.png" /></a><br /><span xmlns:dct="http://purl.org/dc/terms/" property="dct:title">The Founder</span> by <a xmlns:cc="http://creativecommons.org/ns#" href="http://frnsys.com" property="cc:attributionName" rel="cc:attributionURL">Francis Tseng</a> is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/">Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License</a>.