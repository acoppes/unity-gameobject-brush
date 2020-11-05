# Unity's GameObject Paint Tool 

This tool provides both a GameObject Palette Window and customizable Brushes in order to simplify the task of decorating 2d Games scenes using GameObjects.

## The assets pack 

We are using the https://bakudas.itch.io/generic-rpg-pack asset pack, right now is free but you can pay what you want on download, it is not the idea to distribute it from outside itchio with this tool but it is great to show a good example of how the tool works.

## Example 

![Alt text](Images/palette_example.gif?raw=true "Example")

## Roadmap

* Palette size in editor.
* Brush logic for painting
  - Mouse drag + repeat, etc.
* Settings (asset) to configure prefab folders.
* Custom palette filters (assets).
* Cleanup stuff on scene save, load, etc.
* Select more than one prefab at the same time.

## Erase

* For now with key, if clicked while holding alt, then delete nearest object.
* While holding key or while erase selected, highlight the object that is going to be deleted?

## Undo

* After erase painted object.

## Brushes 

* Random Flip
* Random Size
* Paint multiple instances, random, distribute in area.