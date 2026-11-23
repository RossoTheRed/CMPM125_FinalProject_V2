// Technically not needed, but I prefer keeping the leaf, separate 
// from the class itself, and make behavior an abstract class

// Used for actions / conditions
// could be used later, not too sure yet
public class LeafNode : Behavior
{
    protected override Status Update() {return Status.BH_FAILURE;}
}


// NOTE FOR TOMORRROW:
// Condition fixed update references condition in update, that way when fixed update
// is run through the tree, it basically skips the process, letting the update handle
// all conditions if they are true or not
// Actions always return true, that way it keeps on going
// if they return running, ontinue forward
// that way fixed updates keep going through, and are properly run