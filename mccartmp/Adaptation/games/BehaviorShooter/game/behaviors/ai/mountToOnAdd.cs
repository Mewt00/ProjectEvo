//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MountToOnAddBehavior))
{
   %template = new BehaviorTemplate(MountToOnAddBehavior);
   
   %template.friendlyName = "Mount Clone Automatically";
   %template.behaviorType = "AI";
   %template.description  = "Automatically mounts a selected object's clone to this one.";

   %template.addBehaviorField(object, "The object to clone and mount", object, "", t2dSceneObject);
   %template.addBehaviorField(mountPoint, "The mountpoint ID to use", int, 0);
   %template.addBehaviorField(mountForce, "Mount Tracking Force", float, 0.000);
   %template.addBehaviorField(trackRotation, "Track Mount Rotation", bool, "1");
   %template.addBehaviorField(ownedByMount, "Owned by Mount", bool, "0");
   %template.addBehaviorField(inheritAttributes, "Clone Attributes like Visibility", bool, "0");
   
}

function MountToOnAddBehavior::onAddToScene(%this, %scenegraph)
{
   if (!isObject(%this.object))
      return;
   
         
   %clone = %this.object.cloneWithBehaviors();
   
   %mountPoint = %this.owner.getLocalPoint(%this.owner.getLinkPoint(getWord(%this.owner.getAllLinkpointIDs(), %this.mountPoint)));
   //error("Mounting clone of "@%this.object@"("@%clone@") to "@%this.owner@" at link point: "@%mountPoint);
   %clone.mount( %this.owner, %mountPoint, 0, %this.trackRotation, true, %this.ownedByMount, %this.inheritAttributes );
}