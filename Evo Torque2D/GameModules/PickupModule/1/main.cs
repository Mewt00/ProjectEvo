//-----------------------------------------------------------------------------
// PickupModule: pickup class and functions
//-----------------------------------------------------------------------------

function PickupModule::create( %this )
{
    exec("./scripts/Pickup.cs");
    exec("./scripts/SpeedShotPickup.cs");
}

//-----------------------------------------------------------------------------

function PickupModule::destroy( %this )
{
}
