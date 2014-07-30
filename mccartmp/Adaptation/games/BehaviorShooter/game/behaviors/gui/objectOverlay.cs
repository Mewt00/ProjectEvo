//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(ObjectOverlayBehavior))
{
   %template = new BehaviorTemplate(ObjectOverlayBehavior);
   
   %template.friendlyName = "Object Overlay";
   %template.behaviorType = "GUI";
   %template.description  = "Displays an object on top of this object when the mouse hovers over it";

   %template.addBehaviorField(object, "The object to display", object, "", t2dSceneObject);
   %template.addBehaviorField(fadeTime, "The amount of time to fade the object (seconds)", float, 1.0);
   %template.addBehaviorField(alignRotation, "Match the rotation of the object to this object", bool, 1);
}

function ObjectOverlayBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
   %this.owner.enableUpdateCallback();
}

function ObjectOverlayBehavior::onLevelLoaded(%this, %scenegraph)
{   
   if (!isObject(%this.object))
      return;
   
   %this.object.mount(%this.owner);
   %this.object.setBlendAlpha(0.0);
   %this.targetAlpha = 0.0;
}

function ObjectOverlayBehavior::onMouseEnter(%this, %modifier, %worldPos)
{
   if (!isObject(%this.object))
      return;
   
   %this.object.position = %this.owner.position;
   if (%this.alignRotation)
      %this.object.rotation = %this.owner.rotation;
   
   %this.targetAlpha = 1.0;
}

function ObjectOverlayBehavior::onMouseLeave(%this, %modifier, %worldPos)
{
   if (!isObject(%this.object))
      return;
   
   %this.targetAlpha = 0.0;
}

function ObjectOverlayBehavior::onUpdate(%this)
{
   if (!isObject(%this.object))
      return;
   
   %currentAlpha = %this.object.getBlendAlpha();
   if (%currentAlpha == %this.targetAlpha)
      return;
   
   if (%currentAlpha < %this.targetAlpha)
   {
      %currentAlpha += 0.032 / %this.fadeTime;
      if (%currentAlpha > %this.targetAlpha)
         %currentAlpha = %this.targetAlpha;
      
      %this.object.setBlendAlpha(%currentAlpha);
   }
   else if (%currentAlpha > %this.targetAlpha)
   {
      %currentAlpha -= 0.032 / %this.fadeTime;
      if (%currentAlpha > %this.targetAlpha)
         %currentAlpha = %this.targetAlpha;
      
      %this.object.setBlendAlpha(%currentAlpha);
   }
}
