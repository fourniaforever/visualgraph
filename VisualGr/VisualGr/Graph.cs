using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace grafy
{  
    public class Pair<T1, T2>
        {
            public T1 Key;
            public T2 Value;

            public Pair(T1 Key, T2 Value)
            {
                this.Key = Key;
                this.Value = Value;
            }
        }

    class Graph
    {
      
        public SortedList<object, SortedList<object, int>> Graf;
        public bool IsDirected { get; set; }

        public Graph()
        {
            Graf = new SortedList<object, SortedList<object, int>>();
            IsDirected = true;
        }

        public Graph(Graph GraphToCopy)
        {
            Graf = new SortedList<object, SortedList<object, int>>(GraphToCopy.Graf);
            IsDirected = GraphToCopy.IsDirected;
        }

        public Graph(string path)
        {
            Graf = new SortedList<object, SortedList<object, int>>();

            using (StreamReader input = new StreamReader(path))
            {
                while (!input.EndOfStream && input.Peek() != '[')
                {
                    string name = input.ReadLine().Replace(":", "");
                    Graf.Add(name, new SortedList<object, int>());

                    while (!input.EndOfStream && input.Peek() != ':' && input.Peek() != '[')
                    {
                        string[] thing = input.ReadLine().Split(' ');
                        var temp = thing[0].Split('/').OrderBy(x => x);
                        if (temp.Count() > 1)
                        {
                            string res = string.Join("/", temp);
                            Graf[name].Add(res, int.Parse(thing[1]));
                        }
                        else
                        {
                            Graf[name].Add(thing[0], int.Parse(thing[1]));
                        }


                    }
                }

                string grType = input.ReadLine().Replace("[", "").Replace("]", "");

                if (grType == "True")
                {
                    IsDirected = true;
                    Checking();
                }
                else if (grType == "False")
                {
                    IsDirected = false;
                    this.ConvertToUndirected();
                }
                else
                {
                    throw new Exception("The type of graph is not inputted correctly.");
                }
            }
        }

        private void Checking()
        {
            foreach (var node in Graf)
            {
                foreach (var connect in node.Value)
                {
                    if (!Graf.ContainsKey(connect.Key))
                    {
                        throw new Exception("The tree is not correct. One or multiple of the referenced nodes are missing.");
                    }
                }
            }
        }

        public void ConvertToUndirected()
        {
            Checking();

            var nodeNames = Graf.Keys;

            foreach (var node in nodeNames)
            {
                var ConnectNames = Graf[node].Keys.ToArray<object>();

                foreach (object connect in ConnectNames)
                {
                    if (!Graf[connect].ContainsKey(node))
                    {
                        this.AddVertexWeight(connect, node, Graf[node][connect]);
                    }
                    else if (Graf[connect][node] != Graf[node][connect])
                    {
                        Graf[connect][node] = Graf[node][connect];
                    }
                }
            }

            IsDirected = false;
        }

        public void ConvertToDirected()
        {
            IsDirected = true;
        }
        public void AddVertex(object Name)
        {

            SortedList<object, int> temp = new SortedList<object, int>();
            var temp1 = Name.ToString().Split('/').OrderBy(x => x);
            if (temp1.Count() > 1)
            {
                string res = string.Join("/", temp1);
                if (Graf.ContainsKey(res))
                {
                    throw new Exception("This node already exists.");
                }

                Graf.Add(res, temp);
            }
            else
            {
                if (Graf.ContainsKey(Name))
                {
                    throw new Exception("This node already exists.");
                }
                Graf.Add(Name, temp);
            }
        }
        public void AddVertex(object VertexName, object VertexConnect)
        {
            var temp1 = VertexName.ToString().Split('/').OrderBy(x => x);
            if (temp1.Count() > 1)
            {
                string res = string.Join("/", temp1);
                if (Graf.ContainsKey(res))
                {
                    throw new Exception("This node already exists.");
                }

                Graf[VertexName].Add(res, 1);
            }
            else
            {
                if (Graf.ContainsKey(VertexName))
                {
                    throw new Exception("This node already exists.");
                }
                Graf[VertexName].Add(VertexConnect, 1);
            }

            if (!IsDirected)
            {
                try
                {
                    if (temp1.Count() > 1)
                    {
                        string res = string.Join("/", temp1);
                        if (Graf.ContainsKey(res))
                        {
                            throw new Exception("This node already exists.");
                        }

                        Graf[VertexName].Add(res, 1);
                    }
                    else
                    {
                        if (Graf.ContainsKey(VertexName))
                        {
                            throw new Exception("This node already exists.");
                        }
                        Graf[VertexName].Add(VertexConnect, 1);
                    }

                }
                catch (System.ArgumentException)
                {
                    Graf[VertexConnect][VertexName] = 1;
                }
            }
        }
        public void AddVertexWeight(object VertexName, object VertexConnect, int Weight)
        {
            Graf[VertexName].Add(VertexConnect, 1);

            if (!IsDirected)
            {
                try
                {
                    Graf[VertexConnect].Add(VertexName, Weight);
                }
                catch (System.ArgumentException)
                {
                    Graf[VertexConnect][VertexName] = Weight;
                }
            }
        }
        public void AddEdge(object VertexName, object[] VertexConnect)
        {
            if (Graf.ContainsKey(VertexName))
            {
                throw new Exception("This node already exists. Use 'ReplaceNode' function to edit it.");
            }

            SortedList<object, int> temp = new SortedList<object, int>();
            for (int i = 0; i < VertexConnect.Length; i++)
            {
                temp.Add(VertexConnect[i], 1);

                if (!IsDirected)
                {
                    try
                    {
                        Graf[VertexConnect[i]].Add(VertexName, 1);
                    }
                    catch (System.ArgumentException)
                    {
                        Graf[VertexConnect[i]][VertexName] = 1;
                    }
                }
            }

            Graf.Add(VertexName, temp);
        }
        public void AddJoint(object NodeName, object NodeToConnect, int Weight)
        {
            if (!Graf.ContainsKey(NodeName) && !Graf.ContainsKey(NodeToConnect))
            {
                throw new Exception("Referenced node does not exist.");
            }
            else if (Graf[NodeName].ContainsKey(NodeToConnect))
            {
                throw new Exception("This connection already exists.");
            }
            Graf[NodeName].Add(NodeToConnect, Weight);

            if (!IsDirected)
            {
                try
                {
                    Graf[NodeToConnect].Add(NodeName, Weight);
                }
                catch (System.ArgumentException)
                {
                    Graf[NodeToConnect][NodeName] = Weight;
                }
            }
        }
        public void AddEdgeWeight(object VertexName, object[] VertexConnect, int[] weights)
        {
            SortedList<object, int> temp = new SortedList<object, int>();
            for (int i = 0; i < VertexConnect.Length; i++)
            {
                temp.Add(VertexConnect[i], weights[i]);

                if (!IsDirected)
                {
                    try
                    {
                        Graf[VertexConnect[i]].Add(VertexName, weights[i]);
                    }
                    catch (System.ArgumentException)
                    {
                        Graf[VertexConnect[i]][VertexName] = weights[i];
                    }
                }
            }

            Graf.Add(VertexName, temp);
        }
        public void DeleteVertex(object name, object VertDiscon)
        {
            Graf[name].Remove(VertDiscon);

            if (!IsDirected)
            {
                Graf[VertDiscon].Remove(name);
            }
        }
        public void DeleteEdge(object name)
        {
            Graf.Remove(name);
            foreach (var item in Graf)
            {
                this.DeleteVertex(item.Key, name);
            }
        }

        public SortedList<object, int> GetOutwardNeighbours(object p)
        {
            bool check = false;
            foreach (var item in Graf)
            {
                if (Equals(p, item.Key))
                {
                    p = item.Key;
                    check = true;
                    break;
                }
            }

            if (!check)
            {
                throw new Exception("Referenced node does not exist.");
            }

            return new SortedList<object, int>(Graf[p]);
        }
        public SortedList<object, int> GetInwardNeighbours(object Vertex)
        {
            if (Vertex == null)
            {
                throw new Exception("Vertex references 'null' value.");
            }

            if (!IsDirected)
            {
                return GetOutwardNeighbours(Vertex);
            }

            SortedList<object, int> temp = new SortedList<object, int>();
            foreach (var node in Graf)
            {
                if (node.Value.ContainsKey(Vertex))
                {
                    temp.Add(node.Key, node.Value[Vertex]);
                }
            }

            return temp;
        }

        public SortedList<object, SortedList<object, int>> GetNodes()
        {
            return new SortedList<object, SortedList<object, int>>(Graf);
        }
        public void Merger(Graph gr1, Graph gr2)
        {
            var NodeList1 = gr1.GetNodes();
            var Nodelist2 = gr2.GetNodes();

            foreach (var node in NodeList1)
            {
                object temp = null;

                foreach (var node2 in Nodelist2)
                {
                    if (Equals(node.Key, node2.Key))
                    {
                        temp = node2.Key;
                        break;
                    }
                }

                if (temp == null)
                {
                    gr2.AddVertex(node.Key);
                    Nodelist2 = gr2.GetNodes();
                    temp = node.Key;
                }

                foreach (var joint in node.Value)
                {
                    bool allGood = false;

                    foreach (var joint2 in Nodelist2[temp])
                    {
                        if (Equals(joint.Key, joint2.Key))
                        {
                            allGood = true;
                            break;
                        }
                    }

                    if (!allGood)
                    {
                        gr2.AddJoint(temp, joint.Key, joint.Value);
                        Nodelist2 = gr2.GetNodes();
                    }
                }
            }
            gr2.PrintToFile("output2.txt");
        }
        public List<object> GetNodeNames()
        {
            return new List<object>(Graf.Keys.ToList<object>());
        }

        public void DFS(object vertex)
        {
            var visited = new HashSet<object>();
            Traverse(vertex, visited);
        }

        public List<Pair<int, Pair<object, object>>> GetJoints()
        {
            List<Pair<int, Pair<object, object>>> temp = new List<Pair<int, Pair<object, object>>>();

            foreach (var node in Graf)
            {
                foreach (var child in node.Value)
                {
                    var temp2 = new Pair<int, Pair<object, object>>(child.Value, new Pair<object, object>(node.Key, child.Key));

                    if (!IsDirected)
                    {
                        if (!temp.Any(x => (Object.Equals(x.Value.Key, temp2.Value.Value) && Object.Equals(x.Value.Value, temp2.Value.Key))))
                        {
                            temp.Add(temp2);
                        }
                    }
                    else
                    {
                        temp.Add(temp2);
                    }
                }
            }

            return temp;
        }
        private void Traverse(object v, HashSet<object> visited)
        {
            visited.Add(v);
            Console.WriteLine(v);
            if (Graf.ContainsKey(v.ToString()))
            {
                foreach (var neighbour in Graf[v].Where(a => !visited.Contains(a)))
                {
                    Traverse(neighbour, visited);
                }
            }
        }
        //проверка на "образования дерева"
        //если 0 у всех,то дерево образованно
        private bool CheckNumber(SortedList<object, int> subTrees)
        {
            foreach (var item in subTrees)
            {
                if (item.Value != 0) return false;
            }

            return true;
        }
        public Graph BuildSpanningTree()
        {
            SortedList<object, int> subTrees = new SortedList<object, int>();
            var temp = GetNodeNames();
            //построение дерева,где каждая компонент связонности находится в разных поддеревьях(0-6)
            for (int i = 0; i < temp.Count; i++)
            {
                subTrees.Add(temp[i], i);
            }

            Graph SpanningTree = new Graph();
            if (!IsDirected) SpanningTree.ConvertToDirected();

            //пока 
            while (!CheckNumber(subTrees))
            {
                foreach (var node in Graf)
                {
                    KeyValuePair<object, int> joint;
                    try
                    {
                        joint = node.Value.ToArray().OrderBy(x => x.Value).First();
                    }
                    catch
                    {
                        continue;
                    }

                    try { SpanningTree.AddVertex(node.Key); }
                    catch { }
                    try { SpanningTree.AddVertex(joint.Key); }
                    catch { }
                    try { SpanningTree.AddJoint(node.Key, joint.Key, joint.Value); }
                    catch { }
                    DeleteVertex(node.Key, joint.Key);
                    if (subTrees[joint.Key] > subTrees[node.Key])
                    {
                        int t = subTrees[joint.Key];
                        foreach (object n in GetNodeNames())
                        {
                            if (subTrees[n] == t)
                            { subTrees[n] = subTrees[node.Key]; }
                        }
                    }
                    else
                    {

                        int t = subTrees[node.Key];

                        foreach (object n in GetNodeNames())
                        {
                            if (subTrees[n] == t)
                            { subTrees[n] = subTrees[joint.Key]; }
                        }
                    }
                }
            }
            return SpanningTree;
        }
        public List<int> GetListOfWeights()
        {
            List<int> Weights = new List<int>();
            foreach (var component in Graf.Values)
            {
                foreach (var v in component)
                {
                    Weights.Add(v.Value);
                }
            }
            return Weights;
        }
        public List<Stack<object>> FindPaths(object node, object targetNode)
        {
            Stack<object> connectionPath = new Stack<object>();
            connectionPath.Push(node);

            List<Stack<object>> connectionPaths = new List<Stack<object>>();

            CheckPaths(node, targetNode, connectionPath, connectionPaths);

            return connectionPaths;
        }
        public SortedList<object, int> FindPathLengthsBFS(object node)
        {
            bool check = false;
            foreach (var item in Graf)
            {
                if (Equals(node, item.Key))
                {
                    node = item.Key;
                    check = true;
                    break;
                }
            }

            if (!check)
            {
                throw new Exception("Referenced node does not exist.");
            }

            SortedList<object, int> d = new SortedList<object, int>();
            Queue<object> q = new Queue<object>();

            foreach (var item in Graf)
            {
                d.Add(item.Key, int.MaxValue);
            }

            d[node] = 0;

            q.Enqueue(node);

            while (q.Count != 0)
            {
                object u = q.Dequeue();

                foreach (var joint in Graf[u])
                {
                    if (d[joint.Key] == int.MaxValue)
                    {
                        d[joint.Key] = d[u] + 1;
                        q.Enqueue(joint.Key);
                    }
                }
            }

            return d;
        }
        private void CheckPaths(object currentNode, object targetNode, Stack<object> connectionPath, List<Stack<object>> connectionPaths)
        {
            foreach (var nextNode in GetOutwardNeighbours(currentNode))
            {
                if (Object.Equals(nextNode.Key, targetNode))
                {
                    Stack<object> temp = new Stack<object>();
                    temp.Push(targetNode);
                    foreach (var node1 in connectionPath)
                    {
                        temp.Push(node1);
                    }
                    connectionPaths.Add(temp);
                }
                else if (!connectionPath.Contains(nextNode.Key))
                {
                    connectionPath.Push(nextNode.Key);
                    CheckPaths(nextNode.Key, targetNode, connectionPath, connectionPaths);
                    connectionPath.Pop();
                }
            }
        }
        public SortedList<string, int> DoDijkstra(object StartingNode)
        { int sum = 0;
            SortedList<string, int> pathLength = new SortedList<string, int>();
            SortedList<string, bool> nodeVisited = new SortedList<string, bool>();
            var temp = this.GetNodeNames();
            int n = Graf.Count;

            for (int i = 0; i < n; i++)
            {
                pathLength.Add(temp[i].ToString(), int.MaxValue);
                nodeVisited.Add(temp[i].ToString(), false);
            }
            pathLength[StartingNode.ToString()] = 0;
            string v = StartingNode.ToString();


            for (int i = 0; i < n; i++)
            {
                int m = int.MaxValue;

                foreach (var node in Graf.Keys.ToArray())
                {

                    if ((pathLength[node.ToString()] < m) && (!nodeVisited[node.ToString()]))
                    {

                        m = pathLength[node.ToString()];
                        v = node.ToString();

                    }

                }

                nodeVisited[v] = true;

                foreach (object node in Graf.Keys.ToArray())
                {
                    int weight;

                    try
                    {
                        weight = Graf[v][node];
                        if (!nodeVisited[node.ToString()] && ((pathLength[v] + weight) < pathLength[node.ToString()]))
                        {
                            pathLength[node.ToString()] = pathLength[v] + weight;
                        }

                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return pathLength;
        }
        public void PrintToFile(string OutputFilePath)
        {
            using (StreamWriter output = new StreamWriter(OutputFilePath))
            {
                foreach (var node in Graf)
                {
                    output.WriteLine(node.Key.ToString() + ":");

                    foreach (var edges in node.Value)
                    {
                        output.WriteLine(edges.Key.ToString() + " " + edges.Value.ToString());
                    }
                    output.WriteLine();
                }

                output.Write(IsDirected.ToString());
            }
        }
        public void DoFloyd()
        {
            SortedList<object, int> pathLength = new SortedList<object, int>();

            var temp = GetNodeNames();
            for (int i = 0; i < temp.Count; i++)
            {
                pathLength.Add(temp[i], i);
            }

            int[,] mas = new int[pathLength.Count, pathLength.Count];

            var temp1 = GetJoints();

            for (int i = 0; i < pathLength.Count; i++)
            {
                for (int j = 0; j < pathLength.Count; j++)
                {
                    mas[i, j] = int.MaxValue;
                }
            }

            foreach (var item in temp1)
            {
                mas[pathLength[item.Value.Key], pathLength[item.Value.Value]] = item.Key;
            }

            for (int k = 0; k < pathLength.Count; k++)
            {
                for (int i = 0; i < pathLength.Count; i++)
                {
                    for (int j = 0; j < pathLength.Count; j++)
                    {
                        if (mas[i, k] < int.MaxValue && mas[k, j] < int.MaxValue)
                        {
                            mas[i, j] = Math.Min(mas[i, j], mas[i, k] + mas[k, j]);
                        }
                    }
                }
            }
        }

        public SortedList<object, int> DoBellman(object startingNode)
        {
            var joints = GetJoints();
            int n = Graf.Count - 1;
            SortedList<object, int> pathLength = new SortedList<object, int>();

            foreach (object node in GetNodeNames())
            {
                pathLength.Add(node, int.MaxValue);
            }

            pathLength[startingNode.ToString()] = 0;

            for (int i = 0; i < n; i++)
            {
                foreach (var joint in joints)
                {
                    if (pathLength[joint.Value.Key] < int.MaxValue)
                    {
                        pathLength[joint.Value.Value] = Math.Min(pathLength[joint.Value.Value], pathLength[joint.Value.Key] + joint.Key);
                    }
                }
            }

            return pathLength;
        }

  
        private static bool Bfsprivate(Graph graph, object source, out SortedList<object, Pair<int, Pair<object, object>>> parent, object sink, List<Pair<int, Pair<object, object>>> edges)
        {
            parent = new SortedList<object, Pair<int, Pair<object, object>>>();

            foreach (var node in graph.Graf)
            {
                parent.Add(node.Key, null);
            }

            Queue<object> q = new Queue<object>();
            q.Enqueue(source);

            while (q.Count != 0)
            {
                object curr = q.Dequeue();

                foreach (var edge in edges)
                {
                    if ((Equals(edge.Value.Key, curr) || Equals(edge.Value.Value, curr)) && (parent[edge.Value.Value] == null) && !Equals(edge.Value.Value, source) && (edge.Key > 0))
                    {
                        parent[edge.Value.Value] = edge;
                        q.Enqueue(edge.Value.Value);
                    }
                }
            }
            if (parent[sink] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static int DoEdmondsKarp(Graph graph, object source, object sink)
        {
            bool check = false;
            foreach (var item in graph.Graf)
            {
                if (Equals(source, item.Key))
                {
                    source = item.Key;
                    check = true;
                    break;
                }
            }

            if (!check)
            {
                throw new Exception("Referenced source node does not exist.");
            }

            check = false;

            foreach (var item in graph.Graf)
            {
                if (Equals(sink, item.Key))
                {
                    sink = item.Key;
                    check = true;
                    break;
                }
            }

            if (!check)
            {
                throw new Exception("Referenced sink node does not exist.");
            }

            var edges = graph.GetJoints();
            int t = edges.Count;

            if (graph.IsDirected)
            {
                for (int i = 0; i < t; i++)
                {
                    if (!edges.Exists(x => Equals(x.Value.Key, edges[i].Value.Value) && Equals(x.Value.Value, edges[i].Value.Key)))
                    {
                        edges.Add(new Pair<int, Pair<object, object>>(0, new Pair<object, object>(edges[i].Value.Value, edges[i].Value.Key)));
                    }
                }
            }
            else
            {
                for (int i = 0; i < t; i++)
                {
                    edges.Add(new Pair<int, Pair<object, object>>(edges[i].Key, new Pair<object, object>(edges[i].Value.Value, edges[i].Value.Key)));
                }
            }

            int maxFlow = 0;

            SortedList<object, Pair<int, Pair<object, object>>> parent = new SortedList<object, Pair<int, Pair<object, object>>>();
            while (Bfsprivate(graph, source, out parent, sink, edges))
            {
                int pushFlow = int.MaxValue;

                for (Pair<int, Pair<object, object>> edge = parent[sink]; edge != null; edge = parent[edge.Value.Key])
                {
                    pushFlow = Math.Min(pushFlow, edge.Key);
                }

                for (Pair<int, Pair<object, object>> edge = parent[sink]; edge != null; edge = parent[edge.Value.Key])
                {
                    edge.Key -= pushFlow;
                    edges.First(x => Equals(x.Value.Key, edge.Value.Value) && Equals(x.Value.Value, edge.Value.Key)).Key += pushFlow;
                }

                maxFlow += pushFlow;
            }

            return maxFlow;
        }
        public int[,] GetMas(out SortedList<int, object> references)
        {
            references = new SortedList<int, object>();
            var revRefs = new SortedList<object, int>();

            int n = Graf.Count + 1;
            int[,] mas = new int[n, n];

            var temp = GetNodeNames();
            for (int i = 0; i < temp.Count; i++)
            {
                references.Add(i, temp[i]);
                revRefs.Add(temp[i], i);
            }

            foreach (var node1 in Graf)
            {
                foreach (var node2 in Graf)
                {
                    if (node1.Value.ContainsKey(node2.Key))
                    {
                        mas[revRefs[node1.Key] + 1, revRefs[node2.Key] + 1] = Graf[node1.Key][node2.Key];
                    }
                    else
                    {
                        mas[revRefs[node1.Key] + 1, revRefs[node2.Key] + 1] = int.MaxValue;
                    }
                }
            }

            return mas;
        }
       
    }
}

