# 0.0.7



# 0.0.5

## Added

  * Support for multi selection with shift key pressed.
  * Added small distribute logic to default brush.
  * New tool in editor window with custom brush icon.
  * Now dragging while left mouse button pressed will continuously paint.
  * Ctrl key + scroll wheel regenerates preview before painting.
  * Added optional brush modifiers that can be applied in order to modify the brush.

## Changed

  * Now preview objects has common parent.
  * Changed to switch between paint and erase mode with E key, disabled erase while holding left alt key.

# 0.0.4

## Changed
 
  * Added changelog and license to package root folder.

# 0.0.3 

## Added

  * Erase tool, for now by clicking while left alt key is pressed
  * Undo for erasing objects.
  * Visual button to show while erasing is active and to select/unselect erase tool.
  * Undo for erased objects.
  * Escape key and mouse right button unselect palette objects.

## Fixed

  * Previously selected unity tool restored on unselect palette.

## Changed

  * Search in Assets folder by default (not Assets/Palette)

# 0.0.2

## Fixed

  * Bug when become visible was called and no selected entry was selected.
  * Default window size when first time open.

## Changed

  * Now prefabs with renderer in children objects are considered too when generating the palette.

# 0.0.1

## Added 

  * Palette slider to select preview size.

## Changed

  * Preview size slider is now free, not fixed sizes.