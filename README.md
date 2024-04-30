# Unity ML-Agents: Curriculum Training Project

## Overview

This project demonstrates the implementation of curriculum training using Unity's ML-Agents toolkit. The focus is on progressively training an AI agent to navigate complex environments, enhance its walking abilities, and utilize ray perception for obstacle avoidance and checkpoint navigation. The project leverages the Proximal Policy Optimization (PPO) algorithm to optimize agent performance through various training phases.

## Features

- **Phase 1:** Basic Navigation - The agent learns to navigate to a target location in a simple environment.
- **Phase 2:** Balanced Walking - Introduction of walking mechanics; the agent must maintain balance and proper posture while navigating.
- **Phase 3 (Incomplete):** Complex Navigation - The agent uses ray perception to navigate through an environment with dynamic obstacles and checkpoints.

## Installation

Clone this repository to your local machine using:

```bash
git clone https://github.com/yourusername/unity-ml-agents-curriculum-training.git
```

### Prerequisites

- Unity 2018.4 and later
- Python 3.9.13 (virtual environment)
- ML-Agents

## Setup

### For training and inference:

* After creating the first new Unity Project, replace the folders under our repository.

* Create a virtual environment using [`Python 3.9.13`](https://www.python.org/downloads/release/python-3913/). If you have an existing python version, you may create virtual environment with specified python version [here](https://stackoverflow.com/questions/1534210/use-different-python-version-with-virtualenv).

  * ```bash
    python -m venv "venv"
    venv/scripts/activate
    ```

  * Double check your environment with the correct version

  * ```
    python --version

* Now that the python version is correctly installed, we may get these following pip requirements

  * ```bash
    pip3 install mlagents
    pip3 install torch torchvision torchaudio
    ```

  * (Optional) You may choose to use GPU for inference as well, please check [Pytorch CUDA](https://pytorch.org/get-started/locally/) for your configuration. 
    ```bash
    pip3 install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu118
    ```

  * ```bash
    pip3 install protobuf==3.20.3
    ```

  * Once all above is installed, you should be able to get a prompt with the following:
    ```bash
    mlagents-learn -h
    ```

  * ```bash
    pip3 install onnx
    ```

### For inference only:

* After creating the first new Unity Project, replace the folders under our repository.
* Choose one of the brains under our `Assets` folder and drag it into the agent's inspector.

![image-20240429225950598](E:\Github\RL_robot\imgs\img01.png)

* Enter play mode to check with the brain model

## Results

### Curriculum Training - Phase 1: Basic Navigation

### Curriculum Training - Phase 2: Balanced Walking

### Curriculum Training - Phase 3: Complex Navigation with Obstacles

