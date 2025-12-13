## Day 7

### Key Points

Beams via Splitter form DAG with shared paths

      A
     / \
    B   C
    |\ /|
    | X |
    |/ \|
    D   E

Part 1

Simple algo to count the splits

Part 2
Can be solved by DFS (depth first search) but simple DFS becomes computationally expensive,
So we can memoize the shared paths e.g. A->B->X has 2 paths so we momorize X as 2, so when we visit C we know that X has 2 paths 