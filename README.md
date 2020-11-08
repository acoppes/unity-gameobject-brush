# Unity's GameObject Paint Tool 

This tool provides both a GameObject Palette Window and customizable Brushes in order to simplify the task of decorating 2d Games scenes using GameObjects.

## Features

 * Preview what you paint, in the layer you paint it.
 * Customizable brushes.
 * Customizable palette window.

## Example 

![Alt text](Images/palette_example.gif?raw=true "Example")

## The assets pack 

We are using the https://bakudas.itch.io/generic-rpg-pack asset pack, right now is free but you can pay what you want on download, it is not the idea to distribute it from outside itchio with this tool but it is great to show a good example of how the tool works.

## Roadmap

### Next

  * Use Unity EditorTool so we can use events in our way.
    - Can we select our custom tool from our editor window when a palette object is selected?

### Later

* Filters for game objects (decide which objects to show in palette)
  - Customizable (logic)
* Settings (asset) to configure prefab folders.
  - Filters to use
  - Customizable in asset or preferences
  - Default prefabs folder
  - Search for children or not when generating palette.
* Cleanup stuff on scene save, load, etc.
* Erase
  - Filter which objects not to delete (check objects inside root, avoid root, camera, etc)
  - While holding key or while erase selected, highlight the object that is going to be deleted?
  - Bigger "erase" size, now it works one by one and doesn't allow dragging.
* Brushes and paint logic
  - Paint while mouse drag (delay, distance, etc)
  - Random Flip
  - Random Size
  - Paint multiple instances, random, distribute in area.
  - Select more than one prefab at the same time
  - Paint with prefab instance or unpack prefab.
* Misc
  - Selected entries should be a list (or a concept Selection).
  - Visible tools for paint and erase (with icons), with togglable buttons.
  - Configurable modifier key to toggle erase
  - Sprite assets in palette
  - Change to use UIElements.
  - User brush override data in editor window.
  - Refresh preview with mouse wheel up/down (useful if the brush has random modifiers)
  - Maybe add brush modifiers to allow overriding some logic.
    - For example, multiple instances of same prefab (for grass), or random between selected prefabs, etc.
  - Documentation of how to use it and extend.
  - Maybe brushes should be GameObjects to support custom modifiers as components.

## Licence 

This repository is [MIT](./LICENSE.md) licensed.