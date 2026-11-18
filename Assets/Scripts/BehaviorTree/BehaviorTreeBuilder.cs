using System.Collections.Generic;

// Builder/Factory Template using wrappers (my beloved), CMPM 121
public class BehaviorTreeBuilder
{
    private Behavior rootNode;
    // not sure if we need parent stack
    private Stack<Behavior> parentStack = new Stack<Behavior>();

    // Called at the end of finished tree, handles wrappers
    public Behavior Build()
    {
        return rootNode;
    }

    // Attached to node to whatever should be ontop of the stack
    private void AddNodeToParent(Behavior node)
    {
        if (parentStack.Count == 0)
        {
            rootNode = node; // first node is root node
        }
        else
        {
            //parentStack.Peek().AddChild(node); // add to curr parent
            if (parentStack.Peek() is Composite comp)
            {
                comp.AddChild(node);
            }
        }
    }
   

    // add end to make sure it pops the stack
    public BehaviorTreeBuilder End()
    {
        if (parentStack.Count > 0)
        {
            parentStack.Pop();
        }
        return this;
    }
    
    // Composites
    public BehaviorTreeBuilder Sequence()
    {
        var node = new Sequence();
        AddNodeToParent(node);
        parentStack.Push(node);
        return this;
    }
    public BehaviorTreeBuilder Selector()
    {
        var node = new Selector();
        AddNodeToParent(node);
        parentStack.Push(node);
        return this;
    }
    public BehaviorTreeBuilder Parallel(Parallel.Policy success, Parallel.Policy failure)
    {
        var node = new Parallel(success, failure);
        AddNodeToParent(node);
        parentStack.Push(node);
        return this;
    }

    // Leaves
    public BehaviorTreeBuilder Condition(ConditionNode condition)
    {
        AddNodeToParent(condition); 
        // no pushing since childless
        return this;
    }

    public BehaviorTreeBuilder Action(ActionNode action)
    {
        AddNodeToParent(action);
        // no pushing since childless
        return this;
    }

    // Decorators
    public BehaviorTreeBuilder ForceSuccess(Behavior child)
    {
        // NOTE: not sure on this one - chris
        var node = new ForceSuccess(child);
        AddNodeToParent(node);
        // no pushing since childless
        return this;
    }



}
