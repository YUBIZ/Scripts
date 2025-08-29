public readonly record struct Tree<T>(T Value, Tree<T>[] SubTrees);
