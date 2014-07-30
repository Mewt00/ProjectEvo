//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MouseOverGrowBehavior))
{
   %template = new BehaviorTemplate(MouseOverGrowBehavior);
   
   %template.friendlyName = "Mouse Over Grow";
   %template.behaviorType = "GUI";
   %template.description  = "Grow the object when the mouse hovers over it";

   %template.addBehaviorField(scaleTime, "The amount of time to animate the scale (seconds)", float, 0.25);
   %template.addBehaviorField(scaleFactor, "The scale factor of the object", float, 1.2);
}

function MouseOverGrowBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
   %this.owner.enableUpdateCallback();
   
   %this.startSize = %this.owner.size;
   %this.targetSize = %this.startSize;
   
   %this.scaleAmount = t2dVectorSub(t2dVectorScale(%this.startSize, %this.scaleFactor), %this.startSize);
}

function MouseOverGrowBehavior::onMouseEnter(%this, %modifier, %worldPos)
{
   %this.targetSize = t2dVectorScale(%this.startSize, %this.scaleFactor);
}

function MouseOverGrowBehavior::onMouseLeave(%this, %modifier, %worldPos)
{
   %this.targetSize = %this.startSize;
}

function MouseOverGrowBehavior::onUpdate(%this)
{
   %currentX = %this.owner.size.x;
   %targetX = %this.targetSize.x;
   %scaleX = %this.scaleAmount.x;
   
   %currentY = %this.owner.size.y;
   %targetY = %this.targetSize.y;
   %scaleY = %this.scaleAmount.y;
   
   if ((%currentX == %targetX) && (%currentY == %targetY))
      return;
   
   if (%currentX < %targetX)
   {
      %this.owner.setSizeX(%currentX + ((0.032 / %this.scaleTime) * %scaleX));
      if (%this.owner.getSizeX() > %targetX)
         %this.owner.setSizeX(%targetX);
   }
   else if (%currentX > %targetX)
   {
      %this.owner.setSizeX(%currentX - ((0.032 / %this.scaleTime) * %scaleX));
      if (%this.owner.getSizeX() < %targetX)
         %this.owner.setSizeX(%targetX);
   }
   
   if (%currentY < %targetY)
   {
      %this.owner.setSizeY(%currentY + ((0.032 / %this.scaleTime) * %scaleY));
      if (%this.owner.getSizeY() > %targetY)
         %this.owner.setSizeY(%targetY);
   }
   else if (%currentY > %targetY)
   {
      %this.owner.setSizeY(%currentY - ((0.032 / %this.scaleTime) * %scaleY));
      if (%this.owner.getSizeY() < %targetY)
         %this.owner.setSizeY(%targetY);
   }
}
