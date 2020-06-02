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

    [SerializeField] private GameObject groundTile; // éviter gameobject
    private GameObject[,] boardFloorPositions;

    private SubRoom root;

    private void Start()
    {
        root = new SubRoom(new Rect(0, 0, boardRows, boardColumns));
        CreateBSP(root);
        root.CreateRoom();

        boardFloorPositions = new GameObject[boardRows, boardColumns];
        DrawRooms(root);
        DrawCorridors(root);


    }

    void CreateBSP(SubRoom subRoom)
    {
        if(subRoom.IsLeaf()) // if subroom too large
        {
            if(subRoom.Rectangle.width > sizeRoomMax || subRoom.Rectangle.height > sizeRoomMax ||
                (
                    subRoom.Rectangle.width > sizeRoomMin || 
                    subRoom.Rectangle.height > sizeRoomMin) && 
                    Random.Range(0.0f,1.0f) > 0.25
                   )

            {
                subRoom.Split((int)subRoom.Rectangle.width,(int)subRoom.Rectangle.height);
                CreateBSP(subRoom.Left);
                CreateBSP(subRoom.Right);
            }
        }
    }

    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile g_Tile;
    [SerializeField] Tile c_Tile;

    void DrawRooms(SubRoom subRoom)
    {
        if(subRoom == null)
        {
            return;
        }
        if(subRoom.IsLeaf())
        {
            for(int i = Mathf.RoundToInt(subRoom.Room.x); i < subRoom.Room.xMax;i++)
            {
                for (int j = Mathf.RoundToInt(subRoom.Room.y); j < subRoom.Room.yMax; j++)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), g_Tile);
                    //GameObject instance = Instantiate(groundTile, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    //instance.transform.SetParent(transform);
                    //boardFloorPositions[i, j] = instance;
                }
            }
        }
        else
        {
            DrawRooms(subRoom.Left);
            DrawRooms(subRoom.Right);
        }
    }

    void DrawCorridors(SubRoom subRoom)
    {
        if(subRoom == null)
        {
            return;
        }

        DrawCorridors(subRoom.Left);
        DrawCorridors(subRoom.Right);

        foreach(Rect corridor in subRoom.Corridors)
        {
            for(int i = Mathf.RoundToInt(corridor.x);i< corridor.xMax;i++)
            {
                for (int j = Mathf.RoundToInt(corridor.y); j < corridor.yMax; j++)
                {
                    if(boardFloorPositions[i,j] == null)
                    {
                        tilemap.SetTile(new Vector3Int(i, j, 0), c_Tile);
                    }
                }
            }
        }
    }
}


 class SubRoom
 {
    public SubRoom(Rect newRectangle) { rectangle = newRectangle; }
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

    public bool IsLeaf() // know if node in the tree has children
    {
        return (left == null && right == null);
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

        if (rectangle.width / rectangle.height >= 1.25)
        {
            splitVertical = false;
        }
        else if (rectangle.height / rectangle.width >= 1.25)
        {
            splitVertical = true;
        }
        else
        {
            splitVertical = Random.Range(0.0f, 1.0f) > 0.5;
        }

        if (splitVertical)
        {
            // split so the sub rooms widths are not too small
            // for horizontal split

            int split = Random.Range(sizeRoomMax, (Mathf.RoundToInt(rectangle.width - sizeRoomMin)));

            left = new SubRoom(new Rect(rectangle.x, rectangle.y, rectangle.width, split));
            right = new SubRoom(new Rect(rectangle.x, rectangle.y + split, rectangle.width, rectangle.height - split));
        }
        else
        {
            int split = Random.Range(sizeRoomMin, (Mathf.RoundToInt(rectangle.height - sizeRoomMax)));

            left = new SubRoom(new Rect(rectangle.x, rectangle.y, split, rectangle.height));
            right = new SubRoom(new Rect(rectangle.x + split, rectangle.y, rectangle.width - split, rectangle.height));
        }
        return true;
    }

    public void CreateRoom() // tree structure with leaves & each will create room with random size
    {
        if(left != null)
        {
            left.CreateRoom();
        }
        if(right != null)
        {
            right.CreateRoom();
        }
        if(IsLeaf())
        {
            int roomWidth = Mathf.RoundToInt(Random.Range(rectangle.width / 2, rectangle.width - 2)); //random.range (float min, float max)
            int roomHeight = Mathf.RoundToInt(Random.Range(rectangle.height / 2, rectangle.height - 2));
            int roomX = Mathf.RoundToInt(Random.Range(1, rectangle.width - roomWidth - 1));
            int roomY = Mathf.RoundToInt(Random.Range(1, rectangle.height - roomHeight - 1));

            // room position absolute on the board, not relative to subroom
            room = new Rect(rectangle.x + roomX, rectangle.y + roomY, roomWidth, roomHeight);
                
        }
    }

    //method to get room of subroom
    //if subroom not a leaf(doesn't have a room) => return the room of a child
    public Rect GetRoom()
    {
        if(IsLeaf())
        {
            return room;
        }
        if(left != null)
        {
            Rect leftRoom = left.GetRoom();
            if(leftRoom.x != -1)
            {
                return leftRoom;
            }
        }
        if(right != null)
        {
            Rect rightRoom = right.GetRoom();
            if(rightRoom.x != -1)
            {
                return rightRoom;
            }
        }
        return new Rect(-1, -1, 0, 0); // workaround for non nullable structs
    }

    //create corridors between rooms
    public void CreateCorridors(SubRoom left, SubRoom right)
    {
        Rect leftRoom = left.GetRoom();
        Rect rightRoom = right.GetRoom();

        //attach corridor to a random point in each room
        Vector2 leftPoint = new Vector2(Mathf.RoundToInt(Random.Range(leftRoom.x + 1, leftRoom.xMax - 1)),
                                        Mathf.RoundToInt(Random.Range(leftRoom.y + 1, leftRoom.yMax - 1)));
        Vector2 rightPoint = new Vector2(Mathf.RoundToInt(Random.Range(rightRoom.x + 1, rightRoom.xMax - 1)),
                                        Mathf.RoundToInt(Random.Range(rightRoom.y + 1, rightRoom.yMax - 1)));

        //make sure that left point is on the left
        if(leftPoint.x > rightPoint.x) // Q
        {
            Vector2 temp = leftPoint;
            leftPoint = rightPoint;
            rightPoint = temp;
        }

        int w = Mathf.RoundToInt(leftPoint.x - rightPoint.x);
        int h = Mathf.RoundToInt(leftPoint.y - rightPoint.y);

        //if if point are not aligned horizontally choose random to go horizontal then vertical or opposite
        //and add corridor to the right
        if(w != 0)
        {
            if(Random.Range(0,1) > 2)
            {
                corridors.Add(new Rect(leftPoint.x, leftPoint.y, Mathf.Abs(w) + 1, 1));

                //if left point is below right point we go up or else go down
                if(h < 0)
                {
                    corridors.Add(new Rect(rightPoint.x, leftPoint.y, 1, Mathf.Abs(h)));
                }
                else
                {
                    corridors.Add(new Rect(rightPoint.x, leftPoint.y, 1, -Mathf.Abs(h)));
                }
            }
            else //go up or go down
            {
                if(h < 0)
                {
                    corridors.Add(new Rect(leftPoint.x, leftPoint.y, 1, Mathf.Abs(h)));
                }
                else
                {
                    corridors.Add(new Rect(leftPoint.x, rightPoint.y, 1, Mathf.Abs(h)));
                }

                //then go right
                corridors.Add(new Rect(leftPoint.x, rightPoint.y, Mathf.Abs(w) + 1, 1));
            }
        }
        else
        {
            //if points are aligned horizontally go up or down depending on positions
            if(h < 0)
            {
                corridors.Add(new Rect((int)leftPoint.x, (int)leftPoint.y, 1, Mathf.Abs(h)));
            }
            else
            {
                corridors.Add(new Rect((int)rightPoint.x, (int)rightPoint.y, 1, Mathf.Abs(h)));
            }
        }
    }
 }

