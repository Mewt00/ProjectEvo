//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(SpawnOnRemoveBehavior))
{
   %template = new BehaviorTemplate(SpawnOnRemoveBehavior);
   
   %template.friendlyName = "Spawn On Remove";
   %template.behaviorType = "AI";
   %template.description  = "Spawn objects in place of this when this object is deleted";

   %template.addBehaviorField(object, "The object to spawn", object, "", t2dSceneObject);
   %template.addBehaviorField(count, "The number of objects to spawn", int, 2);
   %template.addBehaviorField(offset, "The distance from the center to spawn the objects", float, 4);
}

function SpawnOnRemoveBehavior::onRemove(%this)
{
   %this.spawnChildren();
}

function SpawnOnRemoveBehavior::spawnChildren(%this)
{
   if (!isObject(%this.object))
      return;
   
   for (%i = 0; %i < %this.count; %i++)
   {
      %newObject = %this.object.cloneWithBehaviors();
      
      %direction = mDegToRad(getRandom(0, 360));
      %x = mCos(%direction) * %this.offset;
      %y = mSin(%direction) * %this.offset;
      %offset = %x SPC %y;
      %newObject.position = t2dVectorAdd(%this.owner.position, %offset);
   }
}
