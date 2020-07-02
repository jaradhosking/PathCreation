using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace PathCreation
{
    public class Point
    {
        public double x;
        public double y;

        public Point(double xCood, double yCood)
        {
            x = xCood;
            y = yCood;
        }

        public Point(Tuple<double, double> coods)
        {
            (x, y) = coods;
        }

        public static Point operator +(Point a, Point b)
            => new Point(a.x + b.x, a.y + b.y);

        public static Point operator -(Point a, Point b)
            => new Point(a.x - b.x, a.y - b.y);

        public override string ToString()
            => $"({x}, {y})";

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Point);
        }

        public bool Equals(Point p)
        {
            if (p is null)
            {
                return false;
            }

            if (ReferenceEquals(this, p))
            {
                return true;
            }

            if (this.GetType() != p.GetType())
            {
                return false;
            }
            return (x == p.x) && (y == p.y);
        }

        public override int GetHashCode()
        {
            return (int)(x + y);
        }

        public static bool operator ==(Point a, Point b)
        {
            if (a is null)
            {
                if (b is null)
                {
                    return true;
                }
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }
    }
    public class Edge
    {
        public Point p1;
        public Point p2;

        public Edge(Point edgeP1, Point edgeP2)
        {
            p1 = edgeP1;
            p2 = edgeP2;
        }

        public Edge(Tuple<double, double> edgeP1, Tuple<double, double> edgeP2)
        {
            p1 = new Point(edgeP1);
            p2 = new Point(edgeP2);
        }

        public override string ToString()
            => $"{p1} to {p2}";

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Edge);
        }

        public bool Equals(Edge e)
        {
            if (e is null)
            {
                return false;
            }
            if (ReferenceEquals(this, e))
            {
                return true;
            }
            if (this.GetType() != e.GetType())
            {
                return false;
            }
            return (p1 == e.p1) && (p2 == e.p2);
        }

        public override int GetHashCode()
        {
            return (int)(p1.x + p1.y + p2.x + p2.y);
        }

        public static bool operator ==(Edge a, Edge b)
        {
            if (a is null)
            {
                if (b is null)
                {
                    return true;
                }
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(Edge a, Edge b)
        {
            return !(a == b);
        }
    }

    public class Tile
    {
        public Point pos;
        public int type;
        public List<Edge> edges = new List<Edge>();

        public Edge TopEdge() => new Edge(pos + new Point(-.5, .5), pos + new Point(.5, .5));
        public Edge RightEdge() => new Edge(pos + new Point(.5, .5), pos + new Point(.5, -.5));
        public Edge BotEdge() => new Edge(pos + new Point(-.5, -.5), pos + new Point(.5, -.5));
        public Edge LeftEdge() => new Edge(pos + new Point(-.5, .5), pos + new Point(-.5, -.5));
        public Edge ForwardSlashEdge() => new Edge(pos + new Point(-.5, -.5), pos + new Point(.5, .5));
        public Edge BackSlashEdge() => new Edge(pos + new Point(-.5, .5), pos + new Point(.5, -.5));

        public Tile(Point tilePos, int tileType)
        {
            pos = tilePos;
            type = tileType;
            if (type == 0) // ■
            {
                edges.Add(TopEdge());
                edges.Add(RightEdge());
                edges.Add(BotEdge());
                edges.Add(LeftEdge());
            }
            else if (type == 1) // ◣
            {
                edges.Add(BotEdge());
                edges.Add(LeftEdge());
                edges.Add(BackSlashEdge());
            }
            else if (type == 2) // ◤
            {
                edges.Add(TopEdge());
                edges.Add(LeftEdge());
                edges.Add(ForwardSlashEdge());
            }
            else if (type == 3) // ◥
            {
                edges.Add(TopEdge());
                edges.Add(RightEdge());
                edges.Add(BackSlashEdge());
            }
            else if (type == 4) // ◢
            {
                edges.Add(RightEdge());
                edges.Add(BotEdge());
                edges.Add(ForwardSlashEdge());
            }
        }

        public override string ToString()
            => $"Pos: {pos}, Type: {type}, Edges: {edges}";
    }
    class PathConstructor
    {
        static void Main(string[] args)
        {
            // Take in parameters
            string fIn;
            if (args.Length == 0)
            {
                fIn = @"..\..\test1.JSON";
            }
            else
            {
                fIn = @args[0];
            }


            // Read in from JSON
            Tile[] tiles;
            List<List<Edge>> tileGroups = new List<List<Edge>>();
            var tileJson = TileJson.FromJson(File.ReadAllText(@fIn));
            tiles = new Tile[tileJson.Length];
            int i = 0;
            foreach (TileJson element in tileJson)
            {
                tiles[i] = new Tile(new Point(element.Pos.X, element.Pos.Y), element.Type);
                i++;
            }


            // Sort the Tiles in reading order (left to right and then top to bottom)
            Array.Sort(tiles, delegate (Tile tile1, Tile tile2)
            {
                int x_dif = (int)(tile1.pos.x - tile2.pos.x);
                if (x_dif != 0)
                {
                    return x_dif;
                }
                return (int)(tile1.pos.y - tile2.pos.y);
            });


            // Separate the tiles into groups of connected/adjacent tiles
            foreach (Tile newTile in tiles)
            {
                List<List<Edge>> connectedGroups = new List<List<Edge>>();
                foreach (List<Edge> tileGroup in tileGroups)
                {
                    foreach (Edge edge in newTile.edges)
                    {
                        if (tileGroup.Contains(edge))
                        {
                            connectedGroups.Add(tileGroup);
                        }
                    }
                }
                if (connectedGroups.Count == 0)
                {
                    List<Edge> newGroup = newTile.edges.ToList(); // shallow copying
                    tileGroups.Add(newGroup);
                }
                else
                {
                    List<Edge> newGroup = newTile.edges.ToList(); // shallow copying
                    foreach (List<Edge> connectedGroup in connectedGroups)
                    {
                        foreach (Edge placedEdge in connectedGroup)
                        {
                            newGroup.Add(placedEdge);
                        }
                        tileGroups.Remove(connectedGroup);
                    }
                    tileGroups.Add(newGroup);
                }
            }


            // Remove inner/repeated edges from groups
            foreach (List<Edge> tileGroup in tileGroups)
            {
                foreach (Edge edge in tileGroup.ToList())
                {
                    if (tileGroup.FindAll(x => x == edge).Count > 1)
                    {
                        tileGroup.RemoveAll(x => x == edge);
                    }
                }
            }


            // Find vertices representing changes in direction
            List<List<Point>> pathGroups = new List<List<Point>>();
            foreach (List<Edge> tileGroup in tileGroups)
            {
                List<Point> pathGroup = new List<Point>();
                Edge nextEdge = tileGroup[0];
                Point nextPoint = nextEdge.p2;
                pathGroup.Add(nextEdge.p1);
                Point currentDirection = nextEdge.p2 - nextEdge.p1;
                Point initialDirection = currentDirection;
                while (tileGroup.Count != 0)
                {
                    currentDirection = nextEdge.p2 - nextEdge.p1;
                    tileGroup.Remove(nextEdge);
                    foreach (Edge edge in tileGroup)
                    {
                        if (edge.p1 == nextPoint)
                        {
                            if (edge.p2 - edge.p1 != currentDirection)
                            {
                                pathGroup.Add(nextPoint);
                            }
                            nextPoint = edge.p2;
                            nextEdge = edge;
                            break;
                        }
                        else if (edge.p2 == nextPoint)
                        {
                            if (edge.p2 - edge.p1 != currentDirection)
                            {
                                pathGroup.Add(nextPoint);
                            }
                            nextPoint = edge.p1;
                            nextEdge = edge;
                            break;
                        }
                    }
                }
                // Make sure the vertex of first/last edge is necessary
                if (currentDirection == initialDirection)
                {
                    pathGroup.RemoveAt(0);
                }
                pathGroups.Add(pathGroup);
            }


            // Print results to console
            bool printResults = false;
            if (printResults)
            {
                Console.WriteLine("path results");
                i = 0;
                foreach (List<Point> pathGroup in pathGroups)
                {
                    Console.WriteLine("path " + i);
                    foreach (Point pt in pathGroup)
                    {
                        Console.WriteLine(pt);
                    }
                    i++;
                }
            }


            // Convert output to JSON
            string fOut = fIn.Substring(0, fIn.Length - 5) + "_results.JSON";
            File.WriteAllText(@fOut, JsonConvert.SerializeObject(pathGroups));
        }
    }

    public partial class TileJson
    {
        [JsonProperty("pos")]
        public Pos Pos { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }
    }

    public partial class Pos
    {
        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }
    }

    public partial class TileJson
    {
        public static TileJson[] FromJson(string json) => JsonConvert.DeserializeObject<TileJson[]>(json);
    }

    public static class Serialize
    {
        public static string ToJson(this TileJson[] self) => JsonConvert.SerializeObject(self);
    }
}
