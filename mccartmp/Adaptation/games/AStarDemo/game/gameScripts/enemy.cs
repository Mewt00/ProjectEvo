function Spawner::onLevelLoaded(%this, %sceneGraph)
{
   // register with our possible destinations list
   if (!isObject(SpawnerSet) )
   {
      $Spawners = new SimSet(SpawnerSet);
   }
   SpawnerSet.add(%this);
}

function Destination::onLevelLoaded(%this, %sceneGraph)
{
   // register with our possible destinations list
   if (!isObject(DestinationSet) )
   {
      $targets = new SimSet(DestinationSet);
   }
   DestinationSet.add(%this);
}

function startSpawnCycle(%timeBetweenSpawns)
{
   spawnRandom(%timeBetweenSpawns);
}

function spawnRandom(%timeBetweenSpawns)
{
   
   // create a random enemy
   %newEnemy = EnemyTemplate.cloneWithBehaviors();
   %newEnemy.moveSpeed = %newEnemy.baseSpeed;
   %startCoords = %newEnemy.findSpawn();
   %newEnemy.setPosition(%startCoords);

   %newEnemy.destinationCoords = %newEnemy.findDestination();

   %newEnemy.startPath(%startCoords, %newEnemy.destinationCoords);
   schedule(%timeBetweenSpawns, 0, spawnRandom, %timeBetweenSpawns);
}

function Enemy::startPath(%this, %start, %dest)
{
   // find our path
   %this.pathGrid = $pathGrid;
   %newPath = %this.findDestinationPath(%dest);
   if (isObject(%newPath)  )
   {
     %this.currentPath = %newPath;
     // for games that require the ability to test a world change before making it permanent,
     // we want a testing path that isn't created/deleted often, which will speed up our validation
     // of a player's "move" that changes our world. This is unused in the demo, but we leave it in
     // for other game types:
     
     %testingPath = %this.findDestinationPath(%dest);
     %this.testingPath = %testingPath;
     %this.followAStarPath(%newPath);
     activeActorsSet.add(%this);
   }
   else
   {
      %this.isFrustrated("noPath");
   }
}

function Enemy::findSpawn(%this)
{
   // find a random spawn spot
   if (!isObject(SpawnerSet))
   {
      // we should have spawners in our level
      error("No available spawners in level!");
      return "0 0";
   }
   
   %maxSpawnerCount = SpawnerSet.getCount();
   %spawnerIndex = getRandom(1, %maxSpawnerCount) - 1;
   %randomSpawner = SpawnerSet.getObject(%spawnerIndex);
   %startCoords = %randomSpawner.getPosition();
   return %startCoords;
}

function Enemy::findDestination(%this)
{
      // pick a random destination
   if (!isObject(DestinationSet))
   {
      //  we should have destinations in our level
      error("No available destinations in level!");
      %newEnemy.die();
      return "0 0";
   }
   
   %maxTargetCount = DestinationSet.getCount();
   %targetDestinationIndex = getRandom(1, %maxtargetCount) - 1;
   %targetDestination = DestinationSet.getObject(%targetDestinationIndex);
   %targetCoords = %targetDestination.getPosition();
   return %targetCoords;
}

function Enemy::isFrustrated(%this, %reason)
{
   // note: In most cases our Parent class is aStarActor, which does nothing for the ::isFrustrated() method
   // however, in different hierarchies, a call can be useful. Comment it out if you are using aStarActor
   // as your immediate parent (either class or superClass set to aStarActor)
   
   Parent::isFrustrated(%this, %reason);
   if (%reason $= "noPath")
   {
      // game logic here. In our demo, we will place them in the frustrated
      // actor set if they aren't already, and increment a counter for every "new" frustration case
      
      %this.frustratedTime++;
      %this.moveSpeed = %this.baseSpeed + %this.frustratedTime;
      echo("Enemy::isFrustrated(" @ %this @ "), timer is (" @ %this.frustratedTime @ ")");

      if (!(frustratedActorsSet.isMember(%this)) )
      {
        frustratedActorsSet.add(%this);
      }
   }
}
      
function Enemy::onCollision(%srcObject, %dstObject, %srcRef, %dstRef, %time, %normal, %contacts, %points)
{
   // we should only be colliding with our destination, but let's make sure:
   if (%dstObject.class $= "Destination")
   {
      %srcObject.die("reachedDestination");
   }
}
function Enemy::die(%this, %reason)
{
   // clean up our object, including all of our aStar housekeeping
   aStarActor::die(%this);
   
   // in our tutorial, we can only have one reason for dying ("escaped"), so we don't use
   // the %reason parameter. In other games, you will probably have different reasons for calling .die()
   // on your aStarActors, such as "destroyed", "escaped", "timedOut", or whatever your game requires.
   
   activeActorsSet.remove(%this);
   %this.safeDelete();
}