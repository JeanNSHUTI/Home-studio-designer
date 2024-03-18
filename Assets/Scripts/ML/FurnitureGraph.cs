using System;
using System.Collections.Generic;
using UnityEngine;
using C5;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class FurnitureGraph : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject resultsContainer;
    [SerializeField] private Button recordVoiceButton;
    [SerializeField] private GameObject resultsView;

    [SerializeField] private GameObject _debug;

    private static FurnitureGraph instance;
    private List<DesignItem> furnitureModels;
    private Dictionary<string, DesignItemNode> nodes;

    public static FurnitureGraph Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FurnitureGraph>();
            }
            return instance;
        }
    }

    
    void Start()
    {  

        // Fetch the list of furniture models  
        //furnitureModels = DataHandler.Instance.GetItemsCopy();
        //nodes = new Dictionary<string, DesignItemNode>();
        //BuildGraph();

    }

    void Update()
    {
        
    }

    public void RebuildGraph()
    {
        furnitureModels = DataHandler.Instance.GetItemsCopy();
        nodes = new Dictionary<string, DesignItemNode>();
        BuildGraph();
    }

    private void BuildGraph()
    {
        // Create nodes and add them to the dictionary
        foreach (DesignItem item in furnitureModels)
        {
            DesignItemNode node = new DesignItemNode(item);
            nodes.Add(item.prefabId, node);
        }

        // Create connections between nodes based on attributes and similarity score
        foreach (DesignItemNode node1 in nodes.Values)
        {
            foreach (DesignItemNode node2 in nodes.Values)
            {
                if (node1 != node2)
                {
                    // Calculate similarity score based on type and/or placement tags
                    int similarityScore = CalculateWeight(node1.Item.prefabId, node2.Item.prefabId, 100);
                    if (String.Equals(node1.Item.type, node2.Item.type, StringComparison.OrdinalIgnoreCase))
                    {
                        similarityScore += 3;
                    }
                    if ( String.Equals(node1.Item.placementTags, node2.Item.placementTags, StringComparison.OrdinalIgnoreCase))
                    {
                        similarityScore += 5; 
                    }
                    if (String.Equals(node1.Item.styleTags, node2.Item.styleTags, StringComparison.OrdinalIgnoreCase))
                    {
                        similarityScore += 25;
                    }
                    if (String.Equals(node1.Item.priceRangeTag, node2.Item.priceRangeTag, StringComparison.OrdinalIgnoreCase))
                    {
                        similarityScore += 7;
                    }

                    // Add a connection with the calculated weight
                    node1.AddConnection(node2, similarityScore);
                    //Debug.Log($"Connection: {node1.Item.prefabId} <-> {node2.Item.prefabId}, Score: {similarityScore}");
                    //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += $"Connection: {node1.Item.prefabId} <-> {node2.Item.prefabId}, Score: {similarityScore}";
                }
            }
        }
    }

    // Method to calculate the weight based on styleTags
    public static int CalculateWeight(string x, string y, int multiplier)
    {
        if (x == null || y == null)
        {
            throw new ArgumentException("Strings must not be null");
        }

        int maxLength = Math.Max(x.Length, y.Length);
        if (maxLength > 0)
        {
            double similarity = (maxLength - getEditDistance(x, y)) / (double)maxLength;
            int weight = (int)(similarity * multiplier);
            return weight;
        }
        return multiplier; // If both strings are empty, return the maximum weight
    }

    public static int Heuristic(DesignItemNode currentNode, DesignItemNode targetNode, int heuristicWeight)
    {
        string currentStyleTags = currentNode.Item.prefabId;
        string targetStyleTags = targetNode.Item.prefabId;

        //double normalizedSimilarity = FindSimilarity(currentStyleTags, targetStyleTags); //0-1
        double normalizedSimilarity = CosineSimilarityCalculator.CalculateCosineSimilarity(currentStyleTags, targetStyleTags);
        int heuristicValue = (int)(normalizedSimilarity * heuristicWeight);

        return heuristicValue;
    }

    // Edit distance algorithm thanks to https://www.techiedelight.com/calculate-similarity-between-two-strings-in-csharp/
    public static int getEditDistance(string X, string Y)
    {
        int m = X.Length;
        int n = Y.Length;

        int[][] T = new int[m + 1][];
        for (int i = 0; i < m + 1; ++i)
        {
            T[i] = new int[n + 1];
        }

        for (int i = 1; i <= m; i++)
        {
            T[i][0] = i;
        }
        for (int j = 1; j <= n; j++)
        {
            T[0][j] = j;
        }

        int cost;
        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                cost = X[i - 1] == Y[j - 1] ? 0 : 1;
                T[i][j] = Math.Min(Math.Min(T[i - 1][j] + 1, T[i][j - 1] + 1),
                        T[i - 1][j - 1] + cost);
            }
        }

        return T[m][n];
    }

    public static double FindSimilarity(string x, string y)
    {
        if (x == null || y == null)
        {
            throw new ArgumentException("Strings must not be null");
        }

        double maxLength = Math.Max(x.Length, y.Length);
        if (maxLength > 0)
        {
            // optionally ignore case if needed
            return (maxLength - getEditDistance(x, y)) / maxLength;
        }
        return 1.0;
    }

    private List<DesignItemNode> BFS(DesignItemNode startNode, DesignItemNode goalNode)
    {
        Queue<DesignItemNode> queue = new Queue<DesignItemNode>();
        System.Collections.Generic.HashSet<DesignItemNode> visitedNodes = new System.Collections.Generic.HashSet<DesignItemNode>();
        Dictionary<DesignItemNode, DesignItemNode> cameFrom = new Dictionary<DesignItemNode, DesignItemNode>();

        queue.Enqueue(startNode);
        visitedNodes.Add(startNode);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            if (currentNode == goalNode)
            {
                // Path found, reconstruct and return it
                return ReconstructPath(cameFrom, currentNode);
            }

            foreach (var neighbor in currentNode.Connections.Keys)
            {
                if (!visitedNodes.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visitedNodes.Add(neighbor);
                    cameFrom[neighbor] = currentNode;
                }
            }
        }

        // No path found
        return new List<DesignItemNode>();
    }

    private List<DesignItemNode> AStarSearch(DesignItemNode startNode, DesignItemNode goalNode)
    {
        var openList = new IntervalHeap<DesignItemNode>(); // C5 priority queue
        var cameFrom = new Dictionary<DesignItemNode, DesignItemNode>();
        var gScore = new Dictionary<DesignItemNode, int>();
        var fScore = new Dictionary<DesignItemNode, int>();
        var visitedNodes = new System.Collections.Generic.HashSet<DesignItemNode>();
        int counter = 0;

        gScore[startNode] = 0;
        fScore[startNode] = Heuristic(startNode, goalNode, 100);
        startNode.SetGScore(0); // Set the initial GScore for the start node

        openList.Add(startNode);
        DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().SetText("A*");
        while (!openList.IsEmpty)
        {
            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nOpen list Contents1: " + string.Join(", ", openList.ToArray().Select(node => node.Item.prefabId));
            var current = openList.DeleteMin();
            visitedNodes.Add(current);

            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nCurrent Node: " + current.Item.prefabId;

            if (current == goalNode)
            {
                DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().SetText("Goal reached !");
                return ReconstructPath(cameFrom, current);
            }

            foreach (var neighbor in current.Connections.Keys)
            {
                var tentativeGScore = gScore[current] + current.Connections[neighbor];

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goalNode, 1);
                    neighbor.SetGScore(tentativeGScore);
                    neighbor.CalculateFScore(goalNode, 1);

                    counter++;
                    //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nApplied Heuristics: " + counter + "Node: " + neighbor.Item.prefabId;
                    //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nVisited: " + neighbor.Item.prefabId;
                    //!
                    bool check = visitedNodes.Contains(neighbor);
                    //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nCheck Nighbour node: " + neighbor.Item.prefabId + "bool: " + check.ToString();
                    if (!visitedNodes.Contains(neighbor))
                    {
                        if (!openList.Contains(neighbor))
                        {
                            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nAdding to open: " + neighbor.Item.prefabId;
                            openList.Add(neighbor);
                            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nAdded: " + neighbor.Item.prefabId + "OL Size: " + openList.Count; 
                            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nOpen list Size2: " + openList.Count;
                        }
                    }

                    
                }
                //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nA* foreach visited count: " + visitedNodes.Count;
            }
        }

        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nA* finished. Visited node count:  " + visitedNodes.Count;
        return visitedNodes.Count > 0 ? visitedNodes.ToList() : Enumerable.Empty<DesignItemNode>().ToList(); // No path found
    }

    private List<DesignItemNode> ReconstructPath(Dictionary<DesignItemNode, DesignItemNode> cameFrom, DesignItemNode current)
    {
        var path = new List<DesignItemNode>
        {
            current
        };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        DisplayPathResults(path);

        return path;
    }

    private void DisplayPathResults(List<DesignItemNode> path)
    {
        int counter = 0;
        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nEnteredDIsplay: " + path.Count;

        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nActivating scroll view: " + resultsView.name;
        bool active = resultsView.activeInHierarchy;

        resultsView.GetComponent<ResultsViewManager>().CleanContents();

        if (!active)
        {
            //GameObject.FindGameObjectWithTag("SearchResultsView").SetActive(true);
            resultsView.SetActive(true);
            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nActivated scroll view: " + resultsView.name;
        }

        foreach (DesignItemNode node in path)
        {
            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nPrinting: " + node.Item.prefabId;
            GameObject b = Instantiate(buttonPrefab, resultsContainer.transform);
            b.GetComponent<ButtonManager>().PrefabId = node.Item.prefabId;
            b.GetComponentInChildren<TextMeshProUGUI>().SetText(node.Item.prefabId);
            b.GetComponent<ButtonManager>().ButtonTexture = node.Item.designItemImage;
            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nPrinted: " + node.Item.prefabId;
            counter++;
        }


        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nAdded buttons: " + counter;
        // Call MarkLayoutForRebuild to trigger layout recalculation
        LayoutRebuilder.MarkLayoutForRebuild(resultsContainer.GetComponent<RectTransform>());

        // Call SetLayoutVertical to manually update the layout
        resultsContainer.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate(resultsContainer.GetComponent<RectTransform>());
        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nCalled refresh ";

        //resultsContainer.GetComponent<MyContentFitter>().FitVertical();
    }

    public void OnSearchInputButtonClicked()
    {
        string inputText = GameObject.FindWithTag("SearchInput").transform.GetComponent<TMP_InputField>().text;

        // Select start node based on keywords & prefs
        DesignItemNode startNode = SelectStartNodeBasedOnKeywords(inputText, UserDataHandler.Instance.UserData);

        // Fallback or default start node
        if (startNode == null)
        {
            // Default random start node
            startNode = GetFallbackStartNode();
            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nFallback-Sid: " + startNode.Item.prefabId;
        }

        // Select target node based on keywords & prefs
        DesignItemNode targetNode = SelectTargetNodeBasedOnKeywords(inputText, UserDataHandler.Instance.UserData);

        // Default target node based on edit distance between input and furniture description
        if (targetNode == null)
        {
            // Choose a default or random target node
            targetNode = GetFallbackTargetNode(inputText);
            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nFallback-Gid: " + targetNode.Item.prefabId;
        }

        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nGid: " + targetNode.Item.prefabId;
        //List<DesignItemNode> searchResults = BFS(startNode, targetNode);
        List<DesignItemNode> searchResults = AStarSearch(startNode, targetNode);
        /*DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nSecondary Results Count: " + searchResults.Count;

        if (!GameObject.FindWithTag("SearchResultsView").activeSelf && searchResults.Count > 0)
        {
            GameObject.FindWithTag("SearchResultsView").SetActive(true);
            DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nSecondary display..";
            DisplayPathResults(searchResults);
        }*/
        //DisplayPathResults(searchResults);


    }

    private DesignItemNode GetFallbackStartNode()
    {
        // Choose a random start node from the list of nodes
        if (nodes != null && nodes.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, nodes.Count);
            return nodes.ElementAt(randomIndex).Value;
        }
        else
        {
            // Return null if nodes list is empty
            DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().SetText("\nError fallback start node: list of nodes empty");
            return null; 
        }
        
    }

    private DesignItemNode GetFallbackTargetNode(string searchInput)
    {
        // Calculate similarity scores for each node's description
        Dictionary<DesignItemNode, double> similarityScores = new Dictionary<DesignItemNode, double>();
        foreach (DesignItemNode node in nodes.Values)
        {
            double similarity = CalculateNodeDescriptionSimilarity(searchInput, node.Item.prefabId);
            similarityScores[node] = similarity;
        }

        // Find the node with the highest similarity score
        DesignItemNode mostSimilarNode = similarityScores.OrderByDescending(pair => pair.Value).FirstOrDefault().Key;
        return mostSimilarNode;
    }

    private double CalculateNodeDescriptionSimilarity(string input, string description)
    {
        //return FindSimilarity(input, description);
        return CosineSimilarityCalculator.CalculateCosineSimilarity(input, description);
    }

    public DesignItemNode SelectStartNodeBasedOnKeywords(string searchInput, UserData userPrefs)
    {
        string[] keywords = searchInput.Split(' ');
        int maxScore = 0;
        int loopCounter = 0;
        int loopCounter2 = 0;
        DesignItemNode bestStartNode = null;

        /*DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().SetText("Node count: "
                                                                            + nodes.Count
                                                                            + "KW count: "
                                                                            + keywords.Length);*/

        foreach (var keyword in keywords)
        {
            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nEntered First Foreach";
            if (nodes.Count > 0)
            {
                foreach (var node in nodes.Values)
                {
                    //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nEntered Second Foreach";
                    loopCounter++;
                    int keywordScore = CalculateKeywordScore(keyword, node.Item);
                    int combinedScore = keywordScore + CalculateNodeScore(node.Item, userPrefs);
                    //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().SetText("Current Max Score: " + maxScore);
                    //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nCombinedScore: "+combinedScore;

                    if (combinedScore > maxScore)
                    {
                        loopCounter2++;
                        DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nNew Max Score: " + maxScore;
                        // DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().SetText("New max score: " + combinedScore);
                        maxScore = combinedScore;
                        bestStartNode = node;
                    }
                }
            }
            
            //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().SetText("Current Max Score: " + maxScore);
        }
        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().SetText("Final Max Score: " + maxScore);
        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().SetText("C1: " + loopCounter + "C2: " + loopCounter2);
        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text = "\nC1: " + loopCounter + "C2: " + loopCounter2 + "Sid: " + bestStartNode.Item.prefabId;
        return bestStartNode;
    }

    private int CalculateKeywordScore(string keyword, DesignItem item)
    {
        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nKW calc";
        // Check how well the keyword matches the attributes in the item
        int score = 0;
        if (keyword.Equals(item.type, StringComparison.OrdinalIgnoreCase))
        {
            score += 5;
        }
        if (item.styleTags.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            score += 3; // You can also use similarity scores here
        }
        if (item.placementTags.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            score += 2;
        }
        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nKW calc";
        return score;
    }

    private int CalculateNodeScore(DesignItem item, UserData userPrefs)
    {
        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nNodeScore calc";
        // Calculate a score based on how well the node matches the user preferences
        int score = 0;
        int maxStyleTagsToCheck = Math.Min(userPrefs.UserStylePrefs.Count, item.styleTags.Length);
        int maxPlacementTagsToCheck = Math.Min(userPrefs.UserPlacementPrefs.Count, item.placementTags.Length);

        // Add score calculations based on user preferences, similar to the similarity scoring
        if (item.priceRangeTag.Equals(userPrefs.UserPriceRangePref, StringComparison.OrdinalIgnoreCase))
        {
            score += 3;
        }

        if (userPrefs.UserStylePrefs.Any(stylePref => item.styleTags.Contains(stylePref, StringComparison.OrdinalIgnoreCase)))
        {
            score += 10;
        }

        if (userPrefs.UserPlacementPrefs.Any(placementPref => item.placementTags.Contains(placementPref, StringComparison.OrdinalIgnoreCase)))
        {
            score += 7;
        }


        //DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nNodeScore calc";
        return score;
    }

    public DesignItemNode SelectTargetNodeBasedOnKeywords(string searchInput, UserData userPrefs)
    {
        string[] keywords = searchInput.Split(' ');

        return nodes.Values
            .Where(node => keywords.Any(keyword => node.Item.prefabId.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(node => CalculateNodeScore(node.Item, userPrefs))
            .FirstOrDefault();
    }


}
