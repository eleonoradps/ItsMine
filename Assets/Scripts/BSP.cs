using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSP : MonoBehaviour
{
    [SerializeField] private int boardRows; // rows and columns of the rectangle
    [SerializeField] private int boardColumns;
    [SerializeField] private int sizeRoomMin;
    [SerializeField] private int sizeRoomMax;

    class SubRoom
    {
        [SerializeField] private SubRoom left;
        [SerializeField] private SubRoom right;
        private Rect rectangle;
        private Rect room = new Rect(-1, -1, 0, 0); // float x, float x, float width, float height

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

            bool splitRoom;

            if (rectangle.width / rectangle.height >= 1.25)
            {
                splitRoom = false;
            }
            else if (rectangle.height / rectangle.width >= 1.25)
            {
                splitRoom = true;
            }
            else
            {
                splitRoom = Random.Range(0.0f, 1.0f) > 0.5;
            }

            if (splitRoom)
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
            return false;
        }

    }


    void Start()
    {

    }


    void Update()
    {

    }
}
