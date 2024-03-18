using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DesignItemNode : IComparable<DesignItemNode>
{
    public DesignItem Item { get; }
    public Dictionary<DesignItemNode, int> Connections { get; }
    public int FScore { get; private set; }
    public int GScore { get; private set; }

    public DesignItemNode(DesignItem item)
    {
        Item = item;
        Connections = new Dictionary<DesignItemNode, int>();
    }

    public void AddConnection(DesignItemNode node, int weight)
    {
        Connections.Add(node, weight);
    }

    public int CompareTo(DesignItemNode other)
    {
        // Compare nodes based on their fScore (or any other suitable criteria)
        return this.FScore.CompareTo(other.FScore);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            
            return false;
        }
            
        DesignItemNode otherNode = (DesignItemNode)obj;

        if (this.Item.prefabId.Equals(otherNode.Item.prefabId))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return this.Item.prefabId.GetHashCode();
    }

    public void CalculateFScore(DesignItemNode goalNode, int heuristicWeight)
    {
        // Calculate the heuristic value
        int heuristicValue = FurnitureGraph.Heuristic(this, goalNode, heuristicWeight);

        // Calculate the FScore as the sum of the GScore and heuristic value
        FScore = GScore + heuristicValue;
    }

    public void SetGScore(int gScore)
    {
        GScore = gScore;
    }

}


