//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(WorldLimitWrapBehavior))
{
   %template = new BehaviorTemplate(WorldLimitWrapBehavior);
   
   %template.friendlyName = "World Limit Wrap";
   %template.behaviorType = "Game";
   %template.description  = "Wrap the object to the other side when it hits its world limit";
}

function WorldLimitWrapBehavior::onBehaviorAdd(%this)
{
   %this.owner.worldLimitMode = "NULL";
   %this.owner.worldLimitCallback = true;
}

function WorldLimitWrapBehavior::onWorldLimit(%this, %limitMode, %limit)
{
   switch$ (%limit)
   {
      case "left":
         if (%this.owner.getLinearVelocityX() < 0)
            %this.owner.setPositionX(getWord(%this.owner.getWorldLimit(), 3) - (%this.owner.getWidth() / 2));
      case "right":
         if (%this.owner.getLinearVelocityX() > 0)
            %this.owner.setPositionX(getWord(%this.owner.getWorldLimit(), 1) + (%this.owner.getWidth() / 2));
      case "top":
         if (%this.owner.getLinearVelocityY() < 0)
            %this.owner.setPositionY(getWord(%this.owner.getWorldLimit(), 4) - (%this.owner.getHeight() / 2));
      case "bottom":
         if (%this.owner.getLinearVelocityY() > 0)
            %this.owner.setPositionY(getWord(%this.owner.getWorldLimit(), 2) + (%this.owner.getHeight() / 2));
   }
}
