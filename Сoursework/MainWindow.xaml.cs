﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

namespace Сoursework
{
    /// <summary>
    /// The logic of interaction for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Help help = new Help();
        GraphDraw graph;
        List<Node> vertex, vertextwo;
        List<Ellipse> elip;
        Stack<int> stack;
        int countedge;

        /// <summary>
        /// Мнтод входа в приложения
        /// </summary>
        public MainWindow()
        {
            graph = new GraphDraw();
            InitializeComponent();
            elip = new List<Ellipse>();
            vertex = new List<Node>();
            vertextwo = new List<Node>();
            stack = new Stack<int>();


            toggle.IsChecked = false;
            buttonundo.IsEnabled = false;
            groupBox.IsEnabled = false;
            search.IsEnabled = false;
            buttoneclear.IsEnabled = false;

            doubleedge.IsChecked = true;
            oneedge.IsChecked = false;
        }
        /// <summary>
        /// Обработка двойного клика в элементе GridDraw.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridDraw_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (graph.Editing == true)
            {
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                {
                    Point position = e.GetPosition(GridDraw);
                    Node n = new Node((int)position.X, (int)position.Y, graph.ListNode.Count, Node_MouseRightButtonDown);
                    if (graph.ListNode.Count > 0)
                    {
                        bool build = true;
                        foreach (var item in graph.ListNode)
                        {
                            if (Math.Pow(2 * 17, 2) >= (Math.Pow(item.X - n.X, 2) + Math.Pow(item.Y - n.Y, 2)))
                            {
                                build = false;
                                break;
                            }
                        }
                        if (build)
                        {
                            AddNodeGrid(n);
                        }
                    }
                    else
                    {
                        AddNodeGrid(n);
                    }
                }
            }
        }

        /// <summary>
        /// Метод создания узла.
        /// </summary>
        /// <param name="n"></param>
        private void AddNodeGrid(Node n)
        {
            buttoneclear.IsEnabled = true;
            graph.ListNode.Add(n);
            stack.Push(0);
            buttonundo.IsEnabled = true;
            GridDraw.Children.Add(graph.ListNode[graph.ListNode.Count - 1].NodeV);
            Node.Content = $"Количество узлов: {graph.ListNode.Count.ToString()}";
        }

        /// <summary>
        /// Обработчик события клика по узлу. Создание Ребра.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Node_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (graph.Editing == true)
            {
                Ellipse el = (Ellipse)sender;
                if (vertex.Count != 2)
                {
                    foreach (var item in graph.ListNode)
                    {
                        if (el == item.NodeV)
                        {
                            if (vertex.Count < 2)
                            {
                                vertex.Add(item);
                                elip.Add(el);
                                if (vertex.Count == 1)
                                {
                                    el.StrokeThickness = 4;
                                }

                                if (vertex.Count == 2)
                                {
                                    #region
                                    if (vertex[0] == vertex[1])
                                    {
                                        foreach (var item1 in vertex)
                                        {
                                            bool k = false;
                                            foreach (var x in graph.ListEdge)
                                            {
                                                foreach (var y in x.Listnode)
                                                {
                                                    if (item1 == y)
                                                    {
                                                        k = true;
                                                    }
                                                }
                                            }
                                            if (!k)
                                            {
                                                item1.NodeV.StrokeThickness = 0;
                                            }
                                            else
                                            {
                                                item1.NodeV.StrokeThickness = 1;
                                            }
                                        }
                                        vertex.Clear();
                                        elip.Clear();
                                        return;
                                    }
                                    #endregion
                                    //edge => a---b
                                    if ((bool)this.oneedge.IsChecked)
                                    {
                                        Edge a = new Edge(vertex, (bool)this.oneedge.IsChecked);

                                        for (int i = vertex.Count - 1; i >= 0; i--)
                                        {
                                            vertextwo.Add(vertex[i]);
                                        }
                                        foreach (var ed in graph.ListEdge)
                                        {
                                            if (a.Equals(ed))
                                            {
                                                foreach (var items in elip)
                                                {
                                                    items.StrokeThickness = 1;
                                                }
                                                vertex.Clear();
                                                vertextwo.Clear();
                                                elip.Clear();
                                                return;
                                            }
                                        }

                                        graph.ListEdge.Add(a);
                                        GridDraw.Children.Add(graph.ListEdge[graph.ListEdge.Count - 1].Liner);
                                        Grid.SetZIndex(graph.ListEdge[graph.ListEdge.Count - 1].Liner, -1);
                                       

                                        if (a.Arrow1 != null)
                                        {
                                            GridDraw.Children.Add(graph.ListEdge[graph.ListEdge.Count - 1].Arrow1);
                                            Grid.SetZIndex(graph.ListEdge[graph.ListEdge.Count - 1].Arrow1, -1);
                                        }
                                        if (a.Arrow2 != null)
                                        {
                                            GridDraw.Children.Add(graph.ListEdge[graph.ListEdge.Count - 1].Arrow2);
                                            Grid.SetZIndex(graph.ListEdge[graph.ListEdge.Count - 1].Arrow2, -1);
                                        }
                                        stack.Push(2);
                                        countedge++;
                                        Edge.Content = $"Количество ребер: {(countedge).ToString()}";
                                    }
                                    else
                                    {

                                        Edge a = new Edge(vertex);

                                        for (int i = vertex.Count - 1; i >= 0; i--)
                                        {
                                            vertextwo.Add(vertex[i]);
                                        }
                                        foreach (var ed in graph.ListEdge)
                                        {
                                            if (a.Equals(ed))
                                            {
                                                foreach (var items in elip)
                                                {
                                                    items.StrokeThickness = 1;
                                                }
                                                vertex.Clear();
                                                vertextwo.Clear();
                                                elip.Clear();
                                                return;
                                            }
                                        }

                                        graph.ListEdge.Add(a);
                                        GridDraw.Children.Add(graph.ListEdge[graph.ListEdge.Count - 1].Liner);
                                        Grid.SetZIndex(graph.ListEdge[graph.ListEdge.Count - 1].Liner, -1);

                                        countedge++;

                                        //edge => b---a
                                        Edge b = new Edge(vertextwo);
                                        buttonundo.IsEnabled = true;
                                        graph.ListEdge.Add(b);
                                        GridDraw.Children.Add(graph.ListEdge[graph.ListEdge.Count - 1].Liner);
                                        Grid.SetZIndex(graph.ListEdge[graph.ListEdge.Count - 1].Liner, -1);
                                        Edge.Content = $"Количество ребер: {(countedge).ToString()}";
                                        

                                        stack.Push(1);
                                    }

                                    foreach (var x in elip)
                                    {
                                        x.StrokeThickness = 1;
                                    }
                                    vertex.Clear();
                                    vertextwo.Clear();
                                    elip.Clear();
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Очистка графа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (graph.Editing == true)
            {
                vertex.Clear();
                vertextwo.Clear();
                elip.Clear();
                countedge = 0;
                GridDraw.Children.Clear();
                graph = new GraphDraw();

                Node.Content = $"Количество узлов: {graph.ListNode.Count.ToString()}";
                Edge.Content = $"Количество ребер: {(countedge).ToString()}";
                stack = new Stack<int>();
                buttonundo.IsEnabled = false;
                buttoneclear.IsEnabled = false;
            }
        }

        /// <summary>
        ///  Отмена последнего действия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            switch (stack.Pop())
            {
                case 0:
                    {
                        vertex.Clear();
                        vertextwo.Clear();
                        elip.Clear();
                        graph.ListNode.RemoveAt(graph.ListNode.Count - 1);
                        GridDraw.Children.RemoveAt(GridDraw.Children.Count - 1);
                        Node.Content = $"Количество узлов: {graph.ListNode.Count.ToString()}";
                        if (graph.ListNode.Count == 0)
                        {
                            buttoneclear.IsEnabled = false;
                        }
                        break;
                    }
                case 1:
                    {
                        RemoveEdge(); //a---b
                        RemoveEdge(); //b---a
                        countedge -= 2;
                        Edge.Content = $"Количество ребер: {(countedge).ToString()}";

                        break;
                    }
                case 2:
                    {
                        RemoveEdge(); //a---b
                        countedge--;
                        Edge.Content = $"Количество ребер: {(countedge).ToString()}";
                        break;
                    }
                default:
                    break;
            }
            if (stack.Count == 0)
            {
                buttonundo.IsEnabled = false;
            }
        }

        /// <summary>
        /// Удаление ребра.
        /// </summary>
        private void RemoveEdge()
        {
            Edge item = graph.ListEdge[graph.ListEdge.Count - 1];
            graph.ListEdge.RemoveAt(graph.ListEdge.Count - 1);
            if (graph.ListEdge.Count > 0)
            {
                foreach (var item1 in item.Listnode)
                {
                    bool k = false;
                    foreach (var x in graph.ListEdge)
                    {
                        foreach (var y in x.Listnode)
                        {
                            if (item1 == y)
                            {
                                k = true;
                            }
                        }
                    }
                    if (!k)
                    {
                        item1.NodeV.StrokeThickness = 0;
                    }
                }
            }
            else
            {
                foreach (var g in item.Listnode)
                {
                    g.NodeV.StrokeThickness = 0;
                }
            }

            GridDraw.Children.RemoveAt(GridDraw.Children.Count - 1);
            if (item.Arrow1 != null)
            {
                GridDraw.Children.RemoveAt(GridDraw.Children.Count - 1);
            }

            if (item.Arrow1 != null)
            {
                GridDraw.Children.RemoveAt(GridDraw.Children.Count - 1);
            }
        }

        /// <summary>
        /// Поиск кратчайшего пути
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (graph.Editing == false)
            {
                try
                {
                    if (graph.ListEdge.Count == 0)
                    {
                        throw new ExceptionShow("No one edge. The continuation is not possible. Add an edge");
                    }
                    if (comboBox.Text != string.Empty && comboBox1.Text != string.Empty)
                    {
                        int nodes = int.Parse(comboBox.Text);
                        Node no = null;
                        foreach (var item in graph.ListNode)
                        {
                            if (item.Id == nodes)
                            {
                                no = item;
                                break;
                            }
                        }
                        //check
                        List<bool> builder = new List<bool>();
                        foreach (var item in graph.ListEdge)
                        {
                            if (!item.Listnode.Contains(no))
                            {
                                builder.Add(false);
                            }
                            else
                            {
                                builder.Add(true);
                            }
                        }

                        if (!builder.Contains(true))
                        {
                            throw new ExceptionShow("The selected node is not included in the edge.\nPlease add the node in the edge.");
                        }

                        AdjacencyList AL = new AdjacencyList(graph.ListEdge, graph.ListNode, nodes); // + invoke AdjacencyList.BFS    
                    }
                    else
                    {
                        throw new ExceptionShow("Enter the value");
                    }


                    SolidColorBrush blackBrush = new SolidColorBrush()
                    {
                        Color = Colors.Black
                    };
                    foreach (var item in graph.ListEdge)
                    {
                        item.Liner.Stroke = blackBrush;
                        Grid.SetZIndex(item.Liner, -1);
                        Grid.SetZIndex(item.Listnode[0].NodeV, 0);
                        Grid.SetZIndex(item.Listnode[1].NodeV, 0);
                        item.Liner.StrokeThickness = 4;
                        if (item.Arrow1 != null)
                        {
                            item.Arrow1.Stroke = blackBrush;
                            Grid.SetZIndex(item.Arrow1, -1);
                            item.Arrow1.StrokeThickness = 5;
                        }
                        if (item.Arrow2 != null)
                        {
                            item.Arrow2.Stroke = blackBrush;
                            Grid.SetZIndex(item.Arrow2, -1);
                            item.Arrow2.StrokeThickness = 5;
                        }
                    }

                    int ver = int.Parse(comboBox1.Text);
                    Node n = null;
                    foreach (var item in graph.ListNode)
                    {
                        if (item.Id == ver)
                        {
                            if (item.Numer != -1)
                            {
                                n = item;
                                break;
                            }
                            else
                            {
                                throw new ExceptionShow("To the final node has no edges."); ;
                            }
                        }
                    }

                    SolidColorBrush blueBrush = new SolidColorBrush()
                    {
                        Color = Colors.CornflowerBlue
                    };
                    if (n != null)
                    {
                        while (n.Numer != 0)
                        {
                            foreach (var item in graph.ListEdge)
                            {
                                if (n == item.Listnode[1])
                                {
                                    if (n.Numer > item.Listnode[0].Numer)
                                    {
                                        n = item.Listnode[0];
                                        item.Liner.Stroke = blueBrush;
                                        item.Liner.StrokeThickness = 7;
                                        Grid.SetZIndex(item.Liner, 1);
                                        Grid.SetZIndex(item.Listnode[0].NodeV, 2);
                                        Grid.SetZIndex(item.Listnode[1].NodeV, 2);
                                        if (item.Arrow1 != null)
                                        {
                                            Grid.SetZIndex(item.Arrow1, 1);
                                            item.Arrow1.Stroke = blueBrush;
                                            item.Arrow1.StrokeThickness = 8;
                                        }
                                        if (item.Arrow2 != null)
                                        {
                                            Grid.SetZIndex(item.Arrow2, 1);
                                            item.Arrow2.Stroke = blueBrush;
                                            item.Arrow2.StrokeThickness = 8;
                                        }
                                        foreach (var x in graph.ListNode)
                                        {
                                            if (GridDraw.Children.Contains(x.ValueNode))
                                            {
                                                Grid.SetZIndex(x.ValueNode, 2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (ExceptionShow exc)
                {
                    MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Выход.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            help.Close();
            Close();
        }

        /// <summary>
        /// Вывод информации о программе.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Application developer:\nVladisalv Mayorov\nThank professorweb.ru for the theme for WPF\nVersion: 0.0.1", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Проверка и установка режима редактирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (graph.ListNode.Count != 0)
            {
                if (toggle.IsChecked == true)
                {
                    foreach (var x in graph.ListNode)
                    {
                        comboBox.Items.Add(x.Id);
                        comboBox1.Items.Add(x.Id);
                    }
                    graph.Editing = false;
                    search.IsEnabled = true;
                    groupBox.IsEnabled = true;
                    buttoneclear.IsEnabled = false;
                    buttonundo.IsEnabled = false;
                    foreach (var item in graph.ListNode)
                    {
                        GridDraw.Children.Add(item.ValueNode);
                    }
                    comboBox.Text = "0";
                    comboBox1.Text = "1";
                    foreach (var item1 in vertex)
                    {
                        bool k = false;
                        foreach (var x in graph.ListEdge)
                        {
                            foreach (var y in x.Listnode)
                            {
                                if (item1 == y)
                                {
                                    k = true;
                                }
                            }
                        }
                        if (!k)
                        {
                            item1.NodeV.StrokeThickness = 0;
                        }
                        else
                        {
                            item1.NodeV.StrokeThickness = 1;
                        }
                    }
                    vertex.Clear();
                    elip.Clear();
                }
                else
                {
                    buttonundo.IsEnabled = true;
                    buttoneclear.IsEnabled = true;
                    SolidColorBrush blackBrush = new SolidColorBrush()
                    {
                        Color = Colors.Black
                    };
                    foreach (var item in graph.listedge)
                    {
                        item.Liner.Stroke = blackBrush;
                        Grid.SetZIndex(item.Liner, -1);
                        Grid.SetZIndex(item.Listnode[0].NodeV, 0);
                        Grid.SetZIndex(item.Listnode[1].NodeV, 0);
                        item.Liner.StrokeThickness = 4;

                        if (item.Arrow1 != null)
                        {
                            Grid.SetZIndex(item.Arrow1, -1);
                            item.Arrow1.Stroke = blackBrush;
                            item.Arrow1.StrokeThickness = 5;
                        }
                        if (item.Arrow2 != null)
                        {
                            Grid.SetZIndex(item.Arrow2, -1);
                            item.Arrow2.Stroke = blackBrush;
                            item.Arrow2.StrokeThickness = 5;
                        }
                    }
                    graph.Editing = true;
                    search.IsEnabled = false;
                    groupBox.IsEnabled = false;
                    foreach (var item in graph.ListNode)
                    {
                        if (GridDraw.Children.Contains(item.ValueNode))
                        {
                            GridDraw.Children.Remove(item.ValueNode);
                        }
                    }
                    comboBox.Items.Clear();
                    comboBox1.Items.Clear();
                }
            }
            else
            {
                MessageBox.Show("Create the graph.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                toggle.IsChecked = false;
            }
        }

        /// <summary>
        /// Вызов окна с информацией
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            help.Show();
        }

        /// <summary>
        /// Загрузка графа.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenClick(object sender, RoutedEventArgs e)
        {

            OpenFileDialog open = new OpenFileDialog()
            {
                FileName = "Graph",
                DefaultExt = ".bin",
                Filter = "Documents (.bin)|*.bin*"
            };
            bool? result = open.ShowDialog();
            if (result == true)
            {
                graph = new GraphDraw();
                GridDraw.Children.Clear();
                countedge = 0;

                using (FileStream f = new FileStream(open.FileName, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    try
                    {
                        SaveOpen s = (SaveOpen)bf.Deserialize(f);
                        foreach (var item in s.NodesValue)
                        {
                            graph.ListNode.Add(new Node((int)item.Value.X, (int)item.Value.Y, item.Key, Node_MouseRightButtonDown));
                            GridDraw.Children.Add(graph.ListNode[graph.ListNode.Count - 1].NodeV);
                        }
                        buttoneclear.IsEnabled = true;
                        buttonundo.IsEnabled = true;
                        Node.Content = $"Количество узлов:  {graph.ListNode.Count.ToString()}";
                        CreateEdge(s.matrix);
                    }
                    catch (System.Runtime.Serialization.SerializationException)
                    {
                        MessageBox.Show("File cannot be open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
        }
        /// <summary>
        /// Создание ребер на основе сохраненного файла с узлами.
        /// </summary>
        /// <param name="matrix">Матрица смежности.</param>
        private void CreateEdge(string[][] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix[i].GetLength(0); j++)
                {
                    List<Node> a = new List<Сoursework.Node>();
                    foreach (var item in graph.ListNode)
                    {
                        if (i == item.Id)
                        {
                            a.Add(item);
                            break;
                        }
                    }
                    foreach (var item in graph.ListNode)
                    {
                        if (matrix[i][j] != "")
                        {
                            if (int.Parse(matrix[i][j]) == item.Id)
                            {
                                a.Add(item);
                                break;
                            }
                        }
                    }
                    if (a.Count == 2)
                    {
                        Edge edge = new Edge(a);
                        graph.ListEdge.Add(edge);
                        countedge++;
                        GridDraw.Children.Add(graph.ListEdge[graph.ListEdge.Count - 1].Liner);
                        Grid.SetZIndex(graph.ListEdge[graph.ListEdge.Count - 1].Liner, -1);
                    }
                }
            }
            Edge.Content = $"Количество ребер:  {(countedge).ToString()}";

            foreach (var x in graph.ListNode)
            {
                foreach (var item in graph.ListEdge)
                {
                    if (item.Listnode.Contains(x))
                    {
                        x.NodeV.StrokeThickness = 1;
                    }
                }
            }
        }

        /// <summary>
        /// Сохранение графа.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveOpen s = new SaveOpen(graph);
            SaveFileDialog save = new SaveFileDialog()
            {
                FileName = "Graph",
                DefaultExt = ".bin",
                Filter = "Documents (.bin)|*.bin*"
            };
            bool? result = save.ShowDialog();
            if (result == true)
            {
                using (FileStream f = new FileStream(save.FileName, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(f, s);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            help.Close();
        }
    }
}