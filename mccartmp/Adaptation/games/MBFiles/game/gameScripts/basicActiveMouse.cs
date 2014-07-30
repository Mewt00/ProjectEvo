// controls and callbacks for enhanced UI


sceneWindow2D.setUseWindowMouseEvents(true);


function sceneWindow2D::onMouseDown(%this, %mod, %worldPos, %mouseClicks)
{
   $ActiveMouseObject::mouseDown = true;
}

function sceneWindow2D::onMouseUp(%this, %mod, %worldPos, %mouseClicks)
{
   $ActiveMouseObject::mouseDown = false;
}

function mouseObject::onLevelLoaded(%this, %sceneGraph)
{
   $ActiveMouseObject = %this;
   %this.setPosition = sceneWindow2D.getMousePosition();
   %this.enableUpdateCallback();
}
   
function mouseObject::onUpdate(%this)
{
   // find the mouse pointer
   %mouseCoords = sceneWindow2D.getMousePosition();
   // move us there
   %this.setPosition(%mouseCoords);
}
