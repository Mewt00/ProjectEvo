//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(RigidBodyBehavior))
{
   %template = new BehaviorTemplate(RigidBodyBehavior);
   
   %template.friendlyName = "Rigid Body";
   %template.behaviorType = "Physics";
   %template.description  = "Sets the collision response mode to rigid.";
}

function RigidBodyBehavior::onAddToScene(%this, %scenegraph)
{
   %this.owner.setCollisionResponse("RIGID");
}
