// Technically not needed, but I prefer keeping the leaf, separate 
// from the class itself, and make behavior an abstract class

// Used for actions / conditions
// could be used later, not too sure yet
public class LeafNode : Behavior
{
    protected override Status Update() {return Status.BH_FAILURE;}
}
