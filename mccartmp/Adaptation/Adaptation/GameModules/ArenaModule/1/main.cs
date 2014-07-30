//-----------------------------------------------------------------------------
// Initial setup
//-----------------------------------------------------------------------------


function ArenaModule::create( %this )
{
    exec("./scripts/Arena.cs");
}

//-----------------------------------------------------------------------------

function ArenaModule::destroy( %this )
{
    ArenaModule::cancelPendingEvents();
}
