using grafy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisualGraph
{
    public partial class MainForm : Form
    {
        Graph Graph;
        List<Pair<Pair<int, Pair<object, object>>, bool>> Joints;
        SortedList<object, SortedList<object, int>> Nodes;

        string Begin = "";
        string End = "";

        string CurrentImage = "";

        public MainForm()
        {
            InitializeComponent();
            NextStepButton.Enabled = false;

            richTextBox1.Text = @" ";
        }

        private void SetImage()
        {
            CurrentImage = "";

            foreach (var joint in Joints)
            {
                if (joint.Value)
                {
                    if (Graph.IsDirected)
                    {
                        CurrentImage += joint.Key.Value.Key.ToString() + "->" + joint.Key.Value.Value.ToString() + " [label=" + joint.Key.Key.ToString() + ",color=red];" + Environment.NewLine;
                    }
                    else if (!CurrentImage.Contains(joint.Key.Value.Key.ToString() + "--" + joint.Key.Value.Value.ToString()))
                    {
                        CurrentImage += joint.Key.Value.Key.ToString() + "--" + joint.Key.Value.Value.ToString() + " [label=" + joint.Key.Key.ToString() + ",color=red];" + Environment.NewLine;
                    }
                }
                else
                {
                    if (Graph.IsDirected)
                    {
                        CurrentImage += joint.Key.Value.Key.ToString() + "->" + joint.Key.Value.Value.ToString() + " [label=" + joint.Key.Key.ToString() + "];" + Environment.NewLine;
                    }
                    else if (!CurrentImage.Contains(joint.Key.Value.Key.ToString() + "--" + joint.Key.Value.Value.ToString()))
                    {
                        CurrentImage += joint.Key.Value.Key.ToString() + "--" + joint.Key.Value.Value.ToString() + " [label=" + joint.Key.Key.ToString() + "];" + Environment.NewLine;
                    }
                }
            }
        }

        //public async Task<int> DoEdmondsKarpAsync(object source, object sink)
        //{
        //    richTextBox1.Text = "Начало алгоритма" + Environment.NewLine;

        //    await Task.Delay(2000);

        //    var nodes = Graph.GetNodes();

        //    bool check = false;
        //    foreach (var item in nodes)
        //    {
        //        if (Object.Equals(source, item.Key))
        //        {
        //            source = item.Key;
        //            check = true;
        //            break;
        //        }
        //    }

        //    if (!check)
        //    {
        //        throw new Exception("Referenced source node does not exist.");
        //    }

        //    check = false;

        //    foreach (var item in nodes)
        //    {
        //        if (Object.Equals(sink, item.Key))
        //        {
        //            sink = item.Key;
        //            check = true;
        //            break;
        //        }
        //    }

        //    if (!check)
        //    {
        //        throw new Exception("Referenced sink node does not exist.");
        //    }

        //    var edges = Graph.GetJoints();
        //    int t = edges.Count;

        //    if (Graph.IsDirected)
        //    {
        //        for (int i = 0; i < t; i++)
        //        {
        //            if (!edges.Exists(x => Object.Equals(x.Value.Key, edges[i].Value.Value) && Object.Equals(x.Value.Value, edges[i].Value.Key)))
        //            {
        //                edges.Add(new Pair<int, Pair<object, object>>(0, new Pair<object, object>(edges[i].Value.Value, edges[i].Value.Key)));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < t; i++)
        //        {
        //            edges.Add(new Pair<int, Pair<object, object>>(edges[i].Key, new Pair<object, object>(edges[i].Value.Value, edges[i].Value.Key)));
        //            //Joints.Add(new Pair<Pair<int, Pair<object, object>>, bool>(new Pair<int, Pair<object, object>>(edges[i].Key, new Pair<object, object>(edges[i].Value.Value, edges[i].Value.Key)), false));
        //        }
        //    }

        //    int maxFlow = 0;

        //    while (true)
        //    {
        //        Dictionary<object, Pair<int, Pair<object, object>>> parent = new Dictionary<object, Pair<int, Pair<object, object>>>();
        //        foreach (var node in nodes)
        //        {
        //            parent.Add(node.Key, null);
        //        }

        //        Queue<object> q = new Queue<object>();
        //        q.Enqueue(source);

        //        while (q.Count != 0)
        //        {
        //            object curr = q.Dequeue();

        //            foreach (var edge in edges)
        //            {
        //                if ((Object.Equals(edge.Value.Key, curr) || Object.Equals(edge.Value.Value, curr)) && (parent[edge.Value.Value] == null) && !Object.Equals(edge.Value.Value, source) && (edge.Key > 0))
        //                {
        //                    parent[edge.Value.Value] = edge;
        //                    q.Enqueue(edge.Value.Value);
        //                }
        //            }
        //        }

        //        if (parent[sink] == null)
        //        {
        //            break;
        //        }

        //        int pushFlow = int.MaxValue;

        //        Pair<Pair<int, Pair<object, object>>, bool> tem1;
        //        Pair<Pair<int, Pair<object, object>>, bool> tem2;

                
        //        for (Pair<int, Pair<object, object>> edge = parent[sink]; edge != null; edge = parent[edge.Value.Key])
        //        {
        //            pushFlow = Math.Min(pushFlow, edge.Key);

        //            try
        //            {
        //                tem1 = Joints.First(x => Object.Equals(x.Key.Value.Key, edge.Value.Key) && Object.Equals(x.Key.Value.Value, edge.Value.Value));
        //                tem1.Value = true;

        //                richTextBox1.Text += "Нашли незанятый путь от источника к стоку. Находим минимальный поток через \"" + tem1.Key.Value.Key + " - " + tem1.Key.Value.Value + "\": " + pushFlow + Environment.NewLine;

        //                SetImage();
        //                pictureBox1.Image = FileDotEngine.Run(Begin + CurrentImage + End);
        //                await Task.Delay(1500);
        //            }
        //            catch { }
        //        }

        //        for (Pair<int, Pair<object, object>> edge = parent[sink]; edge != null; edge = parent[edge.Value.Key])
        //        {
        //            edge.Key -= pushFlow;
        //            edges.First(x => Object.Equals(x.Value.Key, edge.Value.Value) && Object.Equals(x.Value.Value, edge.Value.Key)).Key += pushFlow;

        //            try
        //            {
        //                tem1 = Joints.First(x => Object.Equals(x.Key.Value.Key, edge.Value.Key) && Object.Equals(x.Key.Value.Value, edge.Value.Value));

        //                richTextBox1.Text += "Минимальный поток найден. Вычитаем значение из \"запаса\" \"" + tem1.Key.Value.Key + " - " + tem1.Key.Value.Value + "\"" + Environment.NewLine;

        //                tem1.Key.Key -= pushFlow;

        //                SetImage();
        //                pictureBox1.Image = FileDotEngine.Run(Begin + CurrentImage + End);
        //                await Task.Delay(3000);
        //            }
        //            catch { }
        //            try
        //            {
        //                tem2 = Joints.First(x => Object.Equals(x.Key.Value.Key, edge.Value.Value) && Object.Equals(x.Key.Value.Value, edge.Value.Key));

        //                richTextBox1.Text += "Минимальный поток найден. Вычитаем значение из \"запаса\" \"" + tem2.Key.Value.Key + " - " + tem2.Key.Value.Value + "\"" + Environment.NewLine;

        //                tem2.Value = true;

        //                tem2.Key.Key += pushFlow;

        //                SetImage();
        //                pictureBox1.Image = FileDotEngine.Run(Begin + CurrentImage + End);
        //                await Task.Delay(3000);
        //            }
        //            catch { }
        //        }

        //        maxFlow += pushFlow;

        //        SetImage();
        //        pictureBox1.Image = FileDotEngine.Run(Begin + CurrentImage + End);

        //        Label.Text = "Макс.поток: " + maxFlow.ToString();
        //    }

        //    richTextBox1.Text += "Больше путей к стоку нет. Алгоритм завершен.";

        //    return maxFlow;
        //}

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        public async Task<int> DoDijkstraAsync(object StartingNode)
        {
            richTextBox1.Text = "Начало алгоритма" + Environment.NewLine;
            int sum = 0;
            await Task.Delay(2000);

            var temp = Graph.GetNodeNames();
            SortedList<string, int> pathLength = new SortedList<string, int>();
            SortedList<string, bool> nodeVisited = new SortedList<string, bool>();
            int n = Graph.Graf.Count;

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

                foreach (var node in Graph.Graf.Keys.ToArray())
                {

                    if ((pathLength[node.ToString()] < m) && (!nodeVisited[node.ToString()]))
                    {

                        m = pathLength[node.ToString()];
                        v = node.ToString();

                    }

                }

                     nodeVisited[v] = true;
               
                Pair<Pair<int, Pair<object, object>>, bool> tem1;
                //  Pair<Pair<int, Pair<object, object>>, bool> tem2;

                foreach (object node in Graph.Graf.Keys.ToArray())
                {
                    int weight;
             
                    try
                    {
                        weight = Graph.Graf[v][node];
                        if (!nodeVisited[node.ToString()] && ((pathLength[v] + weight) < pathLength[node.ToString()]))
                        {
                            pathLength[node.ToString()] = pathLength[v] + weight;
                            sum += pathLength[node.ToString()];
                        }
                        tem1 = Joints.First(x => Equals(x.Key.Value.Key, node) && Equals(x.Key.Value.Value, node.ToString()));
                        tem1.Value = true;

                        richTextBox1.Text += "Находим минимальное ребро через \"" + tem1.Key.Value.Key + " - " + tem1.Key.Value.Value + "\": " + pathLength + Environment.NewLine;

                        SetImage();
                        pictureBox1.Image = FileDotEngine.Run(Begin + CurrentImage + End);
                        await Task.Delay(1500);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            SetImage();
            pictureBox1.Image = FileDotEngine.Run(Begin + CurrentImage + End);

            Label.Text = "Минимальный путь: " + sum;


            richTextBox1.Text += "Больше путей нет. Алгоритм завершен.";

            return sum;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
                NextStepButton.Enabled = false;
             DoDijkstraAsync(Nodes.First().Key);
        }
        private void LoadButton_Click_1(object sender, EventArgs e)
        {
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                string filename = openFileDialog1.FileName;

                Graph = new Graph(filename);
                Nodes = Graph.GetNodes();

                if (Graph.IsDirected)
                {
                    Begin = "digraph g{" + Environment.NewLine + "rankdir=LR;" + Environment.NewLine + "size = \"760,490\"" + Environment.NewLine;
                    End = Environment.NewLine + "}";
                }
                else
                {
                    Begin = "graph g{" + Environment.NewLine + "rankdir=LR;" + Environment.NewLine + "size = \"760,490\"" + Environment.NewLine;
                    End = Environment.NewLine + "}";
                }

                var temp = Graph.GetJoints();

                Joints = new List<Pair<Pair<int, Pair<object, object>>, bool>>();

                CurrentImage = "";

                foreach (var joint in temp)
                {
                    Joints.Add(new Pair<Pair<int, Pair<object, object>>, bool>(joint, false));
                }

                SetImage();

                pictureBox1.Image = FileDotEngine.Run(Begin + CurrentImage + End);
                Label.Text = "Минимальный путь: 0";

                NextStepButton.Enabled = true;
            }
        }
    }
}
