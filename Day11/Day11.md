### Day 11

#### Key Algorithms

Input paths are DAGs

##### Part 1

- Depth First Search with Backtracking - Similar to Day 7

##### Part 2

- DFS with Memoization - avoid re-exploring devices already explored  

- Further simplification possible
  - There are 2 high level routes to the goal
  - you -> dac -> fft -> out
  - you -> fft -> dac -> out
  - therefore calculate routes from you->dac * dac->fft * fft->out + you->fft * fft->dac * dac-> out
  