
/*
 *This short and stupid static class allows us to change scene and still be able to pass unchanged variables. 
 * the static player variable is used when we select a player in the UI scene and is passed to the instantiator class to disable 
 * and enable cameras for that player.  
 * 
 */

public static class DataHolder
{
    static public bool player = false; 
}
