using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSP : MonoBehaviour
{
    [SerializeField] private int boardRows; // rows and columns of the rectangle
    [SerializeField] private int boardColumns;
    [SerializeField] private int sizeRoomMin;
    [SerializeField] private int sizeRoomMax;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap tilemapWall;
    [SerializeField] private Tile g_Tile; //ground tile
    [SerializeField] private Tile c_Tile; //corridors tile
    [SerializeField] private Tile w_Tile; //wall tile

    [SerializeField] private int corridorsHeight;
    [SerializeField] private int corridorsWidth;


    //[SerializeField] private GameObject groundTile; // éviter gameobject
    private GameObject[,] boardFloorPositions;

    //[SerializeField] private GameObject corridorTile;

    private SubRoom root;

    private void Start()
    {
        root = new SubRoom(new Rect(0, 0, boardRows, boardColumns));
        CreateBSP(root);
        root.CreateRoom();

        boardFloorPositions = new GameObject[boardRows, boardColumns];
        DrawRooms(root);
        BuildCorridors(root);
        BuildWalls(root);

        TruckWays();
    }

    private void CreateBSP(SubRoom subRoom)
    {
        if (subRoom.IsLeaf()) // if subroom too large
        {
            if (subRoom.Rectangle.width > sizeRoomMin && subRoom.Rectangle.height > sizeRoomMin)
            {
                subRoom.Split((int) sizeRoomMin, (int)sizeRoomMax);
                CreateBSP(subRoom.Left);
                CreateBSP(subRoom.Right);
            }
        }
    }


    private void DrawRooms(SubRoom subRoom)
    {
        if (subRoom == null)
        {
            return;
        }
        if (subRoom.IsLeaf())
        {
            for (int i = Mathf.RoundToInt(subRoom.Room.x); i < subRoom.Room.xMax; i++)
            {
                for (int j = Mathf.RoundToInt(subRoom.Room.y); j < subRoom.Room.yMax; j++)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), g_Tile);
                }
            }
        }
        else
        {
            DrawRooms(subRoom.Left);
            DrawRooms(subRoom.Right);
        }
    }

    private void BuildCorridors(SubRoom subRoom) //recursive function
    {
        if (subRoom == null) //check if subroom exists
        {
            return;
        }

        if(!subRoom.IsLeaf())
        {
            BuildCorridors(subRoom.Left);
            BuildCorridors(subRoom.Right);

            subRoom.CreateCorridors(subRoom.Left, subRoom.Right, corridorsHeight, corridorsWidth);
        }

        foreach (Rect corridor in subRoom.Corridors) //get corridor from the room from parameters
        {
            for (int i = Mathf.RoundToInt(corridor.x); i < corridor.xMax; i++)
            {
                for (int j = Mathf.RoundToInt(corridor.y); j < corridor.yMax; j++)
                {
                    if (boardFloorPositions[i, j] == null)
                    {
                        tilemap.SetTile(new Vector3Int(i, j, 0), c_Tile);
                    }
                }
            }
        }
    }

    private void BuildWalls(SubRoom subRoom)
    {
        for (int i = -1 ; i < boardRows; i++)
        {
            for (int j = -1 ; j < boardColumns; j++)
            {
                if (!tilemap.HasTile(new Vector3Int(i, j, 0)))
                { 
                    tilemapWall.SetTile(new Vector3Int(i, j, 0), w_Tile);
                }
            }

        }
    }

    private void TruckWays()
    {
        //player truck way
        tilemap.SetTile(new Vector3Int(0, 29, 0), g_Tile);
        tilemap.SetTile(new Vector3Int(0, 28, 0), g_Tile);
        tilemapWall.SetTile(new Vector3Int(0, 29, 0), null);
        tilemapWall.SetTile(new Vector3Int(0, 28, 0), null);
        
        //AI truck way
        tilemap.SetTile(new Vector3Int(27, -1, 0), g_Tile);
        tilemapWall.SetTile(new Vector3Int(27, -1, 0), null);
    }
}

class SubRoom
{
    private SubRoom left; //child1
    private SubRoom right; //child2
    private Rect rectangle;
    private Rect room = new Rect(-1, -1, 0, 0); // float x, float y, float width, float height

    private List<Rect> corridors = new List<Rect>(); //corridors for each room

    public SubRoom Left
    {
        get => left;
        set => left = value;
    }
    public SubRoom Right
    {
        get => right;
        set => right = value;
    }

    public Rect Rectangle
    {
        get => rectangle;
        set => rectangle = value;
    }

    public Rect Room
    {
        get => room;
        set => room = value;
    }

    public List<Rect> Corridors
    {
        get => corridors;
        set => corridors = value;
    }

    public SubRoom(Rect newRectangle) { rectangle = newRectangle; }

    public bool IsLeaf() // know if node in the tree has children
    {
        return left == null && right == null;
    }

    public bool Split(int sizeRoomMin, int sizeRoomMax)
    {
        if (!IsLeaf())
        {
            return false;
        }

        //vertical or horizontal split depending on proportions
        // if too wide = split vertically, too long = split horizontally
        // nearly square = random split

        bool splitVertical; // know if split vertically or horizontally

        if (rectangle.width / rectangle.height >= 1.25f)
        {
            splitVertical = false;
        }
        else if (rectangle.height / rectangle.width >= 1.25f)
        {
            splitVertical = true;
        }
        else
        {
            splitVertical = Random.Range(0.0f, 1.0f) > 0.5f;
        }

        if (splitVertical)
        {
            // split so the sub rooms widths are not too small
            // for horizontal split

            int split = Random.Range(sizeRoomMin, (Mathf.RoundToInt(rectangle.width - sizeRoomMin)));

            left = new SubRoom(new Rect(rectangle.x, rectangle.y, rectangle.width, split));
            right = new SubRoom(new Rect(rectangle.x, rectangle.y + split, rectangle.width, rectangle.height - split));
        }
        else
        {
            int split = Random.Range(sizeRoomMin, (Mathf.RoundToInt(rectangle.height - sizeRoomMin)));

            left = new SubRoom(new Rect(rectangle.x, rectangle.y, split, rectangle.height));
            right = new SubRoom(new Rect(rectangle.x + split, rectangle.y, rectangle.width - split, rectangle.height));
        }
        return true;
    }

    public void CreateRoom() // tree structure with leaves & each will create room with random size
    {
        if (left != null)
        {
            left.CreateRoom();
        }
        if (right != null)
        {
            right.CreateRoom();
        }
        if (IsLeaf())
        {
            int roomWidth = Mathf.RoundToInt(Random.Range(rectangle.width / 2, rectangle.width - 2)); //random.range (float min, float max)
            int roomHeight = Mathf.RoundToInt(Random.Range(rectangle.height / 2, rectangle.height - 2));
            int roomX = Mathf.RoundToInt(Random.Range(1, rectangle.width - roomWidth - 1));
            int roomY = Mathf.RoundToInt(Random.Range(1, rectangle.height - roomHeight - 1));

            // room position absolute on the board, not relative to subroom
            room = new Rect(rectangle.x, rectangle.y, rectangle.width - 2, rectangle.height - 2);

        }
    }

    //method to get room of subroom
    //if subroom not a leaf(doesn't have a room) => return the room of a child
    public Rect GetRoom()
    {
        if (IsLeaf())
        {
            return room;
        }
        if (left != null)
        {
            Rect leftRoom = left.GetRoom();
            if (leftRoom.x != -1) //security to check if room exists
            {
                return leftRoom;
            }
        }
        if (right != null)
        {
            Rect rightRoom = right.GetRoom();
            if (rightRoom.x != -1) //security to check if room exists
            {
                return rightRoom;
            }
        }
        //function always return rect so return default size
        return new Rect(-1, -1, 0, 0); //workaround for non nullable structs
    }

    //create corridors between rooms
    public void CreateCorridors(SubRoom left, SubRoom right, int corridorsHeight, int corridorsWidth)
    {

        Rect leftRoom = left.GetRoom();
        Rect rightRoom = right.GetRoom();

        //attach corridor to a random point in each room
        Vector2 leftPoint = new Vector2(Mathf.RoundToInt(Random.Range(leftRoom.x + 1, leftRoom.xMax - 1)),
                                        Mathf.RoundToInt(Random.Range(leftRoom.y + 1, leftRoom.yMax - 1)));
        Vector2 rightPoint = new Vector2(Mathf.RoundToInt(Random.Range(rightRoom.x + 1, rightRoom.xMax - 1)),
                                        Mathf.RoundToInt(Random.Range(rightRoom.y + 1, rightRoom.yMax - 1)));

        //check left point is on the left
        if (leftPoint.x > rightPoint.x)
        {
            Vector2 temp = leftPoint;
            leftPoint = rightPoint;
            rightPoint = temp;
        }

        //choose width and height of corridors
        int width = Mathf.RoundToInt(leftPoint.x - rightPoint.x);
        int height = Mathf.RoundToInt(leftPoint.y - rightPoint.y);

        //if point are not aligned horizontally choose random to go horizontal then vertical or opposite
        //and add corridor to the right
        if (width != 0) //check 
        {
            if (Random.Range(0, 2) > 1)
            {
                corridors.Add(new Rect(leftPoint.x, leftPoint.y, Mathf.Abs(width) + 1, corridorsHeight));

                //if left point is below right point we go up or else go down
                if (height < 0)
                {
                    corridors.Add(new Rect(rightPoint.x, leftPoint.y, corridorsWidth, Mathf.Abs(height)));
                }
                else
                {
                    corridors.Add(new Rect(rightPoint.x, leftPoint.y, corridorsWidth, -Mathf.Abs(height)));
                }
            }
            else //go up or go down
            {
                if (height < 0)
                {
                    corridors.Add(new Rect(leftPoint.x, leftPoint.y, corridorsWidth, Mathf.Abs(height)));
                }
                else
                {
                    corridors.Add(new Rect(leftPoint.x, rightPoint.y, corridorsWidth, Mathf.Abs(height)));
                }

                //then go right
                corridors.Add(new Rect(leftPoint.x, rightPoint.y, Mathf.Abs(width) + 1, corridorsHeight));
            }
        }
        else
        {
            //if points are aligned horizontally go up or down depending on positions
            if (height < 0)
            {
                corridors.Add(new Rect((int)leftPoint.x, (int)leftPoint.y, corridorsWidth, Mathf.Abs(height)));
            }
            else
            {
                corridors.Add(new Rect((int)rightPoint.x, (int)rightPoint.y, corridorsWidth, Mathf.Abs(height)));
            }
        }
    }
}


