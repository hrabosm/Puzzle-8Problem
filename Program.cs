using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace Puzzle8
{
    class Node
    {
        public Node parent;
        public int[,] mat;
        public int x, y; //pozice volného místa
        public int cost; //kolik políček není na správném místě
        public int level; //počet kroků
        public int value;
        public bool hasChildren;
        public int movedFrom;
        public Node(int[,] mat, int x, int y, int cost, int level, int movedFrom, Node parent = null)
        {
            this.parent = parent;
            this.mat = mat;
            this.x = x;
            this.y = y;
            this.cost = cost;
            this.level = level;
            this.value = cost+level;
            this.hasChildren = false;
            this.movedFrom = movedFrom;
        }
        public void printMat()
        {
            for(int i = 0; i < 3; i++)
            {
                Console.Write("*-*-*-*\n");
                for(int y = 0; y < 3; y++)
                {
                    Console.Write("|"+this.mat[i,y]);
                }
                Console.Write('|'+"\n");
            }
            Console.Write("*-*-*-*\n");
            Console.WriteLine("Current level: "+this.level);
            Console.Write("------------\n");
        }
    }
    class Generator
    {
        public static int CalculateCost(int[,] currentMat, int[,] finalMat)
        {
            int count = 0;
            for(int i = 0; i < 3; i++)
            {
                for(int o = 0; o < 3; o++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            if(currentMat[i,o] == finalMat[k,l] && currentMat[i,o] != 0)
                            {
                                count+= Math.Abs(i-k)+Math.Abs(o-l);
                            }
                        }
                    }
                }
            }
            return count;
            /*
            int count = 0;
            for(int i = 0; i < 3; i++)
            {
                for(int o = 0; o < 3; o++)
                {
                    if(currentMat[i,o] != finalMat[i,o] && currentMat[i,o] != 0)
                    {
                        count++;
                    }
                }
            }
            return count;
            */
        }
        public static void Solve(int[,] startMat, int x, int y, int[,] finalMat, Stopwatch sw)
        {
            List<Node> priority = new List<Node>();
            Node root = new Node(startMat,x,y,CalculateCost(startMat,finalMat),0,0);
            priority.Add(root);
            sw.Start();
            GenerateChildren(x, y, priority, finalMat, root, sw);
        }
        public static void Generate(List<Node> priority, int[,] finalMat, Stopwatch sw)
        {
            GenerateChildren(priority[0].x,priority[0].y, priority, finalMat, priority[0], sw);
        }
        public static void GenerateChildren(int x, int y, List<Node> priority, int[,] finalMat,  Node parent, Stopwatch sw)
        {
            if(priority[0].hasChildren)
            {
                priority.RemoveAt(0);
                GenerateTheOne(priority, finalMat, sw);
            }
            else
            {
                
                parent.hasChildren = true;
                if(x != 0 && parent.movedFrom !=2)
                {
                    Node child = new Node(Swap(parent,-1,0),x-1,y+0,CalculateCost(Swap(parent,-1,0),finalMat),parent.level+1,1,parent);
                    priority.Add(child);
                }
                if(x != 2 && parent.movedFrom !=1)
                {
                    Node child = new Node(Swap(parent,1,0),x+1,y+0,CalculateCost(Swap(parent,1,0),finalMat),parent.level+1,2,parent);
                    priority.Add(child);
                }
                if(y != 0 && parent.movedFrom !=4)
                {
                    Node child = new Node(Swap(parent,0,-1),x+0,y-1,CalculateCost(Swap(parent,0,-1),finalMat),parent.level+1,3,parent);
                    priority.Add(child);
                }
                if(y != 2 && parent.movedFrom !=3)
                {
                    Node child = new Node(Swap(parent,0,1),x+0,y+1,CalculateCost(Swap(parent,0,1),finalMat),parent.level+1,4,parent);
                    priority.Add(child);
                }
                GenerateTheOne(priority,finalMat, sw);
            }
        }
        public static void GenerateTheOne(List<Node> priority, int[,] finalMat, Stopwatch sw)
        {
            priority = priority.OrderBy(Node => Node.value).ToList();
            if(priority[0].cost == 0 )
                Solution(priority[0], sw);
            else
                Generate(priority,finalMat,sw);
        }
        public static int[,] Swap(Node parent, int xChange, int yChange)
        {
            int[,] newMat = new int[3,3]
            {
                {parent.mat[0,0],parent.mat[0,1],parent.mat[0,2]},
                {parent.mat[1,0],parent.mat[1,1],parent.mat[1,2]},
                {parent.mat[2,0],parent.mat[2,1],parent.mat[2,2]}
            };
            int value1 = parent.mat[parent.x,parent.y];
            int value2 = parent.mat[parent.x+xChange,parent.y+yChange];
            newMat[parent.x,parent.y] = value2;
            newMat[parent.x+xChange,parent.y+yChange] = value1;
            return newMat;
        }
        public static void Solution(Node solution, Stopwatch sw)
        {
            solution.printMat();
            while(solution.parent != null)
            {
                solution = solution.parent;
                solution.printMat();
            }
            sw.Stop();
            Console.WriteLine("Found solution! {0}", sw.Elapsed);
        }
        public static void Main()
        {
            int x = 0;
            int y = 0;
            int[,] startMat=
            {
                {5, 6, 7},
                {4, 0, 8},
                {3, 2, 1}
            };

            for(int i = 0; i < 3; i++)
            {
                for(int o = 0; o < 3; o++)
                {
                    if(startMat[i,o] == 0)
                    {
                        x = i;
                        y = o;
                        break;
                    }
                }
            }
        
            int[,] finalMat=
            {
                {1, 2, 3},
                {8, 0, 4},
                {7, 6, 5}
            };
            Stopwatch sw = new Stopwatch();
            ThreadStart threadStart = new ThreadStart(()=> Solve(startMat, x, y, finalMat, sw));
            Thread t = new Thread(threadStart,1*1024*1024*1024);
            t.Start();
        }
    }
}
