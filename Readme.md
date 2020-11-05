GameObject palette + Brush for 2d Games

For the sake of having a good example, we are using the free asset pack provided at https://bakudas.itch.io/generic-rpg-pack, there is no intention of using it without credit, so if you like it, go there and buy it.

TODO:

* Brush logic for painting
  - API, support for multiple brushes
  - Mouse drag + repeat, etc.
  - Precreate preview using custom logic
* Erase tool (for now with key).
* Undo.
* Settings (asset) to configure prefab folders.
* Custom palette filters (assets).
* Cleanup stuff on scene save, load, etc.
* Create/Destroy brush object on palette selected, or at least for preview object.
* Custom brush logic (scriptable object).
  - brush pixel perfect