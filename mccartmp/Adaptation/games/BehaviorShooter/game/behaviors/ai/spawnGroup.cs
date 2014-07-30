//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(SpawnGroupBehavior))
{
   %template = new BehaviorTemplate(SpawnGroupBehavior);
   
   %template.friendlyName = "Spawn Group";
   %template.behaviorType = "AI";
   %template.description  = "Spawns multiple objects inside the area of this object";

   %template.addBehaviorField(object, "The object to clone", object, "", t2dSceneObject);
   %template.addBehaviorField(count, "The number of objects to clone per wave (-1 for infinite)", int, 50);
   %template.addBehaviorField(groupCount, "The number of objects to clone per wave (-1 for infinite)", int, 50);
   %template.addBehaviorField(spawnTime, "The time between spawns (seconds)", float, 2.0);
   %template.addBehaviorField(spawnVariance, "The variance in the spawn time (seconds)", float, 1.0);
   %template.addBehaviorField(spawnStart, "Time before the first spawn (seconds)", float, 2.0);
   
   %spawnLocations = "Horizontal Line" TAB "Vertical Line";
   %template.addBehaviorField(spawnLocation, "The formation to spawn the clones in", enum, "Horizontal Line", %spawnLocations);
}

function SpawnGroupBehavior::onAddToScene(%this, %scenegraph)
{
   %this.spawnCount = 0;
   %this.schedule(%this.spawnStart * 1000, "spawn");
}

function SpawnGroupBehavior::spawn(%this)
{
   if (!isObject(%this.object) || !%this.owner.enabled)
      return;
   
   for( %num = 0; %num < %this.groupCount; %num++ )
   {
      
      %clone = %this.object.cloneWithBehaviors();
      %xPos = 0;
      %yPos = 0;
      %spawnLocation = %this.spawnLocation;
      %edges = "Top" TAB "Bottom" TAB "Left" TAB "Right";
      if (%spawnLocation $= "Edges")
         %spawnLocation = getField(%edges, getRandom(0, 3));
      
      %xSpacing = (getWord(%this.owner.getAreaMax(), 0) - getWord(%this.owner.getAreaMin(), 0))/%this.groupCount;
      %ySpacing = (getWord(%this.owner.getAreaMax(), 1) - getWord(%this.owner.getAreaMin(), 1))/%this.groupCount;
      
      switch$ (%spawnLocation)
      {
         case "Horizontal Line":
            %xPos = getWord(%this.owner.getAreaMin(), 0) + %xSpacing * %num;
            %yPos = %this.owner.getPositionY();
         case "Vertical Line":
            %yPos = getWord(%this.owner.getAreaMin(), 1) + %ySpacing * %num;
            %xPos = %this.owner.getPositionX();
      }
      
      %clone.position = %xPos SPC %yPos;
   
   }
   
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
