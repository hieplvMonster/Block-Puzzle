using System;
using System.Security.Cryptography;
using UnityEngine;

public class SpiralPrinter : MonoBehaviour
{
    public int sizeArray = 5;
    private void Start()
    {
        int[,] array = GenArray(sizeArray);
        PrintSpiral(array, sizeArray);
    }
     int[,] GenArray(int size)
    {
        int[,] arr = new int[size, size];
        for (int i = 0; i < size * size; i++)
            arr[i / size, i % size] = i;
        for(int i = 0; i < size; i++)
        {
            string s = string.Empty;
            for (int j = 0; j < size; j++)
               string.Format(s + $"{arr[i, j]}+  ");
            Debug.Log(s);
        }
        return arr;
    }
    void PrintSpiral(int[,] matrix, int size)
    {
        int x = 0, y = 0;
        int dir = 0;
        int count = 0;
        int s = 1;

        // Khởi tạo vị trí bắt đầu đúng
        x = Mathf.FloorToInt(size / 2) - (size % 2 == 0 ? 1 : 0);
        y = Mathf.FloorToInt(size / 2) - (size % 2 == 0 ? 1 : 0);
        Debug.LogError($"{x}-{y}");
        // Duyệt từng vòng xoắn ốc
        for (int k = 1; k <= size - 1; k++)
        {
            // Duyệt từng hướng trong vòng xoắn ốc
            for (int j = 0; j < (k < size - 1 ? 2 : 3); j++)
            {
                // Duyệt từng bước trong hướng hiện tại
                for (int i = 0; i < s; i++)
                {
                    // Kiểm tra giới hạn mảng **sau** khi di chuyển
                    switch (dir)
                    {
                        case 0: y++; break; // Phải
                        case 1: x++; break; // Xuống
                        case 2: y--; break; // Trái
                        case 3: x--; break; // Lên
                    }

                    // Kiểm tra giới hạn mảng
                    if (x >= 0 && x < size && y >= 0 && y < size)
                    {
                        Debug.Log($"{x}-{y} : {matrix[x, y]}");
                        count++;
                    }
                    else
                    {
                        // Nếu vượt quá giới hạn mảng, chuyển hướng
                        dir = (dir + 1) % 4;
                        break;
                    }
                }
                dir = (dir + 1) % 4; // Chuyển hướng
            }
            s++; // Tăng số bước cho vòng xoắn ốc tiếp theo
        }
    }
}