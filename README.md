# PathCreation
A brief code sample, written in C#.  Takes in a .JSON file of several 'tiles' with varying linear shapes (squares, triangles), and then returns paths representing the perimeter of clusters of touching tiles.

For example, the sixteen tiles in the image below are grouped into eight (A through H) clusters, each consisting of tiles that much share a side.  THe program identifies such clusters and returns points representing the path around the perimeter of each cluster.

![Image of tiles](https://raw.githubusercontent.com/jaradhosking/PathCreation/master/tiles.png)
