// methods for working with A*


// set up our set to track actors that are actively following a path
if (!isObject(activeActorsSet) )
{
   new SimSet(activeActorsSet);
}

// set up our set to track actors that are currently frustrated (not following a path)
if (!isObject(frustratedActorsSet) )
{
   new SimSet(frustratedActorsSet);
}

function aStarActor::die(%this)
{
   if (isObject(%this.currentPath) )
   {
     //echo("aStarActor::die--removing path (" @ %this.currentpath @ ")");
     %this.pathGrid.deletePathID(%this.currentPath);
     %this.pathGrid = 0;
   }
}
function aStarActor::followAStarPath(%this, %pathID)
{
   // start at node 1 of the path, get it's coords, and move to it
   // callbacks will handle the rest
   // hard coded values for now, fix up later


   if (!isObject(%this.currentPath) )
   {
      return;
   }
   
   %this.currentPathNodeIndex = 0;
   %this.currentPath = %pathID;
   %this.lastAStarNodeIndex = %pathID.getSteps();
   %this.moveToAStarNode(1);
}

function aStarActor::onPositionTarget(%this)
{
   // we're at our target, time to move to next node
   // first, check to make sure we're not finished with our destination
   if (%this.currentPathNodeIndex == %this.lastAStarNodeIndex)
   {
      // we've reached destination!
      %this.onAStarDestinationReached();
      return;
   }
   // increment to our next node
   if (isObject(%this.currentPath) )
   {
     %this.currentPathNodeIndex++;
     %this.moveToAStarNode(%this.currentPathNodeIndex);
   }
   else
   {
      // allow a hook if desired to track a path disappearing out from underneath us
      // %this.noPathExists();
   }
}

function aStarActor::moveToAStarNode(%this, %nodeIndex)
{
   if (!isObject(%this) )
   {
      // we shouldn't be attempting to move as an aStarActor if we aren't a valid object
      error("aStarActor (" @ %this @ ") in game but not object, how?");
      return;
   }
   if (!isObject(%this.currentPath) )
   {
      // we shouldn't be attempting to move to an aStar node if we don't have a path
      error("aStarActor (" @ %this @ ") without a path");
      return;
   }

     // getNodeWorldCoords(%index) is a ConsoleMethod on the pathAStar2d class
     %nodeWorldCoords = %this.currentPath.getNodeWorldCoords(%nodeIndex);
     if (%nodeWorldCoords $= "InvalidNode")
     {
        %this.noPathExists();
     }
     else
     {
       %this.moveTo(%nodeWorldCoords,
                  %this.moveSpeed,     // speed
                  false,  // stop?
                  true,   // callback?
                  false,  // snap?
                  0.8     // margin
                  );
     }

}

function aStarActor::onAStarDestinationReached(%this)
{
   echo("We made it!");
   //  stop moving
   %this.setLinearVelocity("0 0");
   // need to remove the path
   if (isObject(%this.currentPath) )
   {
     %this.pathGrid.deletePathID(%this.currentPath);
   }
   if (isObject(%this.testingpath) )
   {
      %this.pathGrid.deletePathID(%this.testingPath);
   }
   %this.die("escaped");
}

function aStarActor::findDestinationPath(%this, %destination)
{    
   %newPath = %this.pathGrid.createPath(%this.getPosition(),
                                   %destination,
                                   false, // allow diagonal?
                                   true // optimize?
                                   );
                                  
   return %newPath;
}

function aStarActor::testPaths(%this, %grid, %destination)
{
   %validPathExists = %this.testingPath.calculate(%this.getPosition(),
                                              %destination,
                                              false,
                                              false);
   return %validPathExists;
}

function aStarActor::noPathExists(%this, %reasonForTest)
{
   %this.setLinearVelocity("0 0");

   // disable path finding for this actor until game code decides what to do
   if (isObject(%this.currentPath) )
   {
     //echo("aStarActor::die--removing path (" @ %this.currentpath @ ")");
     %this.pathGrid.deletePathID(%this.currentPath);

   }   
   if (isObject(%this.testingPath) )
   {
      %this.pathGrid.deletePathID(%this.testingPath);
      %this.testingPath = 0;
   }
   
   activeActorsSet.schedule(0, remove, %this);
   // game specific result here
   %this.isFrustrated("noPath");

}

function aStarActor::isFrustrated(%this, %reason)
{
   // this should be overloaded for your aStarActor primary class, in this demo's case, Enemy::

}

function notifyActors(%grid, %event)
{
   if (%event $= "WorldChange")
   {
      // iterate over each active actor, calculate new paths
      %actorsCount = activeActorsSet.getCount();
      for (%actorsIndex = 0; %actorsIndex < %actorsCount; %actorsIndex++)
      {
         %thisActor = activeActorsSet.getObject(%actorsIndex);
         if (!isObject(%thisActor) )
         {
            // this shouldn't normally happen. If you suspect issues with object ID's, 
            // uncomment the reporting line:
            //error("notifyActors: found an object with ID -1 in the ActiveActorsSet");
            continue;
         }
         if (isObject(%thisActor.currentPath) )
         {
           %thisActor.pathGrid.deletePathID(%thisActor.currentPath);
         }
         %newPath = %thisActor.findDestinationPath(%thisActor.destinationCoords);
         if (isObject(%newPath) )
         {
           %thisActor.currentPath = %newPath;
           %thisActor.followAStarPath(%newPath);
         }
         else
         {
            %thisActor.currentPath = "";
            %thisActor.noPathExists("WorldChange");
         }
      }
      // iterate over each inactive actor, calculate new paths
      %actorsCount = frustratedActorsSet.getCount();
      for (%actorsIndex = 0; %actorsIndex < %actorsCount; %actorsIndex++)
      {
         %thisActor = frustratedActorsSet.getObject(%actorsIndex);
         if (!isObject(%thisActor) )
         {
            // this shouldn't normally happen. If you suspect issues with object ID's, 
            // uncomment the reporting line:
            //error("notifyActors: found an object with ID -1 in the FrustratedActorsSet");
            continue;
         }
         if (isObject(%thisActor.currentPath) )
         {
           %thisActor.pathGrid.deletePathID(%thisActor.currentPath);
         }
         %newPath = %thisActor.findDestinationPath(%thisActor.destinationCoords);
         if (isObject(%newPath) )
         {
           %thisActor.currentPath = %newPath;
           %thisActor.followAStarPath(%newPath);
           frustratedActorsSet.schedule(0, remove, %thisActor);
           activeActorsSet.schedule(0, add, %thisActor);
         }
         else
         {
            // optional hook if you wish to be notified every world change that a frustrated actor
            // still does not have a path
            // %thisActor.noPathExists("WorldChange");
         }
      }
      return true;
   }
   if (%event $= "TestWorldChange")
   {
      // in a game that has many aStar actors, this event type allows a faster test by always
      // keeping a "testing path" object created, so it can be used to immediately re-calculate
      // if a path exists on a world change.

      // it should only normally be used when a world change event can be denied by game state, i.e. if
      // your game requires that a valid path always exists when a player attempts to change the world state
      // it can be useful to call this event first, and if it returns true, then call the WorldChange state
      
      // iterate over each actor, calculate new paths
      %actorsCount = activeActorsSet.getCount();
      for (%actorsIndex = 0; %actorsIndex < %actorsCount; %actorsIndex++)
      {
         %thisActor = activeActorsSet.getObject(%actorsIndex);
         %newPathExists = %thisActor.testPaths(%thisActor.destinationCoords);
         if (!%newPathExists)
         {
            return false;
         }
      }
      return true;
   }
   // default
   return true;
}