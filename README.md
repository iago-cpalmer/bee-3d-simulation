# Be a Bee - Bee Colony Simulation Game

## Overview
"Be a Bee" is an educational and engaging open-world simulation game designed to raise awareness about the vital role of bees in ecosystems and the threats they face. By simulating the behaviors of a bee colony, the game educates players on sustainable practices to protect bees while offering an entertaining experience.

---

## Full Showcase video
[![Play-through video](https://img.youtube.com/vi/rEgRDfYNCuw/0.jpg)](https://youtu.be/rEgRDfYNCuw)


---

## Features

### Gameplay Modes
- **Spectator Mode**: Observe the colony and its bees, following individual bees and accessing detailed entity information.
- **Player Mode**: Play as a bee, performing key tasks such as exploration, collecting resources, and resting.

### World
- **Procedural Generation**: The game world is procedurally generated for immersive, dynamic environments with terrain, flora, and decorative elements. Using Fbm noise techniques for realistic terrain heightmaps and natural world elements.
- **Chunk Management**: World is divided into manageable sections (WorldChunks), improving memory and rendering efficiency.

### Bee Simulation
- Realistic bee behaviors including exploration, foraging, resting, and dancing for communication.
- Simulated colony lifecycle with population growth and decline based on resources and environmental factors.

### Optimization
- **Unity ECS**: Efficient management of numerous interacting entities using Unity's Entity Component System.
- **Multi-threading**: Using Unity DOTS to execute systems in parallel.
- **Level of Detail (LOD)**: Dynamically adjusts model complexity based on distance from the camera for: textures, models & simulation.
- **Mesh instancing **: Render all the repetitive meshes with instancing to highly increase rendering performance
- **Animation Baking **: 3D animations are baked to textures to avoid calculations in CPU.
- **Custom highly efficient ray-cast **: Created my own Ray-Cast algorithm to interact with Entities avoiding allocations, unnecessary checks and using more efficient math operations
- **Vertex Compression**: Reduces memory usage by compressing vertex data by ~89%.
- **Garbage Collection Optimization**: Minimizes unnecessary memory allocation for smoother gameplay.
- **Procedural Loading**: Efficiently loads and unloads chunks to balance performance with immersion.

### Audio and UI
- Environmental sounds and interactive audio feedback with a custom Sound system. All SFX are made by me.
- User-friendly interface with tutorials and quests.

## Benchmarks, Tests (in a low-medium end computer) & Conclusions

# Performance and Optimization Tests

## FPS Test Results

| **Entities**    | **100**    | **10,000**  | **100,000** |
|-----------------|------------|-------------|-------------|
| **FPS Range**  | [180-300]  | [130-230]   | [50-80]     |

**Figure 1**: FPS results for 100, 10,000, and 100,000 entities.

## Resource Usage Test

This test measures the following for varying numbers of entities:
- **CPU Usage (%)**
- **GPU Usage (%)**
- **RAM Usage (MB)**
- **VRAM Usage (MB)**

### Results

| **Entities**    | **100**    | **10,000**  | **100,000** |
|-----------------|------------|-------------|-------------|
| **% CPU**      | 28%        | 45%         | 80%         |
| **% GPU**      | 0.2%       | 2%          | 3.8%        |
| **RAM (MB)**   | 1004       | 1070        | 2036        |
| **VRAM (MB)**  | 200        | 200         | 200         |

## Animation Performance Comparison (CPU vs GPU)

### CPU Animation Performance

| **Entities**    | **100**    | **10,000**  | **100,000** |
|-----------------|------------|-------------|-------------|
| **FPS Range**  | [150-220]  | [70-110]    | [9-11]      |

### GPU Animation Performance

| **Entities**    | **100**    | **10,000**  | **100,000** |
|-----------------|------------|-------------|-------------|
| **FPS Range**  | [180-300]  | [130-230]   | [50-80]     |

The comparison highlights the overhead of sinusoidal calculations in CPU-based animations, contrasted with GPU-accelerated baking for vertex transformations.

## Raycasting Comparison

This test compares the performance (in milliseconds) of raycasting using Unity's default physics system versus a custom raycasting solution.

| **Entities**               | **100**   | **10,000**  | **100,000** |
|----------------------------|-----------|-------------|-------------|
| **Custom Raycast (ms)**   | 0.0001    | 0.001       | 0.001       |
| **Unity Physics Raycast (ms)** | 0.0015    | 0.02        | 0.1         |

Additionally, the periodic updates required for Unity's physics system scale with the number of entities, as shown below:

| **Entities**          | **100**    | **10,000**  | **100,000** |
|-----------------------|------------|-------------|-------------|
| **Time Per Frame (ms)** | [1-3.2]   | [7-12]      | [25-27]     |

The custom raycasting approach eliminates the overhead of physics world updates and significantly reduces raycasting time.

## System Execution Time

The following table lists the execution times (in milliseconds) for various systems per frame:

| **Entities**                  | **100**   | **10,000** | **100,000** |
|-------------------------------|-----------|------------|-------------|
| **Bee Behaviour System (ms)** | 0.05      | 0.6        | 1.25        |
| **Scan Handler System (ms)**  | 0.053     | 0.12       | 0.98        |
| **Entity Sound System (ms)**  | 0.08      | 0.21       | 0.82        |
| **Polen Request Handler System (ms)** | 0.03  | 0.01       | 0.14        |
| **Nest Handler System (ms)**  | 0.011     | 0.025      | 0.23        |

The most computationally expensive systems are those directly affecting bees, such as the **Bee Behaviour System**.


