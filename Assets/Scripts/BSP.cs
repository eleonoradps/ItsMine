using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSP : MonoBehaviour
{
    [SerializeField] private int boardRows; // rows and columns of the rectangle
    [SerializeField] private int boardColumns;
    [SerializeField] private int sizeRoomMin;
    [SerializeField] private int sizeRoomMax;

    private SubRoom root;

    //// start = root.split

    private void Start()
    {
        root = new SubRoom(new Rect(0, 0, boardRows, boardColumns));
        CreateBSP(root);
        root.CreateRoom();
    }

    void CreateBSP(SubRoom subRoom)
    {
        if(subRoom.isLeaf()) // if subroom too large
        {
            if(subRoom.Rectangle.width > sizeRoomMax
               || subRoom.Rectangle.height > sizeRoomMax
               || Random.Range(0.0f,1.0f) > 0.25)
            {
                CreateBSP(subRoom.Left);
                CreateBSP(subRoom.Right);
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

        public bool isLeaf() // know if node in the tree has children
        {
            return (left == null && right == null);
        }

        public bool Split(int sizeRoomMin, int sizeRoomMax)
        {
            if (!isLeaf())
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
            if(left != null)
            {
                Left.CreateRoom();
            }
            if(right != null)
            {
                right.CreateRoom();
            }
            if(isLeaf())
            {
                int roomWidth = Mathf.RoundToInt(Random.Range(rectangle.width / 2, rectangle.width - 2)); //random.range (float min, float max)
                int roomHeight = Mathf.RoundToInt(Random.Range(rectangle.height / 2, rectangle.height - 2));
                int roomX = Mathf.RoundToInt(Random.Range(1, rectangle.width - roomWidth - 1));
                int roomY = Mathf.RoundToInt(Random.Range(1, rectangle.height - roomHeight - 1));

                // room position absolute (?) on the board, not relative to subroom
                room = new Rect(rectangle.x + roomX, rectangle.y + roomY, roomWidth, roomHeight); // Q
                
            }
        }
    }

