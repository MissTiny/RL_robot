# Test01
Reward Function:
    Reward 3: Whenever move forward, reward the moving distance. 
              Whenever move backward, don't add any reward.
              Disregard the closest point.
     float distanceToGoal = Vector3.Distance(CalculateCentroid(), goal.position);
     float distReward = latestDistance - distanceToGoal;
     latestDistance = distanceToGoal;
     AddReward(distReward);

States:
    Successfully touch the goal.