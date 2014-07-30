//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(SpawnAreaBehavior))
{
   %template = new BehaviorTemplate(SpawnAreaBehavior);
   
   %template.friendlyName = "Spawn Area";
   %template.behaviorType = "AI";
   %template.description  = "Spawns objects inside the area of this object";

   %template.addBehaviorField(object, "The object to clone", object, "", t2dSceneObject);
   %template.addBehaviorField(count, "The number of objects to clone (-1 for infinite)", int, 50);
   %template.addBehaviorField(spawnTime, "The time between spawns (seconds)", float, 2.0);
   %template.addBehaviorField(spawnVariance, "The variance in the spawn time (seconds)", float, 1.0);
   %template.addBehaviorField(spawnStart, "Time before the first spawn (seconds)", float, 2.0);
   
   %spawnLocations = "Area" TAB "Edges" TAB "Center" TAB "Top" TAB "Bottom" TAB "Left" TAB "Right";
   %template.addBehaviorField(spawnLocation, "The area in which objects can be spawned", enum, "Area", %spawnLocations);
}

function SpawnAreaBehavior::onAddToScene(%this, %scenegraph)
{
   %this.spawnCount = 0;
   %this.schedule(%this.spawnStart * 1000, "spawn");
}

function SpawnAreaBehavior::spawn(%this)
{
   if (!isObject(%this.object) || !%this.owner.enabled)
      return;
   
   %clone = %this.object.cloneWithBehaviors();
   %xPos = 0;
   %yPos = 0;
   %spawnLocation = %this.spawnLocation;
   %edges = "Top" TAB "Bottom" TAB "Left" TAB "Right";
   if (%spawnLocation $= "Edges")
      %spawnLocation = getField(%edges, getRandom(0, 3));
   
   switch$ (%spawnLocation)
   {
      case "Area":
         %xPos = getRandom(getWord(%this.owner.getAreaMin(), 0), getWord(%this.owner.getAreaMax(), 0));
         %yPos = getRandom(getWord(%this.owner.getAreaMin(), 1), getWord(%this.owner.getAreaMax(), 1));
      case "Center":
         %xPos = %this.owner.position.x;
         %yPos = %this.owner.position.y;
      case "Top":
         %xPos = getRandom(getWord(%this.owner.getAreaMin(), 0), getWord(%this.owner.getAreaMax(), 0));
         %yPos = getWord(%this.owner.getAreaMin(), 1);
      case "Bottom":
         %xPos = getRandom(getWord(%this.owner.getAreaMin(), 0), getWord(%this.owner.getAreaMax(), 0));
         %yPos = getWord(%this.owner.getAreaMax(), 1);
      case "Left":
         %xPos = getWord(%this.owner.getAreaMin(), 0);
         %yPos = getRandom(getWord(%this.owner.getAreaMin(), 1), getWord(%this.owner.getAreaMax(), 1));
      case "Right":
         %xPos = getWord(%this.owner.getAreaMax(), 0);
         %yPos = getRandom(getWord(%this.owner.getAreaMin(), 1), getWord(%this.owner.getAreaMax(), 1));
   }
   
   %clone.position = %xPos SPC %yPos;
   
   %this.spawnCount++;
   if (%this.spawnCount < %this.count || %this.count == -1)
   {
      %minTime = (%this.spawnTime - %this.spawnVariance) * 1000;
      %maxTime = (%this.spawnTime + %this.spawnVariance) * 1000;
      %spawnTime = getRandom(%minTime, %maxTime);
      if( %spawnTime < 55 ) 
         %spawnTime = 55; 
      
      %this.schedule(%spawnTime, "spawn");
   }
   //might want to think about deleting the spawners if we're spawning spawners...
}
